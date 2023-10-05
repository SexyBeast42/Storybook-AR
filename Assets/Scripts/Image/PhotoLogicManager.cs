using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Allow the player to take a photo and access their photo library. Editing the photo, then saving the photo to be used
/// for the AR minigames.
///     Take photo
///     Choose photo
///     Edit photo
///     Save photo
/// </summary>

[Serializable]
public class PhotoLibrary
{
    public string nameID;
    public Texture2D photoID;
    public Texture2D editedPhotoID;
    public GameObject modelID;
}

public class CurrentEdit
{
    public int number, rotation;
}

public class PhotoLogicManager : MonoBehaviour
{
    public List<PhotoLibrary> PhotoLibrary = new List<PhotoLibrary>();
    public CurrentEdit CurrentEdit = new CurrentEdit();

    private bool isTakingPhoto;

    public GameObject takeUI, chooseUI, editUI, saveUI;

    [SerializeField] private TMP_Text chooseText;
    
    public Image imageDisplayed, boxOutline;
    private RectTransform imageDisplayedRect;
    public Slider sizeSlider;

    public bool isFinished;

    private enum State
    {
        Snapping,
        Choose,
        Editing,
        Finished
    };

    private State _state;
    
    void Awake()
    {
        isTakingPhoto = false;
        isFinished = false;

        ToggleTakeUI(false);
        ToggleChooseUI(false);
        ToggleEditUI(false);
        
        imageDisplayedRect = imageDisplayed.GetComponent<RectTransform>();

        _state = State.Choose;
    }
    
    void Update()
    {
        HandlePhotoStates();
    }

    private void HandlePhotoStates()
    {
        switch (_state)
        {
            case State.Snapping:
                if (!isTakingPhoto) ToggleTakeUI(true);
                
                
                if (CurrentEdit.number == PhotoLibrary.Count())
                {
                    ToggleTakeUI(false);
                    
                    CurrentEdit.number = 0;
                    
                    _state = State.Choose;
                }
                
                break;
            
            case State.Choose:
                ToggleChooseUI(true);
                
                if (CurrentEdit.number == PhotoLibrary.Count())
                {
                    ToggleChooseUI(false);
                    
                    CurrentEdit.number = 0;
                    
                    _state = State.Editing;
                }
                
                chooseText.text = "Choose the drawing for the " + PhotoLibrary[CurrentEdit.number].nameID;
                
                break;
            
            case State.Editing:
                if (!isTakingPhoto)
                {
                    ToggleEditUI(true);
                }
                
                ChangePhotoSize(sizeSlider.value);
                
                DisplayPhoto();
                
                if (CurrentEdit.number == PhotoLibrary.Count())
                {
                    ToggleEditUI(false);
                    
                    _state = State.Finished;
                }
                
                break;
            
            case State.Finished:
                isFinished = true;
                
                break;
        }
    }
    
    public void TakePhoto()
    {
        if (isTakingPhoto) return;
        
        isTakingPhoto = true;
        ToggleTakeUI(false);

        StartCoroutine(TakePhotoCoroutine());
    }

    IEnumerator TakePhotoCoroutine()
    {
        yield return new WaitForEndOfFrame();
        
        // Takes a photo
        Texture2D photo = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        photo.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        photo.Apply();
        
        // Save photo to Gallery/Photos
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(photo, 
            "ARStorybook", 
            PhotoLibrary[CurrentEdit.number].nameID + ".png", 
            ( success, path ) => Debug.Log( "Media save result: " + success + " " + path ));
        
        Destroy(photo); // Avoid memory leaks

        CurrentEdit.number++;
        
        ToggleTakeUI(true);
        isTakingPhoto = false;
    }
        
    public void GetPhotoFromLibrary()
    {
        int maxSize = 512;
        
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery( ( path ) =>
        {
            if (path == null) return;
            
            // Create Texture from selected image
            Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
            
            if (texture == null) return;
            
            PhotoLibrary[CurrentEdit.number].photoID = texture;

            CurrentEdit.number++;
        } );
    }

    public void DisplayPhoto()
    {
        if (CurrentEdit.number == PhotoLibrary.Count) return;
        
        Texture2D currentTexture = PhotoLibrary[CurrentEdit.number].photoID;
        
        Sprite currentSprite = Sprite.Create(currentTexture,
            new Rect(0, 0, currentTexture.width, currentTexture.height), Vector2.one * 0.5f); // Convert to sprite
        
        imageDisplayedRect.sizeDelta = new Vector2(currentTexture.width, currentTexture.height);
        
        imageDisplayed.sprite = currentSprite;
    }

    public void ChangePhotoSize(float newSize)
    {
        imageDisplayed.transform.localScale = new Vector3(newSize, newSize, newSize);
    }

    public void ChangePhotoRotation()
    {
        int rotationIncrement = 90;
        CurrentEdit.rotation += rotationIncrement;
        
        imageDisplayed.transform.rotation = Quaternion.Euler(0f, 0f, CurrentEdit.rotation);
    }

    public void SavePhoto()
    {
        if (isTakingPhoto || CurrentEdit.number == PhotoLibrary.Count) return;

        isTakingPhoto = true;
        ToggleSaveUI(false);

        StartCoroutine(SavePhotoCoroutine());
    }
    
    IEnumerator SavePhotoCoroutine()
    {
        yield return new WaitForEndOfFrame();

        string name = PhotoLibrary[CurrentEdit.number].nameID;

        int width = (int)boxOutline.sprite.rect.width;
        int height = (int)boxOutline.sprite.rect.height;
        
        // Takes a photo
        Texture2D photo = new Texture2D(width, height, TextureFormat.RGB24, false);

        Rect rect = new Rect(boxOutline.rectTransform.transform.position.x - width/2, boxOutline.rectTransform.transform.position.y - height/2, width, height);
        
        photo.ReadPixels(rect, 0, 0);
        photo.Apply();
        
        // Save photo to Gallery/Photos
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(photo, 
            "ARStorybook", 
            name + "edited.png", 
            ( success, path ) => Debug.Log( "Media save result: " + success + " " + path ));
        
        // Sprite currentSprite = Sprite.Create(photo,
        //     new Rect(0, 0, photo.width, photo.height), Vector2.one * 0.5f); // Convert to sprite

        PhotoLibrary[CurrentEdit.number].editedPhotoID = photo;

        // byte[] byteArray = photo.EncodeToPNG();
        // System.IO.File.WriteAllBytes(Application.dataPath + "/" + name + ".png", byteArray);
        
        Destroy(photo); // Avoid memory leaks

        CurrentEdit.number++;
        
        ToggleSaveUI(true);
        isTakingPhoto = false;
    }

    public bool GetPhotoFinished()
    {
        return isFinished;
    }
    
    private void ToggleTakeUI(bool uiState)
    {
        takeUI.SetActive(uiState);
    }

    private void ToggleChooseUI(bool uiState)
    {
        chooseUI.SetActive(uiState);
    }
    
    private void ToggleEditUI(bool uiState)
    {
        editUI.SetActive(uiState);
    }

    private void ToggleSaveUI(bool uiState)
    {
        saveUI.SetActive(uiState);
    }
    
}
