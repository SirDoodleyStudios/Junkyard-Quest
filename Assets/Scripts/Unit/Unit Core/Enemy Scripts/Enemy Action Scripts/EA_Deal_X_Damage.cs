using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Deal_X_Damage : BaseEnemyActions
{
    public override AllCards enumKeyCard => AllCards.EnemyAction;
    public override AllEnemyActions enumKeyEnemyAction => AllEnemyActions.Enemy_Deal_X_Damage;

    public override void InitializeEnemyAction(EnemyUnit enemyStats, GameObject target)
    {
        damage = enemyStats.normalDamage;
        hits = 1;
        CardEffectActivate(target);

    }

    public override void CardEffectActivate(GameObject target)
    {
        AffectPlayer(target);
        DealDamage();
    }
}
