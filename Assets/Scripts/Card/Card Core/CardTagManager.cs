using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTagManager
{
    //for card mechanics when hovering over cards and stuff
    public static Dictionary<CardMechanics, string> popupDescriptions = new Dictionary<CardMechanics, string>();

    //for Jigsaw Descriptions to be copied to Card itself for display
    public static Dictionary<AllJigsaws, string> jigsawDescriptions = new Dictionary<AllJigsaws, string>();

    //for Card Effect Descriptions to be copied to Card itself for display
    public static Dictionary<AllCards, string> cardEffectDescriptions = new Dictionary<AllCards, string>();

    //This is called by combatmanager awake then activates all initializers
    public static void InitializeTextDescriptionDictionaries()
    {
        CardTagDictionaryInitialize();
        JigsawDictionaryInitialize();
        CardEffectDictionaryInitialize();

    }


    //will only be called at the beginningg of combat, applies descriptions in dictionary
    static void CardTagDictionaryInitialize()
    {
        popupDescriptions.Add(CardMechanics.Ability, "Ability: Grants the player an active action that can be used once per turn");
        popupDescriptions.Add(CardMechanics.Slay, "Slay: Activates effect when an enemy is defeated using selected card.");
        popupDescriptions.Add(CardMechanics.Block, "Block: Prevents damage on HP by amount. Expires by next turn ");
        popupDescriptions.Add(CardMechanics.Consume, "Consume: After playing, card becomes unusable for the rest of the battle");
        popupDescriptions.Add(CardMechanics.Forceful, "Forceful: Increases the damage of next attacks by 30%");
        popupDescriptions.Add(CardMechanics.Feeble, "Feeble: Decreaseses next block gains by 30%");
        popupDescriptions.Add(CardMechanics.Confused, "Confused: Targets are chosen at random");
        popupDescriptions.Add(CardMechanics.Shocked, "Shocked: Receives additional damage equal to stack.");
        popupDescriptions.Add(CardMechanics.GenerateForceful, "Generate Forceful: Applies FORCEFUL stacks next turn");
        popupDescriptions.Add(CardMechanics.Momentum, "Momentum: Improve certain card effects when played if Momentum stack level is met.");
        popupDescriptions.Add(CardMechanics.AllOrNothing, "All or Nothing: Apply all stacks of FORCEFUL to next attack. CONSUME");
    }

    //returns descriptions when called depending on CardTag enum passed
    public static string GetCardTagDescriptions(CardMechanics cardTagKey)
    {
        return popupDescriptions[cardTagKey];
    }


    //JIGSAW STUFF

    //called by Deckmanager
    static void JigsawDictionaryInitialize()
    {
        jigsawDescriptions.Add(AllJigsaws.Deal_10_Damage, "Deal 10 damage to target");
        jigsawDescriptions.Add(AllJigsaws.Draw_1_Card, "Draw 1 card");
        jigsawDescriptions.Add(AllJigsaws.Gain_10_Block, "Gain 10 BLOCK");
    }

    public static string GetJigsawDescriptions(AllJigsaws jigsawKey)
    {
        return jigsawDescriptions[jigsawKey];
    }


    public static Sprite DetermineJigsawImage(JigsawLink input, JigsawLink output)
    {
        Sprite jigsawSprite;

        //circle starting
        if (input == JigsawLink.Circle)
        {
            if (output == JigsawLink.Circle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/C2C");

            else if (output == JigsawLink.Square)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/C2S");

            else if (output == JigsawLink.Triangle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/C2T");
            else
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/Blank");

            return jigsawSprite;

        }
        else if (input == JigsawLink.Square)
        {
            if (output == JigsawLink.Circle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/S2C");

            else if (output == JigsawLink.Square)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/S2S");

            else if (output == JigsawLink.Triangle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/S2T");
            jigsawSprite = Resources.Load<Sprite>("Jigsaw/Blank");

            return jigsawSprite;

        }
        else if (input == JigsawLink.Triangle)
        {
            if (output == JigsawLink.Circle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/T2C");

            else if (output == JigsawLink.Square)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/T2S");

            else if (output == JigsawLink.Triangle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/T2T");
            else
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/Blank");

            return jigsawSprite;

        }
        else
            jigsawSprite = Resources.Load<Sprite>("Jigsaw/Blank");
        return jigsawSprite;
    }


    //CARD EFFECT STUFF

    static void CardEffectDictionaryInitialize()
    {
        //NEUTRAL CARDS
        cardEffectDescriptions.Add(AllCards.Neu_OT_Attack, "Deal 5 DAMAGE");
        cardEffectDescriptions.Add(AllCards.Neu_OT_Strike, "Deal 12 DAMAGE");

        cardEffectDescriptions.Add(AllCards.Neu_UD_Defend, "Gain 5 BLOCK");

        cardEffectDescriptions.Add(AllCards.Neu_AD_Brainstorm, "ABILITY. Gain 2 Creativity in exchange for 2 Energy");

        //WARRIOR CARDS
        //OT
        cardEffectDescriptions.Add(AllCards.War_OT_Sting, "Apply SHOCKED 1. Deal 1 damage 2 times.");
        cardEffectDescriptions.Add(AllCards.War_OT_HeavyWeapon, "Deal 25 DAMAGE. Lose 8 health if player doesn't have FORCEFUL.");
        cardEffectDescriptions.Add(AllCards.War_OT_EfficientAttack, "Deal 9 DAMAGE. Gain 1 FORCEFUL if player has FORCEFUL.");
        cardEffectDescriptions.Add(AllCards.War_OT_FinishingBlow, "Deal 6 DAMAGE. Gain 5 FORCEFUL at SLAY.");
        cardEffectDescriptions.Add(AllCards.War_OT_UnrelentingStrike, "Deal 8 DAMAGE. If at 1 MOMENTUM, gain 1 FORCEFUL.");
        cardEffectDescriptions.Add(AllCards.War_OT_CleanHit, "Gain 1 MOMENTUM then deal 5 DAMAGE. Deal 15 DAMAGE instead if at 3 MOMENTUM");
        cardEffectDescriptions.Add(AllCards.War_OT_FortifyingBlow, "Deal 12 damage, gain BLOCK equal to unblocked damage.");
        //OD
        cardEffectDescriptions.Add(AllCards.War_OD_Cleave, "Deal 8 DAMAGE to all enemies.");
        cardEffectDescriptions.Add(AllCards.War_OD_ShrapnelBlast, "Lose half of your BLOCK, deal damage equal to BLOCK lost to all enemies.");
        //UT
        cardEffectDescriptions.Add(AllCards.War_UT_LookForOpenings, "If target is going to attack, gain 1 COUNTER.");
        //UD
        cardEffectDescriptions.Add(AllCards.War_UD_Reinforce, "Gain 20 BLOCK");
        cardEffectDescriptions.Add(AllCards.War_UD_WildSwings, "Gain 1 FORCEFUL and 1 CONFUSED.");
        cardEffectDescriptions.Add(AllCards.War_UD_AllOrNothing, "Apply all FORCEFUL stacks on next DAMAGE dealt.");
        cardEffectDescriptions.Add(AllCards.War_UD_BreathOfBattle, "Gain 3 FORCEFUL");
        cardEffectDescriptions.Add(AllCards.War_UD_Juggernaut, "Gain 3 BLOCK for each stack of FORCEFUL that player has.");
        cardEffectDescriptions.Add(AllCards.War_UD_SteadyImprovement, "Gain FORCEFUL next turn by how many attack hits dealt this turn.");
        cardEffectDescriptions.Add(AllCards.War_UD_Fortress, "Increase BLOCK by half of amount.");
        cardEffectDescriptions.Add(AllCards.War_UD_EyeForAnEye, "Gain 2 COUNTER. Gain 2 VULNERABLE");
        cardEffectDescriptions.Add(AllCards.War_UD_AdrenalineRush, "Become immune to damage this turn. CONSUME.");
        cardEffectDescriptions.Add(AllCards.War_UD_MasterOfDefense, "Gain 35 BLOCK if there are no offense cards in hand.");
        cardEffectDescriptions.Add(AllCards.War_UD_Payback, "Gain 1 COUNTER when a hit is fully blocked.");
        cardEffectDescriptions.Add(AllCards.War_UD_BraceForImpact, "Gain 6 BLOCK. If at MOMENTUM 3, gain 2 COUNTER.");
        cardEffectDescriptions.Add(AllCards.War_UD_Confidefense, "Gain 1 MOMENTUM for each BLOCK card played this turn.");



        //AD
        cardEffectDescriptions.Add(AllCards.War_AD_BruteForce, "ABILITY. Gain 1 FORCEFUL in exchange for 7 BLOCK");
        cardEffectDescriptions.Add(AllCards.War_AD_PrioritizeDefense, "ABILITY. Gain 9 BLOCK for each COUNTER. Remove all COUNTER.");


    }

    public static string GetCardEffectDescriptions(AllCards cardKey)
    {
        return cardEffectDescriptions[cardKey];
    }


}
