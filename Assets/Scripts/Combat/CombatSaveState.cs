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
    public PlayerUnit playerUnit;
    public PlayerUnitWrapper playerStatsWrapper;
    //this int will be filled up externlly once the CombatSaveState is created from the externlScript
    public int currCreativity;

    //for the saving of player unit statuses
    //the key for accessing the status itself and their stacks
    //indices will determine which stck is for what status
    public List<CardMechanics> cardMechanics = new List<CardMechanics>();
    public List<int> statusStacks = new List<int>();

    //enemy related info
    public List<EnemyUnitStatsWrapper> enemyUnitWrappers = new List<EnemyUnitStatsWrapper>();


    public CombatSaveState(DeckManager deckManager, PlayerUnit playerUnit, UnitStatusHolder playerStatuses, List<GameObject> enemyObjects)
    {
        //For Enemy Statuses
        //each gameObject is an enemy, then we extract the enemyUnit in the Onbject's enemy functions
        foreach (GameObject enemyObject in enemyObjects)
        {
            EnemyFunctions enemyFunctions = enemyObject.GetComponent<EnemyFunctions>();
            EnemyUnitStatsWrapper enemyWrapper = new EnemyUnitStatsWrapper(enemyFunctions, enemyObject.GetComponent<UnitStatusHolder>());
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
        this.playerUnit = playerUnit;

        //assigns the statuses and their stacks, will be linked by index of list
        foreach (KeyValuePair<CardMechanics, int> existingStatus in playerStatuses.allStatsAndStacks)
        {
            cardMechanics.Add(existingStatus.Key);
            statusStacks.Add(existingStatus.Value);
        }

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

    //for the saving of enemy unit statuses
    //the key for accessing the status itself and their stacks
    //indices will determine which stck is for what status
    public List<CardMechanics> cardMechanics = new List<CardMechanics>();
    public List<int> statusStacks = new List<int>();

    //The Wrappers are serializable classes for EnemyActionFormat when saving
    //contains EnemyActionFomats in the intentPanel 
    public List<EnemyActionFormatWrapper> intentPanelActions = new List<EnemyActionFormatWrapper>();
    //contains EnemyActionFormats in the actionPanel
    public List<EnemyActionFormatWrapper> actionPanelActions = new List<EnemyActionFormatWrapper>();
    //contains the current list of EnemyActionFormats in the enemy's deck
    //for this one, action deck should already be subtracted with the intent and actionPanelActions
    public List<EnemyActionFormatWrapper> currentActionDeck = new List<EnemyActionFormatWrapper>();



    public EnemyUnitStatsWrapper(EnemyFunctions enemyFunctions, UnitStatusHolder enemyStatuses)
    {
        EnemyUnit enemyUnit = enemyFunctions.enemyUnit;
        enemyEnumName = enemyUnit.enemyEnumName;
        //EnemyFunctions Variables
        block = enemyFunctions.block;
        //enumerates through the children in intentsHolder and add the existing EnemyActionFormats
        //creates a new instance of EnemyActionFormat before saving it
        foreach (Transform iconTrans in enemyFunctions.intentPanel.transform)
        {
            if (iconTrans.gameObject.activeSelf)
            {
                EnemyActionFormatWrapper tempAction = new EnemyActionFormatWrapper(iconTrans.GetComponent<EnemyActionIcon>().enemyAction);
                intentPanelActions.Add(tempAction);
            }

        }
        foreach(Transform iconTrans in enemyFunctions.actionPanel.transform)
        {
            int tempInt = enemyFunctions.actionPanel.transform.childCount;
            if (iconTrans.gameObject.activeSelf)
            {
                EnemyActionFormatWrapper tempAction = new EnemyActionFormatWrapper(iconTrans.GetComponent<EnemyActionIcon>().enemyAction);
                actionPanelActions.Add(tempAction);
            }
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

        //enemyStatuses
        //assigns the statuses and their stacks, will be linked by index of list
        foreach (KeyValuePair<CardMechanics, int> existingStatus in enemyStatuses.allStatsAndStacks)
        {
            cardMechanics.Add(existingStatus.Key);
            statusStacks.Add(existingStatus.Value);
        }

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
