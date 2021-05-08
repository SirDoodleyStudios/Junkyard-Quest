using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

//This is the wrapper class that will contain all information needed to be accessed in all scenes

[Serializable]
public class UniversalInformation
{
    //what scene the game was last left from
    public SceneList scene;

    //used for identifying whether a player and class have already  been selected
    //might not need these actually
    public bool isPlayerSelected;
    public List<AllCards> initialDeck;

    //used for information that is consistently accessed throughout the game
    public List<AllCards> currentDeck;

    //information for the player unit
    public PlayerUnit playerStats;
    public PlayerUnitWrapper playerStatsWrapper;
    public int scraps;

    //chosens
    public ChosenPlayer chosenPlayer;
    public ChosenClass chosenClass;

    //current activities
    //NOT YET SAVED IN UNIVERSALINFO
    public NodeActivityEnum nodeActivity;
    public LinkActivityEnum linkActivity;

    //only used when game session has been saved from combat
    public List<AllCards> combatDeck;
    public List<AllCards> combatHand;
    public List<AllCards> combatDiscard;
    public List<AllCards> combatConsumed;
    public List<EnemyUnit> enemyUnits;

    //combat statistics
    //these are determined during combat and used for determining rewards in the rewards scene
    //NOT YET SAVED IN UNIVERSALINFO
    public int overkills;
    public int enemyCount;

    

}
