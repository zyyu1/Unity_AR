using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using TMPro; // For displaying data in UI

public class UDPReceiver : MonoBehaviour
{
    // UDP server settings
    public string serverAddress = "127.0.0.1";  // Localhost, change if needed
    public int serverPort = 5005;               // Same port as sender
    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private bool isReceiving = false;

    // Bool to control whether to start the connection automatically
    public bool connectionStart = false;        // Set to false by default

    // Event for when new data is received
    public event Action<string> OnDataReceived;

    // UI element to show received data
    public TextMeshProUGUI displayText; // Assign this in the Inspector

    // Start is called before the first frame update
    void Start()
    {
        // Only connect if connectionStart is true
        if (connectionStart)
        {
            ConnectToServer();
        }
    }

    // Method to start the connection
    public void ConnectToServer()
    {
        endPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
        udpClient = new UdpClient(serverPort);
        isReceiving = true;
        ReceiveData();
    }

    // Receive data asynchronously
    private async void ReceiveData()
    {
        try
        {
            while (isReceiving)
            {
                // Wait for data to arrive
                var result = await udpClient.ReceiveAsync();

                // Convert the received bytes to a string (raw data)
                string receivedMessage = System.Text.Encoding.UTF8.GetString(result.Buffer);
                
                // Log the raw data
                Debug.Log($"Received raw data: {receivedMessage}");

                // Trigger the event to notify that new data has been received
                OnDataReceived?.Invoke(receivedMessage);

                // Update UI text if assigned
                if (displayText != null)
                {
                    displayText.text = receivedMessage;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error receiving data: {e.Message}");
        }
    }

    // OnDisable, close the UDP client when the object is disabled
    private void OnDisable()
    {
        isReceiving = false;
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}
