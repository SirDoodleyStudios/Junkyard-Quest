using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragNDrop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    Vector3 OriginalPosition;
    Vector3 OriginalScale;

    Collider2D boxCollide;


    public void Start()
    {
        OriginalPosition = gameObject.transform.localPosition;
        OriginalScale = gameObject.transform.localScale;
        //resets position when in hand to prevent hovering showcase
        if (gameObject.transform.GetComponentInParent<PlayerHand>() != null)
        {
            gameObject.transform.GetComponentInParent<PlayerHand>().d_originalPosition += PositionReset;
        }
        //automatically zooms object when in creative field
        else if (gameObject.transform.GetComponentInParent<CreativeManager>() != null)
        {
            gameObject.transform.localScale = new Vector3(1.3f, 1.3f, gameObject.transform.localScale.z);
        }
        
        //state = CombatState.PlayerTurn;
    }

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
        gameObject.transform.localScale = OriginalScale;
        gameObject.GetComponent<Canvas>().sortingOrder = 0;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject.transform.GetComponentInParent<PlayerHand>() != null &&
            gameObject.transform.GetComponentInParent<PlayerHand>().state == CombatState.PlayerTurn)
        {
            gameObject.transform.localScale = new Vector3(1.3f, 1.3f, gameObject.transform.localScale.z);
            //sets zoomed card to showcase area
            gameObject.GetComponent<Canvas>().sortingOrder = 1;
            gameObject.GetComponent<BoxCollider2D>().enabled = true;

        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        if (gameObject.transform.GetComponentInParent<PlayerHand>() != null &&
            gameObject.transform.GetComponentInParent<PlayerHand>().state == CombatState.PlayerTurn)
        {
            gameObject.transform.localScale = OriginalScale;
            //sets zoomed card to showcase area
            gameObject.GetComponent<Canvas>().sortingOrder = 0;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {

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
