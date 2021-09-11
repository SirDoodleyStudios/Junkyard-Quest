using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlueprintDragNDrop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //transform sizes will be manipulated by mouse hovers
    RectTransform objRect;

    //to call the drafting script when the object is clicked
    BlueprintDrafting blueprintDrafting;

    //the gear or materialSO of the current object
    BluePrintSOHolder blueprintSOholder;

    BluePrintSO blueprintSO;

    private void Awake()
    {
        //calls activate and deactivate popup methods in cardDescriptionLayout
        objRect = gameObject.GetComponent<RectTransform>();
        blueprintDrafting = transform.parent.parent.GetComponent<BlueprintDrafting>();
        //immediately assign the materialSO
        blueprintSOholder = gameObject.GetComponent<BluePrintSOHolder>();
        blueprintSO = blueprintSOholder.blueprintSO;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        objRect.localScale = new Vector2(1.3f, 1.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        objRect.localScale = new Vector2(1, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        blueprintDrafting.AddToInventory(blueprintSO);
    }
}
