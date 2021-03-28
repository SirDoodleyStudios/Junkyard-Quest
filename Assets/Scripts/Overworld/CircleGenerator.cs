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
        PlotNodes(10, .90f);
        PlotNodes(8, .70f);
        PlotNodes(6, .50f);
        PlotNodes(4, .40f);
        PlotNodes(2, .20f);

        //reverse is a test for creating links from the inner circle to the outer ones
        //parentCircleList.Reverse();
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

        // the circle will be divided by the nodeCount
        float degreeIncrement = 360 / (nodeCount * 2);
        float radianIncrement = degreeIncrement*Mathf.Deg2Rad;

        //the i will be multiplied to the increment angle from 0 to 360
        List<Vector2> noRepeatVector = new List<Vector2>();
        for (int i = 0; nodeCount*2 > i; i++)
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
        List<GameObject> nodeLinks = new List<GameObject>();
        //this node is for checking which node is the nearest for the starting node, will be added to list of endings if none is found in the for loop
        GameObject nearestEndingNode = null;

        //distance holder for the nearest, will start as positive infinity
        float nearestDist = Mathf.Infinity;

        foreach (Transform endNodeTrans in targetCircle.transform)
        {
            GameObject endingNode = endNodeTrans.gameObject;

            RectTransform endingNodeRect = endNodeTrans.GetComponent<RectTransform>();
            endingPoint = endingNodeRect.anchoredPosition;
            //calculates the distance between starting and end points
            //will also serve as the height of the linkprefab that will be instantiated later
            float nodeDistance = Vector2.Distance(startingPoint, endingPoint);

            //if distance between startingto ending is less that 15% of the canvas diameter, it is eligible for link
            if (nodeDistance <= diameter * .15f)
            {
                nodeLinks.Add(endingNode);
                //create the links and set it as child of the linkholderparent
                GameObject linkObject = Instantiate(linkprefab, linkholder.transform);
                RectTransform linkRect = linkObject.GetComponent<RectTransform>();
                //for assigning the height of the link image
                linkRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nodeDistance);

                //set the midpoint of start and end node as the anchoredposition of the link
                Vector2 midpoint = new Vector2((startingPoint.x + endingPoint.x) / 2, (startingPoint.y + endingPoint.y) / 2);
                linkRect.anchoredPosition = midpoint;
                //set the orientation based on the x axis of the starting node

                float valueRotate = ((endingPoint.y - startingPoint.y) / (endingPoint.x - startingPoint.x)) ;
                float angleRotate = Mathf.Atan(valueRotate) * Mathf.Rad2Deg;


                linkObject.transform.rotation = Quaternion.Euler(0, 0, angleRotate);
            }

            //if node is not within reach, it will record it if the distance between nodes are less than the recorded distance
            else
            {
                if (nearestDist > nodeDistance)
                {
                    nearestDist = nodeDistance;
                    nearestEndingNode = endingNode;

                }
            }
        }
        //if there were no elligible endnodes found, link to the nearest possible node
        if(nodeLinks.Count <= 0)
        {
            nodeLinks.Add(nearestEndingNode);
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


    }

}
