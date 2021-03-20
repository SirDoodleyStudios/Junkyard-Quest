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



//Deal 8 DAMAGE. If at 1 MOMENTUM, gain 1 FORCEFUL
public class War_OT_UnrelentingStrike : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.War_OT_UnrelentingStrike;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        damage = 8;
        hits = 1;

        ActingUnitStatusLoad(actor);
        AffectSingleEnemy(target);
        DealDamage();

        baseStatus = CardMechanics.Momentum;
        baseStack = 1;
        if (actorUnitStatus.StatusStackChecker(baseStatus) >= baseStack)
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

        baseStatus = CardMechanics.Momentum;
        baseStack = 3;
        if (actorUnitStatus.StatusStackChecker(baseStatus) >= baseStack)
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

//Deal 25 DAMAGE. Lose 8 health if player doesn't have FORCEFUL
public class War_OT_HeavyWeapon : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.War_OT_HeavyWeapon;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);
        AffectSingleEnemy(target);
        damage = 25;
        hits = 1;
        DealDamage();
        //checks if player has atleast 1 Forceful
        if (actorUnitStatus.StatusStackChecker(CardMechanics.Forceful) <= 1)
        {
            damage = 8;
            AffectPlayer(target);
            DealDamage();
        }
    }
}

//Deal 9 DAMAGE. Gain 1 FORCEFUL if player has FORCEFUL.
public class War_OT_EfficientAttack : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.War_OT_EfficientAttack;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);
        AffectSingleEnemy(target);
        damage = 9;
        hits = 1;
        DealDamage();
        //checks if player has atleast 1 Forceful
        if (actorUnitStatus.StatusStackChecker(CardMechanics.Forceful) >= 1)
        {
            AffectPlayer(target);
            status = CardMechanics.Forceful;
            stack = 1;
            ApplyStatus();
        }
    }
}

//Deal 12 damage, gain BLOCK equal to unblocked damage.
public class War_OT_FortifyingBlow : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.War_OT_FortifyingBlow;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);
        //checks current HP of target before reducing it with card damage
        int currHP = target.GetComponent<BaseUnitFunctions>().currHP;
        AffectSingleEnemy(target);
        damage = 12;
        hits = 1;
        DealDamage();

        AffectPlayer(actor);
        //this takes the difference of the first current HP before damage is done on unit and the actual current HP after dealing damage
        block = currHP - target.GetComponent<BaseUnitFunctions>().currHP;
        GainBlock();
    }
}







//Offensive Dropped

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
//Lose half of your BLOCK, deal damage equal to BLOCK lost to all enemies.
public class War_OD_ShrapnelBlast : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.War_OD_ShrapnelBlast;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);
        int currBlock = actor.GetComponent<BaseUnitFunctions>().block;
        AffectPlayer(target);
        //negative because we're losing block
        block = -currBlock / 2;
        GainBlock();

        AffectAllEnemies(target);
        damage = currBlock / 2;
        hits = 1;
        DealDamage();
    }
}

