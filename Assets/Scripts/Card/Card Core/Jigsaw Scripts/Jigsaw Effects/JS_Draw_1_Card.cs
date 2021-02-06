using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_Draw_1_Card : BaseJigsawEffect
{
    //determined by AllCards enum as a Jigsaw
    public override AllCards enumKeyCard => AllCards.Jigsaw;
    public override AllJigsaws enumKeyJigsaw => AllJigsaws.Draw_1_Card;

    public override void CardEffectActivate(GameObject target)
    {
        draw = 1;
        AffectPlayingField(target);
        DrawCard();
    }
}
