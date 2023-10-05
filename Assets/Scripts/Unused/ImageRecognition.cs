using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class ImageRecognition : MonoBehaviour
{
    // Allows the phone to track display an object on top of an image.
    
    private ARTrackedImageManager _arTrackedImageManager;
    
    public Renderer cubeRenderer;
    
    private void Awake()
    {
        _arTrackedImageManager = GetComponent<ARTrackedImageManager>();
    }
    
    public void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }
    
    public void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }
    
    // We can change the code here to make it do whatever we want
    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
        {
            // Insert code here
            
            cubeRenderer.material.color = Color.black;
            
            Debug.Log(trackedImage.name);
        }
    }
}
