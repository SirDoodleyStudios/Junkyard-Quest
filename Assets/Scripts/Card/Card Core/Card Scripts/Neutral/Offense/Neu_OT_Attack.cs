using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Neu_OT_Attack : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.Neu_OT_Attack;

    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        
        damage = 5;
        hits = 1;
        ActingUnitStatusLoad(actor);
        AffectSingleEnemy(target);
        DealDamage();

    }

    public void GetCardTag()
    {

    }


}
