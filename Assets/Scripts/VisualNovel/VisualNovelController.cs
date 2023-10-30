using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class VisualNovelController : MonoBehaviour
{
    public List<PageInformation> pages = new List<PageInformation>();
    
    public GameObject playArea;
    private Renderer playAreaRenderer;
    public TMP_Text visualNovelText;

    private int currentPageNumber = 0;
    private GameObject displayedModel;

    void Start()
    {
        playAreaRenderer = playArea.GetComponent<Renderer>();
    }

    void Update()
    {
        DisplayText();
        DisplayModel();
        CheckForMiniGame();
    }
    
    private void DisplayText()
    {
        visualNovelText.text = pages[currentPageNumber].pageLine;
    }

    private void DisplayModel()
    {
        if (displayedModel != null)
        {
            Destroy(displayedModel);
        }
        
        if (pages[currentPageNumber].hasMiniGame) return;

        displayedModel = Instantiate(pages[currentPageNumber].pageModelScene, playAreaRenderer.bounds.center, playArea.transform.rotation);
    }

    private void CheckForMiniGame()
    {
        if (!pages[currentPageNumber].hasMiniGame) return;
        
        // do stuff
        
    }

    public void NextPage()
    {
        if (currentPageNumber == pages.Count - 1) return;
        
        currentPageNumber++;
    }

    public void LastPage()
    {
        if (currentPageNumber == 0) return;

        currentPageNumber--;
    }
}
