using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    //event for calling all playerObjects when a playerObject is chosen
    public delegate void D_PlayerChosenEvent(int i);
    public event D_PlayerChosenEvent d_PlayerChosenEvent;

    //object holders
    public GameObject playerPanel;
    public GameObject rolePanel;

    //identifier checked by the individual choice objects
    //this is set as true when a choice has been made
    public bool isSomethingChosen { get; set; }

    //indicator for crosschecking of chosen and clicked objects
    int tempInt;

    //Stores the chosen playables
    ChosenPlayer chosenPlayer;
    ChosenClass chosenClass;

    //for object clicking
    Vector2 PointRay;
    Ray ray;
    RaycastHit2D pointedObject;

    // Update is called once per frame
    void Update()
    {
        PointRay = Input.mousePosition;
        ray = Camera.main.ScreenPointToRay(PointRay);
        pointedObject = Physics2D.GetRayIntersection(ray);

        if (Input.GetMouseButtonDown(0))
        {
            if (pointedObject.collider != null)
            {
                GameObject chosenObject = pointedObject.collider.gameObject;
                tempInt = chosenObject.transform.GetSiblingIndex();
                //the index of the clicked object matched the index IDs in the enum list
                chosenPlayer = (ChosenPlayer)tempInt;
                Debug.Log(chosenPlayer);

            }
        }

    }
    //called when an object is clicked so that continuity doesnt get messed up
    public void UnchooseActivate()
    {
        //the int parameter is for crosschecking with the object itself if it is the chosen object, if so, the selection will not change
        d_PlayerChosenEvent(tempInt);
    }
}
