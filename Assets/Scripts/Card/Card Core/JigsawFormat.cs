﻿using UnityEngine;

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


}
