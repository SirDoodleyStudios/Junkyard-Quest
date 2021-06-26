using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionManager : MonoBehaviour
{
    //event for calling all playerObjects when a playerObject is chosen
    public delegate void D_PlayerChosenEvent(int i);
    public event D_PlayerChosenEvent d_PlayerChosenEvent;
    //event for clling ll roleObjects when a roleObject is chosen
    public delegate void D_ClassChosenEvent(int i);
    public event D_ClassChosenEvent d_ClassChosenEvent;

    //object holders
    public GameObject playerPanel;
    public GameObject rolePanel;

    //for storing the initial decks, and player stats
    //public GameObject objectDirectory;
    public InitialObjectDirectory directory;

    //identifier checked by the individual choice objects
    //this is set as true when a choice has been made
    public bool isPlayerChosen { get; set; }
    public bool isRoleChosen { get; set; }

    //indicator for crosschecking of chosen and clicked objects
    int tempInt;

    //Stores the chosen playables
    ChosenPlayer chosenPlayer;
    ChosenClass chosenClass;

    //for object clicking
    Vector2 PointRay;
    Ray ray;
    RaycastHit2D pointedObject;

    private void Start()
    {
        //directory = objectDirectory.GetComponent<InitialObjectDirectory>();
    }

    // Update is called once per frame
    void Update()
    {
        PointRay = Input.mousePosition;
        ray = Camera.main.ScreenPointToRay(PointRay);
        pointedObject = Physics2D.GetRayIntersection(ray);

        if (Input.GetMouseButtonDown(0))
        {
            if (pointedObject.collider != null)
            {
                GameObject chosenObject = pointedObject.collider.gameObject;
                tempInt = chosenObject.transform.GetSiblingIndex();


                if (chosenObject.tag == "Player Choice")
                {
                    //the index of the clicked object matched the index IDs in the enum list
                    chosenPlayer = (ChosenPlayer)tempInt;
                }
                else if (chosenObject.tag == "Class Choice")
                {
                    //the index of the clicked object matched the index IDs in the enum list
                    chosenClass = (ChosenClass)tempInt;
                }
            }
        }

    }
    //called when an object is clicked so that continuity doesnt get messed up
    public void UnchoosePlayer()
    {
        //the int parameter is for crosschecking with the object itself if it is the chosen object, if so, the selection will not change
        d_PlayerChosenEvent(tempInt);
    }

    public void UnchooseClass()
    {
        d_ClassChosenEvent(tempInt);
    }

    //loads the scene when play button is clicked
    public void SwitchToOverworld()
    {
        //for determining the initial deck and stats to be saved first
        UniversalInformation universalInformation = new UniversalInformation();
        List<Card> startingDeck = new List<Card>();
        List<AllCards> cardKeys = new List<AllCards>();
        List<CardAndJigsaWrapper> cardAndJigsawList = new List<CardAndJigsaWrapper>();

        //assigns chosen class and chosen player
        universalInformation.chosenPlayer = chosenPlayer;
        universalInformation.chosenClass = chosenClass;

        //instance will contain the player Unit template of choice
        //assigns the chosen playerstats to the universalInformation instance
        //PlayerUnit playerStats = ScriptableObject.CreateInstance<PlayerUnit>();
        PlayerUnit playerStats;

        //switch cases that will determine the basic decks chosen based on the chosen player and class
        switch (chosenPlayer)
        {
            case ChosenPlayer.Arlen:
                startingDeck.AddRange(directory.arlenBasic);
                playerStats = Instantiate(directory.arlenUnit);
                universalInformation.playerStats = playerStats;
                break;
            case ChosenPlayer.Cedric:
                startingDeck.AddRange(directory.cedricBasic);
                playerStats = Instantiate(directory.cedricUnit);
                universalInformation.playerStats = playerStats;
                break;
            case ChosenPlayer.Francine:
                startingDeck.AddRange(directory.francineBasic);
                playerStats = Instantiate(directory.francineUnit);
                universalInformation.playerStats = playerStats;
                break;
            case ChosenPlayer.Tilly:
                startingDeck.AddRange(directory.tillyBasic);
                playerStats = Instantiate(directory.tillyUnit);
                universalInformation.playerStats = playerStats;
                break;
            case ChosenPlayer.Princess:
                startingDeck.AddRange(directory.princessBasic);
                playerStats = Instantiate(directory.princessUnit);
                universalInformation.playerStats = playerStats;
                break;
            default:
                break;
        }

        switch (chosenClass)
        {
            case ChosenClass.Warrior:
                startingDeck.AddRange(directory.warriorBasic);
                break;
            case ChosenClass.Rogue:
                startingDeck.AddRange(directory.rogueBasic);
                break;
            case ChosenClass.Mage:
                startingDeck.AddRange(directory.mageBasic);
                break;
            default:
                break;
        }



        //extract the Allcards enum key from the card pool
        foreach (Card card in startingDeck)
        {
            //cardKeys.Add(card.enumCardName);
            CardAndJigsaWrapper CJW = new CardAndJigsaWrapper(card);
            cardAndJigsawList.Add(CJW);
        }
        //universalInformation.currentDeck = cardKeys;
        universalInformation.currentDeckWithJigsaw = cardAndJigsawList;

        CardSOFactory.InitializeCardSOFactory(chosenPlayer, chosenClass);
        //saves the universal info and create a json file for loading later
        //sets initial scraps values, the currentForgeCost dictates the first price of forging
        universalInformation.scraps = 300;
        universalInformation.currentForgeCost = 50;
        UniversalSaveState.SaveUniversalInformation(universalInformation);

        SceneManager.LoadScene("OverworldScene");
    }

    
}
