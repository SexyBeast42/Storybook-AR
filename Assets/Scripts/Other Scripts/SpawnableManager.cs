using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField] ARRaycastManager _raycastManager;
    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    
    [SerializeField] private GameObject spawnableObj;
    private GameObject spawnedObj;
    
    private Camera _ARCam;

    private void Awake()
    {
        spawnedObj = null;

        _ARCam = GameObject.Find("AR Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        SpawnObj();
    }

    private void SpawnObj()
    {
        if (Input.touchCount > 0)
            return;

        RaycastHit hit;
        Ray ray = _ARCam.ScreenPointToRay(Input.GetTouch(0).position);

        if (_raycastManager.Raycast(Input.GetTouch(0).position, _hits))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObj == null)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Spawnable")
                    {
                        spawnedObj = hit.collider.gameObject;
                    }
                    else
                    {
                        SpawnPrefab(_hits[0].pose.position);
                    }
                }
            }
            
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObj != null)
            {
                spawnedObj.transform.position = _hits[0].pose.position;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                spawnedObj = null;
            }
        }
    }

    private void SpawnPrefab(Vector3 spawnPosition)
    {
        spawnedObj = Instantiate(spawnableObj, spawnPosition, Quaternion.identity);
    }
}
