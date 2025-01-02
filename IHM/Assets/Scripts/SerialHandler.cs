using System;
using System.Diagnostics;
using System.IO.Ports;
using UnityEngine;


public class SerialHandler : MonoBehaviour
{
    private SerialPort _serial;

    [SerializeField] private string serialPort = "COM3";
    [SerializeField] private int baudrate = 115200;

    [SerializeField] private PlayerController playerController;
    private bool hasResetOnZero = false;

    void Start()
    {
        _serial = new SerialPort(serialPort,baudrate);
        _serial.NewLine = "\n";
        _serial.Open();
    }

    private void Update()
    {
        if (!_serial.IsOpen) return;

        while (_serial.BytesToRead >= 2)  // Ensure at least the 2-byte header is available
        {
            byte[] header = SerialRead(2);  // Read 2-byte header
            char type = (char)header[0];    // First byte: message type
            int length = header[1];         // Second byte: payload length

            byte[] payload = null;
            if (length > 0)
            {
                payload = SerialRead(length);  // Read the payload
            }

            ProcessMessage(type, payload);  // Process the message
        }
    }

    private byte[] SerialRead(int bytesToRead)
    {
        byte[] buffer = new byte[bytesToRead];
        int bytesRead = 0;

        while (bytesRead < bytesToRead)
        {
            bytesRead += _serial.Read(buffer, bytesRead, bytesToRead - bytesRead);
        }

        return buffer;
    }

    private void ProcessMessage(char type, byte[] payload)
    {
        switch (type)
        {
            case 'J': // Jump
                if ((playerController.jumpCounter == playerController.maxJumpAmount) && playerController.canStillJump)
                    playerController.PlayerJumpUp();
                else if ((playerController.jumpCounter > 0) && (playerController.jumpCounter < playerController.maxJumpAmount))
                    playerController.PlayerJumpUp();
                UnityEngine.Debug.Log("Jump message received");
                break;

            case 'D': // Dash
                playerController.serialDashRequested = true;

            case 'S':  // Speed data 
                if (payload != null && payload.Length == 1)
                {
                    int speed = payload[0];
                    playerController.UpdateMoveSpeed(speed / 1.275f);  // Normalize speed (#TODO_N I made it faster, let's have a middle ground)
                }
                break;
            case 'P': // Apply pressure to stomp #TODO_N peut être le faire s'écraser sur le côté. Aussi fixer problème de sursaut
                if (payload != null && payload.Length == 1)
                {
                    int force = payload[0];

                    if (force == 0)
                    {
                        if (!hasResetOnZero)
                        {
                            playerController.transform.localScale = new Vector3(2.5f, 2.5f, 1.0f);
                            hasResetOnZero = true;
                        }
                        break;
                    }

                    hasResetOnZero = false;
                    if (force > 60)
                    {
                        float initialScale = 2.5f;
                        float minScale = 1.0f;
                        float maxScale = initialScale;
                        float scaleFactor = 2.5f;

                        float normalizedScaleY = Mathf.Clamp(initialScale - scaleFactor * (force / 255.0f), minScale, maxScale);
                        playerController.transform.localScale = new Vector3(
                            initialScale,
                            normalizedScaleY,
                            1.0f
                        );

                        //UnityEngine.Debug.Log($"Force détectée : {force}, Échelle Y : {normalizedScaleY}");
                    }
                }
                break;

            case 'X':  // Horizontal movement data
                if (payload != null && payload.Length == 1)
                {
                    int hmove = payload[0];
                    hmove = hmove - 126;
                    if (hmove > 120)
                        hmove = 130;
                    else if (hmove < -120)
                        hmove = -130;
                    playerController.moveX = hmove * playerController.moveSpeed * 1.5f / 130f;

                }
                break;

            case 'Y':  // Vertical movement data
                if (payload != null && payload.Length == 1)
                {
                    int vmove = payload[0];
                    vmove = -(vmove - 124);
                    if (vmove > 120)
                        vmove = 130;
                    else if (vmove < -120)
                        vmove = -130;
                    playerController.moveY = vmove * playerController.moveSpeed * 1.5f / 130f;
                }
                break;

            default:
                //Debug.LogWarning($"Unknown message type: {type}");
                break;

                
        }
    }

    //pour si jamis on veut envoyer des données avec le message mais si non, juste utiliser le char et enlever l'argument de payload
    public void SendMessage(char messageType, byte[] payload)
    {
        if (!_serial.IsOpen)
        {
            UnityEngine.Debug.LogWarning("Serial port is not open. Cannot send message.");
            return;
        }

        byte header1 = (byte)messageType;
        byte header2 = (byte)(payload?.Length ?? 0);
        _serial.Write(new byte[] { header1, header2 }, 0, 2);

        if (payload != null && payload.Length > 0)
        {
            _serial.Write(payload, 0, payload.Length);
        }

        //UnityEngine.Debug.Log($"Sent message to Arduino: {messageType} with payload length: {header2}");
    }

    private void OnDestroy()
    {
        if (!_serial.IsOpen) return;
        _serial.Close();
    }
}
