using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    // The GameObject to instantiate.
    public GameObject emptyCard;

    // An instance of the ScriptableObject defined above.
    public CardDataSO[] cardValues;

    void Start()
    {
        SpawnCards();
    }

    void SpawnCards()
    {
        // Creates an instance of the prefab at the current spawn point.
        GameObject[] CardLocations = GameObject.FindGameObjectsWithTag("CardLocation");

        for (int i = 0; i < CardLocations.Length; i++)
        {
            GameObject newCard = Instantiate(emptyCard, CardLocations[i].transform.position, CardLocations[i].transform.rotation);
            // TODO: change the name of the spawned object so multiple cards don't compete for the same name

            int index = i < cardValues.Length ? i : Random.Range(0, cardValues.Length);

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

            if (cardValues[index] is VillagerDataSO)
            {
                VillagerDataSO villager = (cardValues[index] as VillagerDataSO);

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
                personality.enabled = true;
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
            else if (cardValues[index] is SpecialNPCDataSO)
            {
                SpecialNPCDataSO npc = (cardValues[index] as SpecialNPCDataSO);

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
            else if (cardValues[index] is ToolDataSO)
            {
                ToolDataSO tool = (cardValues[index] as ToolDataSO);

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
            else if (cardValues[index] is FruitDataSO)
            {
                FruitDataSO fruit = (cardValues[index] as FruitDataSO);

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
            backgroundMat.SetColor("_ShadowColor", cardValues[index].DarkColor);
            backgroundMat.SetColor("_Color", cardValues[index].LightColor);

            // Name Text
            newCard.transform.Find("SM_CardName/Name").GetComponent<TextMeshPro>().SetText(cardValues[index].Name);

            // Name Box Color
            nameMat.SetColor("_TopColor", cardValues[index].DarkColor);
            nameMat.SetColor("_BottomColor", cardValues[index].MediumColor);

            // Card Bar Color
            cardBarTop.material.SetColor("_UnlitColor", cardValues[index].DarkColor);
            cardBarMiddle.material.SetColor("_UnlitColor", cardValues[index].DarkColor);

            // Type Text
            typeTop.SetText(cardValues[index].TypeName);
            typeMiddle.SetText(cardValues[index].TypeName);

            // Type Icon
            typeIcon.material.SetTexture("_UnlitColorMap", cardValues[index].TypeIcon);
            typeIcon.material.SetColor("_UnlitColor", cardValues[index].DarkColor);

            // Highlight
            highlight.material.SetColor("_Color", cardValues[index].LightColor);

            // Backface
            backface.material.SetTexture("_UnlitColorMap", cardValues[index].CardBackface);
        }
    }
}
