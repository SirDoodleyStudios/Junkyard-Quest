using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Unit", menuName = "Enemy Unit")]
public class EnemyUnit : Unit
{
    //will be used as an Identifier to fetch the base SO in Resources
    //currently used in saveing functions
    public EnemyEnumName enemyEnumName;

    //in Base
    //public int HP;
    //public int Creativity;
    //public int draw;
    public List<EnemyActionFormat> actionList;

    //Attacks
    public int AttackPattern1;
    public int HitsPattern1;

    public int AttackPattern2;
    public int HitsPattern2;

    public int AttackPattern3;
    public int HitsPattern3;

    //Blocks
    public int BlockPattern1;
    public int BlockPattern2;
    public int BlockPattern3;

    //Enhance
    public CardMechanics EnhancePattern1;
    public int EnhanceStackPattern1;
    public CardMechanics EnhancePattern2;
    public int EnhanceStackPattern2;
    public CardMechanics EnhancePattern3;
    public int EnhanceStackPattern3;

    //Debilitate
    public CardMechanics DebilitatePattern1;
    public int DebilitateStackPattern1;
    public CardMechanics DebilitatePattern2;
    public int DebilitateStackPattern2;
    public CardMechanics DebilitatePattern3;
    public int DebilitateStackPattern3;

    //Special










}