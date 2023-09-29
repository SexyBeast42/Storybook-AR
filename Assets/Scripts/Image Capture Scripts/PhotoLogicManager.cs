using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// Allow the player to take a photo and access their photo library. Editing the photo, then saving the photo to be used
/// for the AR minigames
/// </summary>

[Serializable]
public class ARPhotoID
{
    public string name;
    public Texture2D photoID;
    public Sprite editedPhotoID;
    public GameObject modelID;
}

public class CurrentEdit
{
    public int number, rotation;
}

public class PhotoLogicManager : MonoBehaviour
{
    public List<ARPhotoID> arPhotoList = new List<ARPhotoID>();
    public CurrentEdit CurrentEdit = new CurrentEdit();

    private bool isTakingPhoto;

    public GameObject takeUI, chooseUI, editUI, saveUI;

    [SerializeField] private TMP_Text chooseText;
    
    public Image imageDisplayed, boxOutline;
    private RectTransform imageDisplayedRect;
    public Slider sizeSlider;

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

        ToggleTakeUI(false);
        ToggleChooseUI(false);
        ToggleEditUI(false);
        
        imageDisplayedRect = imageDisplayed.GetComponent<RectTransform>();

        _state = State.Snapping;
    }
    
    void Update()
    {
        switch (_state)
        {
            case State.Snapping:
                if (!isTakingPhoto)
                {
                    ToggleTakeUI(true);
                }
                
                if (CurrentEdit.number == arPhotoList.Count())
                {
                    ToggleTakeUI(false);
                    
                    CurrentEdit.number = 0;
                    
                    _state = State.Choose;
                }
                
                break;
            
            case State.Choose:
                ToggleChooseUI(true);
                
                if (CurrentEdit.number == arPhotoList.Count())
                {
                    ToggleChooseUI(false);
                    
                    CurrentEdit.number = 0;
                    
                    _state = State.Editing;
                }
                
                chooseText.text = "Choose the drawing for the " + arPhotoList[CurrentEdit.number].name;
                
                break;
            
            case State.Editing:
                if (!isTakingPhoto)
                {
                    ToggleEditUI(true);
                }
                
                ChangePhotoSize(sizeSlider.value);
                
                DisplayPhoto();
                
                if (CurrentEdit.number == arPhotoList.Count())
                {
                    ToggleEditUI(false);
                    
                    _state = State.Finished;
                }
                
                break;
            
            case State.Finished:
                
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
            arPhotoList[CurrentEdit.number].name + ".png", 
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
            
            arPhotoList[CurrentEdit.number].photoID = texture;

            CurrentEdit.number++;
        } );
    }

    public void DisplayPhoto()
    {
        if (CurrentEdit.number == arPhotoList.Count) return;
        
        Texture2D currentTexture = arPhotoList[CurrentEdit.number].photoID;
        
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
        if (isTakingPhoto || CurrentEdit.number == arPhotoList.Count) return;

        isTakingPhoto = true;
        ToggleSaveUI(false);

        StartCoroutine(SavePhotoCoroutine());
    }
    
    IEnumerator SavePhotoCoroutine()
    {
        yield return new WaitForEndOfFrame();

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
            arPhotoList[CurrentEdit.number].name + "edited.png", 
            ( success, path ) => Debug.Log( "Media save result: " + success + " " + path ));
        
        Sprite currentSprite = Sprite.Create(photo,
            new Rect(0, 0, photo.width, photo.height), Vector2.one * 0.5f); // Convert to sprite

        arPhotoList[CurrentEdit.number].editedPhotoID = currentSprite;
        
        Destroy(photo); // Avoid memory leaks
        Destroy(currentSprite);

        CurrentEdit.number++;
        
        ToggleSaveUI(true);
        isTakingPhoto = false;
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
