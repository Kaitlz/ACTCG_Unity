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

public struct myTransform
{
    public Vector3 localPosition;
    public Vector3 worldPosition;
    public Quaternion rotation;
    public Vector3 scale;

    public Vector3 up;
}

public class SelectionManager : MonoBehaviour
{
    public LayerMask CardCollision;
    public LayerMask SelectionCollision;

    [SerializeField] private float offset = -50;

    [SerializeField] private string selectableTag = "Card";
    [SerializeField] private float lookAtIntensity = 2;
    [SerializeField] private float moveSpeed = 10f;

    private Camera m_camera;
    private Collider planeCollider;

    private Transform hoverSelection;
    private Transform clickSelection;

    private myTransform clickSelectionOriginal; 

    private gameStates gameState;
    private turnStates turnState;

    [SerializeField] private float cardOffsetTowardsCamera = 5f;

    private void Start()
    {
        m_camera = GameObject.Find("Key Cam").GetComponent<Camera>();
        planeCollider = GameObject.Find("SelectionCollision").GetComponent<BoxCollider>();

        gameState = gameStates.PLAYER_TURN;
        turnState = turnStates.PLACEMENT;

        // Avoid divide by 0 errors
        if (lookAtIntensity == 0)
        {
            lookAtIntensity = 1;
        }
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
        if (Input.GetMouseButtonDown(0)) // Clicking
        {
            if (clickSelection == null) // Starting click selection, not ending one
            {
                Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, CardCollision))
                {
                    Transform selection = hit.transform;

                    Debug.Log("Click selected: " + selection.gameObject.name);

                    if (selection.CompareTag(selectableTag))
                    {
                        //clickSelectionOriginal.localPosition = selection.position;
                        //clickSelectionOriginal.worldPosition = selection.TransformPoint(selection.position);
                        //clickSelectionOriginal.rotation = selection.rotation;
                        //clickSelectionOriginal.scale = selection.localScale;
                        //clickSelectionOriginal.up = selection.up;

                        hoverSelection = null;
                        clickSelection = selection;
                        clickSelection.localScale = clickSelection.localScale * 1.5f;
                    }
                }
            }
            else // Ending a click selection
            {
                // Disable selection highlight
                MeshRenderer SelectionRenderer = clickSelection.Find("Highlight").GetComponent<MeshRenderer>();
                SelectionRenderer.enabled = false;

                // Reset transform of selection
                clickSelection.position = clickSelectionOriginal.localPosition;
                clickSelection.rotation = clickSelectionOriginal.rotation;
                clickSelection.localScale = clickSelectionOriginal.scale;
                clickSelection = null;
                
                // Clear out clickSelectionOriginal
                clickSelectionOriginal.localPosition = Vector3.zero;
                clickSelectionOriginal.rotation = Quaternion.identity;
                clickSelectionOriginal.scale = Vector3.zero;
                clickSelectionOriginal.up = Vector3.zero;
            }
        }
        else if (clickSelection == null) // Not clicking and nothing is click selected
        {
            bool raycastSuccess = false;

            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, CardCollision))
            {
                Transform selection = hit.transform;
                
                if (selection.CompareTag(selectableTag))
                {
                    raycastSuccess = true;

                    if (hoverSelection != selection) // Hover selection was previously null or a different card
                    {
                        if (hoverSelection != null) // Reset whatever card was previously hovered
                        {
                            hoverSelection.position = clickSelectionOriginal.localPosition;
                            hoverSelection.rotation = clickSelectionOriginal.rotation;
                            hoverSelection.localScale = clickSelectionOriginal.scale;

                            MeshRenderer SelectionRenderer = hoverSelection.Find("Highlight").GetComponent<MeshRenderer>();
                            SelectionRenderer.enabled = false;
                        }

                        // Enable hover selection
                        hoverSelection = selection;

                        MeshRenderer selectionRenderer = selection.Find("Highlight").GetComponent<MeshRenderer>();
                        if (selectionRenderer != null)
                        {
                            clickSelectionOriginal.localPosition = selection.position;
                            clickSelectionOriginal.worldPosition = selection.TransformPoint(selection.position);
                            clickSelectionOriginal.rotation = selection.rotation;
                            clickSelectionOriginal.scale = selection.localScale;
                            clickSelectionOriginal.up = selection.up;

                            selectionRenderer.enabled = true;
                            selection.rotation = Quaternion.FromToRotation(Vector3.up, m_camera.transform.position - selection.position);
                            selection.rotation = Quaternion.Euler(selection.rotation.eulerAngles.x, m_camera.transform.rotation.eulerAngles.z, selection.rotation.eulerAngles.z);
                            selection.position = Vector3.MoveTowards(selection.position, m_camera.transform.position, cardOffsetTowardsCamera);
                        }
                    }
                }
            }

            if (!raycastSuccess) // Raycast missed or did not hit something selectable
            {
                // Disable hover selection
                if (hoverSelection != null)
                {
                    hoverSelection.position = clickSelectionOriginal.localPosition;
                    hoverSelection.rotation = clickSelectionOriginal.rotation;
                    hoverSelection.localScale = clickSelectionOriginal.scale;

                    MeshRenderer SelectionRenderer = hoverSelection.Find("Highlight").GetComponent<MeshRenderer>();
                    SelectionRenderer.enabled = false;
                    hoverSelection = null;

                    clickSelectionOriginal.localPosition = Vector3.zero;
                    clickSelectionOriginal.rotation = Quaternion.identity;
                    clickSelectionOriginal.scale = Vector3.zero;
                    clickSelectionOriginal.up = Vector3.zero;
                }
            }
        }
        else // Not clicking, but something IS click selected
        {
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, SelectionCollision))
            {
                if (hit.collider == planeCollider)
                {
                    // Make selected card follow mouse
                    clickSelection.position = Vector3.MoveTowards(clickSelection.position, hit.point, Time.deltaTime * moveSpeed);
                }
            }
        }
    }
}
