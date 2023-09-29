using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

/// <summary>
/// Detects if the player taps the screen, then place the play area on the tapped location. The game will then go into
/// an edit mode, allowing the player to:
///     Move the play area
///     Rotate the play area
///     Resize the play area
///
/// Once the player finishes, the play area will spawn and the player can start the story.
/// </summary>

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlacePlayAreaOnPlane : MonoBehaviour
{
    // Area gameobj
    [SerializeField] private GameObject ghostArea, spawnObj, playArea, finalArea;
    private bool ghostAreaInstantiated, playAreaInstantiated;

    // [SerializeField] private GameObject testObj;

    // For AR
    private ARRaycastManager _arRaycastManager;
    private ARPlaneManager _arPlaneManager;
    private List<ARRaycastHit> _arRaycastHits = new List<ARRaycastHit>();
    private ARPlane currentPlane;

    // Change phase from edit to done
    private enum State {scan, edit, done}
    private State _state;
    
    // Edit ghost area
    [SerializeField] private GameObject editUI;
    public Slider sizeSlider, rotationSlider;
    private bool isDragging;
    private Vector3 offset;
    private float objSize;
    
    void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        _arPlaneManager = GetComponent<ARPlaneManager>();

        _state = State.scan;

        ghostAreaInstantiated = false;
        playAreaInstantiated = false;
        
        editUI.SetActive(false);
    }

    void Update()
    {
        HandleStates();
    }

    private void HandleStates()
    {
        switch (_state)
        {
            case State.scan:
                if (ghostAreaInstantiated) _state = State.edit;
                
                break;
            
            case State.edit:
                // Enable editUI
                editUI.SetActive(true);
                
                // Move ghost area
                //MoveGameObject(EnhancedTouch.Touch.activeTouches[0]);
                
                // Spawn play area through button press
                
                ChangeObjSize(sizeSlider.value);
                ChangeObjRotation(rotationSlider.value);
                
                if (playAreaInstantiated)
                {
                    editUI.SetActive(false);
                    _state = State.done;
                }
                
                break;
            
            case State.done:
                // Disable plane viewer
                DisablePlanePrefab();
                
                break;
        }
    }

    // To enable the EnhancedTouch support
    private void OnEnable()
    {
        // To simulate touch in Unity Editor
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        
        // Subscribe to touch event
        EnhancedTouch.Touch.onFingerDown += InstantiateGhostArea;
        EnhancedTouch.Touch.onFingerDown += MoveGameObject;
    }

    private void OnDisable()
    {
        // To simulate touch in Unity Editor
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        
        // Unsubscribe to touch event
        EnhancedTouch.Touch.onFingerDown -= InstantiateGhostArea;
        EnhancedTouch.Touch.onFingerDown -= MoveGameObject;
    }
    
    // Create ghostArea if the raycast is within a valid plane
    private void InstantiateGhostArea(EnhancedTouch.Finger finger)
    {
        // Cast a raycast in a within detected plane
        if (!ghostAreaInstantiated 
            && 
            _arRaycastManager.Raycast(finger.currentTouch.screenPosition, 
                _arRaycastHits, 
                TrackableType.PlaneWithinPolygon))
        {
            // ARRaycastHit hit = _arRaycastHits[0];
            foreach (ARRaycastHit hit in _arRaycastHits)
            {
                // Checks if the plane is on the floor
                if (_arPlaneManager.GetPlane(hit.trackableId).alignment != PlaneAlignment.HorizontalUp) return;

                currentPlane = _arPlaneManager.GetPlane(hit.trackableId);
                    
                // Position and orientation on the plane
                Pose pose = hit.pose;

                Vector3 pos = pose.position;
                pos.y = 0;
                    
                Vector3 cameraPos = Camera.main.transform.position;
                cameraPos.y = 0;
        
                Vector3 direction = cameraPos - pos;
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                spawnObj = Instantiate(ghostArea, pose.position, targetRotation);

                if (spawnObj != null) ghostAreaInstantiated = true;
            }
        }
    }
    
    // Move the ghostArea within the valid plane
    private void MoveGameObject(EnhancedTouch.Finger finger)
    {
        // Returns if there's more than one finger pressing the screen
        if (finger.index != 0) return;
        
        // Cast raycast within a valid plane, and make sure that they aren't tapping on UI
        if (!playAreaInstantiated 
            && 
            _arRaycastManager.Raycast(finger.currentTouch.screenPosition, 
                _arRaycastHits, 
                TrackableType.PlaneWithinPolygon)
            &&
            !IsPointerOverUIObject())
        {
            ARRaycastHit hit = _arRaycastHits[0];
            
            if (_arPlaneManager.GetPlane(hit.trackableId) != currentPlane) return;

            spawnObj.transform.position = hit.pose.position;
        }
    }
    
    private bool IsPointerOverUIObject() {
        // Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
        // the ray cast appears to require only eventData.position.
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
 
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    
    public void ChangeObjSize(float newSize)
    {
        spawnObj.transform.localScale = new Vector3(newSize, newSize, newSize);

        objSize = newSize;
    }
    
    public void ChangeObjRotation(float newRotation)
    {
        spawnObj.transform.rotation = Quaternion.Euler(0f, newRotation, 0f);
    }
    
    public void InstantiatePlayArea()
    {
        spawnObj.SetActive(false);
        
        finalArea = Instantiate(playArea, spawnObj.transform.position, spawnObj.transform.rotation);

        finalArea.transform.localScale = new Vector3(objSize, objSize, objSize);

        playAreaInstantiated = true;
    }
    
    private void DisablePlanePrefab()
    {
        foreach (ARPlane plane in _arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
    }
}
