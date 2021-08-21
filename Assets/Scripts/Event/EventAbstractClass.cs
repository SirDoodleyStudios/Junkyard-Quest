using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventAbstractClass
{
    public abstract AllEvents eventEnum { get; }

    //reference to UniversalInformation saved
    protected UniversalInformation universalInfo;

    //uniqe texts per event
    protected string eventDescription;
    protected List<string> eventChoices;
    protected int choiceCount;

    //main function to begin activation of event
    //always overriden by the children
    public virtual void ActivateEvent()
    {
        universalInfo = UniversalSaveState.LoadUniversalInformation();
        PopulateChoices();
    }

    //function that populates the buttons for choices
    //this is universal and will always be used by all events
    public void PopulateChoices()
    {

    }

    //used for events that alter meterials in inventory
    public void AlterMaterialInventory()
    {

    }
    //used for events that alter gear inventory
    public void AlterGearInventory()
    {

    }
    //used for events that alter a card's Jigsaw
    public void AlterCardJigsaw()
    {

    }

}
