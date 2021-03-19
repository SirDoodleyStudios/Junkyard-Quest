using UnityEngine;

//Gain 20 BLOCK
public class War_UD_Reinforce : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_Reinforce;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);

        AffectPlayer(target);
        block = 20;
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
//Gain 3 FORCEFUL
public class War_UD_BreathOfBattle : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_BreathOfBattle;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);

        status = CardMechanics.Forceful;
        stack = 3;

        AffectPlayer(target);
        ApplyStatus();
    }

}

//Gain 3 BLOCK for each stack of FORCEFUL that player has.
public class War_UD_Juggernaut : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_Juggernaut;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);

        baseStatus = CardMechanics.Forceful;

        AffectPlayer(target);
        block = actorUnitStatus.StatusStackChecker(baseStatus) * 3;
        GainBlock();
    }

}

//All or Nothing: Apply all stacks of FORCEFUL to next attack. CONSUME
public class War_UD_AllOrNothing : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_AllOrNothing;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);
        AffectPlayer(target);
        status = CardMechanics.AllOrNothing;
        stack = 1;
        ApplyStatus();
    }

}

//Increase BLOCK by half of current amount.
public class War_UD_Fortress: BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_Fortress;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);
        AffectPlayer(target);
        int currBlock = actor.GetComponent<BaseUnitFunctions>().block;
        block = currBlock / 2;
        GainBlock();
    }

}