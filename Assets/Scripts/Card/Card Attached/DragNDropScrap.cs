using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragNDropScrap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    RectTransform cardRect;
    ScrapsDrafting scrapsDrafting;

    private void Awake()
    {
        //calls activate and deactivate popup methods in cardDescriptionLayout
        cardRect = gameObject.GetComponent<RectTransform>();
        scrapsDrafting = transform.parent.parent.GetComponent<ScrapsDrafting>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        cardRect.localScale = new Vector2(1.3f, 1.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardRect.localScale = new Vector2(1f, 1f);
    }

    //call scrapsDrafting to add the value depending on the index of the chosen card
    public void OnPointerClick(PointerEventData eventData)
    {
        scrapsDrafting.ClaimScraps(transform.GetSiblingIndex());
    }
}
