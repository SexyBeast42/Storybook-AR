using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PhotoLogicManager PhotoLogicManager;
    public ARLibraryManager ARLibraryManager;

    private int i = 0;

    void Start()
    {
        // PhotoLogicManager = FindObjectOfType<PhotoLogicManager>();
        // ARLibraryManager = FindObjectOfType<ARLibraryManager>();
    }

    void Update()
    {
        if (PhotoLogicManager.GetPhotoFinished() && i == 0)
        {
            i++;
            ARLibraryManager.StartLibrary();
        }
    }
}
