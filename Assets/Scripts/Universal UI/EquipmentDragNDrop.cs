using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentDragNDrop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    //will be used as scaleFactor for dragging later
    //assigned from InitiateEquipmrntDragNDrop
    float canvasScale;

    //canvas group that allows the dragged object to unblock raycast when ending drag
    CanvasGroup canvasGroup;

    //the content viewer parent
    GameObject parentObject;
    //the last child of the object which is the dragging space where in the currently dragged object will be a child in to allow free movement
    GameObject draggingSpace;
    //the parent's EquipmentInventoryScript
    EquipmentInventoryDrop equipmentInventoryDrop;
    GameObject equipmentInventoryObj;
    //the catcher for inventoryScreenOnDrop
    InventoryOnDropCatcher inventoryOnDrop;

    //this object's rectTransform
    RectTransform objectRect;

    //enum identifiers if the object is from a slot or the inventory
    enum GearOrigin {equipSlot, inventoryScreen }
    GearOrigin gearOrigin;
    //parameters used for returning
    Transform previousEquipmentSlot;
    int previousInventoryIndex;

    private void Awake()
    {
        
    }

    //initiates object assignments
    //called by EquipmentViewer
    //acts as awake but the necessary editor objects are already provided
    public void InitiateEquipmentDragNDrop(GameObject parentObject, GameObject draggingSpace, InventoryOnDropCatcher inventoryOnDrop, float canvasScale)
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        objectRect = gameObject.GetComponent<RectTransform>();
        this.parentObject = parentObject;
        this.canvasScale = canvasScale;
        this.draggingSpace = draggingSpace;
        this.inventoryOnDrop = inventoryOnDrop;

        //assigned parent equipment inventory screen
        equipmentInventoryObj = transform.parent.gameObject;
        equipmentInventoryDrop = equipmentInventoryObj.GetComponent<EquipmentInventoryDrop>();

        //determine where the gear is
        DetermineGearOrigin();
    }
    //determines where the gear is placed
    //called at initialize and by gearSlot and EquipmentInventoryDrop and by firs initialization
    //PENDING FUNCTIONALITY CHANGE, WE CAN JUST CALL THIS ONLY DURING INITIALIZE AND ONBEGIN DRAG BUT MAYBE WE'LL NEED THE CALLS FROM GEARSLOT AND INVENTORY SCRIPTS
    public void DetermineGearOrigin()
    {
        //determines if from inventory
        if (transform.parent.GetComponent<EquipmentInventoryDrop>()!=null)
        {
            gearOrigin = GearOrigin.inventoryScreen;
            previousInventoryIndex = transform.GetSiblingIndex();
        }
        //determines if in equip slot
        else if (transform.parent.GetComponent<EquippedGearSlot>()!=null)
        {
            gearOrigin = GearOrigin.equipSlot;
            previousEquipmentSlot = transform.parent;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //update origin info
        //must run first before switching parents so that the original index is saved for inventory origin logic
        DetermineGearOrigin();

        //checks if this gear was picked up from inventory or a gearslot
        transform.SetParent(draggingSpace.transform);
        canvasGroup.blocksRaycasts = false;

        //make the inventoryDropCatcher targetable by raycasts
        inventoryOnDrop.AlterOnDropCatcherRaycast(true);



    }

    public void OnDrag(PointerEventData eventData)
    {
        objectRect.anchoredPosition += eventData.delta / canvasScale;

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        //call the EquipmentInventoryDrop script to add the object to inventory screen
        //checks first if the drag ended at the side of the inventory screen
        //if (eventData.position.x > Screen.width/2)
        //{
        //    equipmentInventoryDrop.PlaceGearInInventory(eventData.pointerDrag, eventData.position.y);
        //}

        //on endDrag, if gear is still in dragSpace, call the return Function
        if (transform.parent.GetComponent<DragMovementDummyScript>() != null)
        {
            ReturnGearToPreviousPlace(eventData);
            inventoryOnDrop.AlterOnDropCatcherRaycast(false);
        }

    }
    public void OnPointerClick(PointerEventData eventData)
    {

    }

    //logic called when the drag of gear did not end on inventory screen or an equipment slot
    public void ReturnGearToPreviousPlace(PointerEventData eventData)
    {
        if (gearOrigin == GearOrigin.equipSlot)
        {
            //simulate the OnDrop of the gearSlot script
            EquippedGearSlot gearSlot = previousEquipmentSlot.GetComponent<EquippedGearSlot>();
            gearSlot.OnDrop(eventData);
            //transform.SetParent(previousEquipmentSlot);
        }
        else if (gearOrigin == GearOrigin.inventoryScreen)
        {
            //did not mimic the OnDrop because we're avoiding the calculations for correct Index in inventoryScreen
            transform.SetParent(equipmentInventoryObj.transform);
            transform.SetSiblingIndex(previousInventoryIndex);
            equipmentInventoryDrop.RepositionInventory();
        }
    }

}
