using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CombatSaveState
{
    //player related combat info
    //firs was list of Cards, changed to List of wrappers to include jigsaw
    //public List<AllCards> initialDeck = new List<AllCards>();
    //public List<AllCards> battleDeck = new List<AllCards>();
    //public List<AllCards> playerHandList = new List<AllCards>();
    //public List<AllCards> discardPile = new List<AllCards>();
    //public List<AllCards> consumePile = new List<AllCards>();

    public List<CardAndJigsaWrapper> initialDeck = new List<CardAndJigsaWrapper>();
    public List<CardAndJigsaWrapper> battleDeck = new List<CardAndJigsaWrapper>();
    public List<CardAndJigsaWrapper> playerHandList = new List<CardAndJigsaWrapper>();
    public List<CardAndJigsaWrapper> discardPile = new List<CardAndJigsaWrapper>();
    public List<CardAndJigsaWrapper> consumePile = new List<CardAndJigsaWrapper>();

    //same playerUnitStats should be in UniversalInformation except for current creativity
    //player unit has a wrapper class available ion UniversalSaveState with constructor parameter PlayerUnit
    //playerUnitStats is not serialized and will only be used to create the playerStatsWrapper which will be serialized
    public PlayerUnit playerUnit;
    public PlayerUnitWrapper playerStatsWrapper;
    //these ints will be filled up externlly once the CombatSaveState is created from the externlScript
    public int currCreativity;
    public int currEnergy;

    //holds the player ability list
    //these keys are to be used to generte an AbilityFormat SO and then assign it there
    //to be filled-up in combatManager after creating the combatSaveState instance
    public List<AllAbilities> abilityList = new List<AllAbilities>();

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
            CardAndJigsaWrapper cardAndJigsaw = new CardAndJigsaWrapper(card);
            battleDeck.Add(cardAndJigsaw);
            //battleDeck.Add(card.enumCardName);
        }
        foreach(Card card in deckManager.discardPile)
        {
            CardAndJigsaWrapper cardAndJigsaw = new CardAndJigsaWrapper(card);
            discardPile.Add(cardAndJigsaw);
            //discardPile.Add(card.enumCardName);
        }
        foreach (Card card in deckManager.consumePile)
        {
            CardAndJigsaWrapper cardAndJigsaw = new CardAndJigsaWrapper(card);
            consumePile.Add(cardAndJigsaw);
            //consumePile.Add(card.enumCardName);
        }
        foreach (Card card in deckManager.playerHandList)
        {
            CardAndJigsaWrapper cardAndJigsaw = new CardAndJigsaWrapper(card);
            playerHandList.Add(cardAndJigsaw);
            //playerHandList.Add(card.enumCardName);
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

//wrapper class tht represents a card and it's jigsaw, if there is no jigsaw, the jigsaw cariable is nulk
[Serializable]
public class CardAndJigsaWrapper
{
    //mimicvs a dicitonary of the main card's enum and it's jigsaw
    //if there is no jigsaw in the card, put in index 0 or "Jigsaw", this element is no longer in use
    public AllCards cardEnum;
    public AllCards jigsawEnum;
    //jigsawlinks
    public JigsawLink inputLink;
    public JigsawLink outputLink;
    //determines the card method if targetted or dropped
    public CardMethod jigsawMethod;
    public CardAndJigsaWrapper(Card card)
    {
        //give a proper jogsaw entry if the card parameter has a jigsawFormat
        if (card.jigsawEffect != null)
        {
            cardEnum = card.enumCardName;
            JigsawFormat jigsawFormat = card.jigsawEffect;
            jigsawEnum = jigsawFormat.enumJigsawCard;
            inputLink = jigsawFormat.inputLink;
            outputLink = jigsawFormat.outputLink;
            jigsawMethod = jigsawFormat.jigsawMethod;

        }
        else
        {
            cardEnum = card.enumCardName;
            //give jigsaw entry if there is no jigsaw taken from the card parameter, when loading, a "jigsaw" value means null
            jigsawEnum = AllCards.Jigsaw;
        }

    }
}

//wrapper classes
//enemy stats wrapper, will contain information for EnemyUnit, EnemyFunctions, EnemyActionFormat and UnitStatusHolder
[Serializable]
public class EnemyUnitStatsWrapper
{
    //identifier if enemy is dead and if overkilled
    public bool isAlive;
    public bool isBeingOverkilled;
    public bool isOverKilled;

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

//Wrapper to caontain JigsawFormat for cards that has Jigsaws
[Serializable]
public class JigsawFormatWrapper
{
    public JigsawLink inputLink;
    public JigsawLink outputLink;
    //key for the jigsaw card attached
    public AllCards enumJigsawCard;
    //determines the card method if targetted or dropped
    public CardMethod jigsawMethod;


}
