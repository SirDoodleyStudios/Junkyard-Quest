using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CombatSaveState
{
    //player related combat info
    public List<AllCards> initialDeck = new List<AllCards>();
    public List<AllCards> battleDeck = new List<AllCards>();
    public List<AllCards> playerHandList = new List<AllCards>();
    public List<AllCards> discardPile = new List<AllCards>();
    public List<AllCards> consumePile = new List<AllCards>();

    //same playerUnitStats should be in UniversalInformation except for current creativity
    //player unit has a wrapper class available ion UniversalSaveState with constructor parameter PlayerUnit
    public PlayerUnit playerUnitStats;
    public PlayerUnitWrapper playerStatsWrapper;
    public UnitStatusHolder playerStatuses;

    //enemy related info
    public List<EnemyUnitStatsWrapper> enemyUnitWrappers = new List<EnemyUnitStatsWrapper>();
    public List<UnitStatusHolder> enemyListStatuses = new List<UnitStatusHolder>();


    public CombatSaveState(DeckManager deckManager, PlayerUnit playerUnit, List<GameObject> enemyObjects)
    {
        //For Enemy Statuses
        //each gameObject is an enemy, then we extract the enemyUnit in the Onbject's enemy functions
        foreach (GameObject enemyObject in enemyObjects)
        {
            EnemyFunctions enemyFunctions = enemyObject.GetComponent<EnemyFunctions>();
            EnemyUnitStatsWrapper enemyWrapper = new EnemyUnitStatsWrapper(enemyFunctions);
            enemyUnitWrappers.Add(enemyWrapper);
        }

        //For Player Cards saving
        foreach (Card card in deckManager.battleDeck)
        {
            battleDeck.Add(card.enumCardName);
        }
        foreach(Card card in deckManager.discardPile)
        {
            discardPile.Add(card.enumCardName);
        }
        foreach (Card card in deckManager.consumePile)
        {
            consumePile.Add(card.enumCardName);
        }
        foreach (Card card in deckManager.playerHandList)
        {
            playerHandList.Add(card.enumCardName);
        }
        //add everythin for initialDeck
        initialDeck.AddRange(battleDeck);
        initialDeck.AddRange(discardPile);
        initialDeck.AddRange(consumePile);
        initialDeck.AddRange(playerHandList);

        //for Player variables
        //player unit has a wrapper class in universalSaveState
        playerUnitStats = playerUnit;

    }
}

//wrapper classes
//enemy stats wrapper, will contain information for EnemyUnit, EnemyFunctions, EnemyActionFormat and UnitStatusHolder
[Serializable]
public class EnemyUnitStatsWrapper
{
    //EnemyUnit variables
    public EnemyEnumName enemyEnumName;
    public int currHP;
    public int block;

    //contains EnemyActionFomats in the intentPanel 
    public List<EnemyActionFormat> intentPanelActions = new List<EnemyActionFormat>();
    //contains EnemyActionFormats in the actionPanel
    public List<EnemyActionFormat> actionPanelActions = new List<EnemyActionFormat>();
    //contains the current list of EnemyActionFormats in the enemy's deck
    //for this one, action deck should already be subtracted with the intent and actionPanelActions
    public List<EnemyActionFormat> currentActionDeck = new List<EnemyActionFormat>();



    public EnemyUnitStatsWrapper(EnemyFunctions enemyFunctions)
    {
        EnemyUnit enemyUnit = enemyFunctions.enemyUnit;

        //EnemyFunctions Variables
        block = enemyFunctions.block;
        //enumerates through the children in intentsHolder and add the existing EnemyActionFormats
        foreach (Transform iconTrans in enemyFunctions.intentPanel.transform)
        {
            intentPanelActions.Add(iconTrans.GetComponent<EnemyActionIcon>().enemyAction);
        }
        foreach(Transform iconTrans in enemyFunctions.actionPanel.transform)
        {
            actionPanelActions.Add(iconTrans.GetComponent<EnemyActionIcon>().enemyAction);
        }

        currentActionDeck.AddRange(enemyFunctions.actionDeck);

        //EnemyUnit Variables
        currHP = enemyUnit.currHP;    

    }
}
