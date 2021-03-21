using UnityEngine;
using System.Collections.Generic;

//ABILITY. Gain 1 FORCEFUL in exchange for 7 BLOCK
public class War_Abi_BruteForce : BaseAbilityEffect
{
    public override AllCards enumKeyCard => AllCards.Ability;
    public override AllAbilities enumKeyAbility => AllAbilities.War_Abi_BruteForce;

    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingCardLoad(card);
        //affect player is actor since actor is the player gameObject
        ActingUnitStatusLoad(actor);
        AffectPlayer(actor);
        status = CardMechanics.Forceful;
        stack = 1;
        ApplyStatus();
        block = -7;
        GainBlock();
    }

    public override bool RequirementCheck(PlayingField playingField)
    {
        //if block is 7 or more
        if (playingField.playerPrefab.GetComponent<PlayerFunctions>().block >= 7)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}

//ABILITY. Gain 9 BLOCK for each COUNTER. Remove all COUNTER.
public class War_Abi_PrioritizeDefense : BaseAbilityEffect
{
    public override AllCards enumKeyCard => AllCards.Ability;
    public override AllAbilities enumKeyAbility => AllAbilities.War_Abi_PrioritizeDefense;

    public override void CardEffectActivate(GameObject target, GameObject actor)
    {
        ActingCardLoad(card);
        //affect player is actor since actor is the player gameObject
        ActingUnitStatusLoad(actor);
        AffectPlayer(actor);

        int counterStack = actorUnitStatus.usageStatusDict[CardMechanics.Counter];
        //depletes whoule counter stacks
        status = CardMechanics.Counter;
        stack = -counterStack;
        ApplyStatus();
        //gain Block in 9* counter stack amount
        block = 9*counterStack;
        GainBlock();
    }

    public override bool RequirementCheck(PlayingField playingField)
    {
        //if has counter
        
        if (playingField.playerPrefab.GetComponent<UnitStatusHolder>().usageStatusDict.ContainsKey(CardMechanics.Counter))
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
