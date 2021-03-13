using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    //called every time DrawCards is called in DeckManager
    public delegate void D_FixOriginalPositions();
    public event D_FixOriginalPositions d_FixOriginalPositions;

    public CombatState state;

    //used in Position Reset in DragNDrop, might be outdated
    public delegate void D_OriginalPosition();
    public event D_OriginalPosition d_originalPosition;

    public delegate void D_RemoveCardSorting();
    public D_RemoveCardSorting d_RemoveCardSorting;

    public delegate void D_AddCardSorting();
    public D_RemoveCardSorting d_AddCardSorting;

    //for putting disabled cards in player hand to anchored position 0 so that they can be animated again from draw to position
    public delegate void D_ResetToDeckPosition();
    public event D_ResetToDeckPosition d_ResetToDeckPosition;

    private void Start()
    {

    }
    public void StateChanger(CombatState combatManagerState)
    {
        state = combatManagerState;
    }

    public void FixCardPositions()
    {
        d_FixOriginalPositions();
    }
    //called by combatManager
    public void ResetToDeckPosition()
    {
        //d_ResetToDeckPosition();
    }

    public void RemoveCardSorting()
    {

    }

    public void AddCardSorting()
    {

    }
}
