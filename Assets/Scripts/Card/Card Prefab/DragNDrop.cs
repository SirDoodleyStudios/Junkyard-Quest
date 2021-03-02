using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragNDrop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    //Vector3 OriginalPosition;
    Vector3 OriginalScale;
    Canvas sortingCanvas;
    CardDescriptionLayout cardDescriptionLayout;
    //cache of card's transform
    Transform objectTransform;
    //cache of card's and hand's rectTransform
    RectTransform objectRect;
    RectTransform handRect;
    //cache for card object's anchored position
    Vector2 cardAnchor { get; set; }
    //original positions for reference when changing them during hover
    Quaternion OriginalOrientation { get; set; }
    Vector2 OriginalPosition { get; set; }

    public void Awake()
    {
        //gets its own canvas to activate sorting when hovered on
        sortingCanvas = gameObject.GetComponent<Canvas>();
        //transform caches
        objectTransform = gameObject.transform;
        objectRect = gameObject.GetComponent<RectTransform>();
        handRect = gameObject.transform.parent.gameObject.GetComponent<RectTransform>();

        if (objectTransform.GetComponentInParent<PlayerHand>() != null)
        {
            //assign the original position assigner to delegate in custom hand layout so that it's position can be reset everytime a rearrange is called
            objectTransform.parent.gameObject.GetComponent<CustomHandLayout>().d_FixOriginalPositions += AssignInitialPositions;
        }

    }


    public void Start()
    {
        //calls activate and deactivate popup methods in cardDescriptionLayout
        cardDescriptionLayout = gameObject.GetComponent<CardDescriptionLayout>();
        //in here so that cards are drawn at original scale?
        //OriginalPosition = gameObject.transform.localPosition;
        OriginalScale = objectTransform.localScale;
        //resets position when in hand to prevent hovering showcase
        if (objectTransform.GetComponentInParent<PlayerHand>() != null)
        {
            objectTransform.GetComponentInParent<PlayerHand>().d_originalPosition += PositionReset;
        }
        //automatically zooms object when in creative field
        else if (objectTransform.GetComponentInParent<CreativeManager>() != null)
        {
            objectTransform.localScale = new Vector3(1.3f, 1.3f, objectTransform.localScale.z);
        }



        //state = CombatState.PlayerTurn;
    }

    //called by event in CustomHandLayout so that original poisitions are reset everytime a rearrange happens
    public void AssignInitialPositions()
    {
        cardAnchor = objectRect.anchoredPosition;
        OriginalPosition = cardAnchor;
        OriginalOrientation = objectTransform.rotation;
    }


    //called by playerHand for reseting when action is cancelled
    public void PositionReset()
    {
        objectRect.anchoredPosition = OriginalPosition;
        objectTransform.rotation = OriginalOrientation;
        objectTransform.localScale = OriginalScale;
        //this is a hack, assigns a Z position based on index, so child 0 will have 0 z, child 1 has -1 z and 2 will have -2
        // this allows each card will stack in from of each other in a proper manner from left to right
        //gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, (gameObject.transform.GetSiblingIndex()*-1));
        //removes sorting canvas of all cards in hand so hat stacking relies on heirarchy
        sortingCanvas.overrideSorting = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        //gameObject.GetComponent<Canvas>().sortingOrder = 0;
        //gameObject.transform.localPosition = OriginalPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {


        //OriginalPosition = gameObject.transform.localPosition;
        if (objectTransform.GetComponentInParent<PlayerHand>() != null &&
            objectTransform.GetComponentInParent<PlayerHand>().state == CombatState.PlayerTurn)
        {
            //records original position, scale, and rotation first for reverting later on

            //card object anchored position, it's cached here so that the original position is always reset at hover
            //cardAnchor = objectRect.anchoredPosition;
            //OriginalPosition = cardAnchor;
            //OriginalOrientation = objectTransform.rotation;

            //actual setting of scale, rotation and position
            objectTransform.localScale = new Vector3(1.3f, 1.3f, objectTransform.localScale.z);
            objectRect.anchoredPosition = new Vector2(cardAnchor.x, 15);
            objectTransform.rotation = Quaternion.Euler(0,0,0);


            //sets zoomed card to showcase area
            //gameObject.GetComponent<Canvas>().sortingOrder = 1;
            sortingCanvas.overrideSorting = true;
            //gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, -11f);
            gameObject.GetComponent<BoxCollider2D>().enabled = true;

        }
        //shows popus per tag
        cardDescriptionLayout.EnablePopups();


    }

    public void OnPointerExit(PointerEventData eventData)
    {



        if (objectTransform.GetComponentInParent<PlayerHand>() != null &&
            objectTransform.GetComponentInParent<PlayerHand>().state == CombatState.PlayerTurn)
        {

            objectRect.anchoredPosition = OriginalPosition;
            objectTransform.rotation = OriginalOrientation;
            objectTransform.localScale = OriginalScale;
            //sets zoomed card to showcase area
            //gameObject.GetComponent<Canvas>().sortingOrder = 0;
            sortingCanvas.overrideSorting = false;
            //gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, (gameObject.transform.GetSiblingIndex() * -1));
            gameObject.GetComponent<BoxCollider2D>().enabled = false;


        }
        //prevents popup from appearing when mouse is no longer hovered on card
        cardDescriptionLayout.DisablePopups();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        //removes popup after player clicks on card
        cardDescriptionLayout.DisablePopups();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if(eventData.button == PointerEventData.InputButton.Left && state == CombatState.PlayerTurn)
        //{
        //    state = CombatState.ActiveCard;
        //}
        
    }
}
