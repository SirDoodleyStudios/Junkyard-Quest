using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseCardEffect
{
    //Scanned by EffectFactory for identifying which Effect class to activate
    public abstract AllCards enumKeyCard { get; }
    //Scanned by Effect factory when  enumKeyName is Jigsaw
    //public abstract AllJigsaws enumKeyJigsaw { get; }

    protected GameObject targetUnit;
    //determines if DealDamage() will affect a single unit or the children of the enemy holder
    protected bool AOE;

    protected int damage;
    protected int block;
    protected int hits;
    protected int multiplier;
    protected int draw;

    public abstract void CardEffectActivate(GameObject target);

    //set target to actual choice target
    public  void AffectSingleEnemy(GameObject target)
    {
        targetUnit = target;
    }

    //set target to player when card is dropped
    //Child index 1 is player
    public void AffectPlayer(GameObject target)
    {
        if (target.tag == "Player")
        {
            targetUnit = target;
        }
        if (target.tag == "Enemy")
        {
            //gets parent enemy holder then gets parent playing field
            GameObject enemyHolder = target.transform.parent.gameObject;
            targetUnit = enemyHolder.transform.parent.gameObject.transform.GetChild(1).gameObject;
        }
        else
        {
            targetUnit = target.transform.parent.GetChild(1).gameObject;
        }

    }

    //set all enemies as tagets when card is dropped
    //TargetUnit will be the EnemyHolder
    //CHANGE LOGIC, THIS IS WRONG
    public void AffectAllEnemies(GameObject target)
    {
        //sets AOE effect
        AOE = true;
        if (target.tag == "Enemy")
        {
            //gets parent enemy holder then gets parent playing field
            GameObject enemyHolder = target.transform.parent.gameObject;
            targetUnit = enemyHolder.transform.parent.gameObject;
        }
        else
        {
            //child index 2 is the enemy holder
            targetUnit = target.transform.parent.GetChild(2).gameObject;
        }
        //for dropfield approach
        //child index 2 is the enemy holder
        //targetUnit = target.transform.parent.GetChild(2).gameObject;

    }

    public void AffectPlayingField(GameObject target)
    {
        if (target.tag == "Enemy")
        {
            //gets parent enemy holder then gets parent playing field
            GameObject enemyHolder = target.transform.parent.gameObject;
            targetUnit = enemyHolder.transform.parent.gameObject;
        }
        else
        {
            targetUnit = target.transform.parent.gameObject;
        }
        //for dropfield approach
        //targetUnit = target.transform.parent.gameObject;

    }

    //damage needds to be populated
    //hits needs to be populated
    public void DealDamage()
    {
        if(AOE == false)
        {
            for (int i = 1; hits >= i; i++)
            {
                targetUnit.GetComponent<BaseUnitFunctions>().TakeDamage(damage);
            }
        }
        else
        {
            foreach (Transform enemy in targetUnit.transform)
            {
                for (int i = 1; hits >= i; i++)
                {
                    enemy.gameObject.GetComponent<BaseUnitFunctions>().TakeDamage(damage);
                }
            }
            AOE = false;
        }        
    }
    

    public void GainBlock()
    {
        targetUnit.GetComponent<BaseUnitFunctions>().GainBlock(block);
    }

    public void DrawCard()
    {
        targetUnit.GetComponent<PlayingField>().deckManager.DrawCards(draw);
    }







}
