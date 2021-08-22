using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    protected int eventSequence;

    //assigns the reference gameObjects
    public void InitializeEventReferences(EventsManager refManager, Transform refButtonPanel, TextMeshProUGUI refDescText)
    {
        eventManager = refManager;
        eventDescText = refDescText;
        choiceButtonsPanel = refButtonPanel;
        foreach(Transform button in refButtonPanel)
        {
            TextMeshProUGUI buttonText = button.GetChild(0).GetComponent<TextMeshProUGUI>();
            buttonTextList.Add(buttonText);
        }

    }

    //main function to begin activation of event
    //always overriden by the children
    public virtual void ActivateEvent()
    {
        universalInfo = UniversalSaveState.LoadUniversalInformation();
        eventSequence = 0;
        PopulateChoices();
    }
    //called through the events manager when a choice is made by a player or when a choice button is clicked
    //first int parameter indicates the 
    //the int parameter is from the idex of the button choice
    public virtual void EventChoiceMade(int buttonIndex)
    {
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
    public void AlterMaterialInventory()
    {
        //true if randomized
        eventManager.CreateMaterial(true);
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
    public void AlterHP()
    {

    }
    //used for events that alters Creativity
    public void AlterCreativity()
    {

    }
    //used for events that alters status
    public void AlterStatus()
    {

    }
    //used for leaving the event
    public void LeaveEvent()
    {

    }
}
