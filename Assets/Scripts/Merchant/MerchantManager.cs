using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class MerchantManager : MonoBehaviour
{
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

    //identifiers for the UIs if they have already been initiated
    bool isCardOptionsInitialized;
    bool isMaterialOptionsInitialized;
    bool isBlueprintOptionsInitialized;

    //list of available merchant Items
    List<Card> cardList = new List<Card>();

    //int value for remaining scraps of player
    int remainingScraps;

    private void Awake()
    {

        universalInfo = UniversalSaveState.LoadUniversalInformation();
        //will initialize the factory for getting cards using allcard enum
        CardSOFactory.InitializeCardSOFactory(universalInfo.chosenPlayer, universalInfo.chosenClass);
        //initializes the deck viewr
        CardTagManager.InitializeTextDescriptionDictionaries();
        cameraUIScript.GenerateDeck(universalInfo);
        cameraUIScript.AssignUIObjects(universalInfo);
        //available value of scraps taken from cameraScriptUI and universalInfo
        remainingScraps = universalInfo.scraps;

        //script assignments for the Option scripts
        cardOptionsScript = cardOptionsUI.GetComponent<CardOptions>();

        //if there is no MerchantSave, it means that the merchant scene was loaded from current game session
        if (File.Exists(Application.persistentDataPath + "/Merchant.json"))
        {
            merchantSaveState = UniversalSaveState.LoadMerchant();
        }
        else
        {
            merchantSaveState = new MerchantSaveState();
        }
        //initiate the output settings
        InitiateOptionUIs();
    }

    //for initiating all option UIs
    void InitiateOptionUIs()
    {
        //For Card Optios initiation
        CardOptions cardOptionsScript = cardOptionsUI.GetComponent<CardOptions>();
        //if the UI is not yet initialized, initialze it then turn the identifier to true to ensure that it is called only one time
        if (isCardOptionsInitialized == false)
        {
            //initiate cardOptions fresh in current game session
            //the merchantSaveState is empty here and won't be used since the false parameter is for initiating 
            cardOptionsScript.InitiateCardOptions(universalInfo, merchantSaveState, false);
            isCardOptionsInitialized = true;
        }
    }

    //for accessing the options UI
    // button functions

    public void CardOptionsButton()
    {
        cardOptionsUI.SetActive(true);
        //calls the actual command to view available cards
        cardOptionsScript.ViewCardOptions(merchantSaveState);

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
    public void UpdateMerchantSaveState(MerchantSaveState merchantSaveState)
    {
        UniversalSaveState.SaveMerchant(merchantSaveState);
    }

    //Function to leave MerchantManager
    public void GoBackToOverworldButton()
    {
        UniversalSaveState.SaveUniversalInformation(universalInfo);
        File.Delete(Application.persistentDataPath + "/Merchant.json");
        SceneManager.LoadScene("OverWorldScene");        
    }

}
