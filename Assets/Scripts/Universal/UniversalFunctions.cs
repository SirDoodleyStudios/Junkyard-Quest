using System.Collections;
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

}
