using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum gameStates
{
    PLAYER_TURN,
    OPPONENT_TURN,
    GAME_OVER
}

public enum turnStates
{
    PLACEMENT,
    BATTLE
}

public class SelectionManager : MonoBehaviour
{

    [SerializeField] private string selectableTag = "Card";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;

    private Camera m_camera;

    private Transform hoverSelection;
    private Transform clickSelection;

    private gameStates gameState;
    private turnStates turnState;

    private void Start()
    {
        m_camera = Camera.main;

        gameState = gameStates.PLAYER_TURN;
        turnState = turnStates.PLACEMENT;
    }

    /*
    private void OnMouseDown()
    {
        Debug.Log("Mouse clicked!");

        if (clickSelection == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Transform selection = hit.transform;

                if (selection.CompareTag(selectableTag))
                {
                    Debug.Log("CLICK HIT");
                    selection.localScale = selection.localScale * 10;
                    clickSelection = selection;
                }
            }
            else
            {
                Debug.Log("CLICK MISS");
            }
        }
        else
        {
            clickSelection = null;
        }
    }
    */

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse clicked!");

            if (clickSelection == null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Transform selection = hit.transform;

                    if (selection.CompareTag(selectableTag))
                    {
                        Debug.Log("CLICK HIT");
                        selection.localScale = selection.localScale * 1.5f;
                        clickSelection = selection;
                    }
                }
                else
                {
                    Debug.Log("CLICK MISS");
                }
            }
            else
            {
                clickSelection.localScale = clickSelection.localScale / 1.5f;
                clickSelection = null;
            }
        }

        if (clickSelection == null)
        {
            if (hoverSelection != null)
            {
                MeshRenderer SelectionRenderer = hoverSelection.Find("Highlight").GetComponent<MeshRenderer>();
                SelectionRenderer.enabled = false;
                hoverSelection = null;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Transform selection = hit.transform;
                Debug.Log(selection.name);


                if (selection.CompareTag(selectableTag))
                {
                    Debug.Log("HOVER HIT");
                    MeshRenderer selectionRenderer = selection.Find("Highlight").GetComponent<MeshRenderer>();
                    if (selectionRenderer != null)
                    {
                        selectionRenderer.enabled = true;
                    }

                    hoverSelection = selection;
                }
            }
            else
            {
                Debug.Log("HOVER MISS");
            }
        }
        else
        {
            var lookAtPos = Input.mousePosition;
            lookAtPos.z = transform.position.z - m_camera.transform.position.z;
            lookAtPos = m_camera.ScreenToWorldPoint(lookAtPos);
            clickSelection.up = lookAtPos - transform.position;
        }
    }
}
