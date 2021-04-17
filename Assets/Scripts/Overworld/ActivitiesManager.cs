using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivitiesManager : MonoBehaviour
{
    NodeActivityEnum nextNode;
    bool isNodeTraversed;
    bool isLinkTraversed;

    //called by overworld manager to load the target scene
    //the scene name is the NodeActivityEnum string + "Scene"
    //will determine if we load a scene depending on whether the link and node is traversed

    public void LoadActivities()
    {
        UniversalInformation UniversalInfo = UniversalSaveState.LoadUniversalInformation();
        nextNode = UniversalInfo.nextNode;
        isLinkTraversed = UniversalInfo.isPartnerLinkTraversed;
        isNodeTraversed = UniversalInfo.isTargetNodeTraversed;

        if (!isLinkTraversed)
        {
            SceneManager.LoadScene("LinkActivitiesScene");
        }
        else
        {
            if (!isNodeTraversed)
            {
                nextNode = UniversalInfo.nextNode;
                SceneManager.LoadScene(nextNode.ToString() + "Scene");
            }
            else
            {
                Debug.Log("Both Traversed, move along");
            }
        }
    }


    //old functionality
    public void LoadStartNodeActivity(NodeActivityEnum first)
    {

        SceneManager.LoadScene(first.ToString() + "Scene");
    }

}
