using UnityEngine;
using System.Collections.Generic;

//Gain 20 BLOCK
public class War_UD_Reinforce : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_Reinforce;
    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        ActingCardLoad(card);
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
    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        ActingCardLoad(card);
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
    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        ActingCardLoad(card);
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
    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        ActingCardLoad(card);
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
    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        ActingCardLoad(card);
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
    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        ActingCardLoad(card);
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
    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        ActingCardLoad(card);
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
    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        ActingCardLoad(card);
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
//Recover health equal to incoming health damage this turn. CONSUME.
public class War_UD_AdrenalineRush : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_AdrenalineRush;
    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        ActingCardLoad(card);
        ActingUnitStatusLoad(actor);
        AffectPlayer(target);
        status = CardMechanics.AdrenalineRush;
        stack = 1;
        ApplyStatus();
    }
}

//Gain 35 BLOCK if there are no offense cards in hand.
public class War_UD_MasterOfDefense : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_MasterOfDefense;
    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        ActingCardLoad(card);
        ActingUnitStatusLoad(actor);
        //access playingfield from player's parent
        PlayingField playingfield = actor.transform.parent.gameObject.GetComponent<PlayingField>();
        List<Card> handCards = playingfield.deckManager.playerHandList;
        //initial state of identifier is true
        bool doesNotHaveOffense = true;
        foreach (Card handcard in handCards)
        {
            //if an offense card is found, turn identifier into false then immediately break from loop
            if(handcard.cardType == CardType.Offense)
            {
                doesNotHaveOffense = false;
                break;
            }
        }
        //activate effect if there are no offense cards detected in for loop
        if (doesNotHaveOffense)
        {
            AffectPlayer(target);
            block = 35;
            GainBlock();
        }
    }
}

//Gain 1 COUNTER everytime an attack is about to be fully blocked this turn.
public class War_UD_Payback : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_Payback;
    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        ActingCardLoad(card);
        ActingUnitStatusLoad(actor);
        AffectPlayer(actor);
        //unique status payback
        status = CardMechanics.Payback;
        stack = 1;
        ApplyStatus();
    }
}
//Gain 6 BLOCK.If at MOMENTUM 3, gain 2 COUNTER.
public class War_UD_BraceForImpact : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_BraceForImpact;
    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        ActingCardLoad(card);
        ActingUnitStatusLoad(actor);
        AffectPlayer(actor);
        block = 6;
        GainBlock();

        if (actorUnitStatus.StatusStackChecker(CardMechanics.Momentum) >= 3 )
        {
            status = CardMechanics.Counter;
            stack = 2;
            ApplyStatus();
        }

    }
}
//This turn, gain 1 MOMENTUM after playing a BLOCK card.
public class War_UD_Confidefense : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UD_Confidefense;
    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        ActingCardLoad(card);
        //identifiers must come first before actor status loading
        //not using actorUnitStatus from BaseCardEffect because isPlayCounting must be set first before the loading
        UnitStatusHolder actorUnit = actor.GetComponent<UnitStatusHolder>();
        actorUnit.isPlayCounting = true;


        ActingUnitStatusLoad(actor);
        AffectPlayer(actor);

        status = CardMechanics.Confidefense;
        stack = 1;
        ApplyStatus();


    }
}



//TARGETTED UTILITY CARDS

//If target's first action is to attack, gain 1 COUNTER.
public class War_UT_LookForOpenings : BaseCardEffect

{
    public override AllCards enumKeyCard => AllCards.War_UT_LookForOpenings;
    public override void CardEffectActivate(GameObject target, GameObject actor, Card card)
    {
        //access enemy intents and call for the index 0 of enemy's intents
        //if the action is offense type, do the effect
        EnemyActionFormat enemyIntent = target.GetComponent<EnemyFunctions>().AccessEnemyIntents(0);
        if (enemyIntent.actionType == EnemyActionType.Offense)
        {
            ActingCardLoad(card);
            ActingUnitStatusLoad(actor);
            AffectPlayer(actor);
            status = CardMechanics.Counter;
            stack = 1;
            ApplyStatus();
        }

    }
}

