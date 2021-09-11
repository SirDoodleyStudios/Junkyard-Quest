using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public abstract class EventAbstractClass
{
    public abstract AllEvents eventEnum { get; }

    //reference to UniversalInformation saved
    protected UniversalInformation universalInfo;

    //uniqe texts per event
    protected string eventDescription;
    protected List<string> eventChoices = new List<string>();
    protected int choiceCount;

    //references assigned from the first call from eventsManager
    protected Transform choiceButtonsPanel;
    protected TextMeshProUGUI eventDescText;
    protected EventsManager eventManager;
    //stores the list of TMPros from the choiceButtonsPanel Transform
    protected List<TextMeshProUGUI> buttonTextList = new List<TextMeshProUGUI>();

    //used as reference by most multi-stage events to determine what sequence the event is in
    //very important for determining where yoou left off from event
    protected EventSaveState eventSaveState;
    protected int eventSequence;

    //used as an identifier that the player is leaving the event, this prevents other functions that might occur after calling the eventfunction
    protected bool isLeaving;

    //assigns the reference gameObjects
    public void InitializeEventReferences(UniversalInformation universalInfoLoaded, EventsManager refManager, Transform refButtonPanel, TextMeshProUGUI refDescText, EventSaveState eventSavePassed)
    {
        universalInfo = universalInfoLoaded;
        eventManager = refManager;
        eventDescText = refDescText;
        choiceButtonsPanel = refButtonPanel;
        foreach(Transform button in refButtonPanel)
        {
            TextMeshProUGUI buttonText = button.GetChild(0).GetComponent<TextMeshProUGUI>();
            buttonTextList.Add(buttonText);
        }

        //taken from file, it's defaul 0 if file passed is a dummy
        eventSaveState = eventSavePassed;
        eventSequence = eventSavePassed.eventSequence;
    }

    //main function to begin activation of event
    //always overriden by the children
    public virtual void ActivateEvent()
    {
        //default is 0 but this gets overrided if there is a save file
        //eventSequence = 0;
        //update the sequence in saveState
        eventSaveState.eventSequence = eventSequence;
        PopulateChoices();
    }
    //called through the events manager when a choice is made by a player or when a choice button is clicked
    //first int parameter indicates the 
    //the int parameter is from the idex of the button choice
    public virtual void EventChoiceMade(int buttonIndex)
    {
        UpdateEventState();
        EventTextDeterminer();
    }
    //called when a choice is done
    //either player is presented with another choice or completion Description is displayed with Leave choice
    //relies on eventSequence for the state event states
    public virtual void EventTextDeterminer()
    {
        eventChoices.Clear();
    }


    //function that populates the buttons for choices
    //this is universal and will always be used by all events
    //the parameter received are the objects to be populated with text
    public void PopulateChoices()
    {
        //populate the text in descriptions
        eventDescText.text = eventDescription;
        //populate the choices
        //enable buttons only based on the choice count
        //always iterate through everyting to disable or disable button dependin on the choiceCount
        for (int i = 0; 3 >= i; i++)
        {
            //activate button if the button is still within choiceCount
            if (choiceCount - 1 >= i)
            {
                buttonTextList[i].text = eventChoices[i];
                choiceButtonsPanel.GetChild(i).gameObject.SetActive(true);
            }
            //disable the vutton if count exceeds from choiceCount
            else
            {
                choiceButtonsPanel.GetChild(i).gameObject.SetActive(false);
            }

        }
    }

    //used for events that alter meterials in inventory

    public void AlterMaterialInventory(bool isRandomized)
    {
        //true if randomized
        //call manager to create the material then returns it and add it in the universalInfo file
        eventManager.CreateMaterial(isRandomized);


    }
    //used for events that alter gear inventory
    public void AlterGearInventory()
    {

    }
    //used for events that alter a card's Jigsaw
    public void AlterCardJigsaw()
    {

    }
    //used for events that alters HP
    //true for heal, false for damage
    public void AlterHP(bool isToAdd, int amount)
    {
        eventManager.AlterUIHP(isToAdd, amount);
    }
    //used for events that alters Creativity
    public void AlterCreativity()
    {

    }
    //used for events that alters status
    public void AlterStatus()
    {

    }

    //function called by manager when saving
    //returns the states and sequences of the event
    public void UpdateEventState()
    {
        eventSaveState.eventSequence = eventSequence;
        //will only save in update if player is not leaving
        //this prevents an additional Event file being created once the leave function deletes the existing file
        if (!isLeaving)
        {
            UniversalSaveState.SaveEvent(eventSaveState);
        }
        eventManager.SaveFromChoice();
    }

    //function for leaving and ending event
    public void LeaveEvent()
    {
        isLeaving = true;
        eventManager.EndEvent();
    }

}
