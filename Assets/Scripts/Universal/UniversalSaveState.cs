﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class UniversalSaveState
{    //indicates if the Overworld Map is already generated, if this is false, when going back to Overworld scene, it will load the saved Overworld state
    public static bool isMapInitialized { get; set; }


    //called by overworld manager to save the current state
    public static void SaveOverworldMap(List<LinkStatusSave> linkStatusSave, List<List<NodeStatusSave>> nodeStatusHolderSave)
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
        SaveMapState saveMapState = new SaveMapState(linkDataList, nodeHolderDataList);
        //we assign the masterclass wrapper to the json string
        string overWorld = JsonUtility.ToJson(saveMapState);
        File.WriteAllText(Application.persistentDataPath + "/OverWorld.json", overWorld);   
    }


    //Function for Loading
    public static SaveMapState LoadOverWorldMap()
    {
        string overWorld = File.ReadAllText(Application.persistentDataPath + "/OverWorld.json");
        SaveMapState loadedState = JsonUtility.FromJson<SaveMapState>(overWorld);
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

    public SaveMapState(List<LinkData> links, List<NodeDataListWrapper> nodes)
    {
        linkList = links;
        nodeList = nodes;
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
    //will hold node parent index and index of node itself
    public int parentIndex;
    public int linkIndex;
    //index of the list of nodes
    public int linkedInnerNode;
    public int linkedOuterNode;
    //index of nodes' parents so we know which circle to call when loading
    public int linkedInnerParent;
    public int linkedOuterParent;
    public LinkData(LinkStatusSave linkSave)
    {
        linkPosition = linkSave.linkPosition;
        linkRotation = linkSave.linkRotation;
        parentIndex = linkSave.parentIndex;
        linkIndex = linkSave.linkIndex;
        linkedInnerNode = linkSave.linkedInnerNode;
        linkedOuterNode = linkSave.linkedOuterNode;
        linkedInnerParent = linkSave.linkedInnerParent;
        linkedOuterParent = linkSave.linkedOuterParent;
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

/////////////////////////////////