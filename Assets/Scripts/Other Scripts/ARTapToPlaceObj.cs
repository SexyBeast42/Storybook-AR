using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlaceObj : MonoBehaviour
{
    public GameObject gameObjToInstatiate;
    
    private GameObject spawnObj;
    private ARRaycastManager _arRaycastManager;
    private Vector2 touchPos;

    private static List<ARRaycastHit> _hits;

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }

    private bool TryGetTouchPos(out Vector2 touchPos)
    {
        if (Input.touchCount > 0)
        {
            touchPos = Input.GetTouch(0).position;
            
            return true;
        }

        touchPos = default;
        return false;
    }
    
    private void Update()
    {
        if (!TryGetTouchPos(out Vector2 touchPos))
            return;

        // Determine where the obj should spawn within the raycast
        if (_arRaycastManager.Raycast(touchPos, _hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = _hits[0].pose;
            
            // Spawn obj

            if (spawnObj == null)
            {
                spawnObj = Instantiate(gameObjToInstatiate, hitPose.position, hitPose.rotation);
            }
            else
            {
                spawnObj.transform.position = hitPose.position;
            }
        }
    }
}
