using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivitiesManager : MonoBehaviour
{
    List<LinkActivityEnum> loadedLinkActivities = new List<LinkActivityEnum>();
    NodeActivityEnum loadedNodeActivity;
    NodeActivityEnum nextNodeActivity;

    public void LoadStartNodeActivity(NodeActivityEnum first)
    {
        SceneManager.LoadScene(first.ToString() + "Scene");
    }
    void LoadNodeActivities(NodeActivityEnum current, NodeActivityEnum destination)
    {
        loadedNodeActivity = current;
        nextNodeActivity = destination;
    }
    void LoadLinkActivities(List<LinkActivityEnum> loadedActivities)
    {
        loadedLinkActivities.AddRange(loadedActivities);
    }
}
