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

    //the prefab itself
    GameObject parentObject;
    //the last child of the object which is the dragging space where in the currently dragged object will be a child in to allow free movement
    GameObject draggingSpace;

    //this object's rectTransform
    RectTransform objectRect;

    private void Awake()
    {
        
    }

    //initiates object assignments
    //called by EquipmentViewer
    //acts as awake but the necessary editor objects are already provided
    public void InitiateEquipmentDragNDrop(GameObject parentObject, GameObject draggingSpace, float canvasScale)
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        objectRect = gameObject.GetComponent<RectTransform>();
        this.parentObject = parentObject;
        this.canvasScale = canvasScale;
        this.draggingSpace = draggingSpace;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(draggingSpace.transform);
        canvasGroup.blocksRaycasts = false;
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        objectRect.anchoredPosition += eventData.delta / canvasScale;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
    }
    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
