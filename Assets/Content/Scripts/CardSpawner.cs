using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    // The GameObject to instantiate.
    public GameObject emptyCard;

    // An instance of the ScriptableObject defined above.
    public VillagerDataSO[] villagerValues;

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

            int vilValIndex = i < villagerValues.Length ? i : Random.Range(0, villagerValues.Length - 1);

            // Sets the name of the instantiated entity to be the string defined in the ScriptableObject and then appends it with a unique number. 
            newCard.transform.Find("Name").GetComponent<TextMeshPro>().SetText(villagerValues[vilValIndex].Name);
            newCard.transform.Find("Type").GetComponent<TextMeshPro>().SetText(villagerValues[vilValIndex].TypeName);

            Material speciesMat = newCard.transform.Find("Stats/Circle_TopLeft_Type/Species").GetComponent<MeshRenderer>().material;
            if (villagerValues[vilValIndex].TypeIcon.name.Contains("SpeciesIconSilhouette"))
            {
                speciesMat.SetTexture("_Texture", villagerValues[vilValIndex].TypeIcon);
                speciesMat.SetColor("_Color", villagerValues[vilValIndex].DarkColor);
            }
            else
            {
                speciesMat.shader = Shader.Find("HDRP/Unlit");
                speciesMat.SetTexture("_UnlitColorMap", villagerValues[vilValIndex].TypeIcon);
                speciesMat.SetColor("_UnlitColor", villagerValues[vilValIndex].DarkColor);
            }

            Material personalityMat = newCard.transform.Find("Stats/Circle_BottomRight_Personality_Happiness/Personality").GetComponent<MeshRenderer>().material;
            personalityMat.SetTexture("_UnlitColorMap", villagerValues[vilValIndex].PersonalityIcon);
            personalityMat.SetColor("_UnlitColor", villagerValues[vilValIndex].DarkColor);

            newCard.transform.Find("SM_CardTypeBar").GetComponent<MeshRenderer>().material.SetColor("_UnlitColor", villagerValues[vilValIndex].DarkColor);

            Material descMat = newCard.transform.Find("SM_CardDescBox").GetComponent<MeshRenderer>().material;
            descMat.SetColor("_TopColor", villagerValues[vilValIndex].DarkColor);
            descMat.SetColor("_BottomColor", villagerValues[vilValIndex].MediumColor);
            descMat.SetColor("_BackgroundColor", villagerValues[vilValIndex].LightColor);

            Material nameMat = newCard.transform.Find("SM_CardName").GetComponent<MeshRenderer>().material;
            nameMat.SetColor("_TopColor", villagerValues[vilValIndex].DarkColor);
            nameMat.SetColor("_BottomColor", villagerValues[vilValIndex].MediumColor);

            Material backgroundMat = newCard.transform.Find("Solid Background").GetComponent<MeshRenderer>().material;
            backgroundMat.SetColor("_ShadowColor", villagerValues[vilValIndex].DarkColor);
            backgroundMat.SetColor("_Color", villagerValues[vilValIndex].LightColor);

            newCard.transform.Find("SM_CardIllustrationPlane").GetComponent<MeshRenderer>().material = villagerValues[vilValIndex].MainImage;

            Transform Happiness = newCard.transform.Find("Stats/Circle_TopRight_Happiness_Fruit/Happiness");
            Happiness.GetComponent<TextMeshPro>().SetText(villagerValues[vilValIndex].HappinessPoints);
            //Happiness.GetComponent<MeshRenderer>().material.SetColor("_FaceColor", villagerValues[vilValIndex].DarkColor);

            newCard.transform.Find("Stats/Circle_TopRight_Happiness_Fruit/Heart").GetComponent<MeshRenderer>().material.SetColor("_UnlitColor", villagerValues[vilValIndex].DarkColor);

            newCard.transform.Find("Description").GetComponent<TextMeshPro>().SetText(villagerValues[vilValIndex].Description);

            newCard.transform.Find("DOB").GetComponent<TextMeshPro>().SetText(villagerValues[vilValIndex].DOB);

            newCard.transform.Find("Star Sign").GetComponent<MeshRenderer>().material.SetTexture("_UnlitColorMap", villagerValues[vilValIndex].StarSignIcon);
        }
    }
}
