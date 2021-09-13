using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LinkActivitiesManager : MonoBehaviour
{
    //assigned in editor
    //the nodes to be assigned with images
    public Image sourceNode;
    public Image Activity1;
    public Image Activity2;
    public Image Activity3;
    public Image targetNode;
    //universalUI
    public CameraUIScript cameraUIScript;


    NodeActivityEnum nextNode;
    bool isNodeTraversed;


    //this is just a test
    public GameObject ActivityHolder;

    UniversalInformation universalInfo;
    SaveKeyOverworld overworldData;

    //the current list of activities
    List<LinkActivityEnum> linkActivityEnums = new List<LinkActivityEnum>();

    private void Awake()
    {
        universalInfo = UniversalSaveState.LoadUniversalInformation();
        overworldData = UniversalSaveState.LoadOverWorldData();
        cameraUIScript.InitiateUniversalUIInfoData(universalInfo);
        cameraUIScript.AssignUIObjects(universalInfo);
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
        //assign the determined activityList
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


}
