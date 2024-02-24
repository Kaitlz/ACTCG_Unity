using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum FishTurn
{
    NOT_TURNING,
    RIGHT_TURN,
    LEFT_TURN
}

public class FishTail : MonoBehaviour
{
    float currentPhase = 0;
    Material fishMat;
    MeshRenderer renderer;
    FishTurn turnState = FishTurn.NOT_TURNING;

    [SerializeField]
    float frequency = 3.5f;

    const float rightTurnPhase = 3 * Mathf.PI / 2;
    const float leftTurnPhase = Mathf.PI / 2;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        fishMat = renderer.material;
    }
    void Update()
    {
        if (turnState != FishTurn.NOT_TURNING)
        {
            if (turnState == FishTurn.RIGHT_TURN)
            {
                if (currentPhase + (Time.deltaTime * frequency) >= rightTurnPhase)
                {
                    currentPhase = rightTurnPhase;
                }
                else
                {
                    currentPhase += Time.deltaTime * frequency;
                }
            }
            if (turnState == FishTurn.LEFT_TURN)
            {
                if (currentPhase + (Time.deltaTime * frequency) >= leftTurnPhase)
                {
                    currentPhase = leftTurnPhase;
                }
                else
                {
                    currentPhase += Time.deltaTime * frequency;
                }
            }
        }
        else
        {
            currentPhase += Time.deltaTime * frequency;
        }

        if (currentPhase >= 2 * Mathf.PI)
        {
            currentPhase -= 2 * Mathf.PI;
        }

        fishMat.SetFloat("_CurrentPhase", currentPhase);
    }

    public void UpdateTurnState(int newTurnState)
    {
        turnState = (FishTurn) newTurnState;
    }
}
