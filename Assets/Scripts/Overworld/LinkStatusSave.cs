using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LinkStatusSave : MonoBehaviour
{
    CircleGenerator circleGenerator;
    LinkCollisionIdentifier linkIdentifier;
    RectTransform objectRect;

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
    //identifies if the link has activities
    public bool isTraversed;
    
    private void Start()
    {
        //this will only workk for firs execution saves
        //circleGenerator = gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<CircleGenerator>();
        circleGenerator = transform.parent.parent.GetComponent<CircleGenerator>();
        objectRect = gameObject.GetComponent<RectTransform>();
        linkIdentifier = gameObject.GetComponent<LinkCollisionIdentifier>();
        circleGenerator.d_StoreObjectState += LinkObjectSettings;
    }

    public void LinkObjectSettings()
    {
        linkPosition = new float[2] { objectRect.anchoredPosition.x, objectRect.anchoredPosition.y };
        linkWidth = objectRect.rect.width;
        //in euler angles, a full 360 degrees is conidered for rotation
        //if an angle is below 180 or a positive angle, we keep the angle
        //if an angle is above 180 ir a negative angle, we subtract it by 360 to get the original negative angle
        float tempZ;
        if (objectRect.localRotation.eulerAngles.z - 180 <= 0)
        {
            tempZ = objectRect.localRotation.eulerAngles.z;
        }
        else
        {
            tempZ = objectRect.localRotation.eulerAngles.z - 360;
        }
        linkRotation = new float[3] { objectRect.localRotation.eulerAngles.x, objectRect.localRotation.eulerAngles.y, tempZ };
        parentIndex = gameObject.transform.parent.gameObject.transform.GetSiblingIndex();
        linkIndex = gameObject.transform.GetSiblingIndex();
        linkedInnerNode = linkIdentifier.innerNode.transform.GetSiblingIndex();
        linkedOuterNode = linkIdentifier.outerNode.transform.GetSiblingIndex();
        linkedInnerParent = linkIdentifier.innerNode.transform.parent.gameObject.transform.GetSiblingIndex();
        linkedOuterParent = linkIdentifier.outerNode.transform.parent.gameObject.transform.GetSiblingIndex();
        isTraversed = linkIdentifier.isTraversed;
    }

    private void OnDestroy()
    {
        circleGenerator.d_StoreObjectState -= LinkObjectSettings;
    }

}


