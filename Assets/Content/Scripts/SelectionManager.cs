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
    [SerializeField] private float moveSpeed = 5f;

    private Camera m_camera;
    private Collider planeCollider;

    private Transform hoverSelection;
    private Transform clickSelection;

    private myTransform clickSelectionOriginal;

    private Board board;

    private gameStates gameState;
    private turnStates turnState;

    private Vector3 lastPosition = Vector3.zero;

    [SerializeField] private float cardOffsetTowardsCamera = 5f;
    [SerializeField] private float cardRotationIntensity = 2f;
    [SerializeField] private float cardMaxRotation = 10f;
    [SerializeField] private float cardRotationDamping = 5f;

    private void Awake()
    {
        m_camera = GameObject.Find("Key Cam").GetComponent<Camera>();
        planeCollider = GameObject.Find("SelectionCollision").GetComponent<BoxCollider>();
    }

    private void Start()
    {
        if (gameObject.GetComponent<CardManager>())
        {
            board = gameObject.GetComponent<CardManager>().board;
        }

        gameState = gameStates.PLAYER_TURN;
        turnState = turnStates.PLACEMENT;

        // Avoid divide by 0 errors
        if (lookAtIntensity == 0)
        {
            lookAtIntensity = 1;
        }
    }

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

                    if (selection.CompareTag(selectableTag))
                    {
                        if (board.isPlayerCard(selection.gameObject))
                        {
                            if (clickSelectionOriginal.scale == Vector3.zero)
                            {
                                clickSelectionOriginal.localPosition = selection.position;
                                clickSelectionOriginal.worldPosition = selection.TransformPoint(selection.position);
                                clickSelectionOriginal.rotation = selection.rotation;
                                clickSelectionOriginal.scale = selection.localScale;
                                clickSelectionOriginal.up = selection.up;
                            }

                            hoverSelection = null;
                            clickSelection = selection;
                            clickSelection.rotation = Quaternion.identity;
                            clickSelection.localScale = clickSelection.localScale * 1.5f;
                        }
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

                lastPosition = Vector3.zero;
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
                    if (board.isPlayerCard(selection.gameObject))
                    {
                        raycastSuccess = true;

                        if (hoverSelection != selection || clickSelectionOriginal.scale == Vector3.zero) // Hover selection was previously null or a different card
                        {
                            if (hoverSelection != null && clickSelectionOriginal.scale != Vector3.zero) // Reset whatever card was previously hovered
                            {
                                hoverSelection.position = clickSelectionOriginal.localPosition;
                                hoverSelection.rotation = clickSelectionOriginal.rotation;
                                hoverSelection.localScale = clickSelectionOriginal.scale;

                                MeshRenderer SelectionRenderer = hoverSelection.Find("Highlight").GetComponent<MeshRenderer>();
                                SelectionRenderer.enabled = false;
                            }

                            // Enable hover selection
                            hoverSelection = selection;

                            clickSelectionOriginal.localPosition = selection.position;
                            clickSelectionOriginal.worldPosition = selection.TransformPoint(selection.position);
                            clickSelectionOriginal.rotation = selection.rotation;
                            clickSelectionOriginal.scale = selection.localScale;
                            clickSelectionOriginal.up = selection.up;

                            selection.rotation = Quaternion.FromToRotation(Vector3.up, m_camera.transform.position - selection.position);
                            selection.rotation = Quaternion.Euler(selection.rotation.eulerAngles.x, m_camera.transform.rotation.eulerAngles.z, selection.rotation.eulerAngles.z);
                            selection.position = Vector3.MoveTowards(selection.position, m_camera.transform.position, cardOffsetTowardsCamera);

                            MeshRenderer selectionRenderer = selection.Find("Highlight").GetComponent<MeshRenderer>();
                            if (selectionRenderer != null)
                            {
                                selectionRenderer.enabled = true;
                            }
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
                }

                clickSelectionOriginal.localPosition = Vector3.zero;
                clickSelectionOriginal.rotation = Quaternion.identity;
                clickSelectionOriginal.scale = Vector3.zero;
                clickSelectionOriginal.up = Vector3.zero;
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

                    Vector3 velocity = (clickSelection.position - lastPosition) / Time.deltaTime;
                    Vector3 adjustedVelocity = Vector3.ClampMagnitude(new Vector3(velocity.z, 0, -velocity.x), cardMaxRotation);

                    Quaternion desiredRotation = Quaternion.Euler(adjustedVelocity * cardRotationIntensity);
                    clickSelection.rotation = Quaternion.Slerp(clickSelection.rotation, desiredRotation, Time.deltaTime * cardRotationDamping);

                    lastPosition = clickSelection.position;
                }
            }
        }
    }
}
