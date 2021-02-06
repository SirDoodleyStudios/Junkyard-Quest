using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS IS A HOLDER FOR JIGSAW AND ABILITIES
[CreateAssetMenu(fileName = "Jigsaw or Ability", menuName = "Jigsaw or Ability")]
public abstract class BaseScriptJigAbi : ScriptableObject
{
    //everytime a jigsaw is instantiated, inputs and outputs are going to be randomized
    public JigsawLink inputLink;
    public JigsawLink outputLink;

    //THIS SHOULD BE ADDED AFTER RANDOMIZING
    public AllJigsaws enumJigsawName;
}
