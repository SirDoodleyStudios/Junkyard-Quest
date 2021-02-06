using UnityEngine;


public class War_UD_Reinforce : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_Reinforce;
    public override void CardEffectActivate(GameObject target)
    {
        block = 20;
        AffectPlayer(target);
        GainBlock();
    }
}
