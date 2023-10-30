using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Page ", menuName = "Page Information")]
public class PageInformation: ScriptableObject
{
    public string pageLine;
    public GameObject pageModelScene;
    
    public bool hasMiniGame;
    
    public enum MiniGameType
    {
        None,
        HouseBuilding,
        PigEscape
    }

    public MiniGameType miniGameType;
    
    public enum Material
    {
        None,
        Straw,
        Wood,
        Brick
    }

    public Material material;
    
    public bool hasFinishedMiniGame;

    // public GameObject pigPrefab;
    // public GameObject wolfPrefab;
    //
    // public GameObject wallPrefab;
    //
    // public Material strawMaterial;
    // public Material woodMaterial;
    // public Material brickMaterial;
    //
    // public GameObject strawHousePrefab;
    // public GameObject woodHousePrefab;
    // public GameObject brickHousePrefab;
    //
    // public GameObject strawPrefab;
    // public GameObject woodPrefab;
    // public GameObject brickPrefab;
    //
    // public GameObject tablePrefab;
    // public GameObject bowlPrefab;
    // public GameObject potPrefab;
}
