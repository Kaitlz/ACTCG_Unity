using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "fruitData", menuName = "Scriptable Objects/Fruit Data", order = 4)]
public class FruitDataSO : CardDataSO
{
    public string HappinessPoints;
    public Texture2D FruitRender;
}
