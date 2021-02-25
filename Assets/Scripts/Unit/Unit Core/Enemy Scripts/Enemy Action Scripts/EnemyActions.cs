using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_BlockPattern1 : BaseEnemyActions
{
    public override AllCards enumKeyCard => AllCards.EnemyAction;
    public override AllEnemyActions enumKeyEnemyAction => AllEnemyActions.Enemy_BlockPattern1;

    public override void InitializeEnemyAction(EnemyUnit enemyStats, GameObject enemyGameObject)
    {
        block = enemyStats.BlockPattern1;
        ActingUnitStatusLoad(enemyGameObject);
        //enemy as target will be overridden by Affect methods in BaseCardEffect
        //enemy as actor will always be the case for EnemyActions
        CardEffectActivate(enemyGameObject, enemyGameObject);
    }

    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);
        AffectSingleEnemy(target);
        GainBlock();
    }
}

public class Enemy_BlockPattern2 : BaseEnemyActions
{
    public override AllCards enumKeyCard => AllCards.EnemyAction;
    public override AllEnemyActions enumKeyEnemyAction => AllEnemyActions.Enemy_BlockPattern2;

    public override void InitializeEnemyAction(EnemyUnit enemyStats, GameObject enemyGameObject)
    {
        block = enemyStats.BlockPattern2;
        ActingUnitStatusLoad(enemyGameObject);
        //enemy as target will be overridden by Affect methods in BaseCardEffect
        //enemy as actor will always be the case for EnemyActions
        CardEffectActivate(enemyGameObject, enemyGameObject);
    }

    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);
        AffectSingleEnemy(target);
        GainBlock();
    }
}

public class Enemy_BlockPattern3 : BaseEnemyActions
{
    public override AllCards enumKeyCard => AllCards.EnemyAction;
    public override AllEnemyActions enumKeyEnemyAction => AllEnemyActions.Enemy_BlockPattern3;

    public override void InitializeEnemyAction(EnemyUnit enemyStats, GameObject enemyGameObject)
    {
        block = enemyStats.BlockPattern3;
        ActingUnitStatusLoad(enemyGameObject);
        //enemy as target will be overridden by Affect methods in BaseCardEffect
        //enemy as actor will always be the case for EnemyActions
        CardEffectActivate(enemyGameObject, enemyGameObject);
    }

    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);
        AffectSingleEnemy(target);
        GainBlock();
    }
}


public class Enemy_AttackPattern1 : BaseEnemyActions
{
    public override AllCards enumKeyCard => AllCards.EnemyAction;
    public override AllEnemyActions enumKeyEnemyAction => AllEnemyActions.Enemy_AttackPattern1;

    public override void InitializeEnemyAction(EnemyUnit enemyStats, GameObject enemyGameObject)
    {
        damage = enemyStats.AttackPattern1;
        hits = enemyStats.HitsPattern1;
        ActingUnitStatusLoad(enemyGameObject);
        //enemy as target will be overridden by Affect methods in BaseCardEffect
        //enemy as actor will always be the case for EnemyActions
        CardEffectActivate(enemyGameObject, enemyGameObject);

    }

    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        AffectPlayer(target);
        ActingUnitStatusLoad(actor);
        DealDamage();
    }
}

public class Enemy_AttackPattern2 : BaseEnemyActions
{
    public override AllCards enumKeyCard => AllCards.EnemyAction;
    public override AllEnemyActions enumKeyEnemyAction => AllEnemyActions.Enemy_AttackPattern2;

    public override void InitializeEnemyAction(EnemyUnit enemyStats, GameObject enemyGameObject)
    {
        damage = enemyStats.AttackPattern2;
        hits = enemyStats.HitsPattern2;
        ActingUnitStatusLoad(enemyGameObject);
        //enemy as target will be overridden by Affect methods in BaseCardEffect
        //enemy as actor will always be the case for EnemyActions
        CardEffectActivate(enemyGameObject, enemyGameObject);

    }

    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        AffectPlayer(target);
        ActingUnitStatusLoad(actor);
        DealDamage();
    }
}

public class Enemy_AttackPattern3 : BaseEnemyActions
{
    public override AllCards enumKeyCard => AllCards.EnemyAction;
    public override AllEnemyActions enumKeyEnemyAction => AllEnemyActions.Enemy_AttackPattern3;

    public override void InitializeEnemyAction(EnemyUnit enemyStats, GameObject enemyGameObject)
    {
        damage = enemyStats.AttackPattern3;
        hits = enemyStats.HitsPattern3;
        ActingUnitStatusLoad(enemyGameObject);
        //enemy as target will be overridden by Affect methods in BaseCardEffect
        //enemy as actor will always be the case for EnemyActions
        CardEffectActivate(enemyGameObject, enemyGameObject);

    }

    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        AffectPlayer(target);
        ActingUnitStatusLoad(actor);
        DealDamage();
    }
}

public class Enemy_DebilitatePattern1 : BaseEnemyActions
{
    public override AllCards enumKeyCard => AllCards.EnemyAction;
    public override AllEnemyActions enumKeyEnemyAction => AllEnemyActions.Enemy_DebilitatePattern1;

    public override void InitializeEnemyAction(EnemyUnit enemyStats, GameObject enemyGameObject)
    {
        status = enemyStats.DebilitatePattern1;
        stack = enemyStats.DebilitateStackPattern1;
        ActingUnitStatusLoad(enemyGameObject);
        //enemy as target will be overridden by Affect methods in BaseCardEffect
        //enemy as actor will always be the case for EnemyActions
        CardEffectActivate(enemyGameObject, enemyGameObject);

    }

    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        AffectPlayer(target);
        ActingUnitStatusLoad(actor);
        ApplyStatus();
        DealDamage();
    }
}
