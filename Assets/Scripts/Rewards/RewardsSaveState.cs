using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RewardsSaveState
{
    //takes note of availabitiy of current reward
    //not always needed
    public CombatRewards currentReward;
    public bool isCurrentRewardEnabled;
    public int objectOriginIndex;
    public int currentScraps;

    //takes note of what objects are enabled and ready for clicking
    public int claimRewardCounter;
    public List<CombatRewards> rewardsList = new List<CombatRewards>();
    public List<bool> rewardsAvailabilityList = new List<bool>();

    //wrapper class that contains the list of cardDraft ints
    public CardDraftListWrapper cardDraftListWrapper = new CardDraftListWrapper();

    //contains the predetermined materials
    public MaterialDraftListWrapper materialDraftListWrapper = new MaterialDraftListWrapper();

    //contains the predetermined gears
    public GearDraftListWrapper gearDraftListWrapper = new GearDraftListWrapper();

    //contains the prefetermind blueprints
    public BlueprintDraftListWrapper blueprintDraftListWrapper = new BlueprintDraftListWrapper();
    
}
