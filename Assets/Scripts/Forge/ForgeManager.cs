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

    //the universal UI panel above
    //assigned in editor
    public CameraUIScript cameraUIScript;

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
        //make the prefab holder to be automatically false when choosing a card
        mainCardPrefab.SetActive(false);
        cameraUIScript.ViewSavedDeck();
        deckViewState = deckViewingPurpose.MainDeckView;
        isChoosingInDeck = true;
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
            if (Input.GetMouseButtonDown(0) && pointedObject.collider != null)
            {
                //checks what viewing state are we in first
                if(deckViewState == deckViewingPurpose.MainDeckView)
                {
                    //assigns the card 
                    Card chosenCard = pointedObject.collider.gameObject.GetComponent<Display>().card;
                    mainCardPrefab.GetComponent<Display>().card = chosenCard;
                    mainCardPrefab.SetActive(true);
                    isChoosingInDeck = false;
                }
            }
        }
    }


}
