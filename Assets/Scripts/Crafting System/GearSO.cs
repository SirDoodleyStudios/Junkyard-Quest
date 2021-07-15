using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Gear", menuName = "Gear")]
public class GearSO : ScriptableObject
{
    //holds the list of all effects in the gear
    public List<AllMaterialEffects> gearEffects = new List<AllMaterialEffects>();
    //determines the gear type
    public AllGearTypes gearType;
    //determines if the gear has a set bonus
    //the effect of the set bonus hould already be in the gearEffects
    public AllMaterialPrefixes gearSetBonus;
}
