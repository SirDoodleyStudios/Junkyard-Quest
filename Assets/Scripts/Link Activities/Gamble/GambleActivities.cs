using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GambleActivities : MonoBehaviour
{
    //assigned in editor
    public GameObject cardShowcasePanel;
    public CameraUIScript cameraUIScript;
    //gameObjects for the cards
    public GameObject baseCardObj;
    public GameObject firstDrawObj;
    public GameObject secondDrawObj;
    public GameObject thirdDrawObj;
    //texts to be assigned with messages
    public TextMeshProUGUI instructionsText;
    public TextMeshProUGUI messagesText;
    //the takeGamble Button that must be disabled once the player got it wrong
    public Button takeGambleButton;
    //holds te referecenceJigsawFormatSO, used for instantiating cards
    public JigsawFormat referenceJigsawFormat;

    //holds the linkActivitySaveState passed by the linkActivityManager
    LinkActivitiesSaveState linkActivitiesSaveState;

    //type of gamble link activity
    LinkActivityEnum gambleActivityType;

    //list that will hold the current deck
    List<Card> currentDeck = new List<Card>();

    //index that will determine what draw the player is currently in
    int drawIndex;

    //holds the current card that serves as basis for the gamble
    Card baseCard;
    //hold the next draw
    Card firstDraw;
    Card secondDraw;
    Card thirdDraw;

    //will hold the card to be comapred to, used by the methods here to properly decide if the current card matches the previous card
    Card cardForComparison;

    private void Awake()
    {
        //draw index will start at 2 because that is the position of the first draw
        drawIndex = 2;
        //sets the buton as active
        takeGambleButton.interactable = true;
    }

    //initializing function
    public LinkActivitiesSaveState InitializeGambleActivity(LinkActivityEnum gambleType, LinkActivitiesSaveState linkSaveState)
    {
        //assign the gamble type
        gambleActivityType = gambleType;

        //fetch the deck
        currentDeck = cameraUIScript.FetchDeck();

        //if loaded from file
        if (linkSaveState.isInActivity)
        {
            List<Card> savedGambleCards = new List<Card>();

            foreach (CardAndJigsaWrapper CJW in linkSaveState.gambleCards)
            {
                //calls the factory to instantiate a copy of the base card SO
                Card tempCard = Instantiate(CardSOFactory.GetCardSO(CJW.cardEnum));
                tempCard.effectText = CardTagManager.GetCardEffectDescriptions(tempCard);

                //This is a test Jigsaw Generator function
                if ((tempCard.cardType == CardType.Offense || tempCard.cardType == CardType.Utility) && CJW.jigsawEnum != AllCards.Jigsaw)
                {
                    //just for testing, instead of a reandom jigsaw, just install the same card as the jigsaw of the generated card;
                    //Card referenceJigsawCard = CardSOFactory.GetCardSO(CJW);
                    //JigsawFormat instantiatedJigsaw = Instantiate(testJigsawList[Random.Range(0, testJigsawList.Count)]);
                    JigsawFormat instantiatedJigsaw = Instantiate(referenceJigsawFormat);
                    tempCard.jigsawEffect = UniversalFunctions.LoadJigsawFormat(instantiatedJigsaw, CJW);
                    instantiatedJigsaw.jigsawCard = Instantiate(CardSOFactory.GetCardSO(instantiatedJigsaw.enumJigsawCard));

                    tempCard.jigsawEffect = instantiatedJigsaw;
                }
                savedGambleCards.Add(tempCard);
            }
            //assign the cards
            baseCard = savedGambleCards[0];
            firstDraw = savedGambleCards[1];
            secondDraw = savedGambleCards[2];
            thirdDraw = savedGambleCards[3];

        }
        else
        {
            //get random card
            baseCard = currentDeck[Random.Range(0, currentDeck.Count - 1)];

            //remove the chosen card so that it doesn't get drawn anymore
            currentDeck.Remove(baseCard);
            firstDraw = currentDeck[Random.Range(0, currentDeck.Count - 1)];
            currentDeck.Remove(firstDraw);
            secondDraw = currentDeck[Random.Range(0, currentDeck.Count - 1)];
            currentDeck.Remove(secondDraw);
            thirdDraw = currentDeck[Random.Range(0, currentDeck.Count - 1)];

        }
        //assign the cards on the card object already
        baseCardObj.GetComponent<Display>().card = baseCard;
        firstDrawObj.GetComponent<Display>().card = firstDraw;
        secondDrawObj.GetComponent<Display>().card = secondDraw;
        thirdDrawObj.GetComponent<Display>().card = thirdDraw;



        //reveal the base card
        baseCardObj.GetComponentInParent<Image>().enabled = false;
        baseCardObj.SetActive(true);

        //assign base card as the comparison card
        cardForComparison = baseCard;

        //Set Instructions text
        SetInstructions();

        //Save the cards determined and drawIndex currently
        linkSaveState.gambleCards.Add(new CardAndJigsaWrapper(baseCard));
        linkSaveState.gambleCards.Add(new CardAndJigsaWrapper(firstDraw));
        linkSaveState.gambleCards.Add(new CardAndJigsaWrapper(secondDraw));
        linkSaveState.gambleCards.Add(new CardAndJigsaWrapper(thirdDraw));
        return linkSaveState;
    }

    //method for setting up the instructions 
    void SetInstructions()
    {
        if (gambleActivityType == LinkActivityEnum.LinkGamble)
        {
            instructionsText.text = "Guess if the next card drawn from your deck can link with the one currently shown";
        }
        else if (gambleActivityType == LinkActivityEnum.TypeGamble)
        {
            instructionsText.text = "Guess if the next card drawn from your deck is the same type with the one currently shown";
        }
        else if (gambleActivityType == LinkActivityEnum.NameGamble)
        {
            instructionsText.text = "Guess if the next card drawn from your deck has the same initial letter with the one currently shown";
        }
        else
        {
            instructionsText.text = "This should not happen";
        }

    }

    //function to reveal the drawn card
    public void RevealCardButton()
    {
        //the card to be revelealed
        //dictated by the drawIndex
        //the card object itself is childed under a holder image that will serve card back before revealing the card
        //disable the image of the cardback and enable the whole card object
        GameObject showcaseCard = cardShowcasePanel.transform.GetChild(drawIndex).gameObject;
        Card drawnCard = showcaseCard.transform.GetChild(0).GetComponent<Display>().card;
        showcaseCard.GetComponent<Image>().enabled = false;
        showcaseCard.transform.GetChild(0).gameObject.SetActive(true);


        //check if gamble paid off
        if (CheckGambleResult(drawnCard))
        {
            //adjust base card for comparisson to be next
            cardForComparison = drawnCard;
            SetMessage(drawIndex);
        }
        else
        {
            //index 0 is for fail message
            SetMessage(0);
            takeGambleButton.interactable = false;
        }


        drawIndex++;

    }

    //function to check if the player won the gamble after revealing card
    bool CheckGambleResult(Card drawnCard)
    {
        //check the type of gamble first to know if the drawn card is correct
        if (gambleActivityType == LinkActivityEnum.LinkGamble)
        {
            if (cardForComparison.jigsawEffect.outputLink == drawnCard.jigsawEffect.inputLink)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (gambleActivityType == LinkActivityEnum.TypeGamble)
        {
            if (drawnCard.cardType == cardForComparison.cardType)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (gambleActivityType == LinkActivityEnum.NameGamble)
        {
            if (drawnCard.cardName[0] == cardForComparison.cardName[0])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //should not arrive here
        else
        {
            Debug.Log("should not be happening");
            return false;
        }
    }
    //determines the message to be shown after choosing to reveal
    //int paramter is to determine what message should be shown, will differ based on success and failure
    //int parameter is based on drawIndex, it's the the card currently opened's index
    void SetMessage(int messageVariant)
    {
        //at first success
        if(messageVariant == 2)
        {
            messagesText.text = "Awesome!, wanna guess again for double prizes?";
        }
        //at second success
        else if(messageVariant == 3)
        {
            messagesText.text = "Magnificent! wanna guess one last time for double double prizes??";
        }
        //at third success
        else if(messageVariant == 4)
        {
            messagesText.text = "Congrats! you are a Guess Master!";
        }
        //at failure
        else if (messageVariant == 0)
        {
            messagesText.text = "Too bad, you guessed wrong :(";
        }
    }
    //saving function called for every click of the gamble button and during initialize
    void SaveLinkActivityState()
    {
        UniversalSaveState.SaveLinkActivities(linkActivitiesSaveState);
    }


    //button for returning back to linkActivities if player decides to skip gamble
    public void ReturnToLinkActivitiesButton()
    {
        gameObject.SetActive(false);
    }




}
