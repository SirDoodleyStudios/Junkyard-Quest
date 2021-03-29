using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGenerator : MonoBehaviour
{
    List<Vector2> Circle1 = new List<Vector2>();
    GameObject nodeCircleManager;
    Transform nodeCircleTransform;
    RectTransform nodeCircleManagerRect;
    //Holder for the nodes
    public GameObject parentCircle;
    List<GameObject> parentCircleList = new List<GameObject>();
    //the node actual, uses a prefab from inspector
    public GameObject nodePrefab;
    //the link prefab, will have a logic for a randomizer image
    public GameObject linkprefab;
    public GameObject linkholder;


    void Start()
    {
        nodeCircleManager = gameObject;
        nodeCircleManagerRect = nodeCircleManager.GetComponent<RectTransform>();
        nodeCircleTransform = nodeCircleManager.transform;

        //call generation of circle nodes
        //initial i is the first outside circle with the largest number of nodes
        //for (int i = 10; i >= 8; i--)
        //{
        //    float diameterPercent = 1 - ((11 - i) * .2f);
        //    PlotNodes(i, diameterPercent);
        //}
        PlotNodes(7, .95f);
        PlotNodes(6, .65f);
        PlotNodes(5, .35f);
        PlotNodes(4, .15f);

        //reverse is a test for creating links from the inner circle to the outer ones
        parentCircleList.Reverse();
        PlotLinks();
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
        for (int i = 1; nodeCount >= i; i++)
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


    void PlotLinks()
    {
        foreach (GameObject circle in parentCircleList)
        {
            Transform circleTrans = circle.transform;
            GameObject targetCircle;
            //thetargetCircle variable will indicate the circle that will link the nodes to
            //only works for circles that are not the last

            if (parentCircleList.IndexOf(circle) < parentCircleList.Count - 1)
            {
                targetCircle = parentCircleList[parentCircleList.IndexOf(circle) + 1];
                foreach (Transform nodeTrans in circleTrans)
                {
                    GameObject node = nodeTrans.gameObject;
                    RectTransform nodeRect = node.GetComponent<RectTransform>();

                    DeployLinks(targetCircle, nodeRect, nodeCircleManagerRect.rect.height);
                }
            }


            
        }
    }

    void DeployLinks(GameObject targetCircle, RectTransform startingNodeRect, float diameter)
    {
        Vector2 startingPoint = startingNodeRect.anchoredPosition;
        Vector2 endingPoint;
        //will contain nodes that are within reach
        //List<GameObject> nodeLinks = new List<GameObject>();
        Dictionary<GameObject, float> nodeLinks = new Dictionary<GameObject, float>();
        //this node is for checking which node is the nearest for the starting node, will be added to list of endings if none is found in the for loop
        GameObject nearestEndingNode = null;
        GameObject nearestEndingNode2 = null;
        //percentage of the diameter that determines how far a node can reach with a link
        float linkReach = .30f;

        //distance holder for the nearest, will start as positive infinity
        float nearestDist = Mathf.Infinity;
        float nearestDist2 = Mathf.Infinity;

        //counter for how many outside nodes an inner node can link to
        //randomly determines how many links an inner node will link to
        int linkCounter = Random.Range(1, 3);

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

            //if distance between startingto ending is less that 15% of the canvas diameter, it is eligible for link
            //if (nodeDistance <= diameter * linkReach)
            //{
            //    nodeLinks.Add(endingNode);
            //    //create the links and set it as child of the linkholderparent
            //    GameObject linkObject = Instantiate(linkprefab, linkholder.transform);
            //    RectTransform linkRect = linkObject.GetComponent<RectTransform>();
            //    //for assigning the height of the link image
            //    linkRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nodeDistance);

            //    //set the midpoint of start and end node as the anchoredposition of the link
            //    Vector2 midpoint = new Vector2((startingPoint.x + endingPoint.x) / 2, (startingPoint.y + endingPoint.y) / 2);
            //    linkRect.anchoredPosition = midpoint;
            //    //set the orientation based on the x axis of the starting node

            //    float valueRotate = ((endingPoint.y - startingPoint.y) / (endingPoint.x - startingPoint.x)) ;
            //    float angleRotate = Mathf.Atan(valueRotate) * Mathf.Rad2Deg;


            //    linkObject.transform.rotation = Quaternion.Euler(0, 0, angleRotate);
            //}

        }

        //if there's 1 link only
        if(linkCounter == 1)
        {
            //nodeLinks.Add(nearestEndingNode);
            Vector2 nearestEndingPoint = nearestEndingNode.GetComponent<RectTransform>().anchoredPosition;
            //create the links and set it as child of the linkholderparent
            GameObject linkObject = Instantiate(linkprefab, linkholder.transform);
            RectTransform linkRect = linkObject.GetComponent<RectTransform>();
            //for assigning the height of the link image
            linkRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nearestDist);

            //set the midpoint of start and end node as the anchoredposition of the link
            Vector2 midpoint = new Vector2((startingPoint.x + nearestEndingPoint.x) / 2, (startingPoint.y + nearestEndingPoint.y) / 2);
            linkRect.anchoredPosition = midpoint;
            //set the orientation based on the x axis of the starting node

            float valueRotate = ((nearestEndingPoint.y - startingPoint.y) / (nearestEndingPoint.x - startingPoint.x));
            float angleRotate = Mathf.Atan(valueRotate) * Mathf.Rad2Deg;


            linkObject.transform.rotation = Quaternion.Euler(0, 0, angleRotate);
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
                GameObject linkObject = Instantiate(linkprefab, linkholder.transform);
                //GameObject linkObject2 = Instantiate(linkprefab, linkholder.transform);
                RectTransform linkRect = linkObject.GetComponent<RectTransform>();
                //RectTransform linkRect2 = linkObject2.GetComponent<RectTransform>();

                //for assigning the height of the link image
                //first index points to nearest while second index
                //if (nodeLinks.IndexOf(link) == 0)
                //{
                //    linkRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nodeLink.Value);
                //    midpoint = new Vector2((startingPoint.x + nearestEndingPoint.x) / 2, (startingPoint.y + nearestEndingPoint.y) / 2);
                //}
                //else
                //{
                //    linkRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nearestDist2);
                //    midpoint = new Vector2((startingPoint.x + nearestEndingPoint.x) / 2, (startingPoint.y + nearestEndingPoint.y) / 2);
                //}
                linkRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nodeLink.Value);
                midpoint = new Vector2((startingPoint.x + nearestEndingPoint.x) / 2, (startingPoint.y + nearestEndingPoint.y) / 2);
                linkRect.anchoredPosition = midpoint;


                //set the midpoint of start and end node as the anchoredposition of the link


                //set the orientation based on the x axis of the starting node

                float valueRotate = ((nearestEndingPoint.y - startingPoint.y) / (nearestEndingPoint.x - startingPoint.x));
                float angleRotate = Mathf.Atan(valueRotate) * Mathf.Rad2Deg;


                linkObject.transform.rotation = Quaternion.Euler(0, 0, angleRotate);
            }

            
        }
        nodeLinks.Clear();


    }

}
