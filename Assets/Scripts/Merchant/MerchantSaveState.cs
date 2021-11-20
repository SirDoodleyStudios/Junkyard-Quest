using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MerchantSaveState
{
    //for cards
    public List<CardAndJigsaWrapper> cardOptions = new List<CardAndJigsaWrapper>();
    public List<int> cardOptionCosts = new List<int>();

    //for materials
    public List<CraftingMaterialWrapper> materialOptions = new List<CraftingMaterialWrapper>();
    public List<int> materialOptionCosts = new List<int>();

    //for blueprints
    public List<AllGearTypes> blueprintOptions = new List<AllGearTypes>();
    public List<int> blueprintOptionCosts = new List<int>();

    //for card removals, bool that determines if the removal has already been used
    public bool isCardRemovalAvailable;

    //for keeping track of the discount button when using tickets
    public bool isDiscountActivated;


}
