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
    GameObject equipmentViewerObj;
    EquipmentViewer equipmentViewer;
    //the last child of the object which is the dragging space where in the currently dragged object will be a child in to allow free movement
    GameObject draggingSpace;
    DragMovementDummyScript draggingSpaceScript;
    //the parent's EquipmentInventoryScript
    EquipmentInventoryDrop equipmentInventoryDrop;
    GameObject equipmentInventoryObj;
    //the catcher for inventoryScreenOnDrop
    InventoryOnDropCatcher inventoryOnDrop;

    //this object's rectTransform
    RectTransform objectRect;

    //this object's GearSOHolder and the GearSO in it
    //accerssed by inv
    public GearSOHolder gearSOHolder;
    public GearSO gearSO;

    //enum identifiers if the object is from a slot or the inventory
    //public because the gear origin is checked by EquipmentInventoryDrop when moving gearSOs around
    public enum GearOrigin {equipSlot, inventoryScreen }
    public GearOrigin gearOrigin;
    //parameters used for returning
    //oublic because this is accessed by EquipmentInventoryDrop when moving GearSOs
    public Transform previousEquipmentSlot;
    //public because it will be accessed by the equippedGearSlot when replacing a gear and sending it back to the inventory
    public int previousInventoryIndex;

    //bool identifier that allows dragging and dropping
    //will only be called in overworld or maybe in some event
    bool isDragNDroppable;

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
        equipmentViewerObj = parentObject;
        equipmentViewer = equipmentViewerObj.GetComponent<EquipmentViewer>();
        this.canvasScale = canvasScale;
        this.draggingSpace = draggingSpace;
        //draggingSpaceScript = this.draggingSpace.GetComponent<DragMovementDummyScript>();
        this.inventoryOnDrop = inventoryOnDrop;

        //assigned parent equipment inventory screen
        equipmentInventoryObj = transform.parent.gameObject;
        equipmentInventoryDrop = equipmentInventoryObj.GetComponent<EquipmentInventoryDrop>();

        //the harbored GearSO, accessed by EquipmentInventoryDrop when calling the function to move gearSO lists
        gearSOHolder = gameObject.GetComponent<GearSOHolder>();
        gearSO = gearSOHolder.gearSO;

        //assigns the function for disabling the gear object
        equipmentViewer.d_DisableInventoryPrefabs += DisableInventoryPrefab;
        //assigns function for making the gear dragNDroppable
        //only activated when in overworld
        equipmentViewer.d_MakeGearsDragNDroppable += MakeGearDragNDroppable;

        //determine where the gear is
        DetermineGearOrigin();
    }

    //event function called by equipment viewer from overworld
    //makes the gear draggable and droppable
    void MakeGearDragNDroppable()
    {
        isDragNDroppable = true;
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
        if (isDragNDroppable)
        {
            //update origin info
            //must run first before switching parents so that the original index is saved for inventory origin logic
            DetermineGearOrigin();

            //check if the object is in a slot first, if so update the gearSlot identifier immediately
            if (gearOrigin == GearOrigin.equipSlot)
            {
                EquippedGearSlot equipSlot = previousEquipmentSlot.GetComponent<EquippedGearSlot>();
                equipSlot.UpdateGearSlotAvailability(false);
            }

            //checks if this gear was picked up from inventory or a gearslot
            transform.SetParent(draggingSpace.transform);
            canvasGroup.blocksRaycasts = false;

            //make the inventoryDropCatcher targetable by raycasts
            inventoryOnDrop.AlterOnDropCatcherRaycast(true);
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragNDroppable)
        {
            objectRect.anchoredPosition += eventData.delta / canvasScale;
        }


    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragNDroppable)
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

    //called by a deleaget from EquipmentViewer which is called by the proceed button
    void DisableInventoryPrefab()
    {
        //equipmentViewer.d_DisableInventoryPrefabs -= DisableInventoryPrefab;
        gameObject.SetActive(false);
    }

}
