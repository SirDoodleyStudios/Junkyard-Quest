using UnityEngine;

//Gain 20 BLOCK
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
//Gain 1 FORCEFUL and 1 CONFUSED
public class War_UD_WildSwings : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_WildSwings;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);
        AffectPlayer(target);

        status = CardMechanics.Forceful;
        stack = 1;
        ApplyStatus();

        status = CardMechanics.Confused;
        ApplyStatus();

    }

}

//Gain FORCEFUL next turn by how many attack hits dealt this turn.
public class War_UD_SteadyImprovement : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_SteadyImprovement;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);
        AffectPlayer(target);

        status = CardMechanics.GenerateForceful;
        stack = 1;
        actor.GetComponent<UnitStatusHolder>().isHitCounting = true;
        ApplyStatusByCounter();
    }

}
