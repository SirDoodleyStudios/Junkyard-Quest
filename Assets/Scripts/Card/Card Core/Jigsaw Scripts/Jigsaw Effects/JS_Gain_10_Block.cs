using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_Gain_10_Block : BaseJigsawEffect
{
    //determined by AllCards enum as a Jigsaw
    public override AllCards enumKeyCard => AllCards.Jigsaw;
    public override AllJigsaws enumKeyJigsaw => AllJigsaws.Gain_10_Block;

    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        block = 10;
        ActingUnitStatusLoad(actor);
        AffectPlayer(target);
        GainBlock();
    }
}
