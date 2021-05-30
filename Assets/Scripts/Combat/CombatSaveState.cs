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
    //playerUnitStats is not serialized and will only be used to create the playerStatsWrapper which will be serialized
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

    //The Wrappers are serializable classes for EnemyActionFormat when saving
    //contains EnemyActionFomats in the intentPanel 
    public List<EnemyActionFormatWrapper> intentPanelActions = new List<EnemyActionFormatWrapper>();
    //contains EnemyActionFormats in the actionPanel
    public List<EnemyActionFormatWrapper> actionPanelActions = new List<EnemyActionFormatWrapper>();
    //contains the current list of EnemyActionFormats in the enemy's deck
    //for this one, action deck should already be subtracted with the intent and actionPanelActions
    public List<EnemyActionFormatWrapper> currentActionDeck = new List<EnemyActionFormatWrapper>();



    public EnemyUnitStatsWrapper(EnemyFunctions enemyFunctions)
    {
        EnemyUnit enemyUnit = enemyFunctions.enemyUnit;
        enemyEnumName = enemyUnit.enemyEnumName;
        //EnemyFunctions Variables
        block = enemyFunctions.block;
        //enumerates through the children in intentsHolder and add the existing EnemyActionFormats
        //creates a new instance of EnemyActionFormat before saving it
        foreach (Transform iconTrans in enemyFunctions.intentPanel.transform)
        {
            EnemyActionFormatWrapper tempAction = new EnemyActionFormatWrapper(iconTrans.GetComponent<EnemyActionIcon>().enemyAction);
            intentPanelActions.Add(tempAction);
        }
        foreach(Transform iconTrans in enemyFunctions.actionPanel.transform)
        {
            EnemyActionFormatWrapper tempAction = new EnemyActionFormatWrapper(iconTrans.GetComponent<EnemyActionIcon>().enemyAction);
            actionPanelActions.Add(tempAction);
        }

        foreach (EnemyActionFormat action in enemyFunctions.actionDeck)
        {
            EnemyActionFormatWrapper tempAction = new EnemyActionFormatWrapper(action);
            currentActionDeck.Add(tempAction);
        }
        //currentActionDeck.AddRange(enemyFunctions.actionDeck);

        //EnemyUnit Variables
        //current HPis in functions
        currHP = enemyFunctions.currHP;


    }
}

//Wrapper to contain EnemyActionFormat so we xan serialize it
[Serializable]
public class EnemyActionFormatWrapper
{
    //everytime a jigsaw is instantiated, inputs and outputs are going to be randomized
    public JigsawLink inputLink;
    public JigsawLink outputLink;
    public EnemyActionType actionType;

    //THIS SHOULD BE ADDED AFTER RANDOMIZING
    public AllEnemyActions enumEnemyAction;

    public EnemyActionFormatWrapper(EnemyActionFormat action)
    {
        inputLink = action.inputLink;
        outputLink = action.outputLink;
        actionType = action.actionType;
        enumEnemyAction = action.enumEnemyAction;
    }


}
