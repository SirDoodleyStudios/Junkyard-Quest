using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CameraUIScript : MonoBehaviour
{
    public delegate void D_ResizeCameraUI(float i);
    public event D_ResizeCameraUI d_ResizeCameraUI;

    //loaded Information
    UniversalInformation universalInfo;

    TextMeshProUGUI HPText;
    TextMeshProUGUI CreativityText;
    TextMeshProUGUI ScrapsText;

    RectTransform objectRect;

    //for Deck Viewing
    //the actual UI to back drop to be activated 
    public GameObject deckViewUI;
    List<Card> cardList = new List<Card>();
    //card prefab for viewing in deck
    public GameObject deckViewPrefab;
    //Content gameobject, parent of all prefabs to be shown after button click
    public Transform deckScrollContent;



    public void Awake()
    {
        objectRect = gameObject.GetComponent<RectTransform>();

        universalInfo = UniversalSaveState.LoadUniversalInformation();

        //get text object of the overworld UI, the getChild(1) is the the TMPro itself
        //HP slot is in the index 2 under the Overworld UI
        //Creativity slot is in index 3
        //scraps is in index 4
        HPText = transform.GetChild(2).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        CreativityText = transform.GetChild(3).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        ScrapsText = transform.GetChild(4).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
    }

    //called by overworld manager when it instantiates
    public void SetUISize(float orthoSize)
    {
        //sets the size of the UI based on 
        objectRect.sizeDelta = new Vector2(orthoSize/3.6f, 0);
        d_ResizeCameraUI(orthoSize / 3.8f);

        float fontSize = (orthoSize / 3.6f) * .35f;
        //sets the size of texts in UI depending on the Size of slot
        HPText.fontSize = fontSize;
        CreativityText.fontSize = fontSize;
        ScrapsText.fontSize = fontSize;

    }

    //called by overworld manager
    public void AssignUIObjects(UniversalInformation universalInfo)
    {
        //initializes theCardSO factory
        CardSOFactory.InitializeCardSOFactory(universalInfo.chosenPlayer, universalInfo.chosenClass);

        HPText.text = $"{universalInfo.playerStats.currHP}/\n{universalInfo.playerStats.HP}";
        CreativityText.text = $"{universalInfo.playerStats.Creativity}";
        ScrapsText.text = $"{universalInfo.scraps}";

        //generates the CardSO itself
        foreach (AllCards cardKey in universalInfo.currentDeck)
        {
            //calls the factory to instantiate a copy of the base card SO
            Card tempCard = Instantiate(CardSOFactory.GetCardSO(cardKey));
            tempCard.effectText = CardTagManager.GetCardEffectDescriptions(tempCard);
            cardList.Add(tempCard);
        }
    }



    //single method for viewing a card collection in view
    public void ViewSavedDeck()
    {
        deckViewUI.SetActive(true);

        //actual logic to show each card in UI one by one

        foreach (Card deckCard in cardList)
        {
            bool hasNoDisabledPrefabs = true;
            deckViewPrefab.GetComponent<Display>().card = deckCard;

            //checks the scroll contents if there are already instantiated card prefabs that can be recycled
            foreach (Transform content in deckScrollContent)
            {
                GameObject disabledPrefabs = content.gameObject;
                if (!disabledPrefabs.activeSelf)
                {
                    disabledPrefabs.GetComponent<Display>().card = deckCard;
                    disabledPrefabs.SetActive(true);
                    hasNoDisabledPrefabs = false;
                    break;
                }
                //if no card prefab can be recycled, instantiate a new one
            }
            if (hasNoDisabledPrefabs)
            {
                GameObject instantiatedPrefab = Instantiate(deckViewPrefab, deckScrollContent);
                instantiatedPrefab.GetComponent<Display>().card = deckCard;
                instantiatedPrefab.SetActive(true);
            }
        }
    }
    //close deck view
    public void UnviewSavedDeck()
    {
        foreach (Transform content in deckScrollContent)
        {
            content.gameObject.SetActive(false);
        }
        deckViewUI.SetActive(false);
    }
}
