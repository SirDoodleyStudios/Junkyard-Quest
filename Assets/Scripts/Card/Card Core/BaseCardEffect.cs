﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseCardEffect
{
    //Scanned by EffectFactory for identifying which Effect class to activate
    public abstract AllCards enumKeyCard { get; }
    //Scanned by Effect factory when  enumKeyName is Jigsaw
    //public abstract AllJigsaws enumKeyJigsaw { get; }

    //generic cache for target Game Object
    protected GameObject targetObject;
    //cache for playing field, will get initialized if AffectPlayingField is called
    PlayingField targetPlayingField;
    //cache for BaseUnitFunctions applicable to all units, will get initialized
    BaseUnitFunctions targetUnit;

    //determines if DealDamage() will affect a single unit or the children of the enemy holder
    protected bool AOE;

    protected int damage;
    protected int block;
    protected int hits;
    protected int multiplier;
    protected int draw;
    protected int discard;
    protected int creativity;
    protected int energy;


    //Activator Function
    public abstract void CardEffectActivate(GameObject target);

    //set target to actual choice target
    public  void AffectSingleEnemy(GameObject target)
    {
        targetObject = target;
        targetUnit = targetObject.GetComponent<BaseUnitFunctions>();
    }

    //set target to player when card is dropped
    //Child index 1 is player
    public void AffectPlayer(GameObject target)
    {
        if (target.tag == "Player")
        {
            targetObject = target;
            targetUnit = targetObject.GetComponent<BaseUnitFunctions>();
        }
        else if (target.tag == "Enemy")
        {
            //gets parent enemy holder then gets parent playing field
            GameObject enemyHolder = target.transform.parent.gameObject;
            targetObject = enemyHolder.transform.parent.gameObject.transform.GetChild(1).gameObject;
            targetUnit = targetObject.GetComponent<BaseUnitFunctions>();
        }
        else
        {
            targetObject = target.transform.parent.GetChild(1).gameObject;
            targetUnit = targetObject.GetComponent<BaseUnitFunctions>();
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
            targetObject = enemyHolder.transform.parent.gameObject;
            targetUnit = targetObject.GetComponent<BaseUnitFunctions>();
        }
        else
        {
            //child index 2 is the enemy holder
            targetObject = target.transform.parent.GetChild(2).gameObject;
            targetUnit = targetObject.GetComponent<BaseUnitFunctions>();
        }
        //for dropfield approach
        //child index 2 is the enemy holder
        //targetUnit = target.transform.parent.GetChild(2).gameObject;

    }

    //for effects that affects combat like draw and energy gain
    public void AffectPlayingField(GameObject target)
    {
        if (target.tag == "Enemy")
        {
            //gets parent enemy holder then gets parent playing field
            GameObject enemyHolder = target.transform.parent.gameObject;
            targetObject = enemyHolder.transform.parent.gameObject;
            targetPlayingField = targetObject.GetComponent<PlayingField>();
        }
        else
        {
            targetObject = target.transform.parent.gameObject;
            targetPlayingField = targetObject.GetComponent<PlayingField>();
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
                targetUnit.TakeDamage(damage);
            }
        }
        else
        {
            foreach (Transform enemy in targetObject.transform)
            {
                //cache override from AffectAll enemies
                targetUnit = enemy.gameObject.GetComponent<BaseUnitFunctions>();
                for (int i = 1; hits >= i; i++)
                {
                    targetUnit.TakeDamage(damage);
                }
            }
            AOE = false;
        }        
    }
    

    public void GainBlock()
    {
        targetUnit.GainBlock(block);
    }

    public void DrawCard()
    {
        targetPlayingField.deckManager.DrawCards(draw);
    }

    public void DiscardCard()
    {

    }

    public void AlterCreativity()
    {
        targetPlayingField.playerPrefab.GetComponent<PlayerFunctions>().AlterPlayerCreativity(creativity);

    }

    public void AlterEnergy()
    {
        targetPlayingField.combatManager.EnergyUpdater(energy);
    }

    public void GainAbility()
    {

    }

    







}
