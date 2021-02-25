using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class EA_Gain_X_Block : BaseEnemyActions
//{
//    public override AllCards enumKeyCard => AllCards.EnemyAction;
//    public override AllEnemyActions enumKeyEnemyAction => AllEnemyActions.Enemy_Gain_X_Block;

//    public override void InitializeEnemyAction(EnemyUnit enemyStats, GameObject enemyGameObject)
//    {
//        block = enemyStats.normalBlock;
//        ActingUnitStatusLoad(enemyGameObject);
//        //enemy as target will be overridden by Affect methods in BaseCardEffect
//        //enemy as actor will always be the case for EnemyActions
//        CardEffectActivate(enemyGameObject, enemyGameObject);
//    }

//    public override void CardEffectActivate(GameObject target, GameObject actor)
//    {
//        ActingUnitStatusLoad(actor);
//        AffectSingleEnemy(target);
//        GainBlock();
//    }
//}
