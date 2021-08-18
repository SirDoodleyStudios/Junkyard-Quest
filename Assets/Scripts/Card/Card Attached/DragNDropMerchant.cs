using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DragNDropMerchant : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //used when buying card for options
    CardOptions cardOptions;
    //reference for cardRemoval script, 
    CardRemoval cardRemoval;
    //taken from either cardOptions and cardRemoval
    //reference to merchantManager, this one is assigned when the card object is instantiated or enabled
    MerchantManager merchantManager;
    CardDescriptionLayout cardDescriptionLayout;

    //bool identifier that will determine whether the card options are for adding or removing
    //taked from the bool identifer in merchhantManager
    bool isAddToDeck;

    //Reference Objects for the prices
    //the object is a prefab instantiated under the card object itself
    GameObject scrapsValueObj;
    public int scrapsValueInt;
    private void Awake()
    {
        //cardOptions = transform.parent.parent.parent.parent.parent.GetComponent<CardOptions>();
        //merchantManager = cardOptions.merchantManager;
        //SetScrapsValue();
    }
    private void Start()
    {
        ////calls activate and deactivate popup methods in cardDescriptionLayout
        //cardDescriptionLayout = gameObject.GetComponent<CardDescriptionLayout>();
    }
    //called when the prefab is enabled or instantiated to determine if choices are for adding card or removing
    //true for buying, false for removing
    public void InitiateMerchantCard(bool state)
    {
        isAddToDeck = state;
        //if parameter is true, set up card logic for buy
        if (state)
        {
            cardOptions = transform.parent.parent.parent.parent.parent.GetComponent<CardOptions>();
            merchantManager = cardOptions.merchantManager;

        }
        //if parameter is false, set up card logic for removal
        else
        {
            cardRemoval = transform.transform.parent.parent.parent.parent.parent.GetComponent<CardRemoval>();
            merchantManager = cardRemoval.merchantManager;

        }
        //calls activate and deactivate popup methods in cardDescriptionLayout
        cardDescriptionLayout = gameObject.GetComponent<CardDescriptionLayout>();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardDescriptionLayout.EnablePopups();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardDescriptionLayout.DisablePopups();
    }

    //on click, add the card to deck
    public void OnPointerClick(PointerEventData eventData)
    {
        //check the last child of the card which is the newly added price tag
        scrapsValueObj = transform.GetChild(transform.childCount - 1).gameObject;
        //assign the int value in the priceTag script
        PriceTag priceTag = scrapsValueObj.GetComponent<PriceTag>();
        scrapsValueInt = priceTag.priceTag;

        //function for click depends on the buy or remove parameter
        if (isAddToDeck)
        {
            //check first if player has enough scraps
            if (merchantManager.CheckScraps(scrapsValueInt))
            {
                //on click, send the assigned card to the MerchantManager for adding to deck
                Card card = gameObject.GetComponent<Display>().card;
                merchantManager.AddBoughtCard(card);
                //on click, remove the chosen card from the list in cardOptions
                cardOptions.RemoveCardFromOptionList(card);
                //calls disable if the card clicked can be bought
                DisableCardPrefab();
            }
        }
        //if bool identifier is false, click logic is for removing card from deck
        //REMOVING A CARD IS FREE BUT ONLY FOR ONE TIME SO NO NEED FOR SCRAPS CHECK
        else
        {
            //on click, send the assigned card to the MerchantManager for adding to deck
            Card card = gameObject.GetComponent<Display>().card;
            merchantManager.RemoveCardFromDeck(card);
            //calls disable if the card clicked can be bought
            DisableCardPrefab();
        }



    }

    //PRICE IS NOW HARBORED IN THE PRICETAGSCRIPT IN THE PRICETAGPREFAB
    //called by CardOptions when enabling or instantiating the card prefab
    //public void SetScrapsValue(int scrapsValue)
    //{
    //    //assign the last child which is the newly added price tag object
    //    scrapsValueObj = transform.GetChild(transform.childCount-1).gameObject;
    //    //child 1 of scraps object is the text
    //    scrapsValueText = scrapsValueObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

    //    scrapsValueInt = scrapsValue;
    //    scrapsValueText.text = $"{scrapsValueInt}";
    //}

    //disables the card prefab from options
    void DisableCardPrefab()
    {
        gameObject.SetActive(false);
    }

}
