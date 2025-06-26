using UnityEngine;
using UnityEngine.UI;

public class BeamScaler : MonoBehaviour
{
    public Slider sizeSlider;
    public GameObject beamInstance;  // 动态放置出来的 BeamGroup

    private void Update()
    {
        if (beamInstance != null && sizeSlider != null)
        {
            float scale = sizeSlider.value;
            beamInstance.transform.localScale = Vector3.one * scale;
        }
    }

    public void SetBeamTarget(GameObject beam)
    {
        beamInstance = beam;
    }
}
