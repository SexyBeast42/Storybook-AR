using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Allows the image to move around using touch input.
/// </summary>

public class ImageMovement : MonoBehaviour
{
    public LayerMask excludedLayer;
    
    void Update()
    {
        for (var i = 0; i < Input.touchCount; i++)
        {
            if (!IsPointerOverUIObject() && Input.GetTouch(i).phase == TouchPhase.Began)
            {
                // assign new position to where finger was pressed
                transform.position = new Vector3 (Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, transform.position.z);
            }
        }
    }
    
    private bool IsPointerOverUIObject() {
        // Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
        // the ray cast appears to require only eventData.position.
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
 
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        
        results.RemoveAll(result => (1 << result.gameObject.layer & excludedLayer) != 0); // Exclude ignoreraycast
        
        return results.Count > 0;
    }
}
