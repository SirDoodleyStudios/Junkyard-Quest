using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardModificationManager
{
    //NOT USED // JUST FAILED TESTING
    public static JigsawFormat TestJigsawApplication()
    {
        JigsawFormat randomJigsaw = new JigsawFormat();
        JigsawLink input = (JigsawLink)Random.Range(0, 2);
        JigsawLink output = (JigsawLink)Random.Range(0, 2);

        randomJigsaw.inputLink = input;
        randomJigsaw.outputLink = output;

        randomJigsaw.jigsawImage = DetermineJigsawImage(input, output);

        //reason for hardcoded indices is because this functionality is for test only, Jigsaws are predetermined upon entering combat
        int tempIndex = (Random.Range(0, 2));
        randomJigsaw.enumJigsawName = (AllJigsaws)tempIndex;

        if (tempIndex >= 1)
        {
            randomJigsaw.jigsawMethod = CardMethod.Dropped;
        }
        else if (tempIndex <= 0)
        {
            randomJigsaw.jigsawMethod = CardMethod.Targetted;
        }

        //for getting text of jigsaw
        randomJigsaw.jigsawDescription = CardTagManager.GetJigsawDescriptions(randomJigsaw.enumJigsawName);

        return randomJigsaw;

    }

    public static Sprite DetermineJigsawImage(JigsawLink input, JigsawLink output)
    {
        Sprite jigsawSprite;

        //circle starting
        if (input == JigsawLink.Circle)
        {
            if(output == JigsawLink.Circle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/C2C");

            else if (output == JigsawLink.Square)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/C2S");

            else if (output == JigsawLink.Triangle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/C2T");
            else
            jigsawSprite = Resources.Load<Sprite>("Jigsaw/Blank");

            return jigsawSprite;

        }
        else if (input == JigsawLink.Square)
        {
            if (output == JigsawLink.Circle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/S2C");

            else if (output == JigsawLink.Square)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/S2S");

            else if (output == JigsawLink.Triangle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/S2T");
            jigsawSprite = Resources.Load<Sprite>("Jigsaw/Blank");

            return jigsawSprite;

        }
        else if (input == JigsawLink.Triangle)
        {
            if (output == JigsawLink.Circle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/T2C");

            else if (output == JigsawLink.Square)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/T2S");

            else if (output == JigsawLink.Triangle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/T2T");
            else
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/Blank");

            return jigsawSprite;

        }
        else
            jigsawSprite = Resources.Load<Sprite>("Jigsaw/Blank");
        return jigsawSprite;
    }
}
