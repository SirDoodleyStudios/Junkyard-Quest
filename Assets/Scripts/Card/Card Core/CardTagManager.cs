using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTagManager
{
    //for card mechanics when hovering over cards and stuff
    public static Dictionary<CardMechanics, string> popupDescriptions = new Dictionary<CardMechanics, string>();

    //for Jigsaw Descriptions to be copied to Vard itself for display
    public static Dictionary<AllJigsaws, string> jigsawDescriptions = new Dictionary<AllJigsaws, string>();


    public static void InitializeTextDescriptionDictionaries()
    {
        CardTagDictionaryInitialize();
        JigsawDescriptionInitialize();
    }
    //will only be called at the beginningg of combat, applies descriptions in dictionary
    static void CardTagDictionaryInitialize()
    {
        popupDescriptions.Add(CardMechanics.Ability, "Ability: Grants the player an active action that can be used once per turn");
        popupDescriptions.Add(CardMechanics.Block, "Block: Prevents damage on HP by amount. Expires by next turn ");
        popupDescriptions.Add(CardMechanics.Deplete, "Deplete: Becomes unusable for the rest of the battle");
        popupDescriptions.Add(CardMechanics.Forceful, "Forceful: Increases the damage of next attacks by 30%");
        popupDescriptions.Add(CardMechanics.Feeble, "Feeble: Decreaseses next block gains by 30%");
        popupDescriptions.Add(CardMechanics.Confused, "Confused: Targets are chosen at random");
        popupDescriptions.Add(CardMechanics.Shocked, "Shocked: Receives additional damage equal to stack.");
    }

    //returns descriptions when called depending on CardTag enum passed
    public static string GetCardTagDescriptions(CardMechanics cardTagKey)
    {
        return popupDescriptions[cardTagKey];
    }

    //called by Deckmanager
    static void JigsawDescriptionInitialize()
    {
        jigsawDescriptions.Add(AllJigsaws.Deal_10_Damage, "Deal 10 damage to target");
        jigsawDescriptions.Add(AllJigsaws.Draw_1_Card, "Draw 1 card");
        jigsawDescriptions.Add(AllJigsaws.Gain_10_Block, "Gain 10 BLOCK");
    }

    public static string GetJigsawDescriptions(AllJigsaws jigsawKey)
    {
        return jigsawDescriptions[jigsawKey];
    }

}
