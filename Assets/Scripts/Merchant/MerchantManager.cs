using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantManager : MonoBehaviour
{
    //loaded unviversalInfo
    UniversalInformation universalInfo;
    // the universal UI
    //Assigned in editor
    public CameraUIScript cameraUIScript;

    //UI Objects for options
    //assigned in editor
    public GameObject cardOptionsUI;
    public GameObject materialOptionsUI;
    public GameObject blueprintOptionsUI;
    public GameObject cardRemovalUI;
    //identifiers for the UIs if they have already been initiated
    bool isCardOptionsInitialized;
    bool isMaterialOptionsInitialized;
    bool isBlueprintOptionsInitialized;

    private void Awake()
    {
        universalInfo = UniversalSaveState.LoadUniversalInformation();
        //will initialize the factory for getting cards using allcard enum
        CardSOFactory.InitializeCardSOFactory(universalInfo.chosenPlayer, universalInfo.chosenClass);
        //initializes the deck viewr
        CardTagManager.InitializeTextDescriptionDictionaries();
        cameraUIScript.GenerateDeck(universalInfo);
        cameraUIScript.AssignUIObjects(universalInfo);
    }

    //for accessing the options UI
    // button functions

    public void CardOptionsButton()
    {
        cardOptionsUI.SetActive(true);

        CardOptions cardOptionsScript = cardOptionsUI.GetComponent<CardOptions>();

        //if the UI is not yet initialized, initialze it then turn the identifier to true to ensure that it is called only one time
        if (isCardOptionsInitialized == false)
        {
            cardOptionsScript.InitiateCardOptions(universalInfo);
            isCardOptionsInitialized = true;
        }
        //calls the actual command to view available cards
        cardOptionsScript.ViewCardOptions();

    }

}
