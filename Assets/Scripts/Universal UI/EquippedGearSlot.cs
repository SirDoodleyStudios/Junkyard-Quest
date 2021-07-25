using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class EquippedGearSlot : MonoBehaviour, IDropHandler
{
    //reference to the equipment Manager
    //assigned in editor because it's too far and equipment slots are fixed anyway
    public EquipmentInventoryDrop equipmentInventoryDrop;
    private void Awake()
    {
        
    }
    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject.CompareTag("Gear"))
        {
            draggedObject.transform.SetParent(transform);
            //assign anchors in center then centralize position
            RectTransform draggedRect = draggedObject.GetComponent<RectTransform>();
            //give a tweening snap animation
            draggedRect.DOAnchorPos(new Vector2(0, 0), .1f, false);
            //draggedRect.anchoredPosition = new Vector2(0, 0);

            //call the resize and reposition function in the inventory parent
            StartCoroutine(equipmentInventoryDrop.ResizeInventoryScreen());
        }

        //the dragged EquipmentDragNDrop, calls the dragged gear to change it's origin enum
        EquipmentDragNDrop equippedDragged = draggedObject.GetComponent<EquipmentDragNDrop>();
        equippedDragged.DetermineGearOrigin();

    }
}
