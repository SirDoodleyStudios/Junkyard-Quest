using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//choose 1 of 2 unknown materials
public class Event_TwoChestsOneKey : EventAbstractClass
{
    public override AllEvents eventEnum => AllEvents.TwoChestsOneKey;
    //called by the event Manager
    public override void ActivateEvent()
    {

        //assign texts unique per event
        eventDescription = "You see two identical locked chests and a single key placed on a small rock between them. " +
            "It looks like you can open either of them using the key. Which one will you open?";
        eventChoices.Add("Open the right chest with the key");
        eventChoices.Add("Open the left chest with the key");
        eventChoices.Add("Open the right chest with the key then smash the left chest open");
        eventChoices.Add("Open the left chest with the key then smash the right chest open");
        choiceCount = 4;

        //base will reference files like universalInformation
        //will also handle the choices population
        base.ActivateEvent();
    }

}
