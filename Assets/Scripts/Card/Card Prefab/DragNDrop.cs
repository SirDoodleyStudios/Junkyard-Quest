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
    Transform objecTransform;


    public void Start()
    {
        //gets its own canvas to activate sorting when hovered on
        sortingCanvas = gameObject.GetComponent<Canvas>();

        objecTransform = gameObject.transform;

        //calls activate and deactivate popup methods in cardDescriptionLayout
        cardDescriptionLayout = gameObject.GetComponent<CardDescriptionLayout>();

        //OriginalPosition = gameObject.transform.localPosition;
        OriginalScale = objecTransform.localScale;
        //resets position when in hand to prevent hovering showcase
        if (objecTransform.GetComponentInParent<PlayerHand>() != null)
        {
            objecTransform.GetComponentInParent<PlayerHand>().d_originalPosition += PositionReset;
        }
        //automatically zooms object when in creative field
        else if (objecTransform.GetComponentInParent<CreativeManager>() != null)
        {
            objecTransform.localScale = new Vector3(1.3f, 1.3f, objecTransform.localScale.z);
        }



        //state = CombatState.PlayerTurn;
    }
    //public void OnEnable()
    //{
    //    OriginalPosition = gameObject.transform.localPosition;
    //    OriginalScale = gameObject.transform.localScale;
    //    //resets position when in hand to prevent hovering showcase
    //    if (gameObject.transform.GetComponentInParent<PlayerHand>() != null)
    //    {
    //        gameObject.transform.GetComponentInParent<PlayerHand>().d_originalPosition += PositionReset;
    //    }
    //    //automatically zooms object when in creative field
    //    else if (gameObject.transform.GetComponentInParent<CreativeManager>() != null)
    //    {
    //        gameObject.transform.localScale = new Vector3(1.3f, 1.3f, gameObject.transform.localScale.z);
    //    }
    //}

    //gets called by combatmanager
    //public void StateChanger(CombatState tempstate)
    //{
    //    state = tempstate;
    //    //removes zoom effect from card if rightclicked from combatmanager
    //    if(state == CombatState.PlayerTurn)
    //    {
    //        gameObject.transform.localScale = OriginalScale;
    //    }
    //}

    //public void StateChanger(CombatState combatManagerState)
    //{
    //    if (gameObject.transform.GetComponentInParent<PlayerHand>().state == CombatState.PlayerTurn)
    //    {
    //        gameObject.transform.localScale = OriginalScale;
    //    }
    //}

    //called by playerHand for reseting when action is cancelled
    public void PositionReset()
    {
        objecTransform.localScale = OriginalScale;
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
        if (objecTransform.GetComponentInParent<PlayerHand>() != null &&
            objecTransform.GetComponentInParent<PlayerHand>().state == CombatState.PlayerTurn)
        {
            objecTransform.localScale = new Vector3(1.3f, 1.3f, objecTransform.localScale.z);
            
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
        
        if (objecTransform.GetComponentInParent<PlayerHand>() != null &&
            objecTransform.GetComponentInParent<PlayerHand>().state == CombatState.PlayerTurn)
        {
            objecTransform.localScale = OriginalScale;
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
