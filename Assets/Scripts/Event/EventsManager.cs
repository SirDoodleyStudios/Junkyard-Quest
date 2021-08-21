using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    //will contain the randomized event
    AllEvents triggeredEvent;

    //generated Script for the event
    EventAbstractClass eventScript;

    private void Awake()
    {
        triggeredEvent = UniversalFunctions.GetRandomEnum<AllEvents>();
        eventScript = EventFactory.GetEvent(triggeredEvent);
        ChooseEventEffects();
    }


    //huge factory function that determines the effects and choices of an event
    void ChooseEventEffects()
    {
        eventScript.ActivateEvent();
    }

   
}
