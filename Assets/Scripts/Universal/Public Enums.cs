using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatState { PlayerTurn, CreativeMode, UnleashCreativity, ActiveCard, DrawPahase, EnemyTurn }

public enum CardClass { Neutral, Warrior, Rogue, Mage, Arlen, Cedric, Francine, Tilly, Princess }

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
//Readcode from right to left
//XT = Targettable = 1XXXX
//XD = Dropped =  2XXXX
//OX = Offensive = X1XXX
//UX = Utility = X2XXX
//AX = Ability = X3XXX
//Neu = Neutral = XX1XX
//War = Warrior = XX2XX
//Rog = Rogue = XX3XX
//Mag = Mage = XX4XX
//Arl = Arlen = XX5XX
//Ced = Cedric = XX6XX
//Fra = Francine = XX7XX
//Til = Tilly = XX8XX
//Pri = Princess = XX9XX

//Non-Card 0
//Neu_OT=10    War_OT=100   Rog_OT=200   Mag_OT=300   Arl_OT=400   Ced_OT=500   Fra_OT=600   Til_OT=700   Pri_OT=800    
//Neu_OD=900   War_OD=1000  Rog_OD=1100  Mag_OD=1200  Arl_OD=1300  Ced_OD=1400  Fra_OD=1500  Til_OD=1600  Pri_OD=1700    
//Neu_UT=1800  War_UT=1900  Rog_UT=2000  Mag_UT=2100  Arl_UT=2200  Ced_UT=2300  Fra_UT=2400  Til_UT=2500  Pri_OD=2600
//Neu_UD=2700  War_UD=2800  Rog_UD=2900  Mag_UD=3000  Arl_UD=3100  Ced_UD=3200  Fra_UD=3300  Til_UD=3400  Pri_UD=3500
//Neu_AD=3600  War_AD=3700  Rog_AD=3800  Mag_AD=3900  Arl_AD=4000  Ced_AD=4100  Fra_AD=4200  Til_AD=4200  Pri_AD=4300

public enum AllCards: int
{
    //Non-Cards Classifiers
    Jigsaw = 0,
    Ability,
    //Neutral Offensive Targetted
    Neu_OT_Attack = 10,
    Neu_OT_Strike,
    //Neutral Utility Dropped
    Neu_UD_Defend = 2700,
    //Neutral Ability Dropped
    Neu_AD_Brainstorm = 3600,
    //Warrior Utility Dropped
    War_UD_Reinforce = 2800,

}


////Status Effects//////////////////////////////////////////////////////////////////////////////////

public enum StatusEffects
{

}


//////Upgrades List//////////////////////////////////////////////////////////////////////////////








