﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_Deal_10_Damage : BaseJigsawEffect
{
    public override AllCards enumKeyCard => AllCards.Jigsaw;
    public override AllJigsaws enumKeyJigsaw => AllJigsaws.Deal_10_Damage;

    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        damage = 10;
        hits = 1;
        ActingUnitStatusLoad(actor);
        AffectSingleEnemy(target);
        DealDamage();
    }

}
