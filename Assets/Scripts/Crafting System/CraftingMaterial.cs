using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Crafting Material", menuName = "Crafting Material")]
public class CraftingMaterial : ScriptableObject
{
    //what material it is
    public AllMaterialTypes materialType;

    //the bound prefix
    public AllMaterialPrefixes materialPrefix;

    //material's available Effects upon crafting
    public List<AllMaterialEffects> materialEffects = new List<AllMaterialEffects>();
}
