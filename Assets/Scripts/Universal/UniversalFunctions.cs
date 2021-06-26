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

    //for Geneerateing Cards
    
}
