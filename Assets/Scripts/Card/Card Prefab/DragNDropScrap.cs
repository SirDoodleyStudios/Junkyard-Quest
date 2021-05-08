using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragNDropScrap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    RectTransform cardRect;
    ScrapsDrafting scrapsDrafting;

    private void Start()
    {
        //calls activate and deactivate popup methods in cardDescriptionLayout
        cardRect = gameObject.GetComponent<RectTransform>();
        scrapsDrafting = transform.parent.GetComponent<ScrapsDrafting>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        cardRect.localScale = new Vector2(1.3f, 1.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardRect.localScale = new Vector2(1f, 1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //calls cardDrafting to add the chosen card to the deck
        //TEST ONLY, WE NEED TO ADD LOGIC THAT GIVES SCRAPS AMOUNT
        //scrapsDrafting.InitializeScrapValue(100);
    }
}
