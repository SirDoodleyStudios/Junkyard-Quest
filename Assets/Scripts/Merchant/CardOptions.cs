using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardOptions : MonoBehaviour
{
    //assigned in editor
    //card prefab to instantiate
    public GameObject cardObjectPrefab;
    //reference MerchantManager
    public MerchantManager merchantManager;
    //the content view
    public Transform cardOptionsContent;
    //used contains the deck pools
    public DeckPools deckPools;

    //generated at initiate function, should be random
    List<Card> playerCardPool = new List<Card>();
    List<Card> classCardPool = new List<Card>();
    //will contain the actual list of cards generated for merchant
    List<Card> cardList = new List<Card>();


    //called at the beginning of merchantManager to initiate the possible contents of the card options
    public void InitiateCardOptions(UniversalInformation universalInfo)
    {
        //stores the chosen player and class pools
        ChosenPlayer chosenPlayer = universalInfo.chosenPlayer;
        ChosenClass chosenClass = universalInfo.chosenClass;
        //initializes theCardSO factory
        CardSOFactory.InitializeCardSOFactory(chosenPlayer, chosenClass);
        //generate the card pools
        classCardPool = deckPools.GetClassPool(chosenClass);
        playerCardPool = deckPools.GetPlayerPool(chosenPlayer);

        //generate 5 player cards
        //NOT USED FOR NOW BECAUSE THERE AREN'T ANY ARLEN CARDS YET, ALIGN WITH THE CLASS CARD LOGIC
        //for (int i = 0; 4 >= i; i++)
        //{
        //    int randomizedIndex = Random.Range(0, playerCardPool.Count);
        //    cardList.Add(playerCardPool[randomizedIndex]);
        //}

        //generate 5 class cards
        for (int i = 0; 4 >= i; i++)
        {
            int randomizedIndex = Random.Range(0, classCardPool.Count);
            Card instantiatedCard = classCardPool[randomizedIndex];
            instantiatedCard.effectText = CardTagManager.GetCardEffectDescriptions(instantiatedCard);
            cardList.Add(instantiatedCard);
        }
    }


    //shows the randomly generated options for cards

    public void ViewCardOptions()
    {
        //actual logic to show each card in UI one by one

        foreach (Card deckCard in cardList)
        {
            bool hasNoDisabledPrefabs = true;
            cardObjectPrefab.GetComponent<Display>().card = deckCard;

            //sets the size of each cell in the content holder depending on the size of screen
            //the numbers are calculated to get the exact amount needed
            //I just generated the cards on the screen wtih a sixe that I liked with a fixed 100,143.75 cell size then divided the screen sizes from them
            GridLayoutGroup gridLayout = cardOptionsContent.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(Screen.width * .13440860215f, Screen.height * .34389952153f);
            gridLayout.spacing = new Vector2(Screen.width * .0237247924f, Screen.height * .04219409282f);

            //checks the scroll contents if there are already instantiated card prefabs that can be recycled
            foreach (Transform content in cardOptionsContent)
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
                GameObject instantiatedPrefab = Instantiate(cardObjectPrefab, cardOptionsContent);
                RectTransform instantiatedRect = instantiatedPrefab.GetComponent<RectTransform>();
                Display instantiatedDisplay = instantiatedPrefab.GetComponent<Display>();
                CardDescriptionLayout instantiatedPopups = instantiatedPrefab.GetComponent<CardDescriptionLayout>();
                instantiatedRect.sizeDelta = new Vector2(Screen.width * .13440860215f, Screen.height * .34389952153f);
                instantiatedDisplay.card = deckCard;
                instantiatedDisplay.FontResize();
                instantiatedPopups.ResizePopups();
                instantiatedPrefab.SetActive(true);
            }
        }
    }

    //back button
    public void CardOptionBackButton()
    {
        foreach (Transform content in cardOptionsContent)
        {
            content.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }
}
