using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public CombatState state;
    public delegate void D_OriginalPosition();
    public event D_OriginalPosition d_originalPosition;

    public delegate void D_RemoveCardSorting();
    public D_RemoveCardSorting d_RemoveCardSorting;

    public delegate void D_AddCardSorting();
    public D_RemoveCardSorting d_AddCardSorting;

    private void Start()
    {

    }
    public void StateChanger(CombatState combatManagerState)
    {
        state = combatManagerState;
    }

    //called by combatManager
    public void ResetOriginal()
    {
        d_originalPosition();
    }

    public void RemoveCardSorting()
    {

    }

    public void AddCardSorting()
    {

    }
}
