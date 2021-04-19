using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

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
        //UniversalInformation UniversalInfo = UniversalSaveState.LoadUniversalInformation();

        OverworldManager.NextNodeWrapper nexNodeWrapper = UniversalSaveState.LoadOverWorldData().nodeWrapper;

        //nextNode = UniversalInfo.nextNode;
        //isLinkTraversed = UniversalInfo.isPartnerLinkTraversed;
        //isNodeTraversed = UniversalInfo.isTargetNodeTraversed;

        nextNode = nexNodeWrapper.nextNode;
        isLinkTraversed = nexNodeWrapper.isPartnerLinkTraversed;
        isNodeTraversed = nexNodeWrapper.isTargetNodeTraversed;

        if (!isLinkTraversed)
        {
            SceneManager.LoadScene("LinkActivitiesScene");
        }
        else
        {
            if (!isNodeTraversed)
            {
                //nextNode = UniversalInfo.nextNode;
                nextNode = nexNodeWrapper.nextNode;
                SceneManager.LoadScene(nextNode.ToString() + "Scene");
            }
            else
            {
                Debug.Log("Both Traversed, move along");
            }
        }
    }
    //will be called by overworld when resetting
    //the identifier isResetting will be cheched during overworld initial run and will only be true during resetting and will not be saved to json
    public void ResetMap()
    {
        File.Delete(Application.persistentDataPath + "/OverWorldData.json");
        UniversalSaveState.isMapInitialized = false;
        UniversalSaveState.isResetting = true;
        SceneManager.LoadScene("OverworldScene");
    }

    //old functionality
    public void LoadStartNodeActivity(NodeActivityEnum first)
    {

        SceneManager.LoadScene(first.ToString() + "Scene");
    }

}
