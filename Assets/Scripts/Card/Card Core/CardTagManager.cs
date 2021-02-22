using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTagManager
{
    public static Dictionary<CardMechanics, string> popupDescriptions = new Dictionary<CardMechanics, string>();
    
    //will only be called at the beginningg of combat, applies descriptions in dictionary
    public static void CardTagDictionaryInitialize()
    {
        popupDescriptions.Add(CardMechanics.Block, "Block: Prevents damage on HP by amount. Expires by next turn ");
        popupDescriptions.Add(CardMechanics.Deplete, "Deplete: Becomes unusable for the rest of the battle");
        popupDescriptions.Add(CardMechanics.Forceful, "Forceful: Increases the damage of next attacks by 30%");
        popupDescriptions.Add(CardMechanics.Confused, "Confused: Targets are chosen at random");
    }

    //returns descriptions when called depending on CardTag enum passed
    public static string GetCardTagDescriptions(CardMechanics cardTagKey)
    {
        return popupDescriptions[cardTagKey];
    }

}
