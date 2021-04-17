using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

//This is the wrapper class that will contain all information needed to be accessed in all scenes
[Serializable]
public class UniversalInformation
{
    public NodeActivityEnum nextNode;
    public bool isTargetNodeTraversed;
    public bool isPartnerLinkTraversed;


    public void UniversalOverWorldStart()
    {

    }
    //must alwways be filled up before loadLinkscene is called
    public void UniversalOverworldMove(NodeActivityEnum next, bool isNodeTraversed, bool isLinkTraversed)
    {
        nextNode = next;
        isTargetNodeTraversed = isNodeTraversed;
        isPartnerLinkTraversed = isLinkTraversed;
    }


}
