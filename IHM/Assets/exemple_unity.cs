using System;
using System.Globalization;
using System.IO.Ports;
using UnityEngine;
/*
public class SerialHandler : MonoBehaviour
{
    
    private SerialPort _serial;

    // Common default serial device on a Windows machine
    [SerializeField] private string serialPort = "COM3";
    [SerializeField] private int baudrate = 115200;
    
    [SerializeField] private Component river;
    private Rigidbody2D _riverRigidbody2D;
    private SpriteRenderer _riverSprite;
    
    [SerializeField] private LightController lightController;
    
    // Start is called before the first frame update
    void Start()
    {
        _riverRigidbody2D = river.GetComponentInParent<Rigidbody2D>();
        _riverSprite = river.GetComponentInParent<SpriteRenderer>();
        
        _serial = new SerialPort(serialPort,baudrate);
        // Guarantee that the newline is common across environments.
        _serial.NewLine = "\n";
        // Once configured, the serial communication must be opened just like a file : the OS handles the communication.
        _serial.Open();
    }

    
    // Update is called once per frame
    void Update()
    {
        // Return early if not open, prevent spamming errors for no reason.
        if (!_serial.IsOpen) return;
        // Prevent blocking if no message is available as we are not doing anything else
        // Alternative solutions : set a timeout, read messages in another thread, coroutines, futures...
        if (_serial.BytesToRead <= 0) return;
        
        // Trim leading and trailing whitespaces, makes it easier to handle different line endings.
        // Arduino uses \r\n by default with `.println()`.
        var message = _serial.ReadLine().Trim();
        
        // Split the message on spaces, in case we want to pass a value as well.
        var messageParts = message.Split(' ');
        switch (messageParts[0])
        {
            case "dry":
                _riverRigidbody2D.simulated = false;
                _riverSprite.color = new Color32(146,108,77,255);
                break;
            case "wet":
                _riverRigidbody2D.simulated = true;
                _riverSprite.color = new Color32(16,107,255,255);
                break;
            case "speed":
                int speedMulti;
                int.TryParse(messageParts[1], out speedMulti);
                setTimerMultiplier(speedMulti / 1000f);
                break;
            default:
                Debug.Log($"Unknown message: {message}");
                break;
        }
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
            case 'D':  // Dry/Wet toggle
                ToggleRiverState();
                break;

            case 'S':  // Speed data
                if (payload != null && payload.Length == 1)
                {
                    int speed = payload[0];
                    setTimerMultiplier(speed / 127.5f);  // Normalize speed
                }
                break;

            default:
                Debug.LogWarning($"Unknown message type: {type}");
                break;
        }
    }

    private void ToggleRiverState()
    {
        if (_riverRigidbody2D.simulated)
        {
            _riverRigidbody2D.simulated = false;
            _riverSprite.color = new Color32(146, 108, 77, 255);  // Dry color
            Debug.Log("River state: dry");
        }
        else
        {
            _riverRigidbody2D.simulated = true;
            _riverSprite.color = new Color32(16, 107, 255, 255);  // Wet color
            Debug.Log("River state: wet");
        }
    }

    public void setTimerMultiplier(float speed)
    {
        lightController.UpdateTimerMultiplier(speed);
    }

    public void SetLed(bool newState)
    {
        if (!_serial.IsOpen) return;
        _serial.WriteLine(newState ? "LED ON" : "LED OFF");
    }
    
    private void OnDestroy()
    {
        if (!_serial.IsOpen) return;
        _serial.Close();
    }
}
*/