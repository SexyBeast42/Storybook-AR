using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Manages the AR Image library, which allows the game to add, track, and display game objects onto images.
/// </summary>

[RequireComponent(typeof(ARTrackedImageManager))]
public class ARLibraryManager : MonoBehaviour
{
    private static ARTrackedImageManager TrackedImageManager;
    public TextureList textureList;
    
    public List<Texture2D> textures = new List<Texture2D>();
    public List<GameObject> gameObjects = new List<GameObject>();
    public List<Guid> Guids = new List<Guid>();

    private List<String> imageLogs = new List<string>();
    public TMP_Text imagesStatusText, objectStatusText;
    
    void Start()
    {
        TrackedImageManager = GetComponent<ARTrackedImageManager>();
        textureList = FindObjectOfType<TextureList>();

        //TrackedImageManager.requestedMaxNumberOfMovingImages = textureList.Library.Count;
        
        imagesStatusText.enabled = true;
        objectStatusText.enabled = true;
        
        GetTextures();
        GetGUIDs();
        StartLibrary();
    }

    private void GetTextures()
    {
        foreach (var library in textureList.Library)
        {
            textures.Add(library.texture);
        }
    }
    
    private void GetGUIDs()
    {
        foreach (var library in textureList.Library)
        {
            Guids.Add(library.guid);
        }
    }


    private void StartLibrary()
    {
        TrackedImageManager.CreateRuntimeLibrary();
        
        RuntimeReferenceImageLibrary  runtimeLibrary = TrackedImageManager.CreateRuntimeLibrary();

        if (runtimeLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
        {
            mutableLibrary = TrackedImageManager.CreateRuntimeLibrary() as MutableRuntimeReferenceImageLibrary;
            
            StartCoroutine(AddAllImagesToMutableReferenceImageLibraryAR(mutableLibrary));
        }
    }

    private IEnumerator AddAllImagesToMutableReferenceImageLibraryAR(MutableRuntimeReferenceImageLibrary mutableLibrary)
    {
        yield return null;
        
        if (textures == null)
        {
            imagesStatusText.text = "textures empty";
            yield break;
        }
        
        AddReferenceImageJobState job;

        foreach (var library in textureList.Library)
        {
            string name = library.name;
            Texture2D texture = library.texture;

            if (!texture.isReadable)
            {
                imageLogs.Add(name + " isnt readable");
                break;
            }

            job = mutableLibrary.ScheduleAddImageWithValidationJob(texture, name, 1f);
            
            yield return new WaitUntil(() => job.jobHandle.IsCompleted);
            
            imageLogs.Add(name + " is " + job.status);
        }
        
        if (mutableLibrary != null)
        {
            TrackedImageManager.referenceLibrary = mutableLibrary;
        }

        string statuslog = "";

        foreach (var log in imageLogs)
        {
            statuslog += log + "\n";
        }
        
        imagesStatusText.text = statuslog + 
                          "\n" + TrackedImageManager.name + " count: " + mutableLibrary.count + ". lib count: " + textures.Count;
    }
    
    void OnEnable()
    {
        string currentText = objectStatusText.text;
        
        objectStatusText.text = currentText + "\n enabled";
        
        TrackedImageManager.trackedImagesChanged += ImageManagerOnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        string currentText = objectStatusText.text;
        
        objectStatusText.text = currentText + "\n disabled";
        
        TrackedImageManager.trackedImagesChanged -= ImageManagerOnTrackedImagesChanged;
    }

    private void ImageManagerOnTrackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    {
        // added, spawn prefab
        foreach(ARTrackedImage image in obj.added)
        {
            for (int i = 0; i < Guids.Count; i++)
            {
                if (image.referenceImage.guid == Guids[i])
                {
                    objectStatusText.text = "added: " + image.name;
                    
                    GameObject prefab = textureList.Library[i].gameObject;
                    gameObjects[i] = Instantiate(prefab, image.transform.position, image.transform.rotation);
            
                    break;
                }
            }
        }
        
        // updated, set prefab position and rotation
        foreach(ARTrackedImage image in obj.updated)
        {
            // image is no longer tracking, disable visuals TrackingState.Limited TrackingState.None
            if (image.trackingState != TrackingState.Tracking)
            {
                for (int i = 0; i < Guids.Count; i++)
                {
                    if (image.referenceImage.guid == Guids[i])
                    {
                        gameObjects[i].SetActive(false);
            
                        break;
                    }
                }
            }
            
            // image is tracking or tracking with limited state, show visuals and update it's position and rotation
            for (int i = 0; i < Guids.Count; i++)
            {
                if (image.referenceImage.guid == Guids[i])
                {
                    objectStatusText.text = "updated: " + image.name;
                    
                    gameObjects[i].SetActive(true);
                    gameObjects[i].transform.SetPositionAndRotation(image.transform.position, image.transform.rotation);
            
                    break;
                }
            }
        }
        
        // removed, destroy spawned instance
        foreach(ARTrackedImage image in obj.removed)
        {
            for (int i = 0; i < Guids.Count; i++)
            {
                if (image.referenceImage.guid == Guids[i])
                {
                    objectStatusText.text = "removed: " + image.name;
                    
                    Destroy(gameObjects[i]);
            
                    break;
                }
            }
        }
    }
}
