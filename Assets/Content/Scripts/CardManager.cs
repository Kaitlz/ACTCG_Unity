using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public GameObject cardObj;
    public CardDataSO data;

    public Card(GameObject spawnedCard, CardDataSO spawnedCardData)
    {
        cardObj = spawnedCard;
        data = spawnedCardData;
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
        spawner.SpawnCards(board);
    }

    void Update()
    {
        
    }
}
