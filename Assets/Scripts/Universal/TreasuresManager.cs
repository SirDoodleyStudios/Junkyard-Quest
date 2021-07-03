using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TreasuresManager
{
    //contains all active material Effects on player
    public static List<AllMaterialEffects> activeMaterialEffects = new List<AllMaterialEffects>();

    //function to check if the player has an effect active, if so, return true, if not, false
    //the calculations are done and by the calling function, they only need to confirm from this checker
    public static bool CheckIfActiveMaterialEffect(AllMaterialEffects materialEffect)
    {
        if (activeMaterialEffects.Contains(materialEffect))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //adding and removing will always be in lists because crafting will always have atleast 2 parts
    //if only one effect is to be dded, a single element list will work just fine
    public static void AddActiveMaterialEffects(List<AllMaterialEffects> materialEffects)
    {
        activeMaterialEffects.AddRange(materialEffects);
    }
    public static void RemoveActiveMaterialEffects(List<AllMaterialEffects> materialEffects)
    {
        foreach (AllMaterialEffects materialEffect in materialEffects)
        {
            activeMaterialEffects.Remove(materialEffect);
        }
    }
}
