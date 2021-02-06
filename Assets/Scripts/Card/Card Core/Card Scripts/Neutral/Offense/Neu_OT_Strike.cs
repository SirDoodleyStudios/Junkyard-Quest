using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neu_OT_Strike : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.Neu_OT_Strike;
    //public override void CardEffectActivate(GameObject target)
    //{
    //    Debug.Log("getting here");
    //    damage = 12;
    //    hits = 1;
    //    for (int i = 1; hits >= i; i++)
    //    {
    //        targetUnit.GetComponent<BaseUnitFunctions>().DamageUnit(damage);
    //    }



    //}

    public override void CardEffectActivate(GameObject target)
    {
        damage = 12;
        hits = 1;
        AffectSingleEnemy(target);
        DealDamage();

    }
}
