using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyActions : BaseCardEffect
{
    //protected int damage;
    //protected int block;
    //protected int hits;
    //protected int multiplier;
    //protected int draw;
    //protected int discard;
    //protected int creativity;
    //protected int energy;

    //public abstract void EnemyActionActivate(GameObject target);
    public abstract AllEnemyActions enumKeyEnemyAction{ get; }

    public abstract void InitializeEnemyAction(EnemyUnit enemyStats);
}
