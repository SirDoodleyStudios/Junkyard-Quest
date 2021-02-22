using UnityEngine;


public class War_UD_Reinforce : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_Reinforce;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        block = 20;
        ActingUnitStatusLoad(actor);
        AffectPlayer(target);
        GainBlock();
    }


}

public class War_UD_WildSwings : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_WildSwings;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);
        AffectPlayer(target);
        ApplyStatus(CardMechanics.Forceful, 1);
        ApplyStatus(CardMechanics.Confused, 1);

    }


}
