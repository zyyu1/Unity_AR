using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARBeamPlacer : MonoBehaviour
{
    public GameObject beamPrefab;
    private ARRaycastManager raycastManager;
    private GameObject spawnedObject;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (Input.touchCount > 0 && spawnedObject == null)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    spawnedObject = Instantiate(beamPrefab, hitPose.position + new Vector3(0, 0.05f, 0), hitPose.rotation);
                    spawnedObject.transform.localScale = Vector3.one * 0.1f;
                }
            }
        }
    }
}
