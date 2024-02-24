using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    FishTail tail;

    void Start()
    {
        tail = GameObject.Find("Fish").GetComponent<FishTail>();
    }

    public void UpdateTurnState(int newTurnState)
    {
        if (tail)
        {
            tail.UpdateTurnState(newTurnState);
        }
    }
}
