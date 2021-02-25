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

public class JS_Draw_1_Card : BaseJigsawEffect
{
    //determined by AllCards enum as a Jigsaw
    public override AllCards enumKeyCard => AllCards.Jigsaw;
    public override AllJigsaws enumKeyJigsaw => AllJigsaws.Draw_1_Card;

    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        draw = 1;
        ActingUnitStatusLoad(actor);
        AffectPlayingField(target);
        DrawCard();
    }
}

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
