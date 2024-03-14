using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    // The GameObject to instantiate.
    public GameObject cardPrefab;

    // An instance of the ScriptableObject defined above.
    public CardDataSO[] playerInPlay;
    public CardDataSO[] playerHand;
    public CardDataSO[] playerDeck;
    public CardDataSO[] opponentInPlay;
    public CardDataSO[] opponentHand;
    public CardDataSO[] opponentDeck;

    CardManager manager; // TODO: do we need this?
    GameObject zones = null;

    private void Awake()
    {
        manager = gameObject.GetComponent<CardManager>();
        zones = GameObject.Find("Zones");
    }

    Card SpawnCard(CardDataSO data, Transform cardTransform)
    {
        if (data == null || cardTransform == null)
        {
            return null;
        }

        GameObject newCard = Instantiate(cardPrefab, cardTransform.position, cardTransform.rotation);
        newCard.name = "Card_" + data.Name; // TODO: update this if we allow card duplicates
        newCard.SetActive(true);

        // References to card parts
        MeshRenderer illustration = newCard.transform.Find("SM_CardIllustrationPlane").GetComponent<MeshRenderer>();
        MeshRenderer personality = newCard.transform.Find("SM_CardBar_Middle/Personality").GetComponent<MeshRenderer>();
        MeshRenderer starSign = newCard.transform.Find("SM_CardName/BottomInfo/Star Sign").GetComponent<MeshRenderer>();
        MeshRenderer gender = newCard.transform.Find("SM_CardName/BottomInfo/Gender").GetComponent<MeshRenderer>();
        MeshRenderer heart = newCard.transform.Find("Stats/Circle_TopLeft_Happiness/Heart").GetComponent<MeshRenderer>();
        MeshRenderer descBox = newCard.transform.Find("SM_CardDescBox_Bottom").GetComponent<MeshRenderer>();
        MeshRenderer cardBarTop = newCard.transform.Find("SM_CardBar_Top").GetComponent<MeshRenderer>();
        MeshRenderer cardBarMiddle = newCard.transform.Find("SM_CardBar_Middle").GetComponent<MeshRenderer>();
        MeshRenderer fruitRender = newCard.transform.Find("FruitRender").GetComponent<MeshRenderer>();
        MeshRenderer toolRender = newCard.transform.Find("ToolRender").GetComponent<MeshRenderer>();
        MeshRenderer typeIcon = newCard.transform.Find("Stats/Circle_TopLeft_Type/Type").GetComponent<MeshRenderer>();
        MeshRenderer patternFull = newCard.transform.Find("CardPattern").GetComponent<MeshRenderer>();
        MeshRenderer circleHappiness = newCard.transform.Find("Stats/Circle_TopLeft_Happiness").GetComponent<MeshRenderer>();
        MeshRenderer highlight = newCard.transform.Find("Highlight").GetComponent<MeshRenderer>();
        MeshRenderer backface = newCard.transform.Find("Backface").GetComponent<MeshRenderer>();

        TextMeshPro dob = newCard.transform.Find("SM_CardName/BottomInfo/DOB").GetComponent<TextMeshPro>();
        TextMeshPro happiness = newCard.transform.Find("Stats/Circle_TopLeft_Happiness/Happiness").GetComponent<TextMeshPro>();
        TextMeshPro description = newCard.transform.Find("SM_CardDescBox_Bottom/Description").GetComponent<TextMeshPro>();
        TextMeshPro typeTop = newCard.transform.Find("SM_CardBar_Top/Type").GetComponent<TextMeshPro>();
        TextMeshPro typeMiddle = newCard.transform.Find("SM_CardBar_Middle/Type").GetComponent<TextMeshPro>();

        Material backgroundMat = newCard.transform.Find("Solid Background").GetComponent<MeshRenderer>().material;
        Material nameMat = newCard.transform.Find("SM_CardName").GetComponent<MeshRenderer>().material;

        if (data is VillagerDataSO)
        {
            VillagerDataSO villager = (data as VillagerDataSO);

            // Disable Unnecessary Items
            cardBarTop.enabled = false;
            typeTop.enabled = false;
            fruitRender.enabled = false;
            toolRender.enabled = false;
            patternFull.enabled = false;

            // Main Image
            illustration.enabled = true;
            illustration.material = villager.MainImage;

            // Personality
            //personality.enabled = true;
            personality.material.SetTexture("_UnlitColorMap", villager.PersonalityIcon);

            // Star Sign
            starSign.enabled = true;
            starSign.material.SetTexture("_UnlitColorMap", villager.StarSignIcon);

            // Gender
            gender.enabled = true;
            gender.material.SetTexture("_UnlitColorMap", villager.GenderIcon);

            // Date of Birth
            dob.enabled = true;
            dob.SetText(villager.DOB);

            // Happiness Points
            circleHappiness.enabled = true;
            happiness.enabled = true;
            happiness.SetText(villager.HappinessPoints);

            heart.enabled = true;
            heart.material.SetColor("_UnlitColor", villager.DarkColor);

            // Description
            description.enabled = true;
            description.SetText(villager.Description);

            descBox.enabled = true;
            descBox.material.SetColor("_TopColor", villager.DarkColor);
            descBox.material.SetColor("_BottomColor", villager.MediumColor);
            descBox.material.SetColor("_BackgroundColor", villager.LightColor);
            descBox.material.SetTexture("_Texture", villager.BackgroundPattern);
        }
        else if (data is SpecialNPCDataSO)
        {
            SpecialNPCDataSO npc = (data as SpecialNPCDataSO);

            // Disable Unnecessary Items
            cardBarTop.enabled = false;
            typeTop.enabled = false;
            fruitRender.enabled = false;
            toolRender.enabled = false;
            patternFull.enabled = false;
            personality.enabled = false;
            circleHappiness.enabled = false;
            heart.enabled = false;
            happiness.enabled = false;

            // Main Image
            illustration.enabled = true;
            illustration.material = npc.MainImage;

            // Star Sign
            starSign.enabled = true;
            starSign.material.SetTexture("_UnlitColorMap", npc.StarSignIcon);

            // Gender
            gender.enabled = true;
            gender.material.SetTexture("_UnlitColorMap", npc.GenderIcon);

            // Date of Birth
            dob.enabled = true;
            dob.SetText(npc.DOB);

            // Description
            description.enabled = true;
            description.SetText(npc.Description);

            descBox.enabled = true;
            descBox.material.SetColor("_TopColor", npc.DarkColor);
            descBox.material.SetColor("_BottomColor", npc.MediumColor);
            descBox.material.SetColor("_BackgroundColor", npc.LightColor);
            descBox.material.SetTexture("_Texture", npc.BackgroundPattern);
        }
        else if (data is ToolDataSO)
        {
            ToolDataSO tool = (data as ToolDataSO);

            // Disable Unnecessary Items
            cardBarTop.enabled = false;
            typeTop.enabled = false;
            fruitRender.enabled = false;
            illustration.enabled = false;
            personality.enabled = false;
            starSign.enabled = false;
            gender.enabled = false;
            dob.enabled = false;
            descBox.enabled = false;
            circleHappiness.enabled = false;
            heart.enabled = false;
            happiness.enabled = false;

            // Description
            description.enabled = true;
            description.SetText(tool.Description);

            // Fruit Render
            toolRender.enabled = true;
            toolRender.material.SetTexture("_UnlitColorMap", tool.ToolRender);

            // Card Pattern
            patternFull.enabled = true;
            patternFull.material.SetTexture("_UnlitColorMap", tool.BackgroundPattern);
        }
        else if (data is FruitDataSO)
        {
            FruitDataSO fruit = (data as FruitDataSO);

            // Disable Unnecessary Items
            illustration.enabled = false;
            personality.enabled = false;
            starSign.enabled = false;
            gender.enabled = false;
            dob.enabled = false;
            description.enabled = false;
            descBox.enabled = false;
            cardBarMiddle.enabled = false;
            typeMiddle.enabled = false;
            toolRender.enabled = false;

            // Happiness Points
            circleHappiness.enabled = true;
            happiness.enabled = true;
            happiness.SetText(fruit.HappinessPoints);

            heart.enabled = true;
            heart.material.SetColor("_UnlitColor", fruit.DarkColor);

            // Fruit Render
            fruitRender.enabled = true;
            fruitRender.material.SetTexture("_UnlitColorMap", fruit.FruitRender);

            // Card Pattern
            patternFull.enabled = true;
            patternFull.material.SetTexture("_UnlitColorMap", fruit.BackgroundPattern);

            // Top Bar
            cardBarTop.enabled = true;
            typeTop.enabled = true;
        }

        // Solid Background
        backgroundMat.SetColor("_ShadowColor", data.DarkColor);
        backgroundMat.SetColor("_Color", data.LightColor);

        // Name Text
        newCard.transform.Find("SM_CardName/Name").GetComponent<TextMeshPro>().SetText(data.Name);

        // Name Box Color
        nameMat.SetColor("_TopColor", data.DarkColor);
        nameMat.SetColor("_BottomColor", data.MediumColor);

        // Card Bar Color
        cardBarTop.material.SetColor("_UnlitColor", data.DarkColor);
        cardBarMiddle.material.SetColor("_UnlitColor", data.DarkColor);

        // Type Text
        typeTop.SetText(data.TypeName);
        typeMiddle.SetText(data.TypeName);

        // Type Icon
        typeIcon.material.SetTexture("_UnlitColorMap", data.TypeIcon);
        typeIcon.material.SetColor("_UnlitColor", data.DarkColor);

        // Highlight
        highlight.material.SetColor("_Color", data.LightColor);

        // Backface
        backface.material.SetTexture("_UnlitColorMap", data.CardBackface);

        return new Card(newCard, data);
    }

    Card[] SpawnCardGroup(CardDataSO[] group, Transform[] cardLocations)
    {
        Card[] cardGroup = new Card[cardLocations.Length];

        for (int i = 0; i < cardLocations.Length; i++)
        {
            if (i < group.Length && i < cardLocations.Length)
            {
                cardGroup[i] = SpawnCard(group[i], cardLocations[i]);
            }
            else if (i >= group.Length && i < cardLocations.Length)
            {
                // More card locations than there is data.
                if (group.Length > 0)
                {
                    //Spawn random duplicate from group.
                    cardGroup[i] = SpawnCard(group[Random.Range(0, group.Length)], cardLocations[i]);
                }   
                else
                {
                    break;
                }
            }
            else
            {
                Debug.Log("Something went wrong when spawning a card!");
                break;
            }
        }

        return cardGroup; 
    }

    public void SpawnCards(Board board)
    {
        // TODO: basically make a clone of zones and spawn cards in an organized hierarchy

        if (zones != null)
        {
            int index = 0;

            // --- PLAYER --- //

            Transform[] zonePlayerInPlay = new Transform[board.player.inPlay.Length];
            foreach (Transform zpip in zones.transform.Find("Player/In Play"))
            {
                // TODO: I think I need to check the tags??
                zonePlayerInPlay[index] = zpip;
                index++;
            }
            board.player.inPlay = SpawnCardGroup(playerInPlay, zonePlayerInPlay);

            index = 0;

            Transform[] zonePlayerHand = new Transform[board.player.hand.Length];
            foreach (Transform zph in zones.transform.Find("Player/Hand"))
            {
                zonePlayerHand[index] = zph;
                index++;
            }
            board.player.hand = SpawnCardGroup(playerHand, zonePlayerHand);

            index = 0;

            Transform[] zonePlayerDeck = new Transform[board.player.deck.Length];
            foreach (Transform zpd in zones.transform.Find("Player/Deck"))
            {
                zonePlayerDeck[index] = zpd;
                index++;
            }
            board.player.deck = SpawnCardGroup(playerDeck, zonePlayerDeck);



            // --- OPPONENT --- //

            index = 0;

            Transform[] zoneOpponentInPlay = new Transform[board.opponent.inPlay.Length];
            foreach (Transform zoip in zones.transform.Find("Opponent/In Play"))
            {
                zoneOpponentInPlay[index] = zoip;
                index++;
            }
            board.opponent.inPlay = SpawnCardGroup(opponentInPlay, zoneOpponentInPlay);

            index = 0;

            Transform[] zoneOpponentHand = new Transform[board.opponent.hand.Length];
            foreach (Transform zoh in zones.transform.Find("Opponent/Hand"))
            {
                zoneOpponentHand[index] = zoh;
                index++;
            }
            board.opponent.hand = SpawnCardGroup(opponentHand, zoneOpponentHand);

            index = 0;

            Transform[] zoneOpponentDeck = new Transform[board.opponent.deck.Length];
            foreach (Transform zod in zones.transform.Find("Opponent/Deck"))
            {
                zoneOpponentDeck[index] = zod;
                index++;
            }
            board.opponent.deck = SpawnCardGroup(opponentDeck, zoneOpponentDeck);
        }
    }
}
