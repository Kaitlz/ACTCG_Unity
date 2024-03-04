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

    private void Start()
    {
        //m_camera = Camera.main;
        m_camera = GameObject.Find("Key Cam").GetComponent<Camera>();
        Debug.Log("CAMERA POS: " + m_camera.transform.position + " : " + m_camera.transform.TransformPoint(m_camera.transform.position));

        planeCollider = GameObject.Find("Plane").GetComponent<BoxCollider>();

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
        if (Input.GetMouseButtonDown(0))
        {
            if (clickSelection == null)
            {
                Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, CardCollision))
                {
                    Transform selection = hit.transform;

                    Debug.Log(selection.gameObject.name);

                    if (selection.CompareTag(selectableTag))
                    {
                        clickSelectionOriginal.localPosition = selection.position;
                        clickSelectionOriginal.worldPosition = selection.TransformPoint(selection.position);
                        clickSelectionOriginal.rotation = selection.rotation;
                        clickSelectionOriginal.scale = selection.localScale;
                        clickSelectionOriginal.up = selection.up;

                        clickSelection = selection;
                        clickSelection.localScale = clickSelection.localScale * 1.5f;
                    }
                }
            }
            else
            {
                clickSelection.position = clickSelectionOriginal.localPosition;
                clickSelection.rotation = clickSelectionOriginal.rotation;
                clickSelection.localScale = clickSelectionOriginal.scale;
                
                clickSelection = null;
                clickSelectionOriginal.localPosition = Vector3.zero;
                clickSelectionOriginal.rotation = Quaternion.identity;
                clickSelectionOriginal.scale = Vector3.zero;
                clickSelectionOriginal.up = Vector3.zero;
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

            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, CardCollision))
            {
                Transform selection = hit.transform;
                
                if (selection.CompareTag(selectableTag))
                {
                    MeshRenderer selectionRenderer = selection.Find("Highlight").GetComponent<MeshRenderer>();
                    if (selectionRenderer != null)
                    {
                        selectionRenderer.enabled = true;
                    }

                    hoverSelection = selection;
                }
            }
        }
        else
        {
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, SelectionCollision))
            {
                if (hit.collider == planeCollider)
                {
                    clickSelection.position = Vector3.MoveTowards(clickSelection.position, hit.point, Time.deltaTime * moveSpeed);
                    //clickSelection.position = m_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, offset));
                }
            }


                //Vector3 pos = Input.mousePosition;
                //pos.z = 5f;
            //clickSelection.position = m_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, offset));
            //Debug.Log(clickSelection.position + " : " + clickSelection.TransformPoint(clickSelection.position));

            //Vector3 clickSelectionWorldPos = clickSelection.TransformPoint(clickSelection.position);
            //Vector3 cameraWorldPos = m_camera.transform.TransformPoint(m_camera.transform.position);

            //Vector3 lookAtPos = Input.mousePosition;
            //lookAtPos.x = clickSelection.position.x - (m_camera.transform.position.x / lookAtIntensity);
            //lookAtPos.z = clickSelection.position.z - (m_camera.transform.position.z / lookAtIntensity);
            //lookAtPos = m_camera.ScreenToWorldPoint(lookAtPos);
            
            //float angle = Vector3.Angle(lookAtPos - clickSelection.position, clickSelectionOriginal.up);
            //clickSelection.up = lookAtPos - clickSelection.position;
            //clickSelection.up = Quaternion.AngleAxis(angle, Vector3.right) * (lookAtPos - clickSelection.position);
            //clickSelection.up = Quaternion.AngleAxis(angle, Vector3.Cross(lookAtPos - clickSelection.position, clickSelectionOriginal.up)) * (lookAtPos - clickSelection.position);

            //Vector3 blah = Input.mousePosition;
            //blah.z = clickSelectionWorldPos.z - (cameraWorldPos.z / lookAtIntensity);
            //blah = m_camera.ScreenToWorldPoint(blah);
            
            //Vector3 goToPos = new Vector3(lookAtPos.x, clickSelectionOriginal.worldPosition.y, lookAtPos.z);
            //clickSelection.position = goToPos;

            //Debug.Log(angle);
            //Debug.DrawLine(lookAtPos, clickSelection.position, Color.red);
        }
    }
}
