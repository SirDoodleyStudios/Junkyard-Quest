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

    //DONT NEED A CONSTRUCTOR
    //dictionary paramieter is from the card options
    //public MerchantSaveState(Dictionary<CardAndJigsaWrapper, int> cardNCosts)
    //{
    //    foreach (KeyValuePair<CardAndJigsaWrapper, int> cardNCost in cardNCosts)
    //    {
    //        cardOptions.Add(cardNCost.Key);
    //        cardOptionCosts.Add(cardNCost.Value);
    //    }
    //}
}
