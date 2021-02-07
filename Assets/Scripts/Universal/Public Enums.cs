using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatState { PlayerTurn, CreativeMode, UnleashCreativity, ActiveCard, DrawPahase, EnemyTurn }

public enum CardClass { Neutral, Warrior, Rogue, Mage, Arlen, Cedric, Marie, Luca, Firebrand }

public enum CardType { Offense, Utility, Ability }

public enum CardMethod { Targetted, Dropped}

public enum JigsawLink { Circle, Triangle, Square, Finisher}

public enum AllAbilities
{
    Neu_Abi_Brainstorm
}

public enum AllJigsaws
{
    
    Deal_10_Damage,
    Gain_10_Block,
    Draw_1_Card


}


//Neu = Neutral = 1XXXX
//War = Warrior = 2XXXX
//Rog = Rogue = 3XXXX
//Mag = Mage = 4XXXX
//Arl = Arlen = 5XXXX
//Ced = Cedric = 6XXXX
//Fra = Francine = 7XXXX
//Til = Tilly = 8XXXX
//Pri = Princess = 9XXXX
//OX = Offensive = X1XXX
//UX = Utility = X2XXX
//AX = Ability = X3XXX
//XT = Targettable = XX1XX
//XD = Dropped =  XX2XX
public enum AllCards
{
    //Non-Cards Classifiers
    Jigsaw = 0,
    Ability,
    //Neutral Offensive Targetted
    Neu_OT_Attack = 11100,
    Neu_OT_Strike,
    //Neutral Utility Dropped
    Neu_UD_Defend = 12200,
    //Neutral Ability Dropped
    Neu_AD_Brainstorm = 13200,
    Neu_AD_Rest,
    //Warrior Utility Dropped
    War_UD_Reinforce = 22200,

}


////Status Effects//////////////////////////////////////////////////////////////////////////////////

public enum StatusEffects
{

}


//////Upgrades List//////////////////////////////////////////////////////////////////////////////








