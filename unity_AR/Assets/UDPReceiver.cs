using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UDPReceiver : MonoBehaviour
{
    public string serverAddress = "127.0.0.1";
    //public string serverAddress = "192.168.5.1";
    public int serverPort = 5005;
    public bool connectionStart = true;

    public TMP_Text displayText;
    public TMP_Text connectButtonText;
    public Button connectButton;
    public Button stopButton;
    public Button showLogsButton;

    private Thread receiveThread;
    private UdpClient udpClient;
    private bool isReceiving = false;
    private bool showLogs = true;

    private readonly ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

    public GameObject txBeam;
    public GameObject rxBeam;

    private string lastReceivedMessage = "";

    public BeamRotate txRotator;
    public BeamRotate rxRotator;

    public event Action<string> OnDataReceived;




    void Start()
    {
        if (connectButton != null)
            connectButton.onClick.AddListener(StartReceiving);
        if (stopButton != null)
            stopButton.onClick.AddListener(StopReceiving);
        if (showLogsButton != null)
            showLogsButton.onClick.AddListener(ToggleLogs);

        if (connectionStart)
        {
            StartReceiving();
            UpdateButtonText(true);
        }
    }

    void Update()
    {
        while (messageQueue.TryDequeue(out var message))
        {
            lastReceivedMessage = message;

            if (displayText != null && showLogs)
            {
                displayText.text = message;
            }

            if (showLogs)
            {
                Debug.Log(message);
            }

            ApplyRotation(message);
        }
    }






    public void StartReceiving()
    {
        if (isReceiving)
        {
            Debug.Log("Already receiving.");
            return;
        }

        try
        {
            udpClient = new UdpClient(serverPort);
            isReceiving = true;
            receiveThread = new Thread(ReceiveLoop);
            receiveThread.IsBackground = true;
            receiveThread.Start();
            Debug.Log($" UDPReceiver listening on port {serverPort}");

            Debug.Log("UDP Receiver started.");
            UpdateButtonText(true);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to start UDP: " + ex.Message);
        }
    }

    public void StopReceiving()
    {
        if (!isReceiving)
        {
            Debug.Log("Receiver not running.");
            return;
        }

        isReceiving = false;
        udpClient?.Close();
        receiveThread?.Join();
        Debug.Log("UDP Receiver stopped.");
        UpdateButtonText(false);
    }

    private void ReceiveLoop()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, serverPort);

        try
        {
            while (isReceiving)
            {
                byte[] data = udpClient.Receive(ref endPoint);
                string message = Encoding.UTF8.GetString(data);
                Debug.Log($"UDP Raw Received: {message}"); 
                messageQueue.Enqueue(message);

            }
        }
        catch (SocketException ex)
        {
            Debug.LogWarning("UDP Receive interrupted: " + ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("UDP Receive Error: " + ex.Message);
        }
    }

    private void UpdateButtonText(bool connected)
    {
        if (connectButtonText != null)
        {
            connectButtonText.text = connected ? "connected" : "connect";
        }
    }

    public void ToggleLogs()
    {
        showLogs = !showLogs;

        if (!showLogs && displayText != null)
        {
            displayText.text = "";
        }

        Debug.Log($"[UI] ShowLogs = {showLogs}");
    }




    void OnApplicationQuit()
    {
        StopReceiving();
    }

    private string ExtractValue(string message, string key)
    {
        int start = message.IndexOf(key + ":") + key.Length + 1;
        int end = message.IndexOf(",", start);
        if (end == -1) end = message.Length;
        return message.Substring(start, end - start).Trim().Split(' ')[0];
    }

    private void ApplyRotation(string message)
    {
        try
        {
            if (message.Contains("Device:") && message.Contains("Azimuth:") && message.Contains("Elevation:"))
            {
                string device = ExtractValue(message, "Device");
                float azimuth = float.Parse(ExtractValue(message, "Azimuth"));
                float elevation = float.Parse(ExtractValue(message, "Elevation"));

                Quaternion rotation = Quaternion.Euler(elevation, azimuth, 0); // x=elevation, y=azimuth

                if (device == "Tx" && txBeam != null)
                    txBeam.transform.rotation = rotation;
                else if (device == "Rx" && rxBeam != null)
                    rxBeam.transform.rotation = rotation;
                if (displayText != null && showLogs)
                {
                    displayText.text = $"{message}\nAzimuth: {azimuth:F1}°, Elevation: {elevation:F1}°";
                }

            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Failed to apply rotation: " + ex.Message);
        }
    }


}