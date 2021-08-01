﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UniversalFunctions
{
    //for decoding CardAndJigsawWrapper to JigsawFormat
    public static JigsawFormat LoadJigsawFormat(JigsawFormat jigsawFormat, CardAndJigsaWrapper cardJigsawWrapper)
    {
        //since moving this function in universalFunctions, the instantiate must be done in the manager script 
        //JigsawFormat jigsawFormat = Instantiate(referenceJigsawFormat);
        jigsawFormat.enumJigsawCard = cardJigsawWrapper.jigsawEnum;
        jigsawFormat.jigsawMethod = cardJigsawWrapper.jigsawMethod;
        jigsawFormat.inputLink = cardJigsawWrapper.inputLink;
        jigsawFormat.outputLink = cardJigsawWrapper.outputLink;
        jigsawFormat.jigsawCard = CardSOFactory.GetCardSO(cardJigsawWrapper.jigsawEnum);
        //jigsaw visuals
        jigsawFormat.jigsawDescription = CardTagManager.GetJigsawDescriptions(jigsawFormat.enumJigsawCard);
        jigsawFormat.jigsawImage = Resources.Load<Sprite>($"Jigsaw/{jigsawFormat.inputLink}2{jigsawFormat.outputLink}");
        //jigsawFormat.jigsawImage = CardTagManager.DetermineJigsawImage(jigsawFormat.inputLink, jigsawFormat.outputLink);
        //jigsaw Effects
        //jigsawFormat.jigsawCard = Instantiate(CardSOFactory.GetCardSO(cardJigsawWrapper.cardEnum));
        return jigsawFormat;
    }

    //helper function to determine the desired positions of material slotes depending on blueprint
    //all positions here are just created from edito to see what looks good with a given image then listed here
    //the position order is always counted from top to bottom
    public static BluePrintSO AssignUniqueBlueprintValues(BluePrintSO blueprint)
    {
        //temporary lists to be assigned to the return bluepriny\t
        List<Vector2> materialSlotPositions = new List<Vector2>();
        List<AllMaterialTypes> materialTypes = new List<AllMaterialTypes>();

        //each blueprint type has a predefined possition for material tpes and what available material types can be slotted in
        //the two lists must be synchronized by their indices
        switch (blueprint.blueprint)
        {
            case AllGearTypes.Sword:
                materialSlotPositions.Add(new Vector2(-312, 86.5f));
                materialSlotPositions.Add(new Vector2(-312, -165));
                materialTypes.Add(AllMaterialTypes.Slab);
                materialTypes.Add(AllMaterialTypes.Strips);
                blueprint.gearClassifications = AllGearClassifications.MainHand;
                break;
            case AllGearTypes.Axe:
                materialSlotPositions.Add(new Vector2(-204, 105));
                materialSlotPositions.Add(new Vector2(-333, -173));
                materialTypes.Add(AllMaterialTypes.Slab);
                materialTypes.Add(AllMaterialTypes.Stick);
                blueprint.gearClassifications = AllGearClassifications.MainHand;
                break;
            case AllGearTypes.Shield:
                break;
            case AllGearTypes.Hammer:
                break;
            case AllGearTypes.Greatsword:
                break;
            case AllGearTypes.Spear:
                break;
            default:
                break;
        }
        blueprint.materialSlotPositions = materialSlotPositions;
        blueprint.acceptableMaterialTypes = materialTypes;
        return blueprint;
    }

    //convert a list of CraftingMaterialSO into a list of CraftingMaterialWrappers
    public static List<CraftingMaterialWrapper> ConvertCraftingMaterialSOListToWrapper(List<CraftingMaterialSO> craftingMaterialSOList)
    {

        List<CraftingMaterialWrapper> tempWrapperList = new List<CraftingMaterialWrapper>();
        foreach (CraftingMaterialSO craftingMaterial in craftingMaterialSOList)
        {
            CraftingMaterialWrapper craftingMaterialWrapper = new CraftingMaterialWrapper(craftingMaterial);
            tempWrapperList.Add(craftingMaterialWrapper);
            //universalInfo.craftingMaterialWrapperList.Add(craftingMaterialWrapper);
        }
        return tempWrapperList;        
    }


    //convert a list of GearSO into a list of GearSOWrappers
    public static List<GearWrapper> ConvertGearSOListToWrapper(List<GearSO> gearList)
    {
        //gear wrappers are converted here but loading will depend on the calling function
        List<GearWrapper> tempGearWrapperList = new List<GearWrapper>();
        foreach (GearSO gearSO in gearList)
        {
            GearWrapper gearWrapper = new GearWrapper(gearSO);
            tempGearWrapperList.Add(gearWrapper);
        }
        return tempGearWrapperList;
    }
    //array counterpart
    public static GearWrapper[] ConvertMaterialSOListToWrapperArray(GearSO[] equipArray)
    {
        GearWrapper[] tempArray = new GearWrapper[6];
        for (int i = 0; 5 >= i; i++)
        {
            if (equipArray[i] == null)
            {
                tempArray[i] = null;
            }
            else
            {
                GearWrapper gearWrapper = new GearWrapper(equipArray[i]);
                tempArray[i] = gearWrapper;
            }
        }
        return tempArray;

    }


}
