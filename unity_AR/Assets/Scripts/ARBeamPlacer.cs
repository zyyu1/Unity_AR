using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARObjectPlacer : MonoBehaviour
{
    public GameObject txPrefab;  // 发射器 prefab
    public GameObject rxPrefab;  // 接收器 prefab
    public ARRaycastManager raycastManager;

    private GameObject txObject;
    private GameObject rxObject;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    if (txObject == null && rxObject == null)
                    {
                        // 创建两个模型，略微分开一点位置
                        Vector3 txPos = hitPose.position + new Vector3(-0.1f, 0, 0);
                        Vector3 rxPos = hitPose.position + new Vector3(0.1f, 0, 0);

                        txObject = Instantiate(txPrefab, txPos, hitPose.rotation);
                        rxObject = Instantiate(rxPrefab, rxPos, hitPose.rotation);
                        // 找到 BeamController 脚本并绑定生成的小球

                        BeamController controller = FindObjectOfType<BeamController>();
                        if (controller != null)
                        {
                            controller.txBeam = txObject.transform;
                            controller.rxBeam = rxObject.transform;
                        }

                    }
                }
            }
        }
    }
}
