using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventsManager : MonoBehaviour
{
    //assigned in editor
    //reference to the choices UI
    public Transform eventChoicesUIPanel;
    public TextMeshProUGUI eventDescriptionText;
    public EventsManager thisManager;

    //will contain the randomized event
    AllEvents triggeredEvent;

    //generated Script for the event
    EventAbstractClass eventScript;

    //references for Instantiation
    //assigned in editor
    public GameObject referenceMaterialPrefab;
    public CraftingMaterialSO referenceMaterialSO;
    public GameObject referenceGearPrefab;
    public GearSO referenceGearSO;
    public GameObject referenceBlueprintPrefab;
    public BluePrintSO referenceBlueprintSO;
    public GameObject cardPrefab;

    private void Awake()
    {
        //initialize the event Factory first
        EventFactory.InitializeEventFactory();
        //this is the real script
        //triggeredEvent = UniversalFunctions.GetRandomEnum<AllEvents>();
        //this is for test only
        triggeredEvent = AllEvents.TwoChestsOneKey;

        eventScript = EventFactory.GetEvent(triggeredEvent);
        ChooseEventEffects();
        //InstantiateReferences();
    }

    //called at the beginning to instantiate the references for use so that the prefab is not editted
    void InstantiateReferences()
    {
        referenceMaterialPrefab = Instantiate(referenceMaterialPrefab);
        referenceMaterialSO = Instantiate(referenceMaterialSO);
        referenceGearPrefab = Instantiate(referenceGearPrefab);
        referenceGearSO = Instantiate(referenceGearSO);
    }


    //huge factory function that determines the effects and choices of an event
    void ChooseEventEffects()
    {
        eventScript.InitializeEventReferences(thisManager, eventChoicesUIPanel, eventDescriptionText);
        eventScript.ActivateEvent();
    }

    //all buttons that calls the same function but sends a differenct int parameter for the choice selection
    public void ChoiceButton0()
    {
        eventScript.EventChoiceMade(0);
    }
    public void ChoiceButton1()
    {
        eventScript.EventChoiceMade(1);
    }
    public void ChoiceButton2()
    {
        eventScript.EventChoiceMade(2);
    }
    public void ChoiceButton3()
    {
        eventScript.EventChoiceMade(3);
    }


    //Functions to create objects as outcomes of events
    //bool parameter is if randomized
    public void CreateMaterial(bool isRandomized)
    {

    }

   
}
