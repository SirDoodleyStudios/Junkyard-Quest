using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Apply SHOCKED 1. Deal 1 damage 2 times."
public class War_OT_Sting : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.War_OT_Sting;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        status = CardMechanics.Shocked;
        stack = 1;
        damage = 1;
        hits = 2;
        ActingUnitStatusLoad(actor);
        AffectSingleEnemy(target);
        ApplyStatus();
        DealDamage();
    }
}

//Deal 8 DAMAGE to all enemies.
public class War_OD_Cleave : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.War_OD_Cleave;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        damage = 8;
        hits = 1;

        ActingUnitStatusLoad(actor);
        AffectAllEnemies(target);
        DealDamage();
    }

}

//Deal 8 DAMAGE. If at 1 MOMENTUM, gain 1 FORCEFUL
public class War_OT_UnrelentingStrike : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.War_OT_UnrelentingStrike;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        damage = 8;
        hits = 1;
        baseMomentum = 1;
        ActingUnitStatusLoad(actor);
        AffectSingleEnemy(target);
        DealDamage();
        if (actor.GetComponent<UnitStatusHolder>().MomentumLevelChecker() >= baseMomentum)
        {
            status = CardMechanics.Forceful;
            stack = 1;
            AffectPlayer(target);
            ApplyStatus();
        }
    }
}

//Gain 1 MOMENTUM then deal 5 DAMAGE. Deal 15 DAMAGE instead if at 3 MOMENTUM
public class War_OT_CleanHit : BaseCardEffect
{    public override AllCards enumKeyCard => AllCards.War_OT_CleanHit;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        status = CardMechanics.Momentum;
        stack = 1;
        ActingUnitStatusLoad(actor);
        AffectPlayer(target);
        ApplyStatus();

        baseMomentum = 3;
        if (actor.GetComponent<UnitStatusHolder>().MomentumLevelChecker() >= baseMomentum)
        {
            damage = 15;
            hits = 1;
            AffectSingleEnemy(target);
            DealDamage();
        }
        else
        {
            damage = 5;
            hits = 1;
            AffectSingleEnemy(target);
            DealDamage();
        }
    }

}
//Deal 6 DAMAGE. Gain 5 FORCEFUL at SLAY.
public class War_OT_FinishingBlow : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.War_OT_FinishingBlow;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        damage = 6;
        hits = 1;
        ActingUnitStatusLoad(actor);
        AffectSingleEnemy(target);
        DealDamage();
        SlayCheck();

        if (isSlain)
        {
            status = CardMechanics.Forceful;
            stack = 5;
            AffectPlayer(target);
            ApplyStatus();
        }

    }

}
