using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class EventsManager : MonoBehaviour
{
    UniversalInformation universalInfo;
    //assigned in editor
    //reference to the choices UI
    public Transform eventChoicesUIPanel;
    public TextMeshProUGUI eventDescriptionText;
    public EventsManager thisManager;
    public Image eventIllustration;
    public CameraUIScript cameraUIScript;

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

    //loaded EventSaveState if event is loaded from file or not
    EventSaveState eventSaveState;

    private void Awake()
    {
        //load the current universalInfo
        universalInfo = UniversalSaveState.LoadUniversalInformation();
        //will initialize the factory for getting cards using allcard enum
        CardSOFactory.InitializeCardSOFactory(universalInfo.chosenPlayer, universalInfo.chosenClass);
        //initializes the deck viewr
        CardTagManager.InitializeTextDescriptionDictionaries();
        cameraUIScript.InitiateUniversalUIInfoData(universalInfo);
        cameraUIScript.AssignUIObjects(universalInfo);

        //initialize the event Factory first
        EventFactory.InitializeEventFactory();

        //checks if there is an eventSaveState in the files
        if (File.Exists(Application.persistentDataPath + "/Event.json"))
        {
            eventScript = EventFactory.GetEvent(triggeredEvent);
            eventSaveState = UniversalSaveState.LoadEvent();
            triggeredEvent = eventSaveState.eventEnum;
        }
        //generate a new one if none
        else
        {
            eventScript = EventFactory.GetEvent(triggeredEvent);
            eventSaveState = new EventSaveState();
            //this is the real script
            //triggeredEvent = UniversalFunctions.GetRandomEnum<AllEvents>();
            //this is for test only
            triggeredEvent = AllEvents.TwoChestsOneKey;
            eventSaveState.eventEnum = triggeredEvent;

            //immediately Generate the event save file
            UniversalSaveState.SaveEvent(eventSaveState);
        }

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


        eventScript.InitializeEventReferences(universalInfo, thisManager, eventChoicesUIPanel, eventDescriptionText, eventSaveState);
        eventScript.ActivateEvent();
        eventIllustration.sprite = Resources.Load<Sprite>($"Events/{eventScript.eventEnum}");
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
    //return a generated MaterialWrapper to be added to the universalInfo save
    public void CreateMaterial(bool isRandomized)
    {
        //randomize ints for the components of the material being built
        CraftingMaterialSO tempMaterialSO = Instantiate(referenceMaterialSO);
        //if random material is needed
        if (isRandomized)
        {
            tempMaterialSO.materialType = UniversalFunctions.GetRandomEnum<AllMaterialTypes>();
            //material prefix should not have "Normal" which is index 0 in AllMaterialPrefixes enum
            //reiterates until the randomized prefix is not Normal anymore
            AllMaterialPrefixes materialPrefix;
            do
            {
                materialPrefix = UniversalFunctions.GetRandomEnum<AllMaterialPrefixes>();
            }
            while ((int)materialPrefix == 0);
            tempMaterialSO.materialPrefix = materialPrefix;

            for (int j = 0; 1 >= j; j++)
            {
                AllMaterialEffects materialEffect;
                //prevents repeat of material Effect by rerolling the material Effect enum if the SO's material Effect List already contains the randomized effect
                //the materialEffect < 100 condition is for preventing set bonuses that are in the 100+ spot of the enum are not taken during randomization
                do
                {
                    materialEffect = UniversalFunctions.GetRandomEnum<AllMaterialEffects>();
                }
                while (tempMaterialSO.materialEffects.Contains(materialEffect) || (int)materialEffect >= 100);
                tempMaterialSO.materialEffects.Add(materialEffect);
            }
        }

        //generate the prefab
        GameObject materialObject = Instantiate(referenceMaterialPrefab);
        CraftingMaterialSOHolder materialSOHolder = materialObject.GetComponent<CraftingMaterialSOHolder>();
        //assign the SO generated earlier
        materialSOHolder.craftingMaterialSO = tempMaterialSO;

        //add the materialWrapper for saving
        CraftingMaterialWrapper materialWrapper = new CraftingMaterialWrapper(tempMaterialSO);
        universalInfo.craftingMaterialWrapperList.Add(materialWrapper);
        //update the UI objects
        cameraUIScript.UpdateMaterialInventory(materialWrapper, true);

    }

    //for healing and damaging HP in UI
    public void AlterUIHP(bool isToAdd, int amount)
    {
        //alter the HP values in the save file
        int tempCurrHP = universalInfo.playerStatsWrapper.currHP;
        int tempMaxHP = universalInfo.playerStatsWrapper.HP;

        //if healing, add only up to max HP
        if (isToAdd)
        {
            tempCurrHP = Mathf.Min((tempCurrHP + amount), tempMaxHP);
        }
        //if damage, subtrac to only up to 1, player can only die during combat
        else
        {
            tempCurrHP = Mathf.Max((tempCurrHP - amount), 1);
        }

        //update the UI HP text
        cameraUIScript.UpdateUIObjectsHP(tempCurrHP, tempMaxHP);

        //update currentUniversalInfo
        universalInfo.playerStatsWrapper.currHP = tempCurrHP;
        universalInfo.playerStatsWrapper.HP = tempMaxHP;
    }

    //Function to showcase the obtained item
    //will be developed later on
    public void ShowcaseItem()
    {

    }

    //save universalInformation when a choice is made
    public void SaveFromChoice()
    {
        UniversalSaveState.SaveUniversalInformation(universalInfo);
    }


    //used for leaving the event
    //Function to leave MerchantManager
    public void EndEvent()
    {
        UniversalSaveState.SaveUniversalInformation(universalInfo);
        File.Delete(Application.persistentDataPath + "/Event.json");
        SceneManager.LoadScene("OverWorldScene");
    }


}
