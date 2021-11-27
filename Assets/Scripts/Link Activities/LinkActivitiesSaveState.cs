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
    public bool isStillInActivity;

    //For Trades, to store the current object
    public GearWrapper gearWrapper;
    public CraftingMaterialWrapper craftingMaterialWrapper;
    public AllGearTypes blueprint;

    //for Gambles
    //contains the chosen cards
    public List<CardAndJigsaWrapper> gambleCards = new List<CardAndJigsaWrapper>();
    //tells which card is the activity in
    //0 = base card, 1 = first draw, 2 = second draw, 3 = third draw
    public int drawIndex;
    public int ticketCost;
    public int ticketRewards;

}
