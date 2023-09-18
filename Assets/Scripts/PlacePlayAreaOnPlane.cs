using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

/// <summary>
/// Detects if the player taps the screen, then place the play area on the tapped location. The game will then go into
/// an edit mode, allowing the player to:
///     Move the play area
///     Rotate the play area
///     Resize the play area
/// </summary>

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlacePlayAreaOnPlane : MonoBehaviour
{
    [SerializeField] private GameObject ghostArea, spawnObj, playArea;

    private ARRaycastManager _arRaycastManager;
    private ARPlaneManager _arPlaneManager;
    private List<ARRaycastHit> _arRaycastHits = new List<ARRaycastHit>();

    // Change phase from edit to done
    private enum State {scan, edit, done}
    private State _state;
    
    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        _arPlaneManager = GetComponent<ARPlaneManager>();

        _state = State.scan;
    }
    
    // To enable the EnhancedTouch support
    private void OnEnable()
    {
        // To simulate touch in Unity Editor
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        
        // Subscribe to touch event to cast RayCast
        EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable()
    {
        // To simulate touch in Unity Editor
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        
        // Unsubscribe to touch event
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    // Function that detects the player's touch input
    private void FingerDown(EnhancedTouch.Finger finger)
    {
        // Calls the function if only 1 finger is down
        if (finger.index != 0) return;
        
        ////////////////////////////////////////////////////////////Logic here if in detecting mode
        switch (_state)
        {
            case State.scan:
                // Spawn ghost area
                InstiateGhostArea(finger);
                
                break;
            
            case State.edit:
                // Spawn play area
                
                InstatiatePlayArea(spawnObj);
                break;
            
            case State.done:
                // Disable plane viewer
                DisablePlanePrefab();
                
                break;
        }
    }

    // Create PlayArea if the raycast is within a valid plane
    private void InstiateGhostArea(EnhancedTouch.Finger finger)
    {
        // Cast a raycast in a within detected plane
        if (_arRaycastManager.Raycast(finger.currentTouch.screenPosition, 
                _arRaycastHits, 
                TrackableType.PlaneWithinPolygon))
        {
            foreach (ARRaycastHit hit in _arRaycastHits)
            {
                // Checks if the plane is on the floor
                if (_arPlaneManager.GetPlane(hit.trackableId).alignment == PlaneAlignment.HorizontalUp)
                {
                    // Position and orientation on the plane
                    Pose pose = hit.pose;

                    spawnObj = Instantiate(ghostArea, pose.position, pose.rotation);
                    
                    // Rotates the obj so that it faces the player on instatiate
                    Vector3 pos = spawnObj.transform.position;
                    pos.y = 0;
                    
                    Vector3 cameraPos = Camera.main.transform.position;
                    cameraPos.y = 0;

                    Vector3 direction = cameraPos - pos;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    spawnObj.transform.rotation = targetRotation;

                    // Change states
                    _state = State.edit;
                }
            }
        }
    }

    private void EditPlayArea()
    {
        
    }

    private void InstatiatePlayArea(GameObject gameObject)
    {
        spawnObj.SetActive(false);
        
        Instantiate(playArea, gameObject.transform.position, Quaternion.identity);
    }
    private void DisablePlanePrefab()
    {
        foreach (ARPlane plane in _arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(false); 
        }
    }
}
