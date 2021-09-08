using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//attached to the fixed prefabs in the material and gear Draft prefab
public class MaterialDragNDrop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //transform sizes will be manipulated by mouse hovers
    RectTransform objRect;

    //to call the drafting script when the object is clicked
    MaterialDrafting materialDrafting;

    //the gear or materialSO of the current object
    CraftingMaterialSOHolder materialSOHolder;

    CraftingMaterialSO materialSO;

    private void Awake()
    {
        //calls activate and deactivate popup methods in cardDescriptionLayout
        objRect = gameObject.GetComponent<RectTransform>();
        materialDrafting = transform.parent.parent.GetComponent<MaterialDrafting>();
        //immediately assign the materialSO
        materialSOHolder = gameObject.GetComponent<CraftingMaterialSOHolder>();
        materialSO = materialSOHolder.craftingMaterialSO;
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
        materialDrafting.AddToInventory(materialSO);
    }
}
