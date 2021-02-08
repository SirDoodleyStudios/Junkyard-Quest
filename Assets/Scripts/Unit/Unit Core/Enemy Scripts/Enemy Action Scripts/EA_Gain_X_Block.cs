using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Gain_X_Block : BaseEnemyActions
{
    public override AllCards enumKeyCard => AllCards.EnemyAction;
    public override AllEnemyActions enumKeyEnemyAction => AllEnemyActions.Enemy_Gain_X_Block;

    public override void InitializeEnemyAction(EnemyUnit enemyStats)
    {
        block = enemyStats.normalBlock;
    }

    public override void CardEffectActivate(GameObject target)
    {
        AffectSingleEnemy(target);
        DealDamage();
    }
}
