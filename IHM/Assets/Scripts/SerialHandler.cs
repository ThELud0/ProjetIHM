using System;
using System.IO.Ports;
using UnityEngine;


public class SerialHandler : MonoBehaviour
{
    private SerialPort _serial;

    [SerializeField] private string serialPort = "COM3";
    [SerializeField] private int baudrate = 115200;

    [SerializeField] private PlayerController playerController;

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
            case 'S':  // Speed data
                if (payload != null && payload.Length == 1)
                {
                    int speed = payload[0];
                    playerController.UpdateMoveSpeed(speed / 12.75f);  // Normalize speed
                }
                break;

            default:
                Debug.LogWarning($"Unknown message type: {type}");
                break;
        }
    }

    private void OnDestroy()
    {
        if (!_serial.IsOpen) return;
        _serial.Close();
    }
}
