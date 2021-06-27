﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ForgeManager : MonoBehaviour
{
    //state that determines if the deck viewing is for main card or augment card
    private enum deckViewingPurpose {MainDeckView, AugmentDeckView }
    deckViewingPurpose deckViewState;
    //card prefabs in the card slots that will be activated will be displayed when choosing a card
    //set in editor
    public GameObject mainCardPrefab;
    public GameObject augmentCardPrefab;
    public GameObject forgedCardPrefab;
    JigsawFormat forgedCardJigsaw;
    public Button forgedCardButton;
    public Display mainCardDisplay;
    public Display augmentCardDisplay;
    public Display forgedCardDisplay;

    //holds the hidden panel for rerolling of jigsaw links
    //set in editor
    public GameObject alterJigsawLinksPanel;
    public JigsawLinkAlterer alterJigsawLinksScript;

    //the universal UI panel above
    //assigned in editor
    public CameraUIScript cameraUIScript;

    //contains the reference JigsawFormat when generating one from scratch
    public JigsawFormat referenceJigsawFormat;

    //indicator if the player is now choosing a card from deck when mainCard or augmentCard button is clicked
    bool isChoosingInDeck;

    public UniversalInformation universalInfo;
    public List<Card> currentDeck = new List<Card>();

    //current scraps requirement for forging, it will increase consistently after a successful forge
    int scrapsRequirement;
    //text object for the scraps requirement value, assigned in editor
    public TextMeshProUGUI scrapsText;
    bool isInitialLoad;

    //for mouse pointing and clicking
    Vector2 PointRay;
    Ray ray;
    RaycastHit2D pointedObject;

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
    void Start()
    {
        //TESTING
        //the script for loading the deck is in CameraScriptUI
        ////Might migrate to InitializeForge
        //foreach (CardAndJigsaWrapper CJW in universalInfo.currentDeckWithJigsaw)
        //{
        //    //generate the card and the jigsaw if it has one
        //    Card tempCard = Instantiate(CardSOFactory.GetCardSO(CJW.cardEnum));
        //    if (CJW.jigsawEnum != AllCards.Jigsaw)
        //    {
        //        JigsawFormat tempJigsawFormat = Instantiate(referenceJigsawFormat);
        //        tempCard.jigsawEffect = UniversalFunctions.LoadJigsawFormat(tempJigsawFormat, CJW);
        //    }
        //    currentDeck.Add(CardSOFactory.GetCardSO(CJW.cardEnum));
        //}

        scrapsRequirement = universalInfo.currentForgeCost;
        scrapsText.text = $"{scrapsRequirement}";
        if (universalInfo.scraps < scrapsRequirement)
        {
            scrapsText.color = new Color(1, 0, 0, 1);
        }


    }

    //function that starts, changes depend if loaded from file or not
    //MIGHT NOT BE NEEDED, the only value saved here is the current scraps cost which is now saved in universalInfo, migrated back to Start
    void InitializeForge()
    {
        //initial scrapsRequirement is 50
        scrapsRequirement = universalInfo.currentForgeCost;
        scrapsText.text = $"{scrapsRequirement}";
        if (universalInfo.scraps < scrapsRequirement)
        {
            scrapsText.color = new Color(1, 0, 0, 1);
        }
    }

    //Main methods activated by Buttons
    //buttons set in editor

    public void MainCardButton()
    {
        //fetches the current deck that the player has
        currentDeck = cameraUIScript.FetchDeck();
        //return the chosen card back to deck before reassigning
        if (mainCardPrefab.activeSelf)
        {
            cameraUIScript.UpdateCurrentDeck(mainCardDisplay.card, true);
        }

        //cameraUIScript.UpdateCurrentDeck(mainCardPrefab.GetComponent<Display>().card, true);
        //make the prefab holder to be automatically false when choosing a card
        mainCardPrefab.SetActive(false);
        //if player chooses to go back, the main slot will be forced to be blank so turn the forgedCardPrefab to disabled too
        forgedCardPrefab.SetActive(false);
        //make the forge card button disabled immediately, 
        //if the player chooses a card instead of going back, the logic in Update will set it as interactable if both card slots are filled up
        AlterForgeButtonInteractability();

        //will contain temporary filtered deck of cards
        List<Card> filteredDeck = new List<Card>();
        //checks first if there is a card chosen in augment
        //if yes, show all cards except for abilities
        if (augmentCardPrefab.activeSelf)
        {
            //check if augment is ability or not
            if (augmentCardDisplay.card.cardType == CardType.Ability)
            {
                //if augment is an ability, only show cards with jigsaw
                foreach(Card card in currentDeck)
                {
                    if (card.cardType != CardType.Ability && card.jigsawEffect != null)
                    {
                        filteredDeck.Add(card);
                    }
                }
            }
            else
            {
                //if augment is offence or utility, only show jigsawless cards
                foreach (Card card in currentDeck)
                {
                    if (card.cardType != CardType.Ability && card.jigsawEffect == null)
                    {
                        filteredDeck.Add(card);
                    }
                }
            }
        }
        //if augment card is chosen first, aonly show available main cards based on the chosen augment
        //if augment is an ability, show cards with jigsaw, if not, show offence or utility jigsawless cards
        else
        {
            //filter out the cards then view it in deck using the alternate deck viewing function
            foreach (Card card in currentDeck)
            {
                //show all non-ability cards
                if (card.cardType != CardType.Ability)
                {
                    filteredDeck.Add(card);
                }
            }
        }

        cameraUIScript.ViewFilteredDeck(filteredDeck);
        deckViewState = deckViewingPurpose.MainDeckView;
        isChoosingInDeck = true;
    }

    public void AugmentCardButton()
    {
        //fetches the current deck that the player has
        currentDeck = cameraUIScript.FetchDeck();
        //return the chosen card back to deck before reassigning
        if (augmentCardPrefab.activeSelf)
        {
            cameraUIScript.UpdateCurrentDeck(augmentCardDisplay.card, true);
        }

        //cameraUIScript.UpdateCurrentDeck(augmentCardPrefab.GetComponent<Display>().card, true);
        //make the prefab holder to be automatically false when choosing a card
        augmentCardPrefab.SetActive(false);
        //if player chooses to go back, the augment slot will be forced to be blank so turn the forgedCardPrefab to disabled too
        forgedCardPrefab.SetActive(false);
        //make the forge card button disabled immediately, 
        //if the player chooses a card instead of going back, the logic in Update will set it as interactable if both card slots are filled up
        AlterForgeButtonInteractability();

        List<Card> filteredDeck = new List<Card>();
        //filter out the cards then view it in deck using the alternate deck viewing function
        //check if there is already a chosen card, if yes, base the choice of cards that you can choose as augment from the main card
        if (mainCardPrefab.activeSelf)
        {
            //if main card doesnt have a jigsaw, allow offence and utility augment choices only
            if (mainCardDisplay.card.jigsawEffect == null)
            {
                foreach (Card card in currentDeck)
                {
                    if (card.jigsawEffect == null && (card.cardType == CardType.Offense || card.cardType == CardType.Utility))
                    {
                        filteredDeck.Add(card);
                    }
                }
            }
            //if mainCard has a jigsaw, allow ability augments
            else
            {
                foreach (Card card in currentDeck)
                {
                    if (card.jigsawEffect == null &&  card.cardType == CardType.Ability)
                    {
                        filteredDeck.Add(card);
                    }
                }
            }
        }
        //if no card is chosen for mainCard, show all cards without jigsaw
        else
        {
            foreach (Card card in currentDeck)
            {
                // augment cards must be jigsawless
                if (card.jigsawEffect == null)
                {
                    filteredDeck.Add(card);
                }
            }
        }



        cameraUIScript.ViewFilteredDeck(filteredDeck);
        deckViewState = deckViewingPurpose.AugmentDeckView;
        isChoosingInDeck = true;


    }

    //method that shows the aboutto be forged card before forging
    void ShowToBeForgedCard()
    {
        forgedCardJigsaw = Instantiate(referenceJigsawFormat);
        //in the forge card preview, show links as unknown first
        forgedCardJigsaw.inputLink = JigsawLink.Unknown;
        forgedCardJigsaw.outputLink = JigsawLink.Unknown;
        forgedCardJigsaw.jigsawImage = Resources.Load<Sprite>($"Jigsaw/{forgedCardJigsaw.inputLink}2{forgedCardJigsaw.outputLink}");
        forgedCardJigsaw.enumJigsawCard = augmentCardDisplay.card.enumCardName;
        forgedCardJigsaw.jigsawMethod = augmentCardDisplay.card.cardMethod;
        forgedCardJigsaw.jigsawCard = augmentCardDisplay.card;
        forgedCardJigsaw.jigsawDescription = CardTagManager.GetCardEffectDescriptions(augmentCardDisplay.card);

        forgedCardDisplay.card = Instantiate(mainCardDisplay.card);
        forgedCardDisplay.card.jigsawEffect = forgedCardJigsaw;
        forgedCardPrefab.SetActive(true);

        //set the forged button to be interactable
        AlterForgeButtonInteractability();
    }

    //method called to update iteractability of the forge button
    void AlterForgeButtonInteractability()
    {
        //of both main and augment slots are enabled and the scraps requirement are met
        if (mainCardPrefab.activeSelf && augmentCardPrefab.activeSelf && scrapsRequirement <= universalInfo.scraps)
        {
            forgedCardButton.interactable = true;
        }
        else
        {
            forgedCardButton.interactable = false;
        }
    }


    public void ForgeCardButton()
    {
        //true randomization of links
        forgedCardJigsaw.inputLink = (JigsawLink)Random.Range(0, 3);
        forgedCardJigsaw.outputLink = (JigsawLink)Random.Range(0, 3);
        forgedCardJigsaw.jigsawImage = Resources.Load<Sprite>($"Jigsaw/{forgedCardJigsaw.inputLink}2{forgedCardJigsaw.outputLink}");

        //disble the prefabs to set them as empty cards
        mainCardPrefab.SetActive(false);
        augmentCardPrefab.SetActive(false);
        forgedCardPrefab.SetActive(false);
        AlterForgeButtonInteractability();


        //call the jigsaw alterer if the augmenter is an ability
        //after the alter, the deck update is called in the alterer function
        if (augmentCardDisplay.card.cardType == CardType.Ability)
        {
            alterJigsawLinksPanel.SetActive(true);
            //remove the chosen card from deck first because the altered one will be added by the alterer deck later
            cameraUIScript.UpdateCurrentDeck(mainCardDisplay.card, false);
            alterJigsawLinksScript.InitialAlterState(mainCardDisplay.card);
        }
        //if not an ability, proceed immediately to deck updtes
        else
        {
            //create the forged card then remove and disable the cards from the prefabs to make them not have access to the readding of card in currentDeck
            mainCardDisplay.card.jigsawEffect = forgedCardJigsaw;
            //the deck updating function in cameraUIScript, also returns the updated deck for use
            cameraUIScript.UpdateCurrentDeck(mainCardDisplay.card, true);
        }

        //increase scraps reqirement after successful forge
        if (universalInfo.scraps >= scrapsRequirement)
        {
            UpdateScrapsValues();
        }

        SaveForgeChanges();
        

    }
    //return main and augment cards to blank
    public void ResetCardsButton()
    {
        //unchoose all chosen cards
        mainCardPrefab.SetActive(false);
        augmentCardPrefab.SetActive(false);
        forgedCardPrefab.SetActive(false);
        //return the cards to deck
        cameraUIScript.UpdateCurrentDeck(mainCardDisplay.card, true);
        cameraUIScript.UpdateCurrentDeck(augmentCardDisplay.card, true);
        //make Forge button uninteractable
        AlterForgeButtonInteractability();
    }

    //function that alters scraps requirement and update the scraps text\
    void UpdateScrapsValues()
    {

        int tempScraps = universalInfo.scraps - scrapsRequirement;
        universalInfo.scraps = tempScraps;

        //updates the scraps value in header UI
        cameraUIScript.AssignUIObjects(universalInfo);

        //consistently increase scraps requirement everytime it's called
        scrapsRequirement += 10;
        universalInfo.currentForgeCost = scrapsRequirement;

        //if the scraps requirement now exceeds the player's scraps, turn the cost value to red
        if (tempScraps < scrapsRequirement)
        {
            scrapsText.color = new Color(1, 0, 0, 1);
            scrapsText.text = $"{scrapsRequirement}";
        }
        else
        {
            scrapsText.text = $"{scrapsRequirement}";
        }

        UniversalSaveState.SaveUniversalInformation(universalInfo);
    }

    //called for saving
    void SaveForgeChanges()
    {
        //convert cards to CardAndJigsawWrapper for saving
        List<CardAndJigsaWrapper> forgedDeck = new List<CardAndJigsaWrapper>();
        foreach (Card card in currentDeck)
        {
            CardAndJigsaWrapper CJW = new CardAndJigsaWrapper(card);
            forgedDeck.Add(CJW);
        }
        universalInfo.currentDeckWithJigsaw = forgedDeck;
        UniversalSaveState.SaveUniversalInformation(universalInfo);
    }

    //return to overWorld
    //assigned in editor
    public void LeaveButton()
    {
        SceneManager.LoadScene("OverWorldScene");
    }

    private void Update()
    {
        //update is for choosing a card in the deck view and will only bew accessible if the identifier is true
        if (isChoosingInDeck)
        {
            PointRay = Input.mousePosition;
            ray = Camera.main.ScreenPointToRay(PointRay);
            pointedObject = Physics2D.GetRayIntersection(ray);

            //if the clicked object is a card and has a collider
            if (Input.GetMouseButtonDown(0) )
            {

                if (pointedObject.collider != null)
                {
                    GameObject chosenCardPrefab = pointedObject.collider.gameObject;
                    //checks what viewing state are we in first
                    if (deckViewState == deckViewingPurpose.MainDeckView)
                    {
                        //assigns the card 
                        Card chosenCard = chosenCardPrefab.GetComponent<Display>().card;
                        mainCardPrefab.GetComponent<Display>().card = chosenCard;
                        mainCardPrefab.SetActive(true);
                        isChoosingInDeck = false;

                        //temporarily remove chosen card from the deck then update the deck in CameraUIScript
                        cameraUIScript.UpdateCurrentDeck(chosenCard, false);
                        cameraUIScript.UnviewSavedDeck();

                        //calls the resultForgeCardShower function
                        if (mainCardPrefab.activeSelf&&augmentCardPrefab.activeSelf)
                        {
                            ShowToBeForgedCard();
                        }
                    }
                    else if(deckViewState == deckViewingPurpose.AugmentDeckView)
                    {
                        //assigns the card 
                        Card chosenCard = pointedObject.collider.gameObject.GetComponent<Display>().card;
                        augmentCardPrefab.GetComponent<Display>().card = chosenCard;
                        augmentCardPrefab.SetActive(true);
                        isChoosingInDeck = false;

                        //temporarily remove chosen card from the deck
                        cameraUIScript.UpdateCurrentDeck(chosenCard, false);
                        cameraUIScript.UnviewSavedDeck();

                        if (mainCardPrefab.activeSelf && augmentCardPrefab.activeSelf)
                        {
                            ShowToBeForgedCard();
                        }
                    }

                    //this will disable the Card Tag Popups of the chosen card before unviewing the deck
                    chosenCardPrefab.GetComponent<CardDescriptionLayout>().DisablePopups();
                }

            }
        }
    }


}
