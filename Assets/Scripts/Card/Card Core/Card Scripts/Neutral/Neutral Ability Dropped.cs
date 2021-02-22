using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Put Brainstorm Ability in buttons
public class Neu_AD_Brainstorm : BaseCardEffect
{
    public override AllCards enumKeyCard => AllCards.Neu_AD_Brainstorm;

    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        AffectPlayer(target);
    }

}
