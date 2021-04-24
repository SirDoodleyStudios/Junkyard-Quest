using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Jigsaw", menuName = "Jigsaw")]
public class JigsawFormat : ScriptableObject
{
    ////everytime a jigsaw is instantiated, inputs and outputs are going to be randomized
    //public override JigsawLink inputLink => inputLink;
    //public override JigsawLink outputLink => outputLink;

    ////THIS SHOULD BE ADDED AFTER RANDOMIZING
    //public override AllJigsaws enumJigsawName => enumJigsawName;

    //everytime a jigsaw is instantiated, inputs and outputs are going to be randomized
    public JigsawLink inputLink;
    public JigsawLink outputLink;

    //THIS SHOULD BE ADDED AFTER RANDOMIZING
    public AllJigsaws enumJigsawName;

    //For Creative mode to determine whether it's targetted or not
    public CardMethod jigsawMethod;

    //image of jigsawa pattern based on input and output links
    public Sprite jigsawImage;

    //description to be determined 
    public string jigsawDescription;
}
