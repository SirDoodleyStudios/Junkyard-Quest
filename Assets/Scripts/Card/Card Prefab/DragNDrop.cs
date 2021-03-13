using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragNDrop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    //cache for card's parent
    PlayerHand playerHand;
    //cache for card's parent CustomHandLayout
    CustomHandLayout customHandLayout;
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
    [SerializeField]Vector2 cardAnchor { get; set; }
    //original positions for reference when changing them during hover
    Quaternion OriginalOrientation { get; set; }
    Vector2 OriginalPosition { get; set; }

    //gameobject cache for TagDescription Holder, to be disabled during isDragging so that it wont show up during drag
    GameObject tagDescriptionHolder;
    //identifier for drag and end drag when to stop
    bool isDragging;


    //identifier if card is Dropped or targetted, if true, it can be dragged naturally
    CardMethod cardMethod;

    public void Awake()
    {
        //tagDescriptionHolder = gameObject.transform.GetChild(gameObject.transform.childCount).gameObject;

        //gets its own canvas to activate sorting when hovered on
        sortingCanvas = gameObject.GetComponent<Canvas>();
        //transform caches
        objectTransform = gameObject.transform;
        objectRect = gameObject.GetComponent<RectTransform>();
        handRect = gameObject.transform.parent.gameObject.GetComponent<RectTransform>();

        playerHand = objectTransform.GetComponentInParent<PlayerHand>();
        customHandLayout = objectTransform.GetComponentInParent<CustomHandLayout>();

        if (playerHand != null)
        {
            //I dont know why there are two but we only need one
            //assign the original position assigner to delegate in custom hand layout so that it's position can be reset everytime a rearrange is called
            objectTransform.parent.gameObject.GetComponent<PlayerHand>().d_FixOriginalPositions += AssignInitialPositions;
            //objectTransform.parent.gameObject.GetComponent<PlayerHand>().d_FixOriginalPositions += AssignInitialPositions;
            //objectTransform.parent.gameObject.GetComponent<PlayerHand>().d_ResetToDeckPosition += 
        }

    }


    public void Start()
    {

        //in here so that cards are drawn at original scale?
        //OriginalPosition = gameObject.transform.localPosition;
        OriginalScale = objectTransform.localScale;
        //resets position when in hand to prevent hovering showcase
        if (playerHand != null)
        {
            playerHand.d_originalPosition += PositionReset;
        }
        //automatically zooms object when in creative field
        else if (playerHand != null)
        {
            objectTransform.localScale = new Vector3(1.3f, 1.3f, objectTransform.localScale.z);
        }

    }

    void OnEnable()
    {
        //initially, isPositioned is false so that the hover logic is not initiated when drawing


        // everytime the prefab is enabled, determine if the card is a targetted or a  dropped card
        cardMethod = gameObject.GetComponent<Display>().card.cardMethod;

        //calls activate and deactivate popup methods in cardDescriptionLayout
        cardDescriptionLayout = gameObject.GetComponent<CardDescriptionLayout>();
    }

    //called by event in CustomHandLayout so that original poisitions are reset everytime a rearrange happens
    public void AssignInitialPositions()
    {
        if (gameObject.activeSelf)
        {
            cardAnchor = objectRect.anchoredPosition;
            OriginalPosition = cardAnchor;
            OriginalOrientation = objectTransform.rotation;
        }
        else
        {
            cardAnchor = new Vector2(0, 0);
            objectRect.anchoredPosition = cardAnchor;
            OriginalPosition = cardAnchor;
            OriginalOrientation = objectTransform.rotation;
        }

    }

    //called by deckmanager to make disabled cards go back to position 0
    public void ResetToDeckPosition()
    {

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

    //called when card effect is activated and return it to 0 position
    //this is for drawing from deck to hand animation purposes
    public void ReturnToDeckPosition()
    {
    
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        //Debug.Log(playerHand.state);
        //OriginalPosition = gameObject.transform.localPosition;
        if (playerHand != null && playerHand.state == CombatState.PlayerTurn)
        {
            //records original position, scale, and rotation first for reverting later on

            //actual setting of scale, rotation and position
            objectTransform.localScale = new Vector3(1.3f, 1.3f, objectTransform.localScale.z);
            objectRect.anchoredPosition = new Vector2(cardAnchor.x, 15);
            objectTransform.rotation = Quaternion.Euler(0,0,0);


            //sets zoomed card to showcase area
            //gameObject.GetComponent<Canvas>().sortingOrder = 1;
            sortingCanvas.overrideSorting = true;
            //gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, -11f);
            gameObject.GetComponent<BoxCollider2D>().enabled = true;

            //callsCustomHandLayout to rearrange 2 neighbors to the right and 2 negihbors to the left
            //the neighbors are to move away from the hovered card so that they are still visible
            customHandLayout.HoverRearrange(objectTransform.GetSiblingIndex());


            //prevents from showing up during drag
            if (!isDragging)
            {
                //shows popus per tag
                cardDescriptionLayout.EnablePopups();
            }
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {

        if (playerHand != null && playerHand.state == CombatState.PlayerTurn)
        {
            //customHandLayout.UnHoverRearrange();
            //ResetToAssignedPosition();

            objectRect.anchoredPosition = OriginalPosition;
            objectTransform.rotation = OriginalOrientation;
            objectTransform.localScale = OriginalScale;
            customHandLayout.UnHoverRearrange(objectTransform.GetSiblingIndex());

            //sets zoomed card to showcase area
            //gameObject.GetComponent<Canvas>().sortingOrder = 0;
            sortingCanvas.overrideSorting = false;
            //gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, (gameObject.transform.GetSiblingIndex() * -1));
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

        }
        //prevents popup from appearing when mouse is no longer hovered on card
        cardDescriptionLayout.DisablePopups();
    }    

    //originally in OnPointerExit but separated now to be able to acces this certain function from customHandLayout
    //when this is called, the card prefab gameobject will revert to it's assigned fix position
    // might not be needed
    public void ResetToAssignedPosition()
    {
        objectRect.anchoredPosition = OriginalPosition;
        objectTransform.rotation = OriginalOrientation;
        objectTransform.localScale = OriginalScale;
    }

    //on firs instance of click, assign event data so that we can pass it to OnDrag using ActivateSingleClickDrag
    public void OnPointerDown(PointerEventData eventData)
    {
        //removes popup after player clicks on card
        cardDescriptionLayout.DisablePopups();

        //can only do the dragging function during PlayerTurn
        //will allow to drag card until player rightclicks or left clocks a target
        if (cardMethod == CardMethod.Dropped)
        {
            isDragging = true;
        }





    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //dragging only works if its a dropped card
        //if (cardMethod == CardMethod.Dropped)
        //{

        //    gameObject.layer = 2;
        //}

    }
    //called by combatManager update to emulate a drag function
    public void ActivateSingleClickDrag()
    {
        if (isDragging)
        {

            //makes card ignore raycast so that combat manager can register target units
            if (cardMethod == CardMethod.Dropped)
            {
                gameObject.layer = 2;
                gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //tagDescriptionHolder.SetActive(false);

            }
        }

    }
    public void DeactivateDrag()
    {
        if (cardMethod == CardMethod.Dropped && isDragging)
        {
            Debug.Log("Beginning Drag");
            gameObject.layer = 15;
            isDragging = false;

            objectRect.anchoredPosition = OriginalPosition;
            objectTransform.rotation = OriginalOrientation;
            objectTransform.localScale = OriginalScale;
            //sets zoomed card to showcase area
            //gameObject.GetComponent<Canvas>().sortingOrder = 0;
            sortingCanvas.overrideSorting = false;
            //gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, (gameObject.transform.GetSiblingIndex() * -1));
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }       

    }

    public void OnDrag(PointerEventData eventData)
    {
        //dragging only works if its a dropped card
        //if (cardMethod == CardMethod.Dropped)
        //{
        //    gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //}

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //if(cardMethod == CardMethod.Dropped)
        //{
        //    Debug.Log("Beginning Drag");
        //    gameObject.layer = 15;
        //    isDragging = false;

        //    objectRect.anchoredPosition = OriginalPosition;
        //    objectTransform.rotation = OriginalOrientation;
        //    objectTransform.localScale = OriginalScale;
        //    //sets zoomed card to showcase area
        //    //gameObject.GetComponent<Canvas>().sortingOrder = 0;
        //    sortingCanvas.overrideSorting = false;
        //    //gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, (gameObject.transform.GetSiblingIndex() * -1));
        //    gameObject.GetComponent<BoxCollider2D>().enabled = false;
        //}

    }


    public void OnPointerClick(PointerEventData eventData)
    {
        //if(eventData.button == PointerEventData.InputButton.Left && state == CombatState.PlayerTurn)
        //{
        //    state = CombatState.ActiveCard;
        //}
        
    }
}
