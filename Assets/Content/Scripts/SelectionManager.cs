using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private string selectableTag = "Card";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;

    private Transform _selection;

    private void Update()
    {
        if (_selection != null)
        {
            MeshRenderer SelectionRenderer = _selection.GetComponent<MeshRenderer>();
            SelectionRenderer.material = defaultMaterial;
            _selection = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Transform selection = hit.transform;
            Debug.Log(selection.name);


            if (selection.CompareTag(selectableTag))
            {
                Debug.Log("HIT");
                MeshRenderer selectionRenderer = selection.GetComponent<MeshRenderer>();
                if (selectionRenderer != null)
                {
                    selectionRenderer.material = highlightMaterial;
                }

                _selection = selection;
            }
        }
        else
        {
            Debug.Log("MISS");
        }
    }
}
