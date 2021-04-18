using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class OverworldManager : MonoBehaviour
{


    public ActivitiesManager activitiesManager;

    public OverworldState worldState;
    //references to the main circles
    public GameObject nodeCircleManager;
    public CircleGenerator circleGenerator;
    //public GameObject linkHolder;
    //references for camera movement
    public Camera camera;
    bool isPanning;
    Vector2 mouseOriginPos;
    Vector2 panningMousePos;
    public Canvas mainCanvas;
    //boundary vectors for panning
    float xBoundary;
    float yBoundary;
    //last mouse position to be saved and loaded as the carera's current positon
    Vector2 cameraPos = new Vector2();

    //stores the current selected node
    public GameObject currentNode;
    public GameObject targetNode;

    //list that holds all starting positions
    List<GameObject> startingReachableNodes = new List<GameObject>();
    List<GameObject> adjacentNodes = new List<GameObject>();

    //for object clicking
    Vector2 PointRay;
    Ray ray;
    RaycastHit2D pointedObject;

    void Start()
    {
        //checks first if map is alreadey created
        if (UniversalSaveState.isMapInitialized)
        {
            worldState = OverworldState.MoveNode;
        }

        //calls circleManager
        circleGenerator.GenerateMap();

        //orthographic size is equal to your desired screenheight/2. I want the ortho height to be half of screen size, so i divided the canvas height by 4
        camera.orthographicSize = mainCanvas.GetComponent<RectTransform>().rect.height / 4f;
        //boundaries for he map
        //canvas position is at the 0,0 and camera pivot is at center, ortho camera is effectively half of screen size
        xBoundary = camera.aspect * camera.orthographicSize;
        yBoundary = camera.orthographicSize;


        Debug.Log(worldState);

    }
    //called by CircleGenerator once the parent circles are generated
    //only gets called during the first creation of the overworld map
    public void AssignStartingPositions(GameObject outerCircle)
    {
        Transform outerCircleTrans = outerCircle.transform;
        foreach (Transform outerCirc in outerCircleTrans)
        {
            GameObject outerNode = outerCirc.gameObject;
            NodeLinkIdentifier nodeIdentifier = outerNode.GetComponent<NodeLinkIdentifier>();
            nodeIdentifier.MakeNodeClickable();
            startingReachableNodes.Add(outerNode);
        }
        //saves the overworld firs before switchingscenes
        //SaveState(); //save must come after the assignStartingPositions
        worldState = OverworldState.StartingNode;

    }
    //parsing and loading function of overWorld data
    public void LoadOverWorldData()
    {
        SaveKeyOverworld keyData = UniversalSaveState.LoadOverWorldData();
        GameObject parentObject = circleGenerator.transform.GetChild(keyData.nodeParent).gameObject;

        worldState = keyData.moveState;
        currentNode = parentObject.transform.GetChild(keyData.nodeIndex).gameObject;
        UniversalSaveState.isMapInitialized = keyData.isMapInitialized;
        //repopulates adjacent nodes
        for (int i = 0; keyData.adjacentNodeCount > i; i++)
        {
            //parent circle of the adjacent node
            Transform adjParent = circleGenerator.transform.GetChild(keyData.adjacentParents[i]);
            //actual node under paent
            GameObject adjNode = adjParent.GetChild(keyData.adjacentNodeIndex[i]).gameObject;
            adjacentNodes.Add(adjNode);
        }

        //reassigns the camera position from last transition
        //z is always -10 for overworld settings
        camera.transform.position = new Vector3(keyData.cameraX, keyData.cameraY, -10);

    }

    void Update()
    {
        //for dragging camera
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            //bool activates the actual dragging logic below
            isPanning = true;
            mouseOriginPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        //letting go of mouseclick deactivates dragging
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            isPanning = false;
        }
        //gets the difference of original mouse position and current mouse position then checks if they are within the canvas boundaries
        if (isPanning)
        {
            panningMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 distance = panningMousePos - mouseOriginPos;

            float xPanning = camera.transform.position.x - distance.x;
            float yPanning = camera.transform.position.y - distance.y;
            //position of camera will not go beyond the set positive and negative boundaries
            camera.transform.position = new Vector3(Mathf.Clamp(xPanning, -xBoundary, xBoundary), Mathf.Clamp(yPanning, -yBoundary, yBoundary), -10f);
        }
        
        //during start of overworld manager, player will get to choose starting position node
        if (worldState == OverworldState.StartingNode)
        {
            PointRay = Input.mousePosition;
            ray = Camera.main.ScreenPointToRay(PointRay);
            pointedObject = Physics2D.GetRayIntersection(ray);
            //makes sure that the click has a collider
            if (Input.GetMouseButtonDown(0) && pointedObject.collider !=null)
            {
                GameObject clickedObject = pointedObject.collider.gameObject;
                //for when a node is clicked
                if (clickedObject.tag == "Node")
                {
                    NodeLinkIdentifier clickedIden = clickedObject.GetComponent<NodeLinkIdentifier>();
                    //checks all nodes with clickable identifier
                    if (clickedIden.isClickable)
                    {
                        //makes all nodes go back to default state
                        foreach (GameObject otherNodes in startingReachableNodes)
                        {
                            NodeLinkIdentifier nodeIden = otherNodes.GetComponent<NodeLinkIdentifier>();
                            nodeIden.MakeNodeUnselected();
                        }
                        startingReachableNodes.Clear();
                        //identifies current selected node
                        currentNode = clickedObject;
                        NodeLinkIdentifier currentNodeIden = currentNode.GetComponent<NodeLinkIdentifier>();
                        currentNodeIden.MakeNodeSelected();

                        //triggers adjacent nodes
                        //List<GameObject> adjacentNodes = new List<GameObject>();
                        adjacentNodes.AddRange(currentNodeIden.linkedOuterNodes);
                        adjacentNodes.AddRange(currentNodeIden.linkedInnerNodes);

                        foreach (GameObject adjacentNode in adjacentNodes)
                        {
                            NodeLinkIdentifier adjacentIden = adjacentNode.GetComponent<NodeLinkIdentifier>();
                            adjacentIden.MakeNodeClickable();
                        }
                        //marks node as traversed
                        currentNodeIden.MakeNodeTraversed();

                        UniversalInformation universal = new UniversalInformation();
                        universal.UniversalOverWorldStart();

                        worldState = OverworldState.MoveNode;
                        //calls circle generator's save function to save current overworld
                        SaveFunction(universal);
                        //scene transition depending on the NodeActivityEnum of the target Node
                        activitiesManager.LoadStartNodeActivity(currentNodeIden.nodeActivityEnum);
                        //activitiesManager.LoadActivities();

                    }
                }
            }

        }

        if (worldState == OverworldState.MoveNode)
        {
            PointRay = Input.mousePosition;
            ray = Camera.main.ScreenPointToRay(PointRay);
            pointedObject = Physics2D.GetRayIntersection(ray);

            //will contain the initial node before selecting the target current node
            //currentnode will be overwritten at the click, we'll use the initial for finding the correct link partner to be made traversed
            GameObject initialNode = currentNode;
            NodeLinkIdentifier initialNodeIden = initialNode.GetComponent<NodeLinkIdentifier>();

            //makes sure that clicked object has a collider
            if (Input.GetMouseButtonDown(0) && pointedObject.collider != null)
            {
                GameObject clickedObject = pointedObject.collider.gameObject;

                //for when a node is clicked
                if (clickedObject.tag == "Node")
                {
                    NodeLinkIdentifier clickedIden = clickedObject.GetComponent<NodeLinkIdentifier>();

                    if (clickedIden.isClickable)
                    {
                        foreach (GameObject adjacentNode in adjacentNodes)
                        {
                            NodeLinkIdentifier adjacentIden = adjacentNode.GetComponent<NodeLinkIdentifier>();
                            adjacentIden.MakeNodeUnselected();
                        }
                        adjacentNodes.Clear();
                        //identifies current selected node
                        currentNode = clickedObject;
                        NodeLinkIdentifier currentNodeIden = currentNode.GetComponent<NodeLinkIdentifier>();
                        currentNodeIden.MakeNodeSelected();
                        //triggers adjacent nodes
                        //adjacentNodes = new List<GameObject>();
                        adjacentNodes.AddRange(currentNodeIden.linkedOuterNodes);
                        adjacentNodes.AddRange(currentNodeIden.linkedInnerNodes);

                        foreach (GameObject adjacentNode in adjacentNodes)
                        {
                            NodeLinkIdentifier adjacentIden = adjacentNode.GetComponent<NodeLinkIdentifier>();
                            adjacentIden.MakeNodeClickable();
                        }

                        //primes the universalinfo to be saved
                        UniversalInformation universal = new UniversalInformation();
                        //checks first where the target node is in the inner and outer node dictionaries
                        //mark the partner link as traversed
                        LinkCollisionIdentifier partnerLink;

                        //The UniversalInformation primed for saving must always come before the makeTraverse Functions
                        //This is because the loaded file for other scenes must contain formation before they are marked as traversed
                        if (initialNodeIden.pairInnerNodeLink.ContainsKey(currentNode))
                        {

                            partnerLink = initialNodeIden.pairInnerNodeLink[currentNode].GetComponent<LinkCollisionIdentifier>();
                            universal.UniversalOverworldMove(currentNodeIden.nodeActivityEnum, currentNodeIden.isTraversed, partnerLink.isTraversed);
                            partnerLink.MakeLinkTraversed();
                            currentNodeIden.MakeNodeTraversed();
                        }
                        // the else if for pairOuterNodeLink, has risks
                        //else if (initialNodeIden.pairOuterNodeLink.ContainsKey(currentNode))
                        else
                        {
                            partnerLink = initialNodeIden.pairOuterNodeLink[currentNode].GetComponent<LinkCollisionIdentifier>();
                            universal.UniversalOverworldMove(currentNodeIden.nodeActivityEnum, currentNodeIden.isTraversed, partnerLink.isTraversed);
                            partnerLink.MakeLinkTraversed();
                            currentNodeIden.MakeNodeTraversed();
                        }
                        //else
                        //{
                        //    Debug.Log("node being accessed has no link pair");
                        //}



                        //calls circle generator's save function to save current overworld
                        //sends the univesal information to be saved as well
                        SaveFunction(universal);
                        //FOR TEST ONLY, ACTIVATES A TEST SCENE WHICH JUST HAPPENS TO BE THE COMBAT SCENE
                        //activitiesManager.LoadStartNodeActivity(currentNodeIden.nodeActivityEnum);
                        activitiesManager.LoadActivities();

                        ////
                    }
                    else
                    {
                        Debug.Log("not clickable");
                    }
                }             

               
            }

        }

    }
    //calls all save actions
    void SaveFunction(UniversalInformation universalInfo)
    {
        //for universal information
        UniversalSaveState.SaveUniversalInformation(universalInfo);
        //for nodes and links architecture
        circleGenerator.SaveOverworldState();
        //for overworld releveant data like current node selected
        UniversalSaveState.SaveOverWorldData(new SaveKeyOverworld(worldState, currentNode.transform, adjacentNodes, camera.transform.position));
    }

    ////These three works together, this is for saving the overworld  object state when changing scenes
    //public void SaveState()
    //{

    //    List<LinkStatusSave> linkStatusList = LoadLinkActivities();
    //    List<List<NodeStatusSave>> nodeHolderStatusList = LoadNodeActivities();

    //    UniversalSaveState.SaveOverworldMap(linkStatusList, nodeHolderStatusList);
    //}

    //List<LinkStatusSave> LoadLinkActivities()
    //{
    //    List<LinkStatusSave> linkList = new List<LinkStatusSave>();
    //    //index 0 of circle manager should always be the linkHolder
    //    Transform linkHolderTrans = nodeCircleManager.transform.GetChild(0).transform;
    //    foreach(Transform link in linkHolderTrans)
    //    {
    //        LinkStatusSave linkData = link.gameObject.GetComponent<LinkStatusSave>();
    //        linkList.Add(linkData);
    //    }

    //    return linkList;

    //}
    //List<List<NodeStatusSave>> LoadNodeActivities()
    //{
    //    List<List<NodeStatusSave>> nodeHolderList = new List<List<NodeStatusSave>>();

    //    Transform holderParentTrans = circleGenerator.transform;
    //    foreach (Transform holderTrans in holderParentTrans)
    //    {
    //        //will hold the actual nodes
    //        List<NodeStatusSave> nodeList = new List<NodeStatusSave>();
    //        //does not check the first child since the first child holds links
    //        if (holderTrans.GetSiblingIndex() != 0)
    //        {
    //            //loops through holder transform to add 
    //            foreach (Transform nodeTrans in holderTrans)
    //            {
    //                GameObject nodeObject = nodeTrans.gameObject;

    //                NodeStatusSave nodeData = nodeObject.GetComponent<NodeStatusSave>();
    //                nodeList.Add(nodeData);
    //            }
    //            //adds the looped node in the list list
    //            nodeHolderList.Add(nodeList);
    //        }
    //    }

    //    return nodeHolderList;
    //}

}
//inserts all other 
[Serializable]
public class SaveKeyOverworld
{
    public OverworldState moveState;
    public int nodeIndex;
    public int nodeParent;
    public bool isMapInitialized;
    public int adjacentNodeCount;
    public List<int> adjacentParents = new List<int>();
    public List<int> adjacentNodeIndex = new List<int>();
    //for the camera position
    public float cameraX;
    public float cameraY;

    //param 1 = overworldManager state
    //param 2 = currentNode
    //param 3 = current node's adjacent nodes
    //param 4 = position of mouse
    public SaveKeyOverworld(OverworldState worldState, Transform nodeTrans, List<GameObject> adjacentNodes, Vector2 cameraPos)
    {
        moveState = worldState;
        nodeIndex = nodeTrans.GetSiblingIndex();
        nodeParent = nodeTrans.parent.GetSiblingIndex();
        isMapInitialized = UniversalSaveState.isMapInitialized;
        adjacentNodeCount = adjacentNodes.Count;
        cameraX = cameraPos.x;
        cameraY = cameraPos.y;
        if (adjacentNodes.Count != 0)
        {
            foreach (GameObject node in adjacentNodes)
            {
                adjacentParents.Add(node.transform.parent.GetSiblingIndex());
                adjacentNodeIndex.Add(node.transform.GetSiblingIndex());
            }
        }

    }
}







