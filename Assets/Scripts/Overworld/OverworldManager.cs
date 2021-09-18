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
    public CameraUIScript cameraUI;
    bool isPanning;
    Vector2 mouseOriginPos;
    Vector2 panningMousePos;
    public Canvas mainCanvas;
    //boundary vectors for panning
    float xBoundary;
    float yBoundary;
    //last mouse position to be saved and loaded as the carera's current positon
    //Vector2 cameraPos = new Vector2();

    //stores the current selected node
    public GameObject currentNode;
    public GameObject targetNode;
    //this is used to temporarily hold the activities between the current and target node for saving it in the file
    List<LinkActivityEnum> activityList = new List<LinkActivityEnum>();

    //list that holds all starting positions
    List<GameObject> startingReachableNodes = new List<GameObject>();
    List<GameObject> adjacentNodes = new List<GameObject>();

    //holds the universalInformation Loaded\
    //Universal information is first created in Selection Screen
    UniversalInformation universalInformation;

    //wrapper internal class that holds the ionformation for the next node to be loaded by the activity scenes
    NextNodeWrapper nodeWrapper;

    //for object clicking
    Vector2 PointRay;
    Ray ray;
    RaycastHit2D pointedObject;

    void Start()
    {
        //automatically assign the universalInformation file's scene to overworld
        universalInformation = new UniversalInformation();
        universalInformation = UniversalSaveState.LoadUniversalInformation();
        universalInformation.scene = SceneList.Overworld;

        //calls circleManager
        circleGenerator.GenerateMap();

        //after generating map, call method in CameraUIScript to assign the HP, creativity and other UI Visuals and loads the deck as well
        CardTagManager.InitializeTextDescriptionDictionaries();
        cameraUI.InitiateUniversalUIInfoData(universalInformation);
        cameraUI.AssignUIObjects(universalInformation);

        //once inserted before GenerateMap()
        //checks first if map is alreadey created
        if (UniversalSaveState.isMapInitialized)
        {
            worldState = OverworldState.MoveNode;
        }

        //orthographic size is equal to your desired screenheight/2. I want the ortho height to be half of screen size, so i divided the canvas height by 4
        camera.orthographicSize = mainCanvas.GetComponent<RectTransform>().rect.height / 4f;
        //calls the UI panel attached in the camera to reset the size based on the calculated orthographic size
        cameraUI.SetUISize(camera.orthographicSize);

        //boundaries for he map
        //canvas position is at the 0,0 and camera pivot is at center, ortho camera is effectively half of screen size
        xBoundary = camera.aspect * camera.orthographicSize;
        yBoundary = camera.orthographicSize;

        //calls the CameraUIScript to call the EquipmentViewer
        //makes all gears in euipmentViewer draggable
        cameraUI.EnableGearManagement();


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

    //this function is called by the equipment viewer
    //saves the gear lists since you can only manage equipment when in overWorld
    public void SaveEquipmentInOverWorld(GearWrapper[] equippedGear, List<GearWrapper> gearInventory)
    {
        universalInformation.equippedGears = equippedGear;
        universalInformation.gearWrapperList = gearInventory;
        //the save of equipment is separated becasue it can be changed multiple times without any change in the overworld
        //the universalInformation in this state will only update the gear lists
        UniversalSaveState.SaveUniversalInformation(universalInformation);
        //updates the universalInfo in the universalUI
        cameraUI.UpdateUniversalInfo();
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
            //the 15% multiplier in yBoundary lets it leave the boundary above because we'll be putting shit there like map, deck, treasures and more
            //the 40% multiplier in xboundary will cut the excess map area that won't be filled up by the map generator
            //the panel above must be consistent across all scenes
            //THE -10f parameter in the end is important, clicking in overworld messes it up if changed
            camera.transform.position = new Vector3(Mathf.Clamp(xPanning, -xBoundary*.40f, xBoundary*.40f), Mathf.Clamp(yPanning, -yBoundary, yBoundary*1.15f), -10f);
        }

        //Reset Function that immediately resets the map
        //UPDATE THIS TO DELETE ALL SAVE FILES AND GO BACK TO SELECTION SCREEN
        if (Input.GetKey(KeyCode.R))
        {
            activitiesManager.ResetMap();
        }

        //during start of overworld manager, player will get to choose starting position node
        if (worldState == OverworldState.StartingNode)
        {


            PointRay = Input.mousePosition;
            ray = Camera.main.ScreenPointToRay(PointRay);
            pointedObject = Physics2D.GetRayIntersection(ray);

            //test
            if (pointedObject.collider != null)
            {

            }

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

                        //UniversalInformation universal = new UniversalInformation();
                        //universal.UniversalOverWorldStart();

                        worldState = OverworldState.MoveNode;
                        //calls circle generator's save function to save current overworld
                        SaveFunction();
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


            //makes sure that clicked object has a collider
            if (Input.GetMouseButtonDown(0) && pointedObject.collider != null)
            {
                //will contain the initial node before selecting the target current node
                //currentnode will be overwritten at the click, we'll use the initial for finding the correct link partner to be made traversed
                GameObject initialNode = currentNode;
                NodeLinkIdentifier initialNodeIden = initialNode.GetComponent<NodeLinkIdentifier>();
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
                            //universal.UniversalOverworldMove(currentNodeIden.nodeActivityEnum, currentNodeIden.isTraversed, partnerLink.isTraversed);
                            nodeWrapper = new  NextNodeWrapper(currentNodeIden.nodeActivityEnum, currentNodeIden.isTraversed, partnerLink.isTraversed);
                            partnerLink.MakeLinkTraversed();
                            currentNodeIden.MakeNodeTraversed();
                            //store the link activities of link sandwiched between current and target node
                            activityList.AddRange(partnerLink.GetComponent<LinkActivities>().linkActivities);
                        }
                        // the else if for pairOuterNodeLink, has risks
                        //else if (initialNodeIden.pairOuterNodeLink.ContainsKey(currentNode))
                        else
                        {
                            partnerLink = initialNodeIden.pairOuterNodeLink[currentNode].GetComponent<LinkCollisionIdentifier>();
                            //universal.UniversalOverworldMove(currentNodeIden.nodeActivityEnum, currentNodeIden.isTraversed, partnerLink.isTraversed);
                            nodeWrapper = new NextNodeWrapper(currentNodeIden.nodeActivityEnum, currentNodeIden.isTraversed, partnerLink.isTraversed);
                            partnerLink.MakeLinkTraversed();
                            currentNodeIden.MakeNodeTraversed();
                            //store the link activities of link sandwiched between current and target node
                            activityList.AddRange(partnerLink.GetComponent<LinkActivities>().linkActivities);
                        }
                        //else
                        //{
                        //    Debug.Log("node being accessed has no link pair");
                        //}

                        //separates loading 
                        //UniversalSaveState.isNotAtInitialPhase = true;

                        //calls circle generator's save function to save current overworld
                        //sends the univesal information to be saved as well
                        SaveFunction();
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
    void SaveFunction()
    {
        //for universal information
        //UniversalSaveState.SaveUniversalInformation(universalInfo);
        //for nodes and links architecture
        circleGenerator.SaveOverworldState();

        //for overworld releveant data like current node selected
        SaveKeyOverworld saveKeyOverworld = new SaveKeyOverworld(worldState, currentNode.transform, adjacentNodes, camera.transform.position, nodeWrapper, activityList);
        UniversalSaveState.SaveOverWorldData(saveKeyOverworld);

        //for saving the universalInformation
        //immediately increase node count when transitioning to an new node scene if it is still traversible, this is so that the node tracking is all here
        //null check is for determining whether it is move node or not. nodeWrapper is null at first node load
        if (saveKeyOverworld.nodeWrapper != null && saveKeyOverworld.nodeWrapper.isTargetNodeTraversed)
        {
            universalInformation.nodeCount = universalInformation.nodeCount + 1;
        }

        //always make this identifier false
        //will only be true if the file was recently saved from LinkActivities script
        //universalInformation.isFromLinkActivity = false;

        //get the gear slots and inventory list of Gears from load
        //the current
        UniversalSaveState.SaveUniversalInformation(universalInformation);
    }

    //this is a wrapper class for the nodeEnum and the isTraversed identifiers
    //originally the UniersalInformation class
    [Serializable]
    public class NextNodeWrapper
    {
        public NodeActivityEnum nextNode;
        //the traversed identifiers are not the same here compared to the one in the overworld state save 
        //these are saved before actually marking the link and nodes as traversed since these will be used by other scenes to determine what scene to load next
        //not used by the overWorld state loader
        public bool isTargetNodeTraversed;
        public bool isPartnerLinkTraversed;
        public NextNodeWrapper(NodeActivityEnum next, bool isNodeTraversed, bool isLinkTraversed)
        {
            nextNode = next;
            isTargetNodeTraversed = isNodeTraversed;
            isPartnerLinkTraversed = isLinkTraversed;
        }
    }

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

    //migrated from UniversalInformation
    public OverworldManager.NextNodeWrapper nodeWrapper;

    //will contain the activities list taken from the link activities between the current node and target node in overWorld
    public List<LinkActivityEnum> linkActivities = new List<LinkActivityEnum>();

    //param 1 = overworldManager state
    //param 2 = currentNode
    //param 3 = current node's adjacent nodes
    //param 4 = position of mouse
    //param 5 = wrapper for enum for the next node Activity, identifier if next node is already traversed, identifier if next link is already traversed
    //param 6 = list of linkActivities from the generated link between source and target nodes

    public SaveKeyOverworld(OverworldState worldState, Transform nodeTrans, List<GameObject> adjacentNodes, Vector2 cameraPos, OverworldManager.NextNodeWrapper nodeInfo, 
        List<LinkActivityEnum> activityList)
    {
        moveState = worldState;
        nodeIndex = nodeTrans.GetSiblingIndex();
        nodeParent = nodeTrans.parent.GetSiblingIndex();
        isMapInitialized = UniversalSaveState.isMapInitialized;
        adjacentNodeCount = adjacentNodes.Count;
        cameraX = cameraPos.x;
        cameraY = cameraPos.y;
        nodeWrapper = nodeInfo;
        linkActivities = activityList;
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







