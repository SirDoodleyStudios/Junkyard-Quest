using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragNDropDraft : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    CardDescriptionLayout cardDescriptionLayout;
    RectTransform cardRect;
    CardDrafting cardDrafting;

    private void Start()
    {
        //calls activate and deactivate popup methods in cardDescriptionLayout
        cardDescriptionLayout = gameObject.GetComponent<CardDescriptionLayout>();
        cardRect = gameObject.GetComponent<RectTransform>();
        cardDrafting = gameObject.GetComponent<CardDrafting>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        cardDescriptionLayout.EnablePopups();
        cardRect.localScale = new Vector2(1.3f, 1.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardDescriptionLayout.DisablePopups();
        cardRect.localScale = new Vector2(1f, 1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("add to deck");
    }
}
