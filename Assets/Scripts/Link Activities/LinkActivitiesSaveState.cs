using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LinkActivitiesSaveState
{
    //contains the current moving node position
    public int currentLinkNodeIndex;

    //identifier if player is in link activity path or on the activity itself
    //works only for activities that doesn't take player to another scene like skirmish, tests, and gambles
    public bool isInActivity;

    //For Trades, to store the current object
    public GearWrapper gearWrapper;
    public CraftingMaterialWrapper craftingMaterialWrapper;
    public AllGearTypes blueprint;
}
