using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DragNDropMerchant : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //reference to merchantManager, this one is assigned when the card object is instantiated or enabled
    CardOptions cardOptions;
    MerchantManager merchantManager;

    CardDescriptionLayout cardDescriptionLayout;

    //Reference Objects for the prices
    //the object is a prefab instantiated under the card object itself
    GameObject scrapsValueObj;
    TextMeshProUGUI scrapsValueText;
    public int scrapsValueInt;
    private void Awake()
    {
        cardOptions = transform.parent.parent.parent.parent.parent.GetComponent<CardOptions>();
        merchantManager = cardOptions.merchantManager;
        //SetScrapsValue();
    }
    private void Start()
    {
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

    //called by CardOptions when enabling or instantiating the card prefab
    public void SetScrapsValue(int scrapsValue)
    {
        //assign the last child which is the newly added price tag object
        scrapsValueObj = transform.GetChild(transform.childCount-1).gameObject;
        //child 1 of scraps object is the text
        scrapsValueText = scrapsValueObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        scrapsValueInt = scrapsValue;
        scrapsValueText.text = $"{scrapsValueInt}";
    }

    //disables the card prefab from options
    void DisableCardPrefab()
    {
        gameObject.SetActive(false);
    }

}
