using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "villagerData", menuName = "Scriptable Objects/Villager Data", order = 1)]
public class VillagerDataSO : CardDataSO
{
    public Texture PersonalityIcon;
    public Texture StarSignIcon;
    public string DOB;
    public string HappinessPoints;
    public string Description;
}
