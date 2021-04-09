using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UniversalSaveState
{    //indicates if the Overworld Map is already generated, if this is false, when going back to Overworld scene, it will load the saved Overworld state
    public static bool isMapInitialized { get; set; }

    public static List<GameObject> overWorldObjects = new List<GameObject>();
    //called by overworld manager to save the current state
    public static void SaveOverworldMap(List<GameObject> current)
    {
        //currentOverworld = current;
        overWorldObjects.AddRange(current);        
    }
    public static List<GameObject> LoadOverWorldMap()
    {
        return overWorldObjects;
    }



    public static void CheckState()
    {
        Debug.Log("state checked");
    }

}
