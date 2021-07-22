using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquippedGearSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        draggedObject.transform.SetParent(transform);
        //assign anchors in center then centralize position
        RectTransform draggedRect = draggedObject.GetComponent<RectTransform>();
        draggedRect.anchorMin = new Vector2(.5f, .5f);
        draggedRect.anchorMax = new Vector2(.5f, .5f);
        draggedRect.anchoredPosition = new Vector2(0, 0);

    }
}
