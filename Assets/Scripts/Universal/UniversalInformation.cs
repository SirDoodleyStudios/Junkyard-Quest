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
    //will be converted to a wrapper class that will contain the card's jigsaw information
    public List<AllCards> currentDeck;
    public List<CardAndJigsaWrapper> currentDeckWithJigsaw;

    //information for the player unit
    //playerStats is not altered when saving in UniversalInfo, only save through playerStatsWrapper
    public PlayerUnit playerStats;
    public PlayerUnitWrapper playerStatsWrapper;
    public int scraps;
    public int currentForgeCost;
    public int tickets;
    public bool isTicketUsed;

    //information about enemy
    //will be used to find the base SO in resources depending on the key
    public EnemyEnumName enemyEnumKey;

    //chosens
    public ChosenPlayer chosenPlayer;
    public ChosenClass chosenClass;

    //current activities
    //NOT YET SAVED IN UNIVERSALINFO
    public NodeActivityEnum nodeActivity;
    public LinkActivityEnum linkActivity;

    //only used when game session has been saved from combat
    //mgrated to CombatSaveState script
    //public List<AllCards> combatDeck;
    //public List<AllCards> combatHand;
    //public List<AllCards> combatDiscard;
    //public List<AllCards> combatConsumed;
    //public List<EnemyUnit> enemyUnits;
    //public int currEnergy;


    //combat statistics
    //these are determined during combat and used for determining rewards in the rewards scene
    //these must be cleared every battle start
    //enemycount must be determined before entering battle and assign it in combat immediately
    //NOT YET SAVED IN UNIVERSALINFO
    public int overkills;
    public int enemyCount;

    //count of player progress in terms of number of nodes looted
    //this will determine the difficulty of enemy spawns generated
    public int nodeCount;

    //this is a counter that assigns the worn-out status on the player if the last battle that he survived overkilled him
    public int wornOutCount;

    //this is a counter for how many actions you currently have left in Rest
    public int remainingRestActions;

    //contains the blueprints that the plye currently posses
    //all information needed to decode blueprints are in the CraftingManager since that's the only 
    public List<AllGearTypes> bluePrints;

    //contains the materials and its' attributes in a wrapper
    //when saved in universalSaveState, this will be converted in a wrapper for easy loading
    //public List<CraftingMaterialSO> craftingMaterialSOList;
    public List<CraftingMaterialWrapper> craftingMaterialWrapperList;

    //contains all gear that a player has
    //converted to wrapper when saving then to SO while loading
    //public List<GearSO> gearList;
    public List<GearWrapper> gearWrapperList;

    //equipped gears will always have a fixed amount, unequipped gear slots will have a null element
    //0 = mainhand, 1 = offhand, 2 = helm, 3 = armor, 4 = belt, 5 = trinket
    public GearWrapper[] equippedGears = new GearWrapper[6];

    //contains all the gear effects that the player currently posses
    //public List<AllMaterialEffects> materialEffects;
}




