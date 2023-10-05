using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Manages the AR Image library, which allows the game to add, track, and display game objects onto images.
/// </summary>

[RequireComponent(typeof(PhotoLogicManager), typeof(ARTrackedImageManager))]
public class ARLibraryManager : MonoBehaviour
{
    public PhotoLogicManager PhotoLogicManager;
    public static ARTrackedImageManager TrackedImageManager;

    public List<Texture2D> textures;

    public TMP_Text statusText;
    
    void Start()
    {
        PhotoLogicManager = GetComponent<PhotoLogicManager>();
        TrackedImageManager = GetComponent<ARTrackedImageManager>();

        statusText.enabled = false;

        TrackedImageManager.requestedMaxNumberOfMovingImages = PhotoLogicManager.PhotoLibrary.Count;

        TrackedImageManager.CreateRuntimeLibrary();
    }


    public void StartLibrary()
    {
        statusText.enabled = true;
        statusText.text = "ARLibraryManager activated";
        
        foreach (var photoLibrary in PhotoLogicManager.PhotoLibrary)
        {
            statusText.text = "getting textures";
            textures.Add(photoLibrary.editedPhotoID);
        }

        statusText.text = "finish getting";
        
        RuntimeReferenceImageLibrary  runtimeLibrary = TrackedImageManager.CreateRuntimeLibrary();

        if (runtimeLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
        {
            mutableLibrary = TrackedImageManager.CreateRuntimeLibrary() as MutableRuntimeReferenceImageLibrary;

            statusText.text = "starting coroutine";
            StartCoroutine(AddAllImagesToMutableReferenceImageLibraryAR(mutableLibrary));
        }

        // statusText.text = runtimeLibrary.ToString();

        // MutableRuntimeReferenceImageLibrary mutableLibrary = runtimeLibrary as MutableRuntimeReferenceImageLibrary;

        // if (runtimeLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
        // {
        //     mutableLibrary = TrackedImageManager.CreateRuntimeLibrary() as MutableRuntimeReferenceImageLibrary;
        //
        //     // StartCoroutine(AddAllImagesToMutableReferenceImageLibraryAR(mutableLibrary));
        // }
    }

    private IEnumerator AddAllImagesToMutableReferenceImageLibraryAR(MutableRuntimeReferenceImageLibrary mutableLibrary)
    {
        statusText.text = "in coroutine";
        
        yield return null;

        statusText.text = "after null";

        // if (textures == null)
        // {
        //     statusText.text = "textures empty";
        //     yield break;
        // }

        //statusText.text = textures.Count.ToString();

        statusText.text = textures[1].isReadable + " " + textures[1].name;
        
        Texture2D texture2D = textures[1];

        
        statusText.text = texture2D.name;
        statusText.text = texture2D.isReadable + "";
        
        AddReferenceImageJobState job;


        if (texture2D.isReadable)
        {
            string name = "Stick";
            job = mutableLibrary.ScheduleAddImageWithValidationJob(texture2D, name, 1f);
            statusText.text = job.jobHandle.IsCompleted.ToString();
            
            yield return new WaitUntil(() => job.jobHandle.IsCompleted);
        
            statusText.text = mutableLibrary.count + " " + job.status;
        }
        

        
        
        // foreach (Texture2D texture in textures)
        // {
        //     statusText.text = "Testing for: " + texture.name;
        //
        //     if (texture)
        //     {
        //         statusText.text = texture.name + " is readable";
        //
        //         job = mutableLibrary.ScheduleAddImageWithValidationJob(texture, texture.name, 0.1f);
        //         
        //         statusText.text = texture.name + " is being added";
        //         
        //         yield return new WaitUntil(() => job.jobHandle.IsCompleted);
        //
        //         statusText.text = texture.name + " is finished";
        //     }
        // }

        if (mutableLibrary != null)
        {
            TrackedImageManager.referenceLibrary = mutableLibrary;
        }

        //TrackedImageManager.enabled = true;
    }
    
    // private IEnumerator AddAllImagesToMutableReferenceImageLibraryAR(MutableRuntimeReferenceImageLibrary mutableLibrary)
    // {
    //     yield return null;
    //
    //     for (int i = 0; i < PhotoLogicManager.PhotoLibrary.Count; i++)
    //     {
    //         var photoLibrary = PhotoLogicManager.PhotoLibrary[i];
    //         
    //         Texture2D texture2D = photoLibrary.editedPhotoID;
    //         statusText.text = "Testing for: " + photoLibrary.nameID;
    //
    //         AddReferenceImageJobState job;
    //
    //         if (!texture2D.isReadable)
    //         {
    //             statusText.text = "failed for: " + photoLibrary.nameID;
    //             yield break;
    //         }
    //         
    //         job = mutableLibrary.ScheduleAddImageWithValidationJob(texture2D, photoLibrary.nameID, 0.1f);
    //         yield return new WaitUntil(() => job.jobHandle.IsCompleted);
    //         statusText.text = "Complete for: " + photoLibrary.nameID;
    //
    //         TrackedImageManager.referenceLibrary = mutableLibrary;
    //     }
    //
    //
    //     
    //     // foreach (var photoLibrary in PhotoLogicManager.PhotoLibrary)
    //     // {
    //     //     Texture2D textureImg = photoLibrary.editedPhotoID;
    //     //     statusText.text = "Testing for: " + photoLibrary.nameID;
    //     //     
    //     //     AddReferenceImageJobState job;
    //     //
    //     //     if (!textureImg.isReadable)
    //     //     {
    //     //         statusText.text = "Error for: " + photoLibrary.nameID;
    //     //         
    //     //         yield return null;
    //     //     }
    //     //     
    //     //     job = mutableLibrary.ScheduleAddImageWithValidationJob(textureImg, photoLibrary.nameID, 1f);
    //     //     statusText.text = "Success for: " + photoLibrary.nameID;
    //     //     
    //     //     yield return new WaitUntil(() => job.jobHandle.IsCompleted);
    //     // }
    //     //
    //     // TrackedImageManager.referenceLibrary = mutableLibrary;
    // }
    
    void OnEnable()
    {
        TrackedImageManager.trackedImagesChanged += ImageManagerOnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        TrackedImageManager.trackedImagesChanged -= ImageManagerOnTrackedImagesChanged;
    }

    private void ImageManagerOnTrackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    {
        // added, spawn prefab
        foreach(ARTrackedImage image in obj.added)
        {
            // if (image.referenceImage.texture == PhotoLogicManager.PhotoLibrary[i])
            // {
            //     m_SpawnedOnePrefab = Instantiate(m_OnePrefab, image.transform.position, image.transform.rotation);
            //     m_OneNumberManager = m_SpawnedOnePrefab.GetComponent<NumberManager>();
            // }
            // else if (image.referenceImage.guid == s_SecondImageGUID)
            // {
            //     m_SpawnedTwoPrefab = Instantiate(m_TwoPrefab, image.transform.position, image.transform.rotation);
            //     m_TwoNumberManager = m_SpawnedTwoPrefab.GetComponent<NumberManager>();
            // }
        }
    }
}
