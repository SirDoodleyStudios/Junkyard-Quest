using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class EA_Deal_X_Damage : BaseEnemyActions
//{
//    public override AllCards enumKeyCard => AllCards.EnemyAction;
//    public override AllEnemyActions enumKeyEnemyAction => AllEnemyActions.Enemy_Deal_X_Damage;

//    public override void InitializeEnemyAction(EnemyUnit enemyStats, GameObject enemyGameObject)
//    {
//        damage = enemyStats.normalDamage;
//        hits = 1;
//        ActingUnitStatusLoad(enemyGameObject);
//        //enemy as target will be overridden by Affect methods in BaseCardEffect
//        //enemy as actor will always be the case for EnemyActions
//        CardEffectActivate(enemyGameObject, enemyGameObject);

//    }

//    public override void CardEffectActivate(GameObject target, GameObject actor)
//    {
//        AffectPlayer(target);
//        ActingUnitStatusLoad(actor);
//        DealDamage();
//    }
//}
