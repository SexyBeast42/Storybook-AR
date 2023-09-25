using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allow the player to take a photo and access their photo library. Editing the photo, then saving the photo to be used
/// for the AR minigames
/// </summary>

[Serializable]
public class ARPhotoID
{
    public string arID;
    public Texture2D photoID;
    public GameObject modelID;
    public bool isEdited;
}

public class CurrentEdit
{
    public Texture2D currentPhoto;
}

public class PhotoLogicManager : MonoBehaviour
{
    public List<ARPhotoID> arPhotoList = new List<ARPhotoID>();
    public List<GameObject> prefabList = new List<GameObject>();
    public CurrentEdit CurrentEdit;
    
    private bool isTakingPhoto;
    private bool isGettingPhotos;

    void Awake()
    {
        isTakingPhoto = false;
        isGettingPhotos = false;
    }

    void Update()
    {
        if (isTakingPhoto) return;
    }

    // Call this function before calling any other function first
    public async void RequestPermissionAsynchronously( NativeGallery.PermissionType permissionType, NativeGallery.MediaType mediaTypes )
    {
        NativeGallery.Permission permission = await NativeGallery.RequestPermissionAsync( permissionType, mediaTypes );
        Debug.Log( "Permission result: " + permission );
    }
    
    public void GetPhotosFromLibrary()
    {
        if (!NativeGallery.CanSelectMultipleFilesFromGallery()) return; // Check if device is able to select files
        
        int maxSize = 512;
        
        NativeGallery.Permission permission = NativeGallery.GetImagesFromGallery( ( paths ) =>
        {
            if (paths == null) return;

            if (paths.Length != arPhotoList.Count) return; // flag amount

            for (int i = 0; i < paths.Length; i++)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(paths[i], maxSize);
            
                if( texture == null ) return;
            
                CurrentEdit.currentPhoto = texture;
            }
        } );
    }

    IEnumerator GetPhotosFromLibraryCoroutine(int seconds)
    {
        isGettingPhotos = true;
        
        yield return new WaitForSeconds(seconds);
        
        GetPhotosFromLibrary();

        isGettingPhotos = false;
    }
    
    public void TakePhoto(string photoName)
    {
        if (isTakingPhoto) return;

        StartCoroutine(TakePhotoCoroutine(photoName));
    }

    IEnumerator TakePhotoCoroutine(string photoName)
    {
        isTakingPhoto = true;

        yield return new WaitForEndOfFrame();
        
        // Takes a photo
        Texture2D photo = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        photo.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        photo.Apply();
        
        // Save photo to Gallery/Photos
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(photo, "ARStorybook", photoName + ".png", ( success, path ) => Debug.Log( "Media save result: " + success + " " + path ));
        
        Destroy(photo); // Avoid memory leaks

        isTakingPhoto = false;
    }
}
