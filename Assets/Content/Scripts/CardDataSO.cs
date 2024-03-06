using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataSO : ScriptableObject
{
    public Color LightColor;
    public Color MediumColor;
    public Color DarkColor;
    public Material MainImage;
    public Material BackgroundMaterial;
    public Texture2D BackgroundPattern;
    public string Name;
    public string TypeName;
    public Texture2D TypeIcon;
    public Texture2D CardBackface;
}