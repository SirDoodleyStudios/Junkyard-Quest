using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGenerator : MonoBehaviour
{
    //event attached to LinkCollisionIdentifier script of links
    public delegate void D_DestroyLinks();
    public event D_DestroyLinks d_DestroyLinks;

    GameObject nodeCircleManager;
    Transform nodeCircleTransform;
    RectTransform nodeCircleManagerRect;
    //Holder for the nodes
    public GameObject parentCircle;
    List<GameObject> parentCircleList = new List<GameObject>();
    //the node actual, uses a prefab from inspector
    public GameObject nodePrefab;
    //the link prefab, will have a logic for a randomizer image
    public GameObject linkPrefab;
    public GameObject linkHolder;
    Transform linkHolderTrans;


    void Start()
    {
        nodeCircleManager = gameObject;
        nodeCircleManagerRect = nodeCircleManager.GetComponent<RectTransform>();
        nodeCircleTransform = nodeCircleManager.transform;
        linkHolderTrans = linkHolder.transform;

        //call generation of circle nodes
        //initial i is the first outside circle with the largest number of nodes
        //for (int i = 10; i >= 8; i--)
        //{
        //    float diameterPercent = 1 - ((11 - i) * .2f);
        //    PlotNodes(i, diameterPercent);
        //}
        PlotNodes(7, .95f);
        PlotNodes(6, .75f);
        PlotNodes(5, .55f);
        PlotNodes(4, .35f);
        PlotNodes(3, .15f);

        //reverse is a test for creating links from the inner circle to the outer ones
        parentCircleList.Reverse();

        //create links between nearest nodes from adjacent circles
        PlotLinks();

        //
        StartCoroutine(RemoveCollidingLinks());
    }

    //make sure that nodecount is always even
    void PlotNodes(int nodeCount, float diameterPercent)
    {
        //float centerX = nodeCircleManagerRect.anchoredPosition.x;
        //float centerY = nodeCircleManagerRect.anchoredPosition.y;
        float nodeX;
        float nodeY;
        float radius = (nodeCircleManagerRect.rect.height * diameterPercent) / 2;
        float diameter = nodeCircleManagerRect.rect.height * diameterPercent;
        //extra nodes to be added to nodecount for increment and max nodes
        int nodeMargin = 2;

        // the circle will be divided by the nodeCount
        float degreeIncrement = 360 / (nodeCount + nodeMargin);
        float radianIncrement = degreeIncrement*Mathf.Deg2Rad;

        //the i will be multiplied to the increment angle from 0 to 360
        List<Vector2> noRepeatVector = new List<Vector2>();
        for (int i = 0; nodeCount + nodeMargin > i; i++)
        {
            //unity is defaulted to radians, so we convert to degrees
            //formula for getting the X input
            nodeX = radius * Mathf.Cos(radianIncrement * i);
            //abs is needed because sometimes when Y is supposed to be zero, it becomes a small negative decimal
            nodeY = Mathf.Sqrt(Mathf.Abs(Mathf.Pow(radius, 2) - (Mathf.Pow(nodeX, 2))));

            //once the degree increment goes past 180, the Y should be at the negative quadrant
            if (degreeIncrement*i <= 180)
            {
                noRepeatVector.Add(new Vector2(nodeX, nodeY));
            }
            else
            {
                noRepeatVector.Add(new Vector2(nodeX, -nodeY));
            }

        }
        //create the node holders and assign them in the list
        GameObject circle = Instantiate(parentCircle, nodeCircleManager.transform);
        parentCircleList.Add(circle);

        //for this loop, the actual nodeCount is needed because we'll be instantiating the actual number of nodes based on this int
        for (int i = 1; nodeCount /*+ nodeMargin*/ >= i; i++)
        //for (int i = 0; 23 >= i; i++)
        {
            int randomIndex = Random.Range(0, noRepeatVector.Count - 1);
            GameObject instantiatedNode = Instantiate(nodePrefab, circle.transform);
            RectTransform nodeRect = instantiatedNode.GetComponent<RectTransform>();

            nodeRect.anchoredPosition = noRepeatVector[randomIndex];
            //nodeRect.anchoredPosition = noRepeatVector[0];
            noRepeatVector.RemoveAt(randomIndex);
            //noRepeatVector.RemoveAt(0);
        }
    }

    //we access the actor circle here
    void PlotLinks()
    {
        //dictionary of nodes that didnt have inner links and their parent circles
        Dictionary<GameObject, GameObject> missedNodeLinks = new Dictionary<GameObject, GameObject>();
        foreach (GameObject circle in parentCircleList)
        {
            Transform circleTrans = circle.transform;

            foreach (Transform nodeTrans in circleTrans)
            {
                GameObject node = nodeTrans.gameObject;
                //RectTransform nodeRect = node.GetComponent<RectTransform>();
                NodeLinkIdentifier nodeLinks = node.GetComponent<NodeLinkIdentifier>();

                //will only try to deploy to outer if not the outermost circle
                if (parentCircleList.IndexOf(circle) < parentCircleList.Count - 1)
                {
                    //thetargetCircle variable will indicate the circle that will link the nodes to
                    GameObject targetCircle = parentCircleList[parentCircleList.IndexOf(circle) + 1];
                    //counter for how many outside nodes an inner node can link to
                    //randomly determines how many links an inner node will link to
                    int linkCounter = Random.Range(1, 3);
                    //int linkCounter = 1;

                    //StartCoroutine(DeployLinks(targetCircle, node, linkCounter));
                    DeployLinks(targetCircle, node, linkCounter);
                }


                //if the circle scanned is not the innermost circle and the node did not receive a link from an inner circle
                if (parentCircleList.IndexOf(circle) != 0 && nodeLinks.linksToInner.Count == 0)
                {
                    //adds the node and parent circle for processing later
                    missedNodeLinks.Add(node, circle);
                    Debug.Log($"{nodeTrans.GetSiblingIndex()} in {parentCircleList.IndexOf(circle)}");
                }
            }
                  
        }

        //this segment sifts through the missed links and attempts to connect back to the nearesr inner circle node, always connect with 1 node only
        foreach (KeyValuePair<GameObject, GameObject> missedLinks in missedNodeLinks)
        {
            //StartCoroutine(DeployLinks(parentCircleList[parentCircleList.IndexOf(missedLinks.Value) - 1], missedLinks.Key, 1));
            DeployLinks(parentCircleList[parentCircleList.IndexOf(missedLinks.Value) - 1], missedLinks.Key, 1);
        }

       
    }


    //we access the target circle here
    void DeployLinks(GameObject targetCircle, GameObject startingNode, int linkCounter)
    {
        RectTransform startingNodeRect = startingNode.GetComponent<RectTransform>();
        NodeLinkIdentifier startNodeIdentifier = startingNode.GetComponent<NodeLinkIdentifier>();
        Vector2 startingPoint = startingNodeRect.anchoredPosition;
        Vector2 endingPoint;
        //will contain nodes that are within reach
        //List<GameObject> nodeLinks = new List<GameObject>();
        Dictionary<GameObject, float> nodeLinks = new Dictionary<GameObject, float>();
        //this node is for checking which node is the nearest for the starting node, will be added to list of endings if none is found in the for loop
        GameObject nearestEndingNode = null;
        GameObject nearestEndingNode2 = null;
        //this list will contain all Links that needs to be destroyed due to overlapping links
        List<GameObject> destroyListLinks = new List<GameObject>();

        //distance holder for the nearest, will start as positive infinity
        float nearestDist = Mathf.Infinity;
        float nearestDist2 = Mathf.Infinity;

        ////counter for how many outside nodes an inner node can link to
        ////randomly determines how many links an inner node will link to
        //int linkCounter = Random.Range(1, 3);

        foreach (Transform endNodeTrans in targetCircle.transform)
        {
            GameObject endingNode = endNodeTrans.gameObject;

            RectTransform endingNodeRect = endNodeTrans.GetComponent<RectTransform>();
            endingPoint = endingNodeRect.anchoredPosition;
            //calculates the distance between starting and end points
            //will also serve as the height of the linkprefab that will be instantiated later
            float nodeDistance = Vector2.Distance(startingPoint, endingPoint);

            //if link counter is 1, just assign the ending node to nearestEndingNode
            if (linkCounter == 1)
            {
                if (nearestDist > nodeDistance)
                {
                    nearestDist = nodeDistance;
                    nearestEndingNode = endingNode;
                }

            }
            else if (linkCounter == 2)
            {

                //if a new nearest node is calculated, pass nearest to 2nd nearest, and the calculated node to nearestnode variable
                if (nearestDist > nodeDistance)
                {
                    nearestDist2 = nearestDist;
                    nearestDist = nodeDistance;
                    nearestEndingNode2 = nearestEndingNode;
                    nearestEndingNode = endingNode;

                }
                //if the nodeDistance is larger than nearest1 but smaller than nearest2
                else if (nearestDist < nodeDistance && nodeDistance < nearestDist2)
                {
                    nearestDist2 = nodeDistance;
                    nearestEndingNode2 = endingNode;

                }

            }

        }
        //actual establishment of links

        //if there's 1 link only
        if(linkCounter == 1)
        {
            //nodeLinks.Add(nearestEndingNode);
            Vector2 nearestEndingPoint = nearestEndingNode.GetComponent<RectTransform>().anchoredPosition;
            //create the links and set it as child of the linkholderparent
            GameObject linkObject = Instantiate(linkPrefab, linkHolder.transform);
            RectTransform linkRect = linkObject.GetComponent<RectTransform>();
            BoxCollider2D linkCollider = linkObject.GetComponent<BoxCollider2D>();

            //for assigning the height of the link image
            linkRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nearestDist);
            linkCollider.size = new Vector2(nearestDist * .95f, 1);
            //set the midpoint of start and end node as the anchoredposition of the link
            Vector2 midpoint = new Vector2((startingPoint.x + nearestEndingPoint.x) / 2, (startingPoint.y + nearestEndingPoint.y) / 2);
            linkRect.anchoredPosition = midpoint;
            //set the orientation based on the x axis of the starting node

            float valueRotate = ((nearestEndingPoint.y - startingPoint.y) / (nearestEndingPoint.x - startingPoint.x));
            float angleRotate = Mathf.Atan(valueRotate) * Mathf.Rad2Deg;

            linkObject.transform.rotation = Quaternion.Euler(0, 0, angleRotate);

            //assigns the linked inner and outer nodes
            NodeLinkIdentifier endingNodeIdentifier = nearestEndingNode.GetComponent<NodeLinkIdentifier>();
            startNodeIdentifier.linksToOuter.Add(nearestEndingNode);
            endingNodeIdentifier.linksToInner.Add(startingNode);

         

        }
        //if there are 2 links determined
        else if(linkCounter == 2)
        {           
            //nodeLinks.Add(nearestEndingNode);
            //nodeLinks.Add(nearestEndingNode2);
            nodeLinks.Add(nearestEndingNode, nearestDist);
            nodeLinks.Add(nearestEndingNode2, nearestDist2);

            //foreach (GameObject link in nodeLinks)
            foreach(KeyValuePair<GameObject, float> nodeLink in nodeLinks)
            {
                Vector2 nearestEndingPoint = nodeLink.Key.GetComponent<RectTransform>().anchoredPosition;
                //Vector2 nearestEndingPoint2 = link.GetComponent<RectTransform>().anchoredPosition;
                //center of the link, this is where the instantiated link object will be placed
                Vector2 midpoint;

                //create the links and set it as child of the linkholderparent
                GameObject linkObject = Instantiate(linkPrefab, linkHolder.transform);
                //GameObject linkObject2 = Instantiate(linkprefab, linkholder.transform);
                RectTransform linkRect = linkObject.GetComponent<RectTransform>();
                BoxCollider2D linkCollider = linkObject.GetComponent<BoxCollider2D>();

                linkRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nodeLink.Value);
                linkCollider.size = new Vector2(nodeLink.Value*.95f, 1);
                midpoint = new Vector2((startingPoint.x + nearestEndingPoint.x) / 2, (startingPoint.y + nearestEndingPoint.y) / 2);
                linkRect.anchoredPosition = midpoint;

                //set the midpoint of start and end node as the anchoredposition of the link
                //set the orientation based on the x axis of the starting node

                float valueRotate = ((nearestEndingPoint.y - startingPoint.y) / (nearestEndingPoint.x - startingPoint.x));
                float angleRotate = Mathf.Atan(valueRotate) * Mathf.Rad2Deg;

                linkObject.transform.rotation = Quaternion.Euler(0, 0, angleRotate);

                //assigns the linked inner and outer nodes
                NodeLinkIdentifier endingNodeIdentifier = nearestEndingNode.GetComponent<NodeLinkIdentifier>();
                NodeLinkIdentifier endingNodeIdentifier2 = nearestEndingNode2.GetComponent<NodeLinkIdentifier>();

                startNodeIdentifier.linksToOuter.Add(nearestEndingNode);
                startNodeIdentifier.linksToOuter.Add(nearestEndingNode2);
                endingNodeIdentifier.linksToInner.Add(startingNode);
                endingNodeIdentifier2.linksToInner.Add(startingNode);
               
            }            
        }
        nodeLinks.Clear();

    }

    IEnumerator RemoveCollidingLinks()
    {
        yield return null;
        Debug.Log("link destroy in circleGenerator");

        //will contain links to be destroyed
        List<GameObject> destroyList = new List<GameObject>();
        //will contain links to be not destroyed
        List<GameObject> protectionList = new List<GameObject>();

        foreach (Transform linkTrans in linkHolderTrans)
        {
            GameObject link = linkTrans.gameObject;
            LinkCollisionIdentifier linkIdent = link.GetComponent<LinkCollisionIdentifier>();

            if (linkIdent.actualLink != null && linkIdent.collidingLink != null)
            {
                if (!destroyList.Contains(linkIdent.actualLink) && !protectionList.Contains(linkIdent.actualLink))
                {
                    protectionList.Add(linkIdent.actualLink);
                    destroyList.Add(linkIdent.collidingLink);
                }
            }

        }

        if (destroyList.Count > 0)
        {
            foreach (GameObject destroyLink in destroyList)
            {
                LinkCollisionIdentifier destroyIdent = destroyLink.GetComponent<LinkCollisionIdentifier>();
                destroyIdent.isToBeDestroyed = true;
            }
        }


        //event to destroy all links that have the bool identifier as true
        if (d_DestroyLinks != null)
        {
            d_DestroyLinks();
        }        
    }

}
