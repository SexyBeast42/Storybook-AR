using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Library
{
    public string name;
    public Texture2D texture;
    public GameObject gameObject;
    public Guid guid;
}

public class TextureList : MonoBehaviour
{
    public List<Library> Library = new List<Library>();
}
