using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardRemoval : MonoBehaviour
{
    //assigned in editor
    //card prefab to instantiate
    //use the card variant for merchant because they contain the dragNDrop with remove
    public GameObject cardObjectPrefab;
    //reference MerchantManager, only used in merchantManager, can be ignored in other scenes
    public MerchantManager merchantManager;
    //the content view
    public Transform cardOptionsContent;

    //NOT NEEDED FOR CARD REMOVAL
    ////dictionary that holds the Card and it's corresponding cost
    //Dictionary<Card, int> CardNCosts = new Dictionary<Card, int>();
    ////current merchantSaveState to be altered and sent back to MerchantManager once player closes the Option UI
    //MerchantSaveState merchantSaveState;

    public void ViewSavedDeck(List<Card> cardList)
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

                    //the DragNDrop scriopt of the card, for setting the bool identifier to true
                    //false parameter makes the card click for removing the card from deck
                    DragNDropMerchant dragNDrop = disabledPrefabs.GetComponent<DragNDropMerchant>();
                    dragNDrop.InitiateMerchantCard(false);

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

                //the DragNDrop scriopt of the card, for setting the bool identifier to true
                //false parameter makes the card click for removing the card from deck
                DragNDropMerchant dragNDrop = instantiatedPrefab.GetComponent<DragNDropMerchant>();
                dragNDrop.InitiateMerchantCard(false);

                instantiatedDisplay.FontResize();
                instantiatedPopups.ResizePopups();
                instantiatedPrefab.SetActive(true);
            }
        }
    }


    //NOT NEEDD FOR CARD REMOVAL
    //function called to remove the bought card from the card list
    //public void RemoveCardFromOptionList(Card card)
    //{
    //    //cardList.Remove(card);
    //    CardNCosts.Remove(card);
    //}

    //back button
    //SAVE FUNCTION WILL ONLY APPLY WHEN CARD IS CLICKED SINCE IT WILL DISABLE THE CARD REMOVAL OPTION IMMEDIATELY
    public void CardOptionBackButton()
    {
        //disables all children prefab of cards
        foreach (Transform content in cardOptionsContent)
        {
            content.gameObject.SetActive(false);
        }
        //disable the CardOptionUI
        gameObject.SetActive(false);

    }

    //NOT NEEDED FOR CARD REMOVAL
    //helper function
    //retrieves the current cardList and their costs and separates them to two lists so that it can be saved in the merchantSaveState file
    //called when exiting from the card Options UI
    //public void UpdateCardOptionsSaveState()
    //{
    //    //List holders
    //    List<CardAndJigsaWrapper> tempCJWList = new List<CardAndJigsaWrapper>();
    //    List<int> tempCostList = new List<int>();

    //    //converts the current dictionary from Card,int to CJW,int
    //    foreach (KeyValuePair<Card, int> cardNCost in CardNCosts)
    //    {
    //        CardAndJigsaWrapper CJW = new CardAndJigsaWrapper(cardNCost.Key);
    //        tempCJWList.Add(CJW);
    //        tempCostList.Add(cardNCost.Value);
    //    }
    //    //assign the listst in the merchantSaveState instance
    //    merchantSaveState.cardOptions = tempCJWList;
    //    merchantSaveState.cardOptionCosts = tempCostList;
    //}

}
