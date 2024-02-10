using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataSO : ScriptableObject
{
    public Color LightColor;
    public Color MediumColor;
    public Color DarkColor;
    public Material MainImage;
    public Material BackgroundPattern;
    public string Name;
    public string TypeName;
    public Texture2D TypeIcon;
}