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
    public GameObject scrapsValueObj;
    public TextMeshProUGUI scrapsValueText;
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
        scrapsValueInt = scrapsValue;
        scrapsValueText.text = $"{scrapsValueInt}";
    }

    //disables the card prefab from options
    void DisableCardPrefab()
    {
        gameObject.SetActive(false);
    }

}
