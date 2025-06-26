using UnityEngine;

public class BeamRotate : MonoBehaviour
{
    public Vector3 rotationOffset = Vector3.zero;

    public void UpdateRotation(float azimuth, float elevation)
    {
        Vector3 targetRotation = new Vector3(elevation, azimuth, 0) + rotationOffset;
        transform.localRotation = Quaternion.Euler(targetRotation);
        Debug.Log($"[BeamRotate] Set rotation: Azimuth = {azimuth}, Elevation = {elevation}");
    }
}

