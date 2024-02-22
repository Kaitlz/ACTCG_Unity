using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FishTail : MonoBehaviour
{
    float currentPhase = 0;
    Material fishMat;
    MeshRenderer renderer;

    [SerializeField]
    float frequency = 3.5f;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        fishMat = renderer.material;
    }
    void Update()
    {
        currentPhase += Time.deltaTime * frequency;

        /*
        if (currentPhase >= 2 * Mathf.PI)
        {
            currentPhase -= Mathf.PI;
        }
        */

        fishMat.SetFloat("_CurrentPhase", currentPhase);
    }
}
