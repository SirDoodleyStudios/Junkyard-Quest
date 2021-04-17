using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGenerator : MonoBehaviour
{
    public OverworldManager overworldManager;

    //event attached to LinkCollisionIdentifier script of links
    public delegate void D_DestroyLinks();
    public event D_DestroyLinks d_DestroyLinks;
    //event attached to NodeLinkIdentifier script for nodes
    public delegate void D_DestroyNodes();
    public event D_DestroyNodes d_DestroyNodes;
    //event attaached to LinkCollisionIdentifier that removes a linkfrom another link's collidingLinks list before being checked if there is collision
    public delegate void D_RemoveLinkAsCollider(GameObject i);
    public event D_RemoveLinkAsCollider d_RemoveLinkAsCollider;

    //delegate for event that calls all nodes and links and takes their states to be saved like transforms, indexes, parent index etc
    public delegate void D_StoreObjectState();
    public event D_StoreObjectState d_StoreObjectState;


    GameObject nodeCircleManager;
    Transform nodeCircleTransform;
    RectTransform nodeCircleManagerRect;
    //Holder for the nodes
    public GameObject parentCircle;
    public List<GameObject> parentCircleList = new List<GameObject>();
    //the node actual, uses a prefab from inspector
    public GameObject nodePrefab;

    //the link prefab, will have a logic for a randomizer image for different path shapes
    public GameObject linkPrefab;
    public GameObject linkHolderPrefab;
    //Link hlder will be instantiated from script
    GameObject linkHolder;
    Transform linkHolderTrans;

    //controls the setting of link collider widths
    float colliderWidth = 5f;

    //WILL CHANGE DEPENDING ON SAVE STATE LOGIC
    public void GenerateMap()
    {
        //if static bool parameter sent from UniversalSaveState is not true, generate the circles
        if (!UniversalSaveState.isMapInitialized)
        {
            InitialMapGenerate();
            //static bool in UniversakSaveState
            UniversalSaveState.isMapInitialized = true;
        }
        //if not, it means that the circles were already generated
        else
        {
            //load circles. nodes and links of map
            LoadOverWorldState();
            //load important data
            overworldManager.LoadOverWorldData();
        }
    }

    void InitialMapGenerate()
    {
        nodeCircleManager = gameObject;
        nodeCircleManagerRect = nodeCircleManager.GetComponent<RectTransform>();
        nodeCircleTransform = nodeCircleManager.transform;
        //instantiate the linkHolder first
        linkHolder = Instantiate(linkHolderPrefab, gameObject.transform);
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
        //final meeting node
        PlotNodes(1, .01f);

        //reverse is a test for creating links from the inner circle to the outer ones
        parentCircleList.Reverse();

        //create links between nearest nodes from adjacent circles
        PlotLinks();

        //the other coroutines are triggered after each other so that there are no mismatch in frame counts
        StartCoroutine(RemoveCollidingLinks());
        //StartCoroutine(RemoveStragglerNodes());
        //StartCoroutine(OverWorldCaller());

    }

    IEnumerator OverWorldCaller()
    {
        yield return null;

        //call the overworldmanager to relay info
        //all overworldcalls are called in a coRoutine for alignemnt with the predecessor coroutines, not sure if it actually works
        overworldManager.AssignStartingPositions(parentCircleList[parentCircleList.Count - 1]);


        //save the generated map once everything is initially loaded
        SaveOverworldState();
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
        //special scenario for the final node

        //nu,ber of node spaces no be created
        int nodeSpace;
        if (nodeCount == 1)
        {
            nodeSpace = 1;
        }
        else
        {
            nodeSpace = nodeCount + nodeMargin;
        }

        for (int i = 0; nodeSpace > i; i++)
        {
            //unity is defaulted to radians, so we convert to degrees
            //formula for getting the X input
            nodeX = radius * Mathf.Cos(radianIncrement * i);
            //abs is needed because sometimes when Y is supposed to be zero, it becomes a small negative decimal
            nodeY = Mathf.Sqrt(Mathf.Abs(Mathf.Pow(radius, 2) - (Mathf.Pow(nodeX, 2))));

            //once the degree increment goes past 180, the Y should be at the negative quadrant
            if (degreeIncrement * i <= 180)
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

                if (parentCircleList.IndexOf(circle) == 0)
                {
                    //thetargetCircle variable will indicate the circle that will link the nodes to
                    GameObject targetCircle = parentCircleList[parentCircleList.IndexOf(circle) + 1];
                    DeploySimpleLinks(targetCircle, node);


                }
                //will only try to deploy to outer if not the outermost circle
                else if (parentCircleList.IndexOf(circle) < parentCircleList.Count - 1)
                {
                    //thetargetCircle variable will indicate the circle that will link the nodes to
                    GameObject targetCircle = parentCircleList[parentCircleList.IndexOf(circle) + 1];
                    //counter for how many outside nodes an inner node can link to
                    //randomly determines how many links an inner node will link to
                    int linkCounter = Random.Range(1, 3);
                    //int linkCounter = 1;

                    //StartCoroutine(DeployLinks(targetCircle, node, linkCounter));
                    DeployCalculatedLinks(targetCircle, node, linkCounter, true);
                }



                //if the circle scanned is not the innermost circle and the node did not receive a link from an inner circle
                if (parentCircleList.IndexOf(circle) != 0 && nodeLinks.linkedInnerNodes.Count == 0)
                {
                    //adds the node and parent circle for processing later
                    missedNodeLinks.Add(node, circle);
                    //Debug.Log($"{nodeTrans.GetSiblingIndex()} in {parentCircleList.IndexOf(circle)}");
                }
            }
                  
        }

        //this segment sifts through the missed links and attempts to connect back to the nearesr inner circle node, always connect with 1 node only
        foreach (KeyValuePair<GameObject, GameObject> missedLinks in missedNodeLinks)
        {
            //StartCoroutine(DeployLinks(parentCircleList[parentCircleList.IndexOf(missedLinks.Value) - 1], missedLinks.Key, 1));
            DeployCalculatedLinks(parentCircleList[parentCircleList.IndexOf(missedLinks.Value) - 1], missedLinks.Key, 1, false);
        }

       
    }


    //we access the target circle here
    //bool defines where the target node should be, the linktoOuter and linktoOuter are reverersed when false, for the relinking process to assign correctly
    void DeployCalculatedLinks(GameObject targetCircle, GameObject startingNode, int linkCounter, bool toOuter)
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
            GameObject linkObject = Instantiate(linkPrefab, linkHolderTrans);
            RectTransform linkRect = linkObject.GetComponent<RectTransform>();
            BoxCollider2D linkCollider = linkObject.GetComponent<BoxCollider2D>();
            LinkCollisionIdentifier linkIdentifier = linkObject.GetComponent<LinkCollisionIdentifier>();

            //for assigning the height of the link image
            linkRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nearestDist);

            //set the midpoint of start and end node as the anchoredposition of the link
            Vector2 midpoint = new Vector2((startingPoint.x + nearestEndingPoint.x) / 2, (startingPoint.y + nearestEndingPoint.y) / 2);
            linkRect.anchoredPosition = midpoint;

            //set the orientation based on the x axis of the starting node
            float valueRotate = ((nearestEndingPoint.y - startingPoint.y) / (nearestEndingPoint.x - startingPoint.x));
            float actualAngleRad = Mathf.Atan(valueRotate);
            float angleRotate = Mathf.Atan(valueRotate) * Mathf.Rad2Deg;
            //This will be used for calculations that will detemine the perfect size of the link collider that barely touches the node collider
            float nodeSizeMargin;
            if (angleRotate == 0)
            {
                nodeSizeMargin = nearestDist - nodePrefab.GetComponent<RectTransform>().rect.height;
                linkCollider.size = new Vector2(nodeSizeMargin, colliderWidth);
            }
            else if (Mathf.Abs(angleRotate) >= 45)
            {
                nodeSizeMargin = nearestDist - Mathf.Abs(nodePrefab.GetComponent<RectTransform>().rect.height / Mathf.Sin(actualAngleRad));
                linkCollider.size = new Vector2(nodeSizeMargin, colliderWidth);
            }
            else if (Mathf.Abs(angleRotate) < 45)
            {
                nodeSizeMargin = nearestDist - Mathf.Abs(nodePrefab.GetComponent<RectTransform>().rect.height / Mathf.Cos(actualAngleRad));
                linkCollider.size = new Vector2(nodeSizeMargin, colliderWidth);
            }
            //nodeSizeMargin = nearestDist * .95f;
            //linkCollider.size = new Vector2(nodeSizeMargin, colliderWidth);


            linkObject.transform.rotation = Quaternion.Euler(0, 0, angleRotate);

            //normal logic for linking from inner circle to outer
            if (toOuter == true)
            {
                //assigns the linked inner and outer nodes
                NodeLinkIdentifier endingNodeIdentifier = nearestEndingNode.GetComponent<NodeLinkIdentifier>();
                startNodeIdentifier.linkedOuterNodes.Add(nearestEndingNode);
                endingNodeIdentifier.linkedInnerNodes.Add(startingNode);
                //assigns dictionaries in node for nodetolink
                startNodeIdentifier.pairOuterNodeLink.Add(nearestEndingNode, linkObject);
                endingNodeIdentifier.pairInnerNodeLink.Add(startingNode, linkObject);
                //assigns the nodes in node list in link script
                linkIdentifier.outerNode = nearestEndingNode;
                linkIdentifier.innerNode = startingNode;

            }
            //to be used for the relinking function that targets inner circles
            //assignment of gameObjects to the lists are reversed
            else
            {
                //assigns the linked inner and outer nodes
                NodeLinkIdentifier endingNodeIdentifier = nearestEndingNode.GetComponent<NodeLinkIdentifier>();
                startNodeIdentifier.linkedInnerNodes.Add(nearestEndingNode);
                endingNodeIdentifier.linkedOuterNodes.Add(startingNode);
                //assigns dictionaries in node for nodetolink
                startNodeIdentifier.pairInnerNodeLink.Add(nearestEndingNode, linkObject);
                endingNodeIdentifier.pairOuterNodeLink.Add(startingNode, linkObject);
                //assigns the nodes in node list in link script
                linkIdentifier.innerNode = nearestEndingNode;
                linkIdentifier.outerNode = startingNode;
            }        
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
                GameObject linkObject = Instantiate(linkPrefab, linkHolderTrans);
                //GameObject linkObject2 = Instantiate(linkprefab, linkholder.transform);
                RectTransform linkRect = linkObject.GetComponent<RectTransform>();
                BoxCollider2D linkCollider = linkObject.GetComponent<BoxCollider2D>();
                LinkCollisionIdentifier linkIdentifier = linkObject.GetComponent<LinkCollisionIdentifier>();

                linkRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nodeLink.Value);
                midpoint = new Vector2((startingPoint.x + nearestEndingPoint.x) / 2, (startingPoint.y + nearestEndingPoint.y) / 2);
                linkRect.anchoredPosition = midpoint;

                //set the midpoint of start and end node as the anchoredposition of the link
                //set the orientation based on the x axis of the starting node

                float valueRotate = ((nearestEndingPoint.y - startingPoint.y) / (nearestEndingPoint.x - startingPoint.x));
                float actualAngleRad = Mathf.Atan(valueRotate);
                float angleRotate = Mathf.Atan(valueRotate) * Mathf.Rad2Deg;
                //This will be used for calculations that will detemine the perfect size of the link collider that barely touches the node collider
                float nodeSizeMargin;
                if (angleRotate == 0)
                {
                    nodeSizeMargin = nodeLink.Value - nodePrefab.GetComponent<RectTransform>().rect.height;
                    linkCollider.size = new Vector2(nodeSizeMargin, colliderWidth);
                }
                else if (Mathf.Abs(angleRotate) >= 45)
                {
                    nodeSizeMargin = nodeLink.Value - Mathf.Abs(nodePrefab.GetComponent<RectTransform>().rect.height / Mathf.Sin(actualAngleRad));
                    linkCollider.size = new Vector2(nodeSizeMargin, colliderWidth);
                }
                else if (Mathf.Abs(angleRotate) < 45)
                {
                    nodeSizeMargin = nodeLink.Value - Mathf.Abs(nodePrefab.GetComponent<RectTransform>().rect.height / Mathf.Cos(actualAngleRad));
                    linkCollider.size = new Vector2(nodeSizeMargin, colliderWidth);
                }
                //nodeSizeMargin = nodeLink.Value * .95f;
                //linkCollider.size = new Vector2(nodeSizeMargin, colliderWidth);

                linkObject.transform.rotation = Quaternion.Euler(0, 0, angleRotate);

                //assigns the linked inner and outer nodes
                //NodeLinkIdentifier endingNodeIdentifier = nearestEndingNode.GetComponent<NodeLinkIdentifier>();
                //NodeLinkIdentifier endingNodeIdentifier2 = nearestEndingNode2.GetComponent<NodeLinkIdentifier>(); ///
                NodeLinkIdentifier endingNodeIdentifier = nodeLink.Key.GetComponent<NodeLinkIdentifier>();

                //normal logic for linking from inner circle to outer
                if (toOuter == true)
                {
                    //startNodeIdentifier.linkedOuterNodes.Add(nearestEndingNode);
                    //startNodeIdentifier.linkedOuterNodes.Add(nearestEndingNode2);
                    //endingNodeIdentifier.linkedInnerNodes.Add(startingNode);
                    //endingNodeIdentifier2.linkedInnerNodes.Add(startingNode);

                    startNodeIdentifier.linkedOuterNodes.Add(nodeLink.Key);
                    endingNodeIdentifier.linkedInnerNodes.Add(startingNode);

                    startNodeIdentifier.pairOuterNodeLink.Add(nodeLink.Key, linkObject);
                    endingNodeIdentifier.pairInnerNodeLink.Add(startingNode, linkObject);

                    linkIdentifier.outerNode = nodeLink.Key;
                    linkIdentifier.innerNode = startingNode;
                }
                //to be used for the relinking function that targets inner circles
                //assignment of gameObjects to the lists are reversed
                else
                {
                    //startNodeIdentifier.linkedInnerNodes.Add(nearestEndingNode);
                    //startNodeIdentifier.linkedInnerNodes.Add(nearestEndingNode2);
                    //endingNodeIdentifier.linkedOuterNodes.Add(startingNode);
                    //endingNodeIdentifier2.linkedOuterNodes.Add(startingNode);

                    startNodeIdentifier.linkedInnerNodes.Add(nodeLink.Key);
                    endingNodeIdentifier.linkedOuterNodes.Add(startingNode);

                    startNodeIdentifier.pairInnerNodeLink.Add(nodeLink.Key, linkObject);
                    endingNodeIdentifier.pairOuterNodeLink.Add(startingNode, linkObject);

                    linkIdentifier.innerNode = nodeLink.Key;
                    linkIdentifier.outerNode = startingNode;
                }               
            }            
        }
        nodeLinks.Clear();

    }

    //only used by the central circle with only one node, it will link to ALL outer nodes
    void DeploySimpleLinks(GameObject targetCircle, GameObject startingNode)
    {
        RectTransform startingNodeRect = startingNode.GetComponent<RectTransform>();
        NodeLinkIdentifier startNodeIdentifier = startingNode.GetComponent<NodeLinkIdentifier>();
        Vector2 startingPoint = startingNodeRect.anchoredPosition;
        //will contain nodes that are within reach

        foreach (Transform targetTrans in targetCircle.transform)
        {
            GameObject endingNode = targetTrans.gameObject;
            //nodeLinks.Add(nearestEndingNode);
            Vector2 endingPoint = endingNode.GetComponent<RectTransform>().anchoredPosition;
            //create the links and set it as child of the linkholderparent
            GameObject linkObject = Instantiate(linkPrefab, linkHolderTrans);
            RectTransform linkRect = linkObject.GetComponent<RectTransform>();
            BoxCollider2D linkCollider = linkObject.GetComponent<BoxCollider2D>();

            float nodeDistance = Vector2.Distance(startingPoint, endingPoint);

            //for assigning the height of the link image
            linkRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nodeDistance);

            //set the midpoint of start and end node as the anchoredposition of the link
            Vector2 midpoint = new Vector2((startingPoint.x + endingPoint.x) / 2, (startingPoint.y + endingPoint.y) / 2);
            linkRect.anchoredPosition = midpoint;
            LinkCollisionIdentifier linkIdentifier = linkObject.GetComponent<LinkCollisionIdentifier>();
            //set the orientation based on the x axis of the starting node
            float valueRotate = ((endingPoint.y - startingPoint.y) / (endingPoint.x - startingPoint.x));
            float actualAngleRad = Mathf.Atan(valueRotate);
            float angleRotate = Mathf.Atan(valueRotate) * Mathf.Rad2Deg;

            //This will be used for calculations that will detemine the perfect size of the link collider that barely touches the node collider
            float nodeSizeMargin;
            if (angleRotate == 0)
            {
                nodeSizeMargin = nodeDistance - nodePrefab.GetComponent<RectTransform>().rect.height;
                linkCollider.size = new Vector2(nodeSizeMargin, colliderWidth);
            }
            else if(Mathf.Abs(angleRotate) >= 45)
            {
                nodeSizeMargin = nodeDistance - Mathf.Abs(nodePrefab.GetComponent<RectTransform>().rect.height / Mathf.Sin(actualAngleRad));
                linkCollider.size = new Vector2(nodeSizeMargin, colliderWidth);
            }
            else if(Mathf.Abs(angleRotate) < 45)
            {
                nodeSizeMargin = nodeDistance - Mathf.Abs(nodePrefab.GetComponent<RectTransform>().rect.height / Mathf.Cos(actualAngleRad));
                linkCollider.size = new Vector2(nodeSizeMargin, colliderWidth);
            }
            //nodeSizeMargin = nodeDistance * .95f;
            //linkCollider.size = new Vector2(nodeSizeMargin, colliderWidth);          

            linkObject.transform.rotation = Quaternion.Euler(0, 0, angleRotate);

            //assigns the linked inner and outer nodes
            NodeLinkIdentifier endingNodeIdentifier = endingNode.GetComponent<NodeLinkIdentifier>();
            startNodeIdentifier.linkedOuterNodes.Add(endingNode);
            endingNodeIdentifier.linkedInnerNodes.Add(startingNode);

            startNodeIdentifier.pairOuterNodeLink.Add(endingNode, linkObject);
            endingNodeIdentifier.pairInnerNodeLink.Add(startingNode, linkObject);

            linkIdentifier.outerNode = endingNode;
            linkIdentifier.innerNode = startingNode;
        }
    }

    //Called for destroying links that overlaps with other links
    IEnumerator RemoveCollidingLinks()
    {
        //this frame lag will make sure that the script attached in links will run first
        //this is so that the actual and collider gameObjects are properly assigned from the LinkCollisionIdentifier
        yield return null;

        //will contain links to be destroyed
        List<GameObject> destroyList = new List<GameObject>();
        //will contain links to be not destroyed
        List<GameObject> protectionList = new List<GameObject>();
        //will hold the link list from the linkholder then reverse it so that we can get to the later links first as they should be the ones to be deleted first
        List<GameObject> tempLinkHolder = new List<GameObject>();
        tempLinkHolder.Reverse();


        foreach (Transform linkTrans in linkHolderTrans)
        {
            GameObject link = linkTrans.gameObject;
            tempLinkHolder.Add(link);
        }
        foreach(GameObject link in tempLinkHolder)
        {
            LinkCollisionIdentifier linkIdent = link.GetComponent<LinkCollisionIdentifier>();
            //so that we only look through links that actually have collisions
            if (/*(linkIdent.collidingLinks != null || linkIdent.collidingNodes != null)*/ /*||*/ (linkIdent.collidingLinks.Count > 0 || linkIdent.collidingNodes.Count > 0))
            {
                //the link being inspected here will add its actual gameobject to the protect list and assign the colliderlink to the destroy list
                //it checks first if the inspected object exists in the destroy and protection lists, if it doesnt proceed to adding
                //if it does, it means that the pair link that collided with the inspected link was already inspected therefore the current link is about to be destroyed
                //this will prevent overlaps as well as destroying both overlapping objects

                if (!destroyList.Contains(linkIdent.actualLink) /*&& !protectionList.Contains(linkIdent.actualLink)*/)
                {
                    destroyList.Add(linkIdent.actualLink);
                    //protectionList.Add(linkIdent.collidingLinks); //muted for a test
                    if (d_RemoveLinkAsCollider != null)
                    {
                        d_RemoveLinkAsCollider(link);
                        yield return null;
                    }

                    //new functionality that distributed the removal of references within the nodes and link scripts themselves
                    linkIdent.RemoveNodeReferences();
                }
            }
        }

        yield return null;

        //sets the bool identifier in the link script to true, if it is, it is primed to be destroyed
        if (destroyList.Count > 0)
        {
            foreach (GameObject destroyLink in destroyList)
            {
                LinkCollisionIdentifier destroyIdent = destroyLink.GetComponent<LinkCollisionIdentifier>();
                destroyIdent.isToBeDestroyed = true;
            }
            //event to destroy all links that have the bool identifier as true
            if (d_DestroyLinks != null)
            {
                d_DestroyLinks();
            }
        }

        yield return null;
        //activated as a link so that the yield null timings does not interfere with execution
        StartCoroutine(RemoveStragglerNodes());



    }
    //removes isolated nodes with no links only
    IEnumerator RemoveStragglerNodes()
    {
        yield return null;
        List<GameObject> destroyNodes = new List<GameObject>();
        foreach (GameObject circle in parentCircleList)
        {
            foreach (Transform nodeTrans in circle.transform)
            {
                GameObject node = nodeTrans.gameObject;
                NodeLinkIdentifier nodeIdentifier = node.GetComponent<NodeLinkIdentifier>();

                //if the node does not have any partner nodes, it gets added to the destroy list
                if (nodeIdentifier.linkedInnerNodes.Count <= 0 && nodeIdentifier.linkedOuterNodes.Count <= 0)
                {
                    destroyNodes.Add(node);
                }
            }
        }
        //triggers the nodeLinkIdentifier to have the isToBeDestroyedBool to true
        if (destroyNodes.Count > 0)
        {
            foreach (GameObject destroyNode in destroyNodes)
            {
                NodeLinkIdentifier destroyNodeIden = destroyNode.GetComponent<NodeLinkIdentifier>();
                destroyNodeIden.isToBeDestroyed = true;
            }
        }

        if (d_DestroyNodes != null)
        {
            d_DestroyNodes();
        }

        //calls all nodes and links to record their positions and node and link index references

        //activated as a link so that the yield null timings does not interfere with execution
        StartCoroutine(OverWorldCaller());
    }



    //These three works together, this is for saving the overworld  object state when changing scenes
    public void SaveOverworldState()
    {

        //calls an event attached to all node and links then stores their overworld settings for saving and loading later on
        d_StoreObjectState();

        //after updating each node and link data, save them in these lists to save to json later
        List<LinkStatusSave> linkStatusList = DetermineLinkData();
        List<List<NodeStatusSave>> nodeHolderStatusList = DetermineNodeData();
        //createss the instance of wraoper class for universal parameters
        //Transform currentNode = overworldManager.currentNode.transform;
        //UniversalParameters uniWrapper = new UniversalParameters(overworldManager.worldState, currentNode.GetSiblingIndex(), currentNode.parent.GetSiblingIndex());
        Debug.Log($"link list is {linkStatusList.Count}");
        //function for saving the lists gathered to json
        UniversalSaveState.SaveOverworldMap(linkStatusList, nodeHolderStatusList);
    }

    List<LinkStatusSave> DetermineLinkData()
    {
        List<LinkStatusSave> linkList = new List<LinkStatusSave>();
        //index 0 of circle manager should always be the linkHolder
        Transform linkHolderTrans = gameObject.transform.GetChild(0).transform;
        foreach (Transform link in linkHolderTrans)
        {
            LinkStatusSave linkData = link.gameObject.GetComponent<LinkStatusSave>();
            linkList.Add(linkData);
        }

        return linkList;

    }
    List<List<NodeStatusSave>> DetermineNodeData()
    {
        List<List<NodeStatusSave>> nodeHolderList = new List<List<NodeStatusSave>>();

        Transform holderParentTrans = gameObject.transform;
        foreach (Transform holderTrans in holderParentTrans)
        {
            //will hold the actual nodes
            List<NodeStatusSave> nodeList = new List<NodeStatusSave>();
            //does not check the first child since the first child holds links
            if (holderTrans.GetSiblingIndex() != 0)
            {
                //loops through holder transform to add 
                foreach (Transform nodeTrans in holderTrans)
                {
                    GameObject nodeObject = nodeTrans.gameObject;

                    NodeStatusSave nodeData = nodeObject.GetComponent<NodeStatusSave>();
                    nodeList.Add(nodeData);
                }
                //adds the looped node in the list list
                nodeHolderList.Add(nodeList);
            }
        }

        return nodeHolderList;
    }

    //This is called if we only need to load the overworld from the last save state
    public void LoadOverWorldState()
    {
        SaveMapState loadedOverWorld = UniversalSaveState.LoadOverWorldMap();

        List<LinkData> linkList = loadedOverWorld.linkList;
        List<NodeDataListWrapper> nodeHolderList = loadedOverWorld.nodeList;
        
        //we use the nodeList as baseline for how many holders to create because the needed hodlers are 1 link holder plus how many nodeData is in the list
        for (int holdersIndex = 0; nodeHolderList.Count >= holdersIndex; holdersIndex++)
        {
            //the holder prefab used is link for the first one then nodeCircles for the consecutive holders
            GameObject holderPrefab;
            if (holdersIndex == 0)
            {
                //holders and objects are related to links
                holderPrefab = Instantiate(linkHolderPrefab, gameObject.transform);
                for (int linkCount = 0; linkList.Count-1 >= linkCount; linkCount++)
                {
                    Instantiate(linkPrefab, holderPrefab.transform);
                }
            }
            else
            {
                //holder and object prefavs related to nodes
                holderPrefab = Instantiate(parentCircle, gameObject.transform);
                //this in represents the count of NodeData in the nodeWrapper
                //the -1 in the index search of nodeDataCount is needed so that we can start looping at nodeList[0] since the first instance of a nodeList is at index 1 of holdersIndex
                int nodeDataCount = nodeHolderList[holdersIndex - 1].nodeDataList.Count;
                for (int nodeCount = 0; nodeDataCount-1 >= nodeCount; nodeCount++ )
                {
                    Instantiate(nodePrefab, holderPrefab.transform);
                }
            }
        }

        //iterates through the nodeHolderList and extracts the lists of nodeData inside it
        foreach (NodeDataListWrapper nodeList in nodeHolderList)
        {
            //for each nodeWrapper, it has a list of nodeData inside it, and each nodeData is a representation of node's NodeLinkIdentifier
            foreach (NodeData nodeData in nodeList.nodeDataList)
            {
                Transform parentTrans = gameObject.transform.GetChild(nodeData.parentIndex);
                Transform nodeTrans = parentTrans.GetChild(nodeData.nodeIndex);

                GameObject nodeObject = nodeTrans.gameObject;
                RectTransform nodeRect = nodeObject.GetComponent<RectTransform>();
                NodeLinkIdentifier nodeIden = nodeObject.GetComponent<NodeLinkIdentifier>();

                nodeRect.anchoredPosition = new Vector2(nodeData.nodePosition[0], nodeData.nodePosition[1]);

                //local node and link reference holders to be added to target NodeLinkIdentifier
                for (int i = 0; nodeData.linkedInnerNodes.Count-1 >= i; i++)
                {
                    GameObject partnerNode = gameObject.transform.GetChild(nodeData.linkedInnerParents[i]).GetChild(nodeData.linkedInnerNodes[i]).gameObject;
                    nodeIden.linkedInnerNodes.Add(partnerNode);
                    nodeIden.pairInnerNodeLink.Add(partnerNode, gameObject.transform.GetChild(0).GetChild(nodeData.innerLinkValueIndex[i]).gameObject);
                }
                for (int i = 0; nodeData.linkedOuterNodes.Count - 1 >= i; i++)
                {
                    GameObject partnerNode = gameObject.transform.GetChild(nodeData.linkedOuterParents[i]).GetChild(nodeData.linkedOuterNodes[i]).gameObject;
                    nodeIden.linkedOuterNodes.Add(partnerNode);
                    nodeIden.pairOuterNodeLink.Add(partnerNode, gameObject.transform.GetChild(0).GetChild(nodeData.outerLinkValueIndex[i]).gameObject);
                }

                if (nodeData.isClickable == true)
                {
                    nodeIden.MakeNodeClickable();
                }
                if (nodeData.isSelected == true)
                {
                    nodeIden.MakeNodeSelected();
                }
                if(nodeData.isTraversed == true)
                {
                    nodeIden.MakeNodeTraversed();
                }
                nodeIden.nodeActivityEnum = nodeData.nodeActivity;
                nodeIden.AssignNodeIconImage();

                //nodeIden.isClickable = nodeData.isClickable;
                //nodeIden.isSelected = nodeData.isSelected;

            }
        }

        //iterate through all holders then assign the values parsed from the json
        foreach (LinkData linkData in linkList)
        {
            Transform parentTrans = gameObject.transform.GetChild(linkData.parentIndex);
            Transform linkTrans = parentTrans.GetChild(linkData.linkIndex);

            GameObject linkObject = linkTrans.gameObject;
            RectTransform linkRect = linkObject.GetComponent<RectTransform>();
            LinkCollisionIdentifier linkIden = linkObject.GetComponent<LinkCollisionIdentifier>();

            linkRect.anchoredPosition = new Vector2(linkData.linkPosition[0], linkData.linkPosition[1]);
            linkTrans.rotation = Quaternion.Euler(0, 0, linkData.linkRotation[2]);
            linkIden.innerNode = gameObject.transform.GetChild(linkData.linkedInnerParent).GetChild(linkData.linkedInnerNode).gameObject;
            linkIden.outerNode = gameObject.transform.GetChild(linkData.linkedOuterParent).GetChild(linkData.linkedOuterNode).gameObject;
            linkRect.sizeDelta = new Vector2(linkData.linkWidth, linkRect.rect.height);

            if (linkData.isTraversed)
            {
                linkIden.MakeLinkTraversed();
            }
        }

    }

}


