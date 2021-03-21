using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neu_OT_Strike2 : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.Neu_OT_Strike2;



    //}

    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        Debug.Log("strike2");
        damage = 12;
        hits = 1;
        ActingUnitStatusLoad(actor);
        AffectSingleEnemy(target);
        DealDamage();

    }
}
