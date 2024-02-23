using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "specialNPCData", menuName = "Scriptable Objects/Special NPC Data", order = 2)]
public class SpecialNPCDataSO : CardDataSO
{
    public Texture StarSignIcon;
    public Texture GenderIcon;
    public string DOB;
    public string Description;
}
