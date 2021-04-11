using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class NodeStatusSave : MonoBehaviour
{
    CircleGenerator circleGenerator;
    NodeLinkIdentifier nodeIdentifier;
    RectTransform objectRect;

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
    private void Start()
    {
        //this will only workk for firs execution saves
        circleGenerator = gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<CircleGenerator>();
        objectRect = gameObject.GetComponent<RectTransform>();
        nodeIdentifier = gameObject.GetComponent<NodeLinkIdentifier>();
        circleGenerator.d_StoreObjectState += NodeObjectSettings;
    }

    public  void NodeObjectSettings()
    {
        nodePosition = new float[2] { objectRect.anchoredPosition.x, objectRect.anchoredPosition.y };
        parentIndex = gameObject.transform.parent.gameObject.transform.GetSiblingIndex();
        nodeIndex = gameObject.transform.GetSiblingIndex();
        //populates the inner node list and parent with their index reference later for loading
        foreach (GameObject innerNode in nodeIdentifier.linkedInnerNodes)
        {
            Transform nodeTrans = innerNode.transform;
            linkedInnerNodes.Add(nodeTrans.GetSiblingIndex());
            linkedInnerParents.Add(nodeTrans.parent.gameObject.transform.GetSiblingIndex());
        }
        //populates the outer node list and parent with their index reference later for loading
        foreach (GameObject outerNode in nodeIdentifier.linkedOuterNodes)
        {
            Transform nodeTrans = outerNode.transform;
            linkedOuterNodes.Add(nodeTrans.GetSiblingIndex());
            linkedOuterParents.Add(nodeTrans.parent.gameObject.transform.GetSiblingIndex());
        }
        //contains the link in the nodelink parir dictionary
        foreach (KeyValuePair<GameObject, GameObject> innerPair in nodeIdentifier.pairInnerNodeLink)
        {
            Transform linkTrans = innerPair.Value.transform;
            innerLinkValueIndex.Add(linkTrans.GetSiblingIndex());
        }
        foreach (KeyValuePair<GameObject, GameObject> outerPair in nodeIdentifier.pairOuterNodeLink)
        {
            Transform linkTrans = outerPair.Value.transform;
            outerLinkValueIndex.Add(linkTrans.GetSiblingIndex());
        }
        isClickable = nodeIdentifier.isClickable;
        isSelected = nodeIdentifier.isSelected;
    }

    private void OnDestroy()
    {
        circleGenerator.d_StoreObjectState -= NodeObjectSettings;
    }
}
