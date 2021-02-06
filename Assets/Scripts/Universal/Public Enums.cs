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

//Neu = Neutral
//War = Warrior
//OT = Offensive Targettable
//UD = Utility Dropped
//AD = Ability Dropped
public enum AllCards
{
    Jigsaw,
    Ability,
    Neu_OT_Attack,
    Neu_OT_Strike,
    Neu_UD_Defend,
    Neu_AD_Brainstorm,
    War_UD_Reinforce,

}


////Status Effects//////////////////////////////////////////////////////////////////////////////////

public enum StatusEffects
{

}


//////Upgrades List//////////////////////////////////////////////////////////////////////////////








