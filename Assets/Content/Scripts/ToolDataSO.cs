using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "toolData", menuName = "Scriptable Objects/Tool Data", order = 3)]
public class ToolDataSO : CardDataSO
{
    public Texture2D ToolRender;
    public string Description;
}
