using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class UniversalSaveState
{    //indicates if the Overworld Map is already generated, if this is false, when going back to Overworld scene, it will load the saved Overworld state
    public static bool isMapInitialized { get; set; }
    //only accessed during gameplay, will set this then load the Overworld again during resetting
    public static bool isResetting { get; set; }

    //indicates whether a the map is at its initial phase
    //if false, this will ignore loading of overworldData during loading from file
    //public static bool isNotAtInitialPhase { get; set; }

    //called by overworld manager to save the current state
    public static void SaveOverworldMap(List<LinkStatusSave> linkStatusSave, List<List<NodeStatusSave>> nodeStatusHolderSave/*, UniversalParameters universalParameters*/)
    {
        //calls a private class that transfers the linkStatusSave data to a fully serializable class linkData
        List<LinkData> linkDataList = new List<LinkData>();
        foreach (LinkStatusSave linkStatusData in linkStatusSave)
        {
            LinkData data = new LinkData(linkStatusData);
            linkDataList.Add(data);
        }

        //calls a private class that transfers the list of list of nodeStatusSaves to a list of list of o a fully serializable class NodeData
        //the List<NodeData> will then be transferred inside a wrapper class because we cant serialize nested lists
        List<NodeDataListWrapper> nodeHolderDataList = new List<NodeDataListWrapper>();
        foreach (List<NodeStatusSave> holderStatusSave in nodeStatusHolderSave)
        {
            List<NodeData> nodeDataList = new List<NodeData>();
            foreach (NodeStatusSave nodeStatusSave in holderStatusSave)
            {
                NodeData data = new NodeData(nodeStatusSave);
                nodeDataList.Add(data);
            }
            NodeDataListWrapper dataWrapper = new NodeDataListWrapper(nodeDataList);
            nodeHolderDataList.Add(dataWrapper);
        }
        //once we get the serializable link and data lists, we wrap it in a master class
        SaveMapState saveMapState = new SaveMapState(linkDataList, nodeHolderDataList/*, universalParameters*/);
        //we assign the masterclass wrapper to the json string
        string overWorld = JsonUtility.ToJson(saveMapState);
        File.WriteAllText(Application.persistentDataPath + "/OverWorld.json", overWorld);   
    }

    //Function for Loading
    public static SaveMapState LoadOverWorldMap()
    {
        //for overworld node and link positions and references
        string overWorld = File.ReadAllText(Application.persistentDataPath + "/OverWorld.json");
        SaveMapState loadedState = JsonUtility.FromJson<SaveMapState>(overWorld);
        return loadedState;
    }

    //Saves the important key info of overworld state, gets saved to a different file
    public static void SaveOverWorldData(SaveKeyOverworld saveKey)
    {
        string overWorld = JsonUtility.ToJson(saveKey);
        File.WriteAllText(Application.persistentDataPath + "/OverWorldData.json", overWorld);
    }

    public static SaveKeyOverworld LoadOverWorldData()
    {
        //for loading universal data like current node selected and where it's at
        string overWorldData = File.ReadAllText(Application.persistentDataPath + "/OverWorldData.json");
        SaveKeyOverworld loadedState = JsonUtility.FromJson<SaveKeyOverworld>(overWorldData);
        return loadedState;
    }


    //saves info that are to be used in multiple scenes
    public static void SaveUniversalInformation(UniversalInformation universalInfo)
    {
        PlayerUnitWrapper playerWrapper = new PlayerUnitWrapper(universalInfo.playerStats);
        universalInfo.playerStatsWrapper = playerWrapper;

        string universal = JsonUtility.ToJson(universalInfo);
        File.WriteAllText(Application.persistentDataPath + "/UniversalInfo.json", universal);
    }

    public static UniversalInformation LoadUniversalInformation()
    {
        //for loading universal data like current node selected and where it's at
        string universalInfo = File.ReadAllText(Application.persistentDataPath + "/UniversalInfo.json");
        UniversalInformation loadedState = JsonUtility.FromJson<UniversalInformation>(universalInfo);

        //extracts the player wrapper and reverts it back to PlayerUnit
        PlayerUnitWrapper unitWrapper = loadedState.playerStatsWrapper;

        PlayerUnit loadedUnit = Resources.Load<PlayerUnit>("PlayerSO/Current Stats");
        
        loadedUnit.HP = unitWrapper.HP;
        loadedUnit.Creativity = unitWrapper.Creativity;
        loadedUnit.draw = unitWrapper.draw;
        loadedUnit.energy = unitWrapper.energy;
        loadedUnit.chosenPlayer = unitWrapper.chosenPlayer;
        loadedUnit.currHP = unitWrapper.currHP;
        //after the extract, assign the playerUnit in the universalInformation
        loadedState.playerStats = loadedUnit;

        return loadedState;
    }

    //Specifically foir elements in the RewardsScene
    public static void SaveRewardsState(RewardsSaveState rewardsSaveState)
    {
        string rewards = JsonUtility.ToJson(rewardsSaveState);
        File.WriteAllText(Application.persistentDataPath + "/Rewards.json", rewards);
    }
    public static RewardsSaveState LoadRewardsState()
    {
        string rewards = File.ReadAllText(Application.persistentDataPath + "/Rewards.json");
        RewardsSaveState rewardsState = JsonUtility.FromJson<RewardsSaveState>(rewards);
        return rewardsState;
    }

    //FOR COMBAT SAVES
    public static void SaveCombatState(CombatSaveState combatSaveState)
    {
        PlayerUnitWrapper playerWrapper = new PlayerUnitWrapper(combatSaveState.playerUnit);
        combatSaveState.playerStatsWrapper = playerWrapper;

        string combat = JsonUtility.ToJson(combatSaveState);
        File.WriteAllText(Application.persistentDataPath + "/Combat.json", combat);
    }
    public static CombatSaveState LoadCombatState()
    {
        //for loading other components in CombatSave
        string combatState = File.ReadAllText(Application.persistentDataPath + "/Combat.json");
        CombatSaveState loadedState = JsonUtility.FromJson<CombatSaveState>(combatState);

        //extracts the player wrapper and reverts it back to PlayerUnit
        PlayerUnitWrapper unitWrapper = loadedState.playerStatsWrapper;

        PlayerUnit loadedUnit = Resources.Load<PlayerUnit>("PlayerSO/Current Stats");

        loadedUnit.HP = unitWrapper.HP;
        loadedUnit.Creativity = unitWrapper.Creativity;
        loadedUnit.draw = unitWrapper.draw;
        loadedUnit.energy = unitWrapper.energy;
        loadedUnit.chosenPlayer = unitWrapper.chosenPlayer;
        loadedUnit.currHP = unitWrapper.currHP;
        //after the extract, assign the playerUnit in the universalInformation
        loadedState.playerUnit = loadedUnit;

        return loadedState;
    }

}



//HELPER CLASSES for saving the OverWorld

//actual class that holds the node and link data list
[Serializable]
public class SaveMapState
{
    public List<LinkData> linkList = new List<LinkData>();
    public List<NodeDataListWrapper> nodeList = new List<NodeDataListWrapper>();
    //public bool isNotAtInitialPhase;

    //public UniversalParameters universalParameters

    public SaveMapState(List<LinkData> links, List<NodeDataListWrapper> nodes)
    {
        linkList = links;
        nodeList = nodes;
        //isNotAtInitialPhase = UniversalSaveState.isNotAtInitialPhase;

    }
}
//serializing class for links
[Serializable]
public class LinkData
{
    //Values to Serialize//
    //will hold the vector2 anchored position
    public float[] linkPosition;
    //will hold the vector3 rotation of the link
    public float[] linkRotation;
    //the length of the link
    public float linkWidth;
    //will hold node parent index and index of node itself
    public int parentIndex;
    public int linkIndex;
    //index of the list of nodes
    public int linkedInnerNode;
    public int linkedOuterNode;
    //index of nodes' parents so we know which circle to call when loading
    public int linkedInnerParent;
    public int linkedOuterParent;
    //identifier if link was already treaded upon
    public bool isTraversed;
    public LinkData(LinkStatusSave linkSave)
    {
        linkPosition = linkSave.linkPosition;
        linkRotation = linkSave.linkRotation;
        linkWidth = linkSave.linkWidth;
        parentIndex = linkSave.parentIndex;
        linkIndex = linkSave.linkIndex;
        linkedInnerNode = linkSave.linkedInnerNode;
        linkedOuterNode = linkSave.linkedOuterNode;
        linkedInnerParent = linkSave.linkedInnerParent;
        linkedOuterParent = linkSave.linkedOuterParent;
        isTraversed = linkSave.isTraversed;
    }

}

//serializing class for nodes
[Serializable]
public class NodeData
{
    //Values to Serialize//
    //will hold the vector2 anchored position
    public float[] nodePosition;
    //will hold node parent index and index of node itself
    public int parentIndex;
    public int nodeIndex;
    //list of indices of the list of nodes
    public List<int> linkedInnerNodes;
    public List<int> linkedOuterNodes;
    //list of the nodes' parents so we know which circle to call when loading
    public List<int> linkedInnerParents;
    public List<int> linkedOuterParents;
    //these will depend on the indices captured in the nodes as keys
    //these are the dictioanry values whil the list of indices above will serve as the keys
    public List<int> innerLinkValueIndex;
    public List<int> outerLinkValueIndex;
    //bool identifiers of the node
    public bool isClickable;
    public bool isSelected;
    public bool isTraversed;
    //nodeActivity num of the node
    public NodeActivityEnum nodeActivity;

    public NodeData(NodeStatusSave nodeData)
    {
        nodePosition = nodeData.nodePosition;
        parentIndex = nodeData.parentIndex;
        nodeIndex = nodeData.nodeIndex;
        linkedInnerNodes = nodeData.linkedInnerNodes;
        linkedOuterNodes = nodeData.linkedOuterNodes;
        linkedInnerParents = nodeData.linkedInnerParents;
        linkedOuterParents = nodeData.linkedOuterParents;
        innerLinkValueIndex = nodeData.innerLinkValueIndex;
        outerLinkValueIndex = nodeData.outerLinkValueIndex;
        isClickable = nodeData.isClickable;
        isSelected = nodeData.isSelected;
        isTraversed = nodeData.isTraversed;
        nodeActivity = nodeData.nodeActivityEnum;
    }
}

//wrapper class for nodes because unity cannot serialize List<List<T>>
[Serializable]
public class NodeDataListWrapper
{
    public List<NodeData> nodeDataList = new List<NodeData>();
    public NodeDataListWrapper(List<NodeData> nodeDataWrap)
    {
        nodeDataList = nodeDataWrap;
    }
}

//this is the serializable version of the PlayerUnit that can be saved in the json
[Serializable]
public class PlayerUnitWrapper
{
    public int HP;
    public int Creativity;
    public int draw;
    public int currHP;

    //for player
    public int energy;
    public ChosenPlayer chosenPlayer;

    //for enemy
    public PlayerUnitWrapper(PlayerUnit unit)
    {
        HP = unit.HP;
        Creativity = unit.Creativity;
        draw = unit.draw;
        energy = unit.energy;
        chosenPlayer = unit.chosenPlayer;
        currHP = unit.currHP;
    }
}

//di ko alaam ang gagawin dito
//public class UniversalParameters
//{
//    //will contain the enum as string
//    public string currentState;
//    //node selected
//    public int NodeIndex;
//    public int NodeParent;
//    //bool found in UniversaolSaveState script
//    public bool isInitialized;

//    UniversalParameters(OverworldState worldState, int nodeIndex, int parentIndex)
//    {
//        currentState = worldState.ToString();
//        NodeIndex = nodeIndex;
//        NodeParent = parentIndex;
//        isInitialized = UniversalSaveState.isMapInitialized;
//    }
//}

/////////////////////////////////
