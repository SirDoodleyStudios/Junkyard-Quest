using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public CombatState state;
    public delegate void D_OriginalPosition();
    public event D_OriginalPosition d_originalPosition;

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
}
