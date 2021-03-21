using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Put Brainstorm Ability in buttons
// I dont think this works baby
public class Neu_AD_Brainstorm : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.Neu_AD_Brainstorm;

    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        ActingCardLoad(card);
        ActingUnitStatusLoad(actor);
        AffectPlayer(target);

    }

}
