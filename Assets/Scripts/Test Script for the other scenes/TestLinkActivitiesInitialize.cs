using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestLinkActivitiesInitialize : MonoBehaviour
{
    NodeActivityEnum nextNode;
    bool isNodeTraversed;


    //this is just a test
    GameObject ActivityHolder;

    void Start()
    {
        UniversalInformation UniversalInfo = UniversalSaveState.LoadUniversalInformation();
        nextNode = UniversalInfo.nextNode;
        //isLinkTaversed = UniversalInfo.isPartnerLinkTraversed;
        isNodeTraversed = UniversalInfo.isTargetNodeTraversed;

        //test iteration for activity and test
        for (int i = 0; 3>i; i++)
        {
            Text activitext = ActivityHolder.transform.GetChild(i).gameObject.GetComponent<Text>();
            LinkActivityEnum linkActivity = (LinkActivityEnum)Random.Range(0, 3);
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
