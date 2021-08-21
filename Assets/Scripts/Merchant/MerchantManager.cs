using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MerchantManager : MonoBehaviour
{
    ////delegate that calls all Option Scripts and update all of their internal MerchantSaveStates with the latest one
    //public delegate MerchantSaveState D_UpdateOptionMerchantSaveStates(MerchantSaveState i);
    //public event D_UpdateOptionMerchantSaveStates d_UpdateOptionMerchantSaveStates;

    //loaded unviversalInfo
    UniversalInformation universalInfo;

    //loaded or new merchantSaveState
    MerchantSaveState merchantSaveState;

    // the universal UI
    //Assigned in editor
    public CameraUIScript cameraUIScript;

    //UI Objects for options
    //assigned in editor
    public GameObject cardOptionsUI;
    public GameObject materialOptionsUI;
    public GameObject blueprintOptionsUI;
    public GameObject cardRemovalUI;
    CardOptions cardOptionsScript;
    MaterialOptions materialOptionsScript;
    BlueprintOptions blueprintOptionsScript;
    CardRemoval cardRemovalScript;
    //reference to the Card Removal button, will be disabled after the first use
    public Button cardRemovalButton;

    //identifiers for the UIs if they have already been initiated
    bool isOptionsInitialized;

    //int value for remaining scraps of player
    int remainingScraps;

    //identifier to check if the merchantSaveState is loaded fron an existing file
    bool isLoadedFromFile;


    private void Awake()
    {

        universalInfo = UniversalSaveState.LoadUniversalInformation();
        //will initialize the factory for getting cards using allcard enum
        CardSOFactory.InitializeCardSOFactory(universalInfo.chosenPlayer, universalInfo.chosenClass);
        //initializes the deck viewr
        CardTagManager.InitializeTextDescriptionDictionaries();
        cameraUIScript.InitiateUniversalUIInfoData(universalInfo);
        cameraUIScript.AssignUIObjects(universalInfo);
        //available value of scraps taken from cameraScriptUI and universalInfo
        remainingScraps = universalInfo.scraps;

        //script assignments for the Option scripts
        cardOptionsScript = cardOptionsUI.GetComponent<CardOptions>();
        materialOptionsScript = materialOptionsUI.GetComponent<MaterialOptions>();
        blueprintOptionsScript = blueprintOptionsUI.GetComponent<BlueprintOptions>();
        cardRemovalScript = cardRemovalUI.GetComponent<CardRemoval>();

        //if there is no MerchantSave, it means that the merchant scene was loaded from current game session
        if (File.Exists(Application.persistentDataPath + "/Merchant.json"))
        {
            merchantSaveState = UniversalSaveState.LoadMerchant();
            isLoadedFromFile = true;
        }
        else
        {
            merchantSaveState = new MerchantSaveState();
            isLoadedFromFile = false;
        }
        //initiate the output settings
        InitiateOptionUIs();
    }

    //for initiating all option UIs
    void InitiateOptionUIs()
    {
        //if the UI is not yet initialized, initialze it then turn the identifier to true to ensure that it is called only one time
        if (isOptionsInitialized == false)
        {
            //initiate Options fresh in current game session
            //the merchantSaveState is either loaded from file or empty depending on the file check function in awake
            //the bool parameter will dictate if the optios are to be loaded randomly or from file

            //if loaded from file, no need to update the merchantSaveStates because if loaded, we only handin the info to the OptionsUI
            if (isLoadedFromFile)
            {
                //for card options
                cardOptionsScript.InitiateCardOptions(universalInfo, merchantSaveState, isLoadedFromFile);
                //for material options
                materialOptionsScript.InitiateMaterialOptions(universalInfo, merchantSaveState, isLoadedFromFile);
                //for blueprint options
                blueprintOptionsScript.InitiateBlueprintOptions(merchantSaveState, isLoadedFromFile);
                //enables or disables the button for card removal
                cardRemovalButton.interactable = merchantSaveState.isCardRemovalAvailable;

            }
            //if fresh initiate, we'll be passing the merchantSvaeState around while the options scripts populate their appropriate lists
            else
            {
                //for card options
                merchantSaveState = cardOptionsScript.InitiateCardOptions(universalInfo, merchantSaveState, isLoadedFromFile);
                //for material options
                merchantSaveState = materialOptionsScript.InitiateMaterialOptions(universalInfo, merchantSaveState, isLoadedFromFile);
                //for blueprint options
                merchantSaveState = blueprintOptionsScript.InitiateBlueprintOptions(merchantSaveState, isLoadedFromFile);
                //for card removal identifier, always false 
                merchantSaveState.isCardRemovalAvailable = true;

                //immediately create the merchantSaveState
                UpdateMerchantSaveState(merchantSaveState);
            }

            isOptionsInitialized = true;

        }

        //for materialOptions initiation

    }

    //for accessing the options UI
    // button functions

    public void CardOptionsButton()
    {
        cardOptionsUI.SetActive(true);
        //calls the actual command to view available cards
        cardOptionsScript.ViewCardOptions(merchantSaveState);
    }

    public void MaterialOptionsButton()
    {
        materialOptionsUI.SetActive(true);
        materialOptionsScript.ViewMaterialOptions();
        
    }

    public void BlueprintOptionsButton()
    {
        blueprintOptionsUI.SetActive(true);
        blueprintOptionsScript.ViewBlueprintOptions();
    }
    public void CardRemovalButton()
    {
        cardRemovalUI.SetActive(true);
        //view deck in cardRemovalUI sent with the cardList generated
        cardRemovalScript.ViewSavedDeck(cameraUIScript.FetchDeck());
    }

    //called when adding a card in deck via merchant buy
    public void AddBoughtCard(Card card)
    {
        //update the cardList saved in cameraScriptUI, does not edit the deck from save file
        cameraUIScript.UpdateCurrentDeck(card, true);
        //create wrapper for saving to universalInfo
        CardAndJigsaWrapper CJW = new CardAndJigsaWrapper(card);
        universalInfo.currentDeckWithJigsaw.Add(CJW);

        //removed for now
        //save function to be done when exiting from the merchant scene
        //UniversalSaveState.SaveUniversalInformation(universalInfo);
        ////update the universalInfo in the cameraUIScript
        //cameraUIScript.UpdateUniversalInfo();
    }
    //called when adding a material in deck via merchant buy
    public void AddBoughtMaterial(CraftingMaterialSO materialSO)
    {
        CraftingMaterialWrapper craftingMaterialWrapper = new CraftingMaterialWrapper(materialSO);
        universalInfo.craftingMaterialWrapperList.Add(craftingMaterialWrapper);
        cameraUIScript.UpdateMaterialInventory(craftingMaterialWrapper, true);
    }
    //called when adding a blueprint in deck via merchant buy
    public void AddBoughtBlueprint(BluePrintSO blueprintSO)
    {
        AllGearTypes blueprint = blueprintSO.blueprint;
        universalInfo.bluePrints.Add(blueprintSO.blueprint);
        cameraUIScript.UpdateBlueprintInventory(blueprint, true);
    }
    //called when removing a card
    //will only be called once since card removal is limited to 1 use per merchant visit
    //unlike the other options, it will save the current merchantSavState immediately after being called
    public void RemoveCardFromDeck(Card card)
    {
        //update the cardList saved in cameraScriptUI, does not edit the deck from save file
        cameraUIScript.UpdateCurrentDeck(card, false);

        //fetch the deck then convert it to a new CJW list to save to universalInfo
        List<Card> tempDeck = cameraUIScript.FetchDeck();
        List<CardAndJigsaWrapper> tempCJWList = new List<CardAndJigsaWrapper>();
        foreach (Card tempCard in tempDeck)
        {
            CardAndJigsaWrapper CJW = new CardAndJigsaWrapper(tempCard);
            tempCJWList.Add(CJW);
        }
        universalInfo.currentDeckWithJigsaw = tempCJWList;        

        //disables the rmove card button after removal
        cardRemovalButton.interactable = false;
        merchantSaveState.isCardRemovalAvailable = false;
        //save the deck in universalUI and update the universalUI
        UpdateMerchantSaveState(merchantSaveState);
        cameraUIScript.UpdateUniversalInfo();

        //also calls the back button of the CardRemovalScript to close the window after removing card
        cardRemovalScript.CardOptionBackButton();


    }

    //function called by items to be bought to check if there is enough scraps
    public bool CheckScraps(int scrapsCost)
    {
        if (remainingScraps >= scrapsCost)
        {
            //subtract cost from remaining value
            remainingScraps -= scrapsCost;
            //update text in universalUI
            cameraUIScript.UpdateUIObjectsActiveScraps(remainingScraps);
            universalInfo.scraps = remainingScraps;
            return true;
        }
        //if remainingScraps not enough, return false
        else
        {
            return false;
        }

    }

    //called by the OptionUIs, this triggers the save and update of the MerchantSaveState file
    //caled when exiting the Options view
    public void UpdateMerchantSaveState(MerchantSaveState merchantSaveState)
    {
        UniversalSaveState.SaveMerchant(merchantSaveState);
        //save the current universalInfo as well
        //CURRENTLY TURNED OFF FOR TESTING
        UniversalSaveState.SaveUniversalInformation(universalInfo);
    }

    //Function to leave MerchantManager
    public void GoBackToOverworldButton()
    {
        UniversalSaveState.SaveUniversalInformation(universalInfo);
        File.Delete(Application.persistentDataPath + "/Merchant.json");
        SceneManager.LoadScene("OverWorldScene");        
    }

}
