using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
