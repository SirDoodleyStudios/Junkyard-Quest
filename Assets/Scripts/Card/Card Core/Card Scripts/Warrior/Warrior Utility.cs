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
//Gain 2 COUNTER. Gain 2 VULNERABLE
public class War_UD_EyeForAnEye : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_EyeForAnEye;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);
        AffectPlayer(target);
        status = CardMechanics.Counter;
        stack = 2;
        ApplyStatus();

        status = CardMechanics.Vulnerable;
        stack = 2;
        ApplyStatus();
    }
}
//Become immune to damage this turn. CONSUME.
public class War_UD_AdrenalineRush : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_AdrenalineRush;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingUnitStatusLoad(actor);
        AffectPlayer(target);
        status = CardMechanics.AdrenalineRush;
        stack = 1;
        ApplyStatus();
    }
}



//TARGETTED UTILITY CARDS

//If target's first action is to attack, gain 1 COUNTER.
public class War_UT_LookForOpenings : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UT_LookForOpenings;
    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        //access enemy intents and call for the index 0 of enemy's intents
        //if the action is offense type, do the effect
        EnemyActionFormat enemyIntent = target.GetComponent<EnemyFunctions>().AccessEnemyIntents(0);
        if (enemyIntent.actionType == EnemyActionType.Offense)
        {
            ActingUnitStatusLoad(actor);
            AffectPlayer(actor);
            status = CardMechanics.Counter;
            stack = 1;
            ApplyStatus();
        }

    }
}