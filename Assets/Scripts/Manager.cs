using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Manager : MonoBehaviour
{

    private const string PictureDir = "pictures";
    private string fullPath;
    private bool TakenPhotos = false;
    private void Awake()
    {
        StorePicture();
        fullPath = Path.Combine(Application.persistentDataPath, PictureDir);
        if(!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }




    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(TakenPhotos)
        {
            LoadPicture
        }
    }


    void StorePicture()
    {
        Debug.Log(Application.persistentDataPath);
    }

    void TakePhoto()
    {
        StorePicture(photo, "brick");
    }

    void StorePicture(byte[] photo, string name)
    {
        using (var outStream = new FileStream(Path.Combine(fullPath, name), FileMode.CreateNew, FileAccess.Write))
        {

        }

    }

    byte[] LoadPicture(string name)
    {
        return new byte[2];
    }


}
