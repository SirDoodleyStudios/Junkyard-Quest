using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//attached to the fixed prefabs in the material and gear Draft prefab
public class GearDragNDrop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //transform sizes will be manipulated by mouse hovers
    RectTransform objRect;

    //to call the drafting script when the object is clicked
    GearDrafting gearDrafting;

    //the gear or materialSO of the current object
    GearSOHolder gearSOHolder;

    GearSO gearSO;

    private void Awake()
    {
        //calls activate and deactivate popup methods in cardDescriptionLayout
        objRect = gameObject.GetComponent<RectTransform>();
        gearDrafting = transform.parent.parent.GetComponent<GearDrafting>();
        //immediately assign the materialSO
        gearSOHolder = gameObject.GetComponent<GearSOHolder>();
        gearSO = gearSOHolder.gearSO;
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
        gearDrafting.AddToInventory(gearSO);

    }
}
