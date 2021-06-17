using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    public Display mainCardDisplay;
    public Display augmentCardDisplay;
    public Display forgedCardDisplay;

    //the universal UI panel above
    //assigned in editor
    public CameraUIScript cameraUIScript;

    //contains the reference JigsawFormat when generating one from scratch
    public JigsawFormat referenceJigsawFormat;

    //indicator if the player is now choosing a card from deck when mainCard or augmentCard button is clicked
    bool isChoosingInDeck;

    public UniversalInformation universalInfo;
    public List<Card> currentDeck = new List<Card>();

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
        cameraUIScript.AssignUIObjects(universalInfo);

    }
    void Start()
    {

        foreach (CardAndJigsaWrapper CJW in universalInfo.currentDeckWithJigsaw)
        {
            //generate the card and the jigsaw if it has one
            Card tempCard = Instantiate(CardSOFactory.GetCardSO(CJW.cardEnum));
            if (CJW.jigsawEnum != AllCards.Jigsaw)
            {
                JigsawFormat tempJigsawFormat = new JigsawFormat();
                tempCard.jigsawEffect = UniversalFunctions.LoadJigsawFormat(tempJigsawFormat, CJW);
            }
            currentDeck.Add(CardSOFactory.GetCardSO(CJW.cardEnum));
        }
    }

    //Main methods activated by Buttons
    //buttons set in editor

    public void MainCardButton()
    {
        //return the chosen card back to deck before reassigning
        if (mainCardPrefab.activeSelf)
        {
            cameraUIScript.UpdateCurrentDeck(mainCardDisplay.card, true);
        }

        //cameraUIScript.UpdateCurrentDeck(mainCardPrefab.GetComponent<Display>().card, true);
        //make the prefab holder to be automatically false when choosing a card
        mainCardPrefab.SetActive(false);
        cameraUIScript.ViewSavedDeck();
        deckViewState = deckViewingPurpose.MainDeckView;
        isChoosingInDeck = true;
    }

    public void AugmentCardButton()
    {
        //return the chosen card back to deck before reassigning
        if (augmentCardPrefab.activeSelf)
        {
            cameraUIScript.UpdateCurrentDeck(augmentCardDisplay.card, true);
        }

        //cameraUIScript.UpdateCurrentDeck(augmentCardPrefab.GetComponent<Display>().card, true);
        //make the prefab holder to be automatically false when choosing a card
        augmentCardPrefab.SetActive(false);
        cameraUIScript.ViewSavedDeck();
        deckViewState = deckViewingPurpose.AugmentDeckView;
        isChoosingInDeck = true;
    }

    public void ForgeCardButton()
    {
        JigsawFormat instantiatedJigsaw = Instantiate(referenceJigsawFormat);
        instantiatedJigsaw.inputLink = (JigsawLink)Random.Range(0,3);
        instantiatedJigsaw.outputLink = (JigsawLink)Random.Range(0,3);
        instantiatedJigsaw.enumJigsawCard = augmentCardDisplay.card.enumCardName;
        instantiatedJigsaw.jigsawMethod = augmentCardDisplay.card.cardMethod;
        instantiatedJigsaw.jigsawCard = augmentCardDisplay.card;
        instantiatedJigsaw.jigsawDescription = CardTagManager.GetCardEffectDescriptions(augmentCardDisplay.card);
        instantiatedJigsaw.jigsawImage = DetermineJigsawImage(instantiatedJigsaw.inputLink, instantiatedJigsaw.outputLink);

        //create the forged card then remove and disable the cards from the prefabs to make them not have access to the readding of card in currentDeck
        mainCardDisplay.card.jigsawEffect = instantiatedJigsaw;
        cameraUIScript.UpdateCurrentDeck(mainCardDisplay.card, true);
        mainCardPrefab.SetActive(false);
        augmentCardPrefab.SetActive(false);


    }
    //THIS IS A TEST METHOD
    //THINKING OF JUST NAMING THE JIGSAWIMAGES AS CircleToSquare SO THAT WE CAN JUST USE THE RESOURCES.LOAD USING THE JIGSAWLINK ENUM NAMES
    Sprite DetermineJigsawImage(JigsawLink input, JigsawLink output)
    {
        Sprite jigsawSprite;

        //circle starting
        if (input == JigsawLink.Circle)
        {
            if (output == JigsawLink.Circle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/C2C");

            else if (output == JigsawLink.Square)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/C2S");

            else if (output == JigsawLink.Triangle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/C2T");
            else
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/Blank");

            return jigsawSprite;

        }
        else if (input == JigsawLink.Square)
        {
            if (output == JigsawLink.Circle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/S2C");

            else if (output == JigsawLink.Square)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/S2S");

            else if (output == JigsawLink.Triangle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/S2T");
            jigsawSprite = Resources.Load<Sprite>("Jigsaw/Blank");

            return jigsawSprite;

        }
        else if (input == JigsawLink.Triangle)
        {
            if (output == JigsawLink.Circle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/T2C");

            else if (output == JigsawLink.Square)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/T2S");

            else if (output == JigsawLink.Triangle)
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/T2T");
            else
                jigsawSprite = Resources.Load<Sprite>("Jigsaw/Blank");

            return jigsawSprite;

        }
        else
            jigsawSprite = Resources.Load<Sprite>("Jigsaw/Blank");
        return jigsawSprite;
    }



    public void EnableCardChoice(GameObject cardPrefab, Card cardChoice)
    {
        
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

                    //checks what viewing state are we in first
                    if (deckViewState == deckViewingPurpose.MainDeckView)
                    {
                        //assigns the card 
                        Card chosenCard = pointedObject.collider.gameObject.GetComponent<Display>().card;
                        mainCardPrefab.GetComponent<Display>().card = chosenCard;
                        mainCardPrefab.SetActive(true);
                        isChoosingInDeck = false;

                        //temporarily remove chosen card from the deck then update the deck in CameraUIScript
                        cameraUIScript.UpdateCurrentDeck(chosenCard, false);
                        cameraUIScript.UnviewSavedDeck();
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
                    }
                }

            }
        }
    }


}
