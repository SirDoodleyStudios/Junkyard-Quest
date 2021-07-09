using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NEVER EDIT THE TEXT HERE WITHOUT KNOWING WHAT YOU'RE DOING!!!
//A LOT OF THINGS ARE BASED ON THE ENUM NAMES
public enum CombatState { PlayerTurn, CreativeMode, UnleashCreativity, ActiveCard, DrawPhase, Tweening, EnemyTurn }

//STATE OF OVERWORLD WHEN MOVING
public enum OverworldState { StartingNode, MoveNode}
//list of scenes
public enum SceneList { PlayerSelectScreen, Overworld, Combat, Event, Merchant, Forge, Rest, Boon, LinkActivities, Rewards }
public enum ChosenPlayer { Arlen, Cedric, Francine, Tilly, Princess};
public enum ChosenClass { Warrior, Rogue, Mage};

//SCENES DEPENDED
public enum NodeActivityEnum { Combat, Event, Merchant, Forge, Rest, Boon, Rival }
//Link activites are not seen from overworld and random like darkest dungeon
//SCENES DEPENDENT
public enum LinkActivityEnum {Skirmish, Chest, Booth, SmallEvent}

//Options for Combat Rewards
public enum CombatRewards {CardDraft, Scraps, Treasures, Abilities, Upgrades }

public enum CardClass { Neutral, Warrior, Rogue, Mage, Arlen, Cedric, Francine, Tilly, Princess }

public enum CardType { Offense, Utility, Ability, Material, Prosthetic }

public enum CardMethod { Targetted, Dropped}

public enum JigsawLink { Circle, Triangle, Square, Material, Finisher, Unknown}

//ABILITIES DEPENDENT
//name of ability format must match this
public enum AllAbilities
{
    //Neutral Abilities
    Brainstorm = 30,

    //Warrior Abilities
    BruteForce = 60,
    PrioritizeDefense
}

public enum AllJigsaws
{
    // The division will determine the cardmethod of jigsaws

    // Targetted Jigsaws
    Deal_10_Damage=0,

    //Droped Jigsaws
    Gain_10_Block,
    Draw_1_Card,




}

//TREASURE RELATED
//Type of material, determines what is available to use in blueprint
public enum AllMaterialTypes
{
    Slab,
    Hunk,
    Pointy,
    Strips,
    Stick,
    Board,
    Ornament
}
//Relic Effects
public enum AllMaterialEffects
{
    LasterStand, //take 2 turns before being overkilled, doubles Worn-out stacks
    Eureka, //gain 10 creativity at start of combat, creativity no longer recovers every turn
    DeepSleep, //Rest consumes 2 actions but fully heals HP
    ForgeFriendly //Forging cost starts at 40 scraps
}
//prefix of material that dictates the bonus when crafting
public enum AllMaterialPrefixes
{
    Sturdy,
    Mysterious,
    Fancy
}
//Gear types, which is also the blueprint types
public enum AllGearTypes
{
    Sword,
    Axe,
    Shield,
    Hammer,
    Greatsword,
    Spear
}


//ENEMY RELATED
//
public enum EnemyEnumName
{
    //rivals, exactly 15, each playable with 3 classes
    //giving way to a new class and new character in the future


    //normal enemies
    Spearie = 25

    //bosses


}

public enum EnemyActionType { Offense, Block, Enhance, Debilitate, Special}
public enum AllEnemyActions
{
    Enemy_AttackPattern1,
    Enemy_AttackPattern2,
    Enemy_AttackPattern3,

    Enemy_BlockPattern1,
    Enemy_BlockPattern2,
    Enemy_BlockPattern3,

    Enemy_EnhancePattern1,
    Enemy_EnhancePattern2,
    Enemy_EnhancePattern3,

    Enemy_DebilitatePattern1,
    Enemy_DebilitatePattern2,
    Enemy_DebilitatePattern3

}


////Status Effects//////////////////////////////////////////////////////////////////////////////////

public enum StatusEffects
{

}


//////Upgrades List//////////////////////////////////////////////////////////////////////////////

public enum CardMechanics
{
    //Miscellaneous
    None,
    Ability,
    Block,
    Slay,

    //Card only Mechanics
    Consume = 10,
    Keep,
    Combo,
    Discover,
    Attune,
    Riddle, 

    //Usage Stack Statuses
    Strong = 40,
    Forceful,
    Feeble,
    AllOrNothing, //unique
    Counter,

    //Turn Stack Status
    Confused = 70,
    Shocked,
    Vulnerable,
    AdrenalineRush, // unique
    Payback, //unique
    Confidefense, //unique
    WornOut,
    LastStand,
    
    //Consume Usage Stack Statuses


    //Consume Turn Stack Statuses
    GenerateForceful = 130, //unique
    Momentum

    //Permanent Stack Statuses



}

//TEST ANIMATION KEYS
//EACH CARD MIGHT HAVE ITS OWN EFFECT AND I MIGHT NEED TO JUST USE ALLCARD ENUMS AS KEYS
public enum AllCardAnimations
{
    Slash,
    Strike,
    Block,
    Buff,
    Debuff
}

//Non-Card 0
//Neu_OT=10    War_OT=100   Rog_OT=200   Mag_OT=300   Arl_OT=400   Ced_OT=500   Fra_OT=600   Til_OT=700   Pri_OT=800    
//Neu_OD=900   War_OD=1000  Rog_OD=1100  Mag_OD=1200  Arl_OD=1300  Ced_OD=1400  Fra_OD=1500  Til_OD=1600  Pri_OD=1700    
//Neu_UT=1800  War_UT=1900  Rog_UT=2000  Mag_UT=2100  Arl_UT=2200  Ced_UT=2300  Fra_UT=2400  Til_UT=2500  Pri_OD=2600
//Neu_UD=2700  War_UD=2800  Rog_UD=2900  Mag_UD=3000  Arl_UD=3100  Ced_UD=3200  Fra_UD=3300  Til_UD=3400  Pri_UD=3500
//Neu_AD=3600  War_AD=3700  Rog_AD=3800  Mag_AD=3900  Arl_AD=4000  Ced_AD=4100  Fra_AD=4200  Til_AD=4200  Pri_AD=4300
public enum AllCards : int
{
    //Non-Cards Classifiers
    Jigsaw = 0,
    Ability,
    EnemyAction,
    //Neutral Offensive Targetted
    Neu_OT_Attack = 10,
    Neu_OT_Strike,
    Neu_OT_Strike2,
    //Warrior Offensive Targetted
    War_OT_Sting = 100,
    War_OT_UnrelentingStrike,
    War_OT_FinishingBlow,
    War_OT_HeavyWeapon,
    War_OT_EfficientAttack,
    War_OT_CleanHit,
    War_OT_FortifyingBlow,
    //Warrior Offensive Dropped
    War_OD_Cleave = 1000,
    War_OD_ShrapnelBlast,

    //Warrior Utility Targetted
    War_UT_LookForOpenings = 1900,
    //Neutral Utility Dropped
    Neu_UD_Defend = 2700,

    //Warrior Utility Dropped
    War_UD_Reinforce = 2800,
    War_UD_WildSwings,
    War_UD_SteadyImprovement,
    War_UD_BreathOfBattle,
    War_UD_Juggernaut,
    War_UD_AllOrNothing,
    War_UD_Fortress,
    War_UD_EyeForAnEye,
    War_UD_AdrenalineRush,
    War_UD_MasterOfDefense,
    War_UD_Payback,
    War_UD_BraceForImpact,
    War_UD_Confidefense,

    //Neutral Ability Dropped
    Neu_AD_Brainstorm = 3600,

    //Warrior Ability Dropped
    War_AD_BruteForce = 3700,
    War_AD_PrioritizeDefense,



}







