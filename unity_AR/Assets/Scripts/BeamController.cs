using UnityEngine;

public class BeamController : MonoBehaviour
{
    public Transform txBeam;
    public Transform rxBeam;

    public UDPReceiver udpReceiver;  

    void Start()
    {

        if (udpReceiver != null)
        {
            udpReceiver.OnDataReceived += HandleData;
        }
        else
        {
            Debug.LogWarning("udpReceiver 未绑定！");
        }
    }

    public void HandleData(string message)
    {
        Debug.Log($"[DEBUG] BeamController.HandleData CALLED");

        Debug.Log("Received in Unity: " + message);

        float azimuth = ParseValue(message, "Azimuth");
        float elevation = ParseValue(message, "Elevation");
        Quaternion rotation = Quaternion.Euler(-elevation, azimuth, 0f);

        Debug.Log($"azimuth={azimuth}, elevation={elevation}");

        if (message.Contains("Device: Tx"))
        {
            Debug.Log("→ enter TX ");

            if (txBeam != null)
            {
                txBeam.rotation = rotation;
                Debug.Log("Rotated TX");
            }
            else
            {
                Debug.LogWarning("txBeam is NULL!");
            }
        }
        else if (message.Contains("Device: Rx"))
        {
            Debug.Log("→ enter RX ");

            if (rxBeam != null)
            {
                rxBeam.rotation = rotation;
                Debug.Log("Rotated RX");
            }
            else
            {
                Debug.LogWarning("rxBeam is NULL!");
            }
        }
    }

    float ParseValue(string message, string key)
    {
        try
        {
            int start = message.IndexOf(key + ":") + key.Length + 1;
            int end = message.IndexOf(",", start);
            if (end == -1) end = message.Length;
            string number = message.Substring(start, end - start).Trim().Split(' ')[0];
            return float.Parse(number);
        }
        catch
        {
            return 0f;
        }
    }

    void OnDestroy()
    {
        
        if (udpReceiver != null)
        {
            udpReceiver.OnDataReceived -= HandleData;
        }
    }
}
