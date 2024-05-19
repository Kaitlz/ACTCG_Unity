using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

public class Triangle
{
    public Vector3 A;
    public Vector3 B;
    public Vector3 C;

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        A = a;
        B = b;
        C = c;    
    }
}

public class SelectionManager : MonoBehaviour
{
    // TODO: remove after transferring functionality out of cardspawner
    CardSpawner spawner;

    public LayerMask CardCollision;
    public LayerMask SelectionCollision;

    [SerializeField] private float offset = -50;

    [SerializeField] private string selectableTag = "Card";
    [SerializeField] private float lookAtIntensity = 2;
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private Vector2 h1 = new Vector2(0, 0);
    [SerializeField] private Vector2 h2 = new Vector2(0, 1);
    [SerializeField] private Vector2 h3 = new Vector2(1, 1);
    [SerializeField] private Vector2 h4 = new Vector2(1, 0);
    [SerializeField] private float hDist = 30;

    private Vector3 c1, c2, c3, c4;

    [SerializeField] private Camera m_camera;
    private Collider planeCollider;
    
    private Transform hoverSelection;
    private Transform clickSelection;

    private myTransform clickSelectionOriginal;

    private Board board;

    private gameStates gameState;
    private turnStates turnState;

    private Vector3 lastPosition = Vector3.zero;

    [SerializeField] private float cardOffsetTowardsCamera = 5f;
    [SerializeField] private float handHoverOffset = 2.5f;
    [SerializeField] private float cardRotationIntensity = 2f;
    [SerializeField] private float cardMaxRotation = 10f;
    [SerializeField] private float cardRotationDamping = 5f;

    // ---

    [Header("Gameplay Settings")]

    [Tooltip("Whether cards can be used or not. Still allows selection & reordering. e.g. Could be used to disable using cards when it's not the player's turn.")]
    public bool canUseCards = true;
    [Tooltip("Whether cards can interact with the mouse. Cards also cannot be used. (If a card is held, it will be returned to the hand)")]
    public bool canSelectCards = true;

    [Header("Settings")]

    [SerializeField]
    [Tooltip("Force cards to face upwards when selected, for easier readability.")]
    private bool cardUprightWhenSelected = true;
    
    [SerializeField]
    [Tooltip("Allow cards to tilt when not in hand, based on the velocity of mouse movement.")]
    private bool cardTilt = true;

    [SerializeField]
    [Range(0, 5)]
    [Tooltip("Controls the strength of the spacing given to cards nearby to the selected card.")]
    private float selectionSpacing = 1;

    private bool updateHierarchyOrder = false;

    [SerializeField]
    [Tooltip("Controls the curve that the hand uses.")]
    private Vector3 curveStart = new Vector3(4.5f, 13.5f, 40), curveEnd = new Vector3(-12, 13.5f, 40);

    // TODO: think about how I want to rework this
        // need to be able to specify a plane in 3d space. probably directly aligned with the camera? plane must have bounds, not endless
        // could maybe use curveStart/End as 2 points on the plane. then maybe specify another point or line to define the plane?
        // or maybe the vector from curveStart to curveEnd could be an axis to rotate a rectangle around?
        // or could just expect a plane child obj?? but that's not guaranteed to face the camera? I suppose I could use LookAt()?
            // we might not WANT it aligned with the camera... we might need it aligned to the camera's view frustum or far plane or smth
    [SerializeField]
    [Tooltip("Controls the area which is considered 'in-hand', allowing cards to be selected/reordered. " +
        "If a card leaves this area it can be used upon releasing the mouse button. " +
        "Recommend having the hand bounds go past the screen edges to prevent accidental use when reordering cards quickly")]
    private Vector2 handOffset = new Vector2(0, -0.3f), handSize = new Vector2(9, 1.7f);


    private Plane plane; // world XY plane, used for mouse position raycasts
    private Vector3 a, b, c; // Used for shaping hand into curve

    // TODO: store a reference to the board! the board should also use lists instead of arrays
    // the 'add' function should work for any card group -- player hand, enemy deck, etc.
    private List<Card> hand; // Cards currently in hand

    private int selected = -1;  // Card index that is nearest to mouse
    private int dragged = -1;   // Card index that is held by mouse (inside of hand)
    private Card heldCard;      // Card that is held by mouse (when outside of hand)
    private Vector3 heldCardOffset;
    private Vector2 heldCardTilt;
    private Vector2 force;
    private Vector3 mouseWorldPos;
    private Vector2 prevMousePos;
    private Vector2 mousePosDelta;

    private Rect handBounds;
    private bool mouseInsideHand;

    private bool showDebugGizmos = true;

    // ---

    private void Awake()
    {
        m_camera = GameObject.Find("Key Cam").GetComponent<Camera>();
        planeCollider = GameObject.Find("SelectionCollision").GetComponent<BoxCollider>();
        spawner = gameObject.GetComponent<CardSpawner>(); // TODO: remove later, see top comment
    }

    //private void Start()
    public void BasicallyStart() // TODO: DEFINITELY CHANGE LATER
    {
        board = gameObject.GetComponent<CardManager>().board;

        // TODO: add shit

        // TODO: removing transform.TransformPoint() allows curveStart/curveEnd to refer to WORLD SPACE points
        a = curveStart; // transform.TransformPoint(curveStart);
        b = transform.position;
        c = curveEnd; // transform.TransformPoint(curveEnd);
        handBounds = new Rect((handOffset - handSize / 2), handSize);
        plane = new Plane(-Vector3.forward, transform.position);
        prevMousePos = Input.mousePosition;

        c1 = m_camera.ViewportToWorldPoint(new Vector3(h1.x, h1.y, hDist));
        c2 = m_camera.ViewportToWorldPoint(new Vector3(h2.x, h2.y, hDist));
        c3 = m_camera.ViewportToWorldPoint(new Vector3(h3.x, h3.y, hDist));
        c4 = m_camera.ViewportToWorldPoint(new Vector3(h4.x, h4.y, hDist));

        // TODO: I think this is expecting all the cards to ALREADY be a child of this component?
        // instead, we need an exposed list the user can add to
        // OR maybe we KEEP the CardSpawner component for this purpose, and rip off the children of THAT component? this seems right.
        int count = board.player.hand.Length; // TODO: remove later, this hand list is what will be added to the board so we actually need the number of CardSOs the user specifies
        hand = new List<Card>(count);
        for (int i = 0; i < count; i++)
        {
            hand.Add(board.player.hand[i]); // TODO: will change later as well
        }

        /*
        // Add transform children to hand
        int count = transform.childCount;
        hand = new List<Card>(count);
        for (int i = 0; i < count; i++)
        {
            Transform cardTransform = transform.GetChild(i);
            Card card = cardTransform.GetComponent<Card>();
            hand.Add(card);
        }
        */

        // ----------------------------//

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

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(curveStart, 0.1f);
        //Gizmos.DrawSphere(Vector3.zero, 0.03f);
        Gizmos.DrawSphere(curveEnd, 0.1f);

        Vector3 p1 = curveStart;
        for (int i = 0; i < 20; i++)
        {
            float t = (i + 1) / 20f;
            Vector3 p2 = GetCurvePoint(curveStart, Vector3.zero, curveEnd, t);
            Gizmos.DrawLine(p1, p2);
            p1 = p2;
        }

        if (mouseInsideHand)
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireCube(handOffset, handSize);

        // -- //

        Gizmos.color = Color.yellow;
        
        Vector3 d1, d2, d3, d4;
        d1 = m_camera.ViewportToWorldPoint(new Vector3(0, 0, hDist));
        d2 = m_camera.ViewportToWorldPoint(new Vector3(0, 1, hDist));
        d3 = m_camera.ViewportToWorldPoint(new Vector3(1, 1, hDist));
        d4 = m_camera.ViewportToWorldPoint(new Vector3(1, 0, hDist));

        //Vector3 viewportPoint = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, hDist);
        Vector3 viewportPoint = new Vector3(Input.mousePosition.x / m_camera.pixelWidth, Input.mousePosition.y / m_camera.pixelHeight, hDist);
        //Vector3 viewportPoint = new Vector3((Input.mousePosition.x - Screen.width) / (Screen.width / 2), (Input.mousePosition.y - Screen.height) / (Screen.height / 2), hDist);
        //Debug.Log("(" + Input.mousePosition.x + " / " + Screen.width + "), (" + Input.mousePosition.y + " / " + Screen.height + ")  --  " + viewportPoint);
        Vector3 point = m_camera.ViewportToWorldPoint(viewportPoint);

        Gizmos.DrawLine(m_camera.transform.position, point);

        Gizmos.DrawLine(d1, d2);
        Gizmos.DrawLine(d2, d3);
        Gizmos.DrawLine(d3, d4);
        Gizmos.DrawLine(d4, d1);

        Gizmos.color = mouseInsideHand ? Color.green : Color.red;

        c1 = m_camera.ViewportToWorldPoint(new Vector3(h1.x, h1.y, hDist));
        c2 = m_camera.ViewportToWorldPoint(new Vector3(h2.x, h2.y, hDist));
        c3 = m_camera.ViewportToWorldPoint(new Vector3(h3.x, h3.y, hDist));
        c4 = m_camera.ViewportToWorldPoint(new Vector3(h4.x, h4.y, hDist));
        
        Gizmos.DrawLine(c1, c2);
        Gizmos.DrawLine(c2, c3);
        Gizmos.DrawLine(c3, c4);
        Gizmos.DrawLine(c4, c1);

        Gizmos.DrawSphere(point, 1f);
    }

    private bool pointInTriangle(Vector3 p, Triangle tri)
    {
        Vector3 a = tri.A;
        Vector3 b = tri.B;
        Vector3 c = tri.C;

        a -= p;
        b -= p;
        c -= p;

        Vector3 u = Vector3.Cross(b, c);
        Vector3 v = Vector3.Cross(c, a);
        Vector3 w = Vector3.Cross(a, b);

        if (Vector3.Dot(u, v) < 0f)
        {
            return false;
        }

        if (Vector3.Dot(u, w) < 0f)
        {
            return false;
        }

        return true;
    }
        
    private void Update()
    {
        
        // --------------------------------------------------------
        // HANDLE MOUSE & RAYCAST POSITION
        // --------------------------------------------------------

        Vector2 mousePos = Input.mousePosition;

        // Allows mouse to go outside game window but keeps cards within window
        // If mouse doesn't need to go outside, could use "Cursor.lockState = CursorLockMode.Confined;" instead
        mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);

        // Mouse movement velocity
        if (cardTilt)
        {
            mousePosDelta = (mousePos - prevMousePos) * new Vector2(1600f / Screen.width, 900f / Screen.height) * Time.deltaTime;
            prevMousePos = mousePos;

            float tiltStrength = 3f;
            float tiltDrag = 3f;
            float tiltSpeed = 50f;

            force += (mousePosDelta * tiltStrength - heldCardTilt) * Time.deltaTime;
            force *= 1 - tiltDrag * Time.deltaTime;
            heldCardTilt += force * Time.deltaTime * tiltSpeed;
            // these calculations probably aren't correct, but hey, they work...? :P

            if (showDebugGizmos)
            {
                Debug.DrawRay(mouseWorldPos, mousePosDelta, Color.red);
                Debug.DrawRay(mouseWorldPos, heldCardTilt, Color.cyan);
            }
        }

        // Get world position from mouse
        Ray ray = m_camera.ScreenPointToRay(mousePos);
        if (plane.Raycast(ray, out float enter))
        {
            mouseWorldPos = ray.GetPoint(enter);
        }

        // Get distance to current selected card (for comparing against other cards later, to find closest)
        int count = hand.Count; //transform.childCount;
        float sqrDistance = 1000;
        if (selected >= 0 && selected < count)
        {
            float t = (selected + 0.5f) / count;
            Vector3 p = GetCurvePoint(a, b, c, t);
            sqrDistance = (p - mouseWorldPos).sqrMagnitude;
        }

        // Check if mouse is inside hand bounds
        Triangle bounds1 = new Triangle(c1, c2, c3);
        Triangle bounds2 = new Triangle(c1, c3, c4);

        //Vector3 viewportPoint = new Vector3((Input.mousePosition.x / Screen.width), (Input.mousePosition.y / Screen.height), hDist);
        Vector3 viewportPoint = new Vector3(Input.mousePosition.x / m_camera.pixelWidth, Input.mousePosition.y / m_camera.pixelHeight, hDist);
        Debug.Log("(" + Input.mousePosition.x + " / " + m_camera.pixelWidth + "), (" + Input.mousePosition.y + " / " + m_camera.pixelHeight + ")  --  " + viewportPoint);
        Vector3 point = m_camera.ViewportToWorldPoint(viewportPoint);

        mouseInsideHand = pointInTriangle(point, bounds1) || pointInTriangle(point, bounds2);

        //Vector3 point = transform.InverseTransformPoint(mouseWorldPos);
        //mouseInsideHand = handBounds.Contains(point);


        bool mouseButton = Input.GetMouseButton(0);

        // --------------------------------------------------------
        // HANDLE CARDS IN HAND
        // --------------------------------------------------------

        for (int i = 0; i < count; i++)
        {
            Card card = hand[i];
            Transform cardTransform = card.cardObj.transform; // TODO: change later with something closer to the line below
            //Transform cardTransform = card.transform;

            bool noCardHeld = (heldCard == null); // Whether a card is "held" (outside of hand)
            bool onSelectedCard = (noCardHeld && selected == i);
            bool onDraggedCard = (noCardHeld && dragged == i);

            // Get Position along Curve (for card positioning)
            float selectOffset = 0;
            if (noCardHeld)
            {
                selectOffset = 0.02f * Mathf.Clamp01(1 - Mathf.Abs(Mathf.Abs(i - selected) - 1) / (float)count * 3) * Mathf.Sign(i - selected);
            }
            float t = (i + 0.5f) / count + selectOffset * selectionSpacing;
            Vector3 p = GetCurvePoint(a, b, c, t);

            float d = (p - mouseWorldPos).sqrMagnitude;
            bool mouseCloseToCard = d < 0.5f;
            bool mouseHoveringOnSelected = onSelectedCard && mouseCloseToCard && mouseInsideHand; //  && mouseInsideHand

            // Handle Card Position & Rotation
            //Vector3 cardUp = p - (transform.position + Vector3.down * 7);
            Vector3 cardUp = GetCurveNormal(a, b, c, t);
            Vector3 cardPos = p + (mouseHoveringOnSelected ? cardTransform.up * 0.3f : Vector3.zero);
            Vector3 cardForward = Vector3.forward;

            /* Card Tilt is disabled when in hand as they can clip through eachother :(
            if (cardTilt && onSelectedCard && mouseButton) {
                cardForward -= new Vector3(heldCardOffset.x, heldCardOffset.y, 0);
            }*/

            // Sorting Order
            if (mouseHoveringOnSelected || onDraggedCard)
            {
                // When selected bring card to front
                if (cardUprightWhenSelected) cardUp = Vector3.up;
                cardPos.z = transform.position.z - 0.2f;
            }
            else
            {
                cardPos.z = transform.position.z + t * 0.5f; // TODO: cards currently only move towards the z pos of THIS gameobject
            }

            // Rotation
            cardTransform.rotation = Quaternion.RotateTowards(cardTransform.rotation, Quaternion.LookRotation(cardForward, cardUp), 80f * Time.deltaTime);

            // Handle Start Dragging
            if (mouseHoveringOnSelected)
            {
                bool mouseButtonDown = Input.GetMouseButtonDown(0);
                if (mouseButtonDown)
                {
                    dragged = i;
                    heldCardOffset = cardTransform.position - mouseWorldPos;
                    heldCardOffset.z = -0.1f;
                }
            }

            // Handle Card Position
            if (onDraggedCard && mouseButton)
            {
                // Held by mouse / dragging
                cardPos = mouseWorldPos + heldCardOffset;
                cardTransform.position = cardPos;
            }
            else
            {
                cardPos = Vector3.MoveTowards(cardTransform.position, cardPos, 6f * Time.deltaTime);
                cardTransform.position = cardPos;
            }

            // Get Selected Card
            if (canSelectCards)
            {
                //float d = (p - mouseWorldPos).sqrMagnitude;
                if (d < sqrDistance)
                {
                    sqrDistance = d;
                    selected = i;
                }
            }
            else
            {
                selected = -1;
                dragged = -1;
            }

            // Debug Gizmos
            if (showDebugGizmos)
            {
                Color c = new Color(0, 0, 0, 0.2f);
                if (i == selected)
                {
                    c = Color.red;
                    if (sqrDistance > 2)
                    {
                        c = Color.blue;
                    }
                }
                Debug.DrawLine(p, mouseWorldPos, c);
            }
        }

        // --------------------------------------------------------
        // HANDLE DRAGGED CARD
        // (Card held by mouse, inside hand)
        // --------------------------------------------------------

        if (!mouseButton)
        {
            // Stop dragging
            heldCardOffset = Vector3.zero;
            dragged = -1;
        }

        if (dragged != -1)
        {
            Card card = hand[dragged];
            if (mouseButton && !mouseInsideHand)
            { //  && sqrDistance > 2.1f
              //if (cardPos.y > transform.position.y + 0.5) {
              // Card is outside of the hand, so is considered "held" ready to be used
              // Remove from hand, so that cards in hand fill the hole that the card left
                heldCard = card;
                RemoveCardFromHand(dragged);
                count--;
                dragged = -1;
            }
        }

        if (heldCard == null && mouseButton && dragged != -1 && selected != -1 && dragged != selected)
        {
            // Move dragged card
            MoveCardToIndex(dragged, selected);
            dragged = selected;
        }

        // --------------------------------------------------------
        // HANDLE HELD CARD
        // (Card held by mouse, outside of the hand)
        // --------------------------------------------------------

        if (heldCard != null)
        {
            Transform cardTransform = heldCard.transform;
            Vector3 cardUp = Vector3.up;
            Vector3 cardPos = mouseWorldPos + heldCardOffset;
            Vector3 cardForward = Vector3.forward;
            if (cardTilt && mouseButton)
            {
                cardForward -= new Vector3(heldCardTilt.x, heldCardTilt.y, 0);
            }

            // Bring card to front
            cardPos.z = transform.position.z - 0.2f;

            // Handle Position & Rotation
            cardTransform.rotation = Quaternion.RotateTowards(cardTransform.rotation, Quaternion.LookRotation(cardForward, cardUp), 80f * Time.deltaTime);
            cardTransform.position = cardPos;

            //if (!canSelectCards || cardTransform.position.y <= transform.position.y + 0.5f) {
            if (!canSelectCards || mouseInsideHand)
            { //  || sqrDistance <= 2
              // Card has gone back into hand
                AddCardToHand(heldCard, selected);
                dragged = selected;
                selected = -1;
                heldCard = null;
                return;
            }

            // Use Card
            bool mouseButtonUp = Input.GetMouseButtonUp(0);
            if (mouseButtonUp)
            {
                if (canUseCards)
                {
                    heldCard.Use();
                }
                else
                {
                    // Cannot use card / Not enough mana! Return card to hand!
                    AddCardToHand(heldCard, selected);
                }
                heldCard = null;
            }
        }
    }

    /// <summary>
    /// Obtains a point along a curve based on 3 points. Equal to Lerp(Lerp(a, b, t), Lerp(b, c, t), t).
    /// </summary>
    public static Vector3 GetCurvePoint(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return (oneMinusT * oneMinusT * a) + (2f * oneMinusT * t * b) + (t * t * c);
    }

    /// <summary>
    /// Obtains the derivative of the curve (tangent)
    /// </summary>
    public static Vector3 GetCurveTangent(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        return 2f * (1f - t) * (b - a) + 2f * t * (c - b);
    }

    /// <summary>
    /// Obtains a direction perpendicular to the tangent of the curve
    /// </summary>
    public static Vector3 GetCurveNormal(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 tangent = GetCurveTangent(a, b, c, t);
        return Vector3.Cross(tangent, Vector3.forward);
    }

    /// <summary>
    /// Moves the card in hand from the currentIndex to the toIndex. If you want to move a card that isn't in hand, use AddCardToHand
    /// </summary>
    public void MoveCardToIndex(int currentIndex, int toIndex)
    {
        if (currentIndex == toIndex) return; // Same index, do nothing
        Card card = hand[currentIndex];
        hand.RemoveAt(currentIndex);
        hand.Insert(toIndex, card);

        if (updateHierarchyOrder)
        {
            card.transform.SetSiblingIndex(toIndex);
        }
    }

    /// <summary>
    /// Adds a card to the hand. Optional param to insert it at a given index.
    /// </summary>
    public void AddCardToHand(Card card, int index = -1)
    {
        if (index < 0)
        {
            // Add to end
            hand.Add(card);
            index = hand.Count - 1;
        }
        else
        {
            // Insert at index
            hand.Insert(index, card);
        }
        if (updateHierarchyOrder)
        {
            card.transform.SetParent(transform);
            card.transform.SetSiblingIndex(index);
        }
    }

    /// <summary>
    /// Remove the card at the specified index from the hand.
    /// </summary>
    public void RemoveCardFromHand(int index)
    {
        if (updateHierarchyOrder)
        {
            Card card = hand[index];
            card.transform.SetParent(transform.parent);
            card.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
        }
        hand.RemoveAt(index);
    }

        /* -------------------- */
        // below was originally part of Update()
    /*
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

                            Cursor.visible = false;
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
                Cursor.visible = true;

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

                            Vector3 desiredPos = selection.position;

                            if (board.isHandCard(selection.gameObject))
                            {
                                desiredPos = new Vector3(selection.position.x, selection.position.y, selection.position.z - handHoverOffset);
                            }

                            selection.position = Vector3.MoveTowards(desiredPos, m_camera.transform.position, cardOffsetTowardsCamera);

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
    }*/
}
