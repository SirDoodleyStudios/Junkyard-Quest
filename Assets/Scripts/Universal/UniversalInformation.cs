using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

//This is the wrapper class that will contain all information needed to be accessed in all scenes

[Serializable]
public class UniversalInformation
{
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

    //only used when game session has been saved from combat
    public List<AllCards> combatDeck;
    public List<AllCards> combatHand;
    public List<AllCards> combatDiscard;
    public List<AllCards> combatConsumed;
    public List<EnemyUnit> enemyUnits;
    

}
