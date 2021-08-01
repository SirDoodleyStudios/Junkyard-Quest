using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class EquippedGearSlot : MonoBehaviour, IDropHandler
{
    //assigned in editor because it's too far and equipment slots are fixed anyway
    //reference to the equipment Manager
    public EquipmentViewer equipmentViewer;
    //the space for the gear inventory
    public Transform equipemntInventoryTrans;
    public EquipmentInventoryDrop equipmentInventoryDrop;
    //holds the acceptable Gear classification for this slot
    public AllGearClassifications gearClassification;
    

    //identifier if slot is already equipped
    bool isEquipped;

    //the currently equipped gear in slot
    GameObject equippedGear;
    GearSO equippedGearSO;

    private void Awake()
    {
        //checks what transform sibling the slot is in then assignes the Gear Classification limitation for it
        int slotIndex = transform.GetSiblingIndex();
        switch (slotIndex)
        {
            case 0:
                gearClassification = AllGearClassifications.MainHand;
                break;
            case 1:
                gearClassification = AllGearClassifications.OffHand;
                break;
            case 2:
                gearClassification = AllGearClassifications.Helm;
                break;
            case 3:
                gearClassification = AllGearClassifications.Armor;
                break;
            case 4:
                gearClassification = AllGearClassifications.Belt;
                break;
            case 5:
                gearClassification = AllGearClassifications.Trinket;
                break;
            default:
                break;
        }



    }

    //for initiating the gearSlot if there is already a gear equipped at open
    //only called one time
    public void InitiateGearSlot()
    {
        if (transform.childCount == 4)
        {
            isEquipped = true;
            equippedGear = transform.GetChild(3).gameObject;
            equippedGearSO = equippedGear.GetComponent<GearSOHolder>().gearSO;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        GearSO draggedGearSO = draggedObject.GetComponent<GearSOHolder>().gearSO;


        //if object dragged has a gear tag in prefab and if their gear classifications match, it will trigger the onDrop logic
        if (draggedObject.CompareTag("Gear") && draggedGearSO.gearClassifications == gearClassification)
        {


            draggedObject.transform.SetParent(transform);
            //assign anchors in center then centralize position
            RectTransform draggedRect = draggedObject.GetComponent<RectTransform>();
            //give a tweening snap animation
            draggedRect.DOAnchorPos(new Vector2(0, 0), .1f, false);
            //draggedRect.anchoredPosition = new Vector2(0, 0);

            //call the function return the currently equipped object back to inventory using the index number of the new gear
            //then we leave the identifier as true
            if (isEquipped)
            {
                //current equipped gear's return to inventory is processed first before replacement
                ReplaceGear(draggedObject);

                //assign the dragged object as this slot's equipped object
                equippedGear = draggedObject;
                equippedGearSO = draggedGearSO;
            }
            //simply assign object and update identifier to true if the identifier is false
            else
            {
                //assign the dragged object as this slot's equipped object
                equippedGear = draggedObject;
                equippedGearSO = draggedGearSO;
                UpdateGearSlotAvailability(true);
            }

            //move the GearSO to the slot Array
            equipmentViewer.MoveGearSOToSlot(draggedGearSO, equipemntInventoryTrans.GetSiblingIndex());

            //call the resize and reposition function in the inventory parent
            StartCoroutine(equipmentInventoryDrop.ResizeInventoryScreen());
        }

        //the dragged EquipmentDragNDrop, calls the dragged gear to change it's origin enum
        EquipmentDragNDrop equippedDragged = draggedObject.GetComponent<EquipmentDragNDrop>();


        equippedDragged.DetermineGearOrigin();

    }

    //if there is already an object under the slot, remove that object then replace with the OnDrop
    //the parameter is the gear that is about to replace the old one
    void ReplaceGear(GameObject replacementGear)
    {
        //get the sibling index of the replacement gear, it will be used as the assigned sibling index for the to be replaced gear
        int replacementIndex = replacementGear.GetComponent<EquipmentDragNDrop>().previousInventoryIndex;

        //the target here is the current equipped gear before being replaced with a new onDrop
        Transform equippedGearTrans = equippedGear.transform;
        equippedGearTrans.SetParent(equipemntInventoryTrans);
        equippedGearTrans.SetSiblingIndex(replacementIndex);

        //call the equipmentViewer to reassign the replaced gear back to the inventory gearSOList
        equipmentViewer.MoveGearSOToInventory(equippedGearSO, transform.GetSiblingIndex());

    }


    //called by the gearSlot Ondrop and the EquipmentDragNDrop OnEndDrag
    //bool parameter dictates the isEquipped value
    public void UpdateGearSlotAvailability(bool equipState)
    {
        isEquipped = equipState;
    }
}


