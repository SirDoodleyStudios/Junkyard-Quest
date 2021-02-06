using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "CardEffect", menuName = "Attack")]

public class Neu_OT_Attack : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.Neu_OT_Attack;

    //public override void CardEffectActivate(GameObject target)
    //{
    //    damage = 5;
    //    hits = 1;
    //    for (int i = 1; hits >= i; i++)
    //    {
    //        target.GetComponent<BaseUnitFunctions>().DamageUnit(damage);
    //    }


    //}

    public override void CardEffectActivate(GameObject target)
    {
        damage = 5;
        hits = 1;
        AffectSingleEnemy(target);
        DealDamage();
    }


}
