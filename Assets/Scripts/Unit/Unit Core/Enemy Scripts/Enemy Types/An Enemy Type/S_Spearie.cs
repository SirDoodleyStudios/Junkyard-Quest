using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Spearie : EnemyActionsLogic
{
    public CardPool actionPool;

    private void Start()
    {
        //how many iterations of actions for this enemy
        cycleIndex = 2;
        //determines what action to do first
        actionIndex = Random.Range(0, cycleIndex);
        Debug.Log($"First action is {actionIndex}");


    }

    public override void EnemyAct()
    {
        //remove block at start of turn
        gameObject.GetComponent<BaseUnitFunctions>().RemoveBlock();
        switch (actionIndex)

        {
            //Direct deal damage
            case 0:
                TargetPlayer();
                DealDamage(10);
                break;

            //Direct deal damage
            case 1:
                TargetSelf();
                GainBlock(10);
                break;

            //Deal smaller damage but gain block
            case 2:
                TargetPlayer();
                DealDamage(7);

                TargetSelf();
                GainBlock(6);
                break;
        }
        //goes to next action next enemy turn
        actionIndex++;
        if (actionIndex > cycleIndex)
        {
            actionIndex = 0;
        }
        //removes block after turn
        
        Debug.Log($"Next action is {actionIndex}");


    }



}
