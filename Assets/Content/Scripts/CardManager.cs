using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class Card
{
    public GameObject cardObj;
    public CardDataSO data;

    // TODO: might need to make this a monobehavior so one card is always associated with one game object, and I can access the transform data
    public Card(GameObject spawnedCard, CardDataSO spawnedCardData)
    {
        cardObj = spawnedCard;
        data = spawnedCardData;
    }
}*/

public class Card : MonoBehaviour
{
    protected Color color;
    protected Color color2;

    protected MeshRenderer meshRenderer;
    protected Material material;

    protected Vector2 dissolveOffset = new Vector2(0.1f, 0);
    protected Vector2 dissolveSpeed = new Vector2(2f, 2f);
    protected Color dissolveColor;

    protected bool isInactive;

    // ----------
    public GameObject cardObj; // TODO: this is a monobehavior now, we don't need this
    public CardDataSO data;

    public Card(GameObject spawnedCard, CardDataSO spawnedCardData)
    {
        cardObj = spawnedCard;
        data = spawnedCardData;
    }
    // ----------

    protected virtual void Start()
    {
    }

    /// <summary>
    /// <para>Triggered when the card is used (dragged up then mouse released, with required mana).</para>
    /// <para>This should probably be overriden by a class that inherits this class, to trigger something
    /// or maybe add something here to trigger an event / UnityEvent, etc.</para>
    /// <para>Base applies a dissolve effect, which can be adjusted using dissolveOffset, dissolveSpeed, dissolveColor</para>
    /// </summary>
    public virtual void Use()
    {
        // Handle Dissolve Effect
        //StartCoroutine(Dissolve());
    }
}

public class PlayerSide
{
    public Card[] inPlay;
    public Card[] hand;
    public Card[] deck;

    public PlayerSide(int playSize, int handSize, int deckSize)
    {
        inPlay = new Card[playSize];
        hand = new Card[handSize];
        deck = new Card[deckSize];
    }
}

public class Board
{
    public PlayerSide player;
    public PlayerSide opponent;

    public Board(int playSize, int handSize, int deckSize)
    {
        player = new PlayerSide(playSize, handSize, deckSize);
        opponent = new PlayerSide(playSize, handSize, deckSize);
    }

    public bool isHandCard(GameObject cardObj)
    {
        if (cardObj == null)
        {
            return false;
        }

        foreach (Card handCard in player.hand)
        {
            if (handCard != null)
            {
                if (handCard.cardObj == cardObj)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool isPlayerCard(GameObject cardObj)
    {
        if (cardObj == null)
        {
            return false;
        }

        foreach (Card playCard in player.inPlay)
        {
            if (playCard != null)
            {
                if (playCard.cardObj == cardObj)
                {
                    return true;
                }
            }
        }

        foreach (Card handCard in player.hand)
        {
            if (handCard != null)
            {
                if (handCard.cardObj == cardObj)
                {
                    return true;
                }
            }
        }

        foreach (Card deckCard in player.deck)
        {
            if (deckCard != null)
            {
                if (deckCard.cardObj == cardObj)
                {
                    return true;
                }
            }
        }

        return false;
    }
}

public class CardManager : MonoBehaviour
{
    GameObject zones = null;
    int sidePlaySize = 0;
    int sideHandSize = 0;
    int sideDeckSize = 0;

    public Board board;
    CardSpawner spawner;

    void Awake()
    {
        spawner = gameObject.GetComponent<CardSpawner>();
        zones = GameObject.Find("Zones");

        if (zones != null)
        {
            sidePlaySize = zones.transform.Find("Player/In Play").childCount;
            sideHandSize = zones.transform.Find("Player/Hand").childCount;
            sideDeckSize = zones.transform.Find("Player/Deck").childCount;
        }
        else
        {
            Debug.Log("Zones was null!");
        }

        board = new Board(sidePlaySize, sideHandSize, sideDeckSize);
    }

    void Start()
    {
        if (spawner != null)
        {
            spawner.SpawnCards(board);
            //gameObject.GetComponent<SelectionManager>().BasicallyStart(); // TODO: remove later!
        }
    }

    void Update()
    {
        
    }
}
