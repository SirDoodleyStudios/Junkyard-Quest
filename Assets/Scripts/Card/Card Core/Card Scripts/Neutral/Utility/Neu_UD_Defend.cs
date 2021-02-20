using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Neu_UD_Defend : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.Neu_UD_Defend;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        block = 5;
        ActingUnitStatusLoad(actor);
        AffectPlayer(target);
        GainBlock();
    }
}
