﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class LinkActivitiesManager : MonoBehaviour
{
    //assigned in editor
    //universalUI
    public CameraUIScript cameraUIScript;
    //parent of the node objects
    public GameObject nodePanel;
    //the nodes to be assigned with images
    public Image sourceNode;
    public Image Activity1;
    public Image Activity2;
    public Image Activity3;
    public Image targetNode;
    //gameObjects for the activity nodes
    public GameObject sourceNodeObj;
    public GameObject activityNodeObj1;
    public GameObject activityNodeObj2;
    public GameObject activityNodeObj3;
    public GameObject targetNodeObj;
    //gameObject for the moving node
    public GameObject movingNodeObj;
    MovingNode movingNodeScript;
    RectTransform movingNodeRect;
    //gameObject of the next node
    GameObject nextTargetNode;
    //reference that contains the script for Card Removal
    public CardRemoval cardRemoval;
    //the proceed button, uninteractable by default
    public Button proceedButton;

    //will contain the saveState, either generated new or loaded ffrom a file
    LinkActivitiesSaveState linkActivitySaveState;

    //next node in overworld
    NodeActivityEnum nextNode;
    bool isNodeTraversed;

    //indicates states for activities inside the link
    int currentLinkNodeIndex;

    //this is just a test
    public GameObject ActivityHolder;

    //references to save files
    UniversalInformation universalInfo;
    SaveKeyOverworld overworldData;

    //the playerstats to be updated and for pulling values from
    PlayerUnit playerStats;

    //bool identifier if an loaded from file
    //indicates if we'll load from file or generate a fresh one
    bool isLoadedFromFile;

    //the current list of activities
    //generated in overworld and automatically assigned here
    List<LinkActivityEnum> linkActivityEnums = new List<LinkActivityEnum>();

    private void Awake()
    {
        //checks first if there is a linkActivity save file
        if (File.Exists(Application.persistentDataPath + "/LinkActivities.json"))
        {
            isLoadedFromFile = true;
        }
        else
        {
            isLoadedFromFile = false;
        }

        //calls initialization of descriptions first for card draft window
        CardTagManager.InitializeTextDescriptionDictionaries();
        universalInfo = UniversalSaveState.LoadUniversalInformation();
        overworldData = UniversalSaveState.LoadOverWorldData();
        cameraUIScript.InitiateUniversalUIInfoData(universalInfo);
        cameraUIScript.AssignUIObjects(universalInfo);
        playerStats = universalInfo.playerStats;
        //assign the movingNode script
        movingNodeScript = movingNodeObj.GetComponent<MovingNode>();
        movingNodeRect = movingNodeObj.GetComponent<RectTransform>();


    }

    void Start()
    {
        //UniversalInformation UniversalInfo = UniversalSaveState.LoadUniversalInformation();
        OverworldManager.NextNodeWrapper nodeWrapper = UniversalSaveState.LoadOverWorldData().nodeWrapper;
        //nextNode = UniversalInfo.nextNode;
        nextNode = nodeWrapper.nextNode;
        //isLinkTaversed = UniversalInfo.isPartnerLinkTraversed;
        //isNodeTraversed = UniversalInfo.isTargetNodeTraversed;
        isNodeTraversed = nodeWrapper.isTargetNodeTraversed;
        //assign the determined activityList from overworld
        linkActivityEnums = overworldData.linkActivities;

        //assign the node Images
        //source node won't get one cuz I dont want additional work, just make it so that the node becomes empty once player uses it
        Activity1.sprite = Resources.Load<Sprite>($"NodeIcon/{linkActivityEnums[0]}");
        Activity2.sprite = Resources.Load<Sprite>($"NodeIcon/{linkActivityEnums[1]}");
        Activity3.sprite = Resources.Load<Sprite>($"NodeIcon/{linkActivityEnums[2]}");
        targetNode.sprite = Resources.Load<Sprite>($"NodeIcon/{nodeWrapper.nextNode}");


        //test iteration for activity and test
        for (int i = 0; 2>=i; i++)
        {
            Text activitext = ActivityHolder.transform.GetChild(i).gameObject.GetComponent<Text>();
            //LinkActivityEnum linkActivity = (LinkActivityEnum)Random.Range(0, 3);
            LinkActivityEnum linkActivity = UniversalFunctions.GetRandomEnum<LinkActivityEnum>();
            activitext.text = linkActivity.ToString();
        }

        InitializeLinkActivities();
    }

    private void Update()
    {
        //loads the next node activity scene if the node is not traversed

        if (Input.GetKey(KeyCode.Backspace))
        {
            if (!isNodeTraversed)
            {
                SceneManager.LoadScene(nextNode.ToString() + "Scene");
            }
            else
            {
                SceneManager.LoadScene("OverworldScene");
            }

        }

    }

    //called at start to initialize linkActivities info
    void InitializeLinkActivities()
    {
        //make the button interactable
        proceedButton.interactable = true;

        //if loaded from file, get info from there
        if (isLoadedFromFile)
        {
            linkActivitySaveState = UniversalSaveState.LoadLinkActivities();
            currentLinkNodeIndex = linkActivitySaveState.currentLinkNodeIndex;

            //sets the player node 
            StartCoroutine(SetBeginningNode());
        }
        //if not loaded, generate new data
        else
        {
            //assign the current and next nodes
            currentLinkNodeIndex = 0;

            //generate na new LinkActivity saveState
            linkActivitySaveState = new LinkActivitiesSaveState();
            linkActivitySaveState.currentLinkNodeIndex = currentLinkNodeIndex;
            UniversalSaveState.SaveLinkActivities(linkActivitySaveState);

            //sets the player node 
            StartCoroutine(SetBeginningNode());
        }

        //assigns the next node object immediately
        if (currentLinkNodeIndex <= 3)
        {
            nextTargetNode = nodePanel.transform.GetChild(currentLinkNodeIndex + 1).gameObject;
        }

    }
    //supporting method just to get the anchoredPosition of the starting node
    //anchoredPositions needs a 1 frame buffer for it to retrieve the actual numbers
    IEnumerator SetBeginningNode()
    {
        yield return new WaitForEndOfFrame();
        //set the moving node instantly in the starting position
        Vector3 startingPosition = nodePanel.transform.GetChild(currentLinkNodeIndex).GetComponent<RectTransform>().anchoredPosition;
        movingNodeScript.MovePlayerNode(startingPosition);
    }

    //called through button when player wants to proceed, cals the moving node to move to the next node spot
    public void ProceedToNextNodeButton()
    {
        //upon clicking, makes the button uninteractable to avoid clicking before scene transition
        proceedButton.interactable = false;

        //the destination coordinates sent to the moving node is the next target node's anchored position
        Vector3 nodeDestination = nextTargetNode.GetComponent<RectTransform>().anchoredPosition;
        movingNodeScript.MovePlayerNode(nodeDestination);

        //after proceeding, update the currentNodecounter
        currentLinkNodeIndex++;

        //for activating the activity in next node
        //limit is 4 index because there are 5 nodes, node 4 is the overworld activity node
        if (currentLinkNodeIndex <= 4)
        {
            nextTargetNode = nodePanel.transform.GetChild(currentLinkNodeIndex).gameObject;
            StartCoroutine(ActivateLinkActivity());
        }
    }
    //called after proceeding to next node
    //waits for a time until moving node arrives at target, then switch scenes if necessary
    IEnumerator ActivateLinkActivity()
    {
        //time must be a bit longer than movingNode's tween
        yield return new WaitForSeconds(.5f);

        //if currentNode is 5, that's the target node already
        if (currentLinkNodeIndex == 4)
        {
            if (!isNodeTraversed)
            {
                File.Delete(Application.persistentDataPath + "/LinkActivities.json");
                SceneManager.LoadScene(nextNode.ToString() + "Scene");
            }
        }
        //if not, proceed to link activities
        else
        {
            //assigns the universalInfo's link and Node activities
            //when using the currentLinkNodeIndex as index for the linkActivity, subtract it by one because the current count is already iterated
            //at node 1, the linkActivity should be 0
            universalInfo.linkActivity = linkActivityEnums[currentLinkNodeIndex - 1];
            //universalInfo.isFromLinkActivity = true;
            UniversalSaveState.SaveUniversalInformation(universalInfo);

            //save the currentNodeIndex after moving
            linkActivitySaveState.currentLinkNodeIndex = currentLinkNodeIndex;
            UniversalSaveState.SaveLinkActivities(linkActivitySaveState);

            //get's the linkActivity to activate
            LinkActivityEnum activity = linkActivityEnums[currentLinkNodeIndex - 1];

            //determines what scene is next
            //the current node at this instance is already the target node
            //if (activity == LinkActivityEnum.Skirmish)
            //{
            //    SceneManager.LoadScene("CombatScene");
            //}
            //else
            //{
            //    SceneManager.LoadScene("CombatScene");
            //    Debug.Log($"supposed to be {linkActivityEnums[currentLinkNodeIndex-1]}");
            //}

            //REMOVE THE currentLinkNodeIndex++ IN THE SWITCH
            //FIND OUT WHY THE NODE MOVEMENT DOES NOT WORK PROPERLY FOR NON-SCREEN TRANSITION ACTIVITIES
            switch (activity)
            {
                case LinkActivityEnum.Skirmish:
                    SceneManager.LoadScene("CombatScene");
                    break;

                case LinkActivityEnum.HPGain:
                    int hpGain = (int)(playerStats.HP * .05f);
                    playerStats.currHP = Mathf.Min(playerStats.currHP + hpGain, playerStats.HP);
                    cameraUIScript.UpdateUIObjectsHP(playerStats.currHP, playerStats.HP);
                    universalInfo.playerStats = playerStats;
                    UniversalSaveState.SaveUniversalInformation(universalInfo);
                    //gains will update the scene UniversalUI since we wont transition
                    cameraUIScript.UpdateUniversalInfo();
                    //gain activities will always make the proceed button interactable since there is no scene transition
                    proceedButton.interactable = true;
                    //gain activities will immediately increment the currentLinkNodeCounter so that we can get the next destination
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    break;

                case LinkActivityEnum.CreativityGain:
                    int creativityGain = (int)(playerStats.creativity * .05f);
                    playerStats.currCreativity = Mathf.Min(playerStats.currCreativity + creativityGain, playerStats.creativity);
                    cameraUIScript.UpdateUIObjectsCretivity(playerStats.currCreativity, playerStats.creativity);
                    universalInfo.playerStats = playerStats;
                    UniversalSaveState.SaveUniversalInformation(universalInfo);
                    //gains will update the scene UniversalUI since we wont transition
                    cameraUIScript.UpdateUniversalInfo();
                    //gain activities will always make the proceed button interactable since there is no scene transition
                    proceedButton.interactable = true;
                    //gain activities will immediately increment the currentLinkNodeCounter so that we can get the next destination
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    break;

                case LinkActivityEnum.ScrapsGain:
                    int scrapsGain = UnityEngine.Random.Range(10,30);
                    universalInfo.scraps = universalInfo.scraps + scrapsGain;
                    cameraUIScript.UpdateUIObjectsActiveScraps(universalInfo.scraps);
                    //gains will update the scene UniversalUI since we wont transition
                    cameraUIScript.UpdateUniversalInfo();
                    //gain activities will always make the proceed button interactable since there is no scene transition
                    proceedButton.interactable = true;
                    //gain activities will immediately increment the currentLinkNodeCounter so that we can get the next destination
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    break;

                case LinkActivityEnum.TicketGain:
                    //currently for test only so we can proceed
                    proceedButton.interactable = true;
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    break;

                case LinkActivityEnum.CardRemove:
                    //currently for test only so we can proceed
                    proceedButton.interactable = true;
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    //cardRemoval.ViewSavedDeck(cameraUIScript.FetchDeck());
                    break;

                case LinkActivityEnum.StrengthTest:
                    //currently for test only so we can proceed
                    proceedButton.interactable = true;
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    break;

                case LinkActivityEnum.EnduranceTest:
                    //currently for test only so we can proceed
                    proceedButton.interactable = true;
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    break;

                case LinkActivityEnum.LuckTest:
                    //currently for test only so we can proceed
                    proceedButton.interactable = true;
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    break;

                case LinkActivityEnum.IntelligenctTest:
                    //currently for test only so we can proceed
                    proceedButton.interactable = true;
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    break;

                case LinkActivityEnum.LinkGamble:
                    //currently for test only so we can proceed
                    proceedButton.interactable = true;
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    break;

                case LinkActivityEnum.NameGamble:
                    //currently for test only so we can proceed
                    proceedButton.interactable = true;
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    break;

                case LinkActivityEnum.TypeGamble:
                    //currently for test only so we can proceed
                    proceedButton.interactable = true;
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    break;

                case LinkActivityEnum.MaterialTrade:
                    //currently for test only so we can proceed
                    proceedButton.interactable = true;
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    break;

                case LinkActivityEnum.GearTrade:
                    //currently for test only so we can proceed
                    proceedButton.interactable = true;
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    break;

                case LinkActivityEnum.BlueprintTrade:
                    //currently for test only so we can proceed
                    proceedButton.interactable = true;
                    currentLinkNodeIndex++;
                    Debug.Log($"{activity}");
                    break;

                default:
                    break;
            }


        }

    }

}