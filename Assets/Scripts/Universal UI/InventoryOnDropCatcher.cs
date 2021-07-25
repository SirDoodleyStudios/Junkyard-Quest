using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class InventoryOnDropCatcher : MonoBehaviour, IDropHandler
{
    //assigned in editor
    public EquipmentInventoryDrop equipmentInventoryDrop;
    //reference that will contain the raycast target, contained in gameObject, raycast is default false
    public Image inventoryCover;


    public void OnDrop(PointerEventData eventData)
    {
        //call the EquipmentInventoryDrop script to add the object to inventory screen
        //checks first if the drag ended at the side of the inventory screen
        if (eventData.position.x > Screen.width / 2)
        {
            equipmentInventoryDrop.PlaceGearInInventory(eventData.pointerDrag, eventData.position.y);
        }

        //turn off raycast when not dragging anything
        AlterOnDropCatcherRaycast(false);
    }

    //called also from EquipmentDragNDrop in the OnBeginDrag
    public void AlterOnDropCatcherRaycast(bool identifier)
    {
        inventoryCover.raycastTarget = identifier;
    }
}
