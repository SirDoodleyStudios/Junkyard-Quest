using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class OverworldManager : MonoBehaviour
{


    public ActivitiesManager activitiesManager;

    OverworldState worldState;
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
    float xBoundary;
    float yBoundary;

    //stores the current selected node
    GameObject currentNode;

    //list that holds all starting positions
    List<GameObject> startingReachableNodes = new List<GameObject>();
    List<GameObject> adjacentNodes = new List<GameObject>();

    //for object clicking
    Vector2 PointRay;
    Ray ray;
    RaycastHit2D pointedObject;

    void Start()
    {
        //calls circleManager
        circleGenerator.GenerateMap();

        //orthographic size is equal to your desired screenheight/2. I want the ortho height to be half of screen size, so i divided the canvas height by 4
        camera.orthographicSize = mainCanvas.GetComponent<RectTransform>().rect.height / 4;
        //boundaries for he map
        //canvas position is at the 0,0 and camera pivot is at center, ortho camera is effectively half of screen size
        xBoundary = camera.aspect * camera.orthographicSize;
        yBoundary = camera.orthographicSize;

        if (UniversalSaveState.isMapInitialized)
        {
            worldState = OverworldState.MoveNode;
        }

    }
    //called by CircleGenerator once the parent circles are generated
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
                        //adjacentNodes = new List<GameObject>();
                        adjacentNodes.AddRange(currentNodeIden.linkedOuterNodes);
                        adjacentNodes.AddRange(currentNodeIden.linkedInnerNodes);

                        foreach (GameObject adjacentNode in adjacentNodes)
                        {
                            NodeLinkIdentifier adjacentIden = adjacentNode.GetComponent<NodeLinkIdentifier>();
                            adjacentIden.MakeNodeClickable();
                        }

                        worldState = OverworldState.MoveNode;
                        //calls circle generator's save function to save current overworld
                        circleGenerator.SaveOverworldState();
                        //scene transition depending on the NodeActivityEnum of the target Node
                        activitiesManager.LoadStartNodeActivity(NodeActivityEnum.Combat);

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

                        //calls circle generator's save function to save current overworld
                        circleGenerator.SaveOverworldState();
                        //FOR TEST ONLY, ACTIVATES A TEST SCENE WHICH JUST HAPPENS TO BE THE COMBAT SCENE
                        activitiesManager.LoadStartNodeActivity(NodeActivityEnum.Combat);

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


