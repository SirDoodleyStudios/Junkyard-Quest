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
    //texts for the cost and reward numbers
    public TextMeshProUGUI costText;
    public TextMeshProUGUI rewardText;
    //the takeGamble Button that must be disabled once the player got it wrong
    public Button takeGambleButton;
    //holds te referecenceJigsawFormatSO, used for instantiating cards
    public JigsawFormat referenceJigsawFormat;

    //holds the linkActivitySaveState passed by the linkActivityManager
    LinkActivitiesSaveState linkActivitiesSaveState;
    //reference to the LinkActivitiesManager
    LinkActivitiesManager linkActivitiesManager;

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
    public LinkActivitiesSaveState InitializeGambleActivity(LinkActivityEnum gambleType, LinkActivitiesSaveState linkSaveState, LinkActivitiesManager manager)
    {
        //assign the original manager to access the universalInfo in it for the ticketChanges
        linkActivitiesManager = manager;

        //assign the linkSaveState to local variable
        linkActivitiesSaveState = linkSaveState;

        //assign the gamble type
        gambleActivityType = gambleType;

        //fetch the deck
        currentDeck = cameraUIScript.FetchDeck();

        //if loaded from file
        if (linkSaveState.isStillInActivity)
        {
            //load the cost and rewards loaded from the linkSaveState file
            //int parameter is just nothing since the logic for this is an else
            SetCostAndRewards(9);

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
            //set the initial costs and rewards
            SetCostAndRewards(1);

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
        List<CardAndJigsaWrapper> tempHolder = new List<CardAndJigsaWrapper>();
        tempHolder.Add(new CardAndJigsaWrapper(baseCard));
        tempHolder.Add(new CardAndJigsaWrapper(firstDraw));
        tempHolder.Add(new CardAndJigsaWrapper(secondDraw));
        tempHolder.Add(new CardAndJigsaWrapper(thirdDraw));
        linkActivitiesSaveState.gambleCards = tempHolder;

        //linkSaveState.gambleCards.Add(new CardAndJigsaWrapper(baseCard));
        //linkSaveState.gambleCards.Add(new CardAndJigsaWrapper(firstDraw));
        //linkSaveState.gambleCards.Add(new CardAndJigsaWrapper(secondDraw));
        //linkSaveState.gambleCards.Add(new CardAndJigsaWrapper(thirdDraw));

        //if cards are loaded from file, iterate reveal Cards depending on the saved drawIndex
        //identify where the player is in the gambling draw
        //checks first if drawIndex has drawn the base card since it should always be activated
        if (linkSaveState.drawIndex > 0)
        {
            //holder for the original drawIndex in file so that it doesnt get overriden when revelaing cards at initialization
            int tempDrawIndex = linkSaveState.drawIndex;
            for (int i = 1; tempDrawIndex >= i; i++)
            {
                //the local draw index is the card object's heirarchy position in the editor
                //0 = card back, 1 = base draw. 2 = first, 3= second, 4 = third draw
                drawIndex = i + 1;
                RevealCardButton();
            }
        }
        //if drawIndex in file is 0 then player has not taken a gamble yet so message text should be blank
        else if(linkSaveState.drawIndex == 0)
        {
            messagesText.text = "";
        }




        return linkActivitiesSaveState;
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
            SetCostAndRewards(drawIndex);
        }
        else
        {
            //index 0 is for fail message
            SetCostAndRewards(0);
            takeGambleButton.interactable = false;
        }

        //checks the draw index if we're already on the final draw to disable the takeGambleOption
        //index 4 is the third draw
        if (drawIndex == 4)
        {
            takeGambleButton.interactable = false;
            //saves in the linkActivitySaveState the current position of the drawIndex
            //the minus 1 is because the drawIndex in the linkActivity file does not include the card back while the drawIndex is local includes the cradback because it's in the editor heirarchy
            linkActivitiesSaveState.drawIndex = drawIndex - 1;
            UniversalSaveState.SaveLinkActivities(linkActivitiesSaveState);
        }
        else
        {
            //saves in the linkActivitySaveState the current position of the drawIndex
            //the minus 1 is because the drawIndex in the linkActivity file does not include the card back while the drawIndex is local includes the cradback because it's in the editor heirarchy
            linkActivitiesSaveState.drawIndex = drawIndex - 1;
            UniversalSaveState.SaveLinkActivities(linkActivitiesSaveState);

            drawIndex++;
        }


    }

    //function to check if the player won the gamble after revealing card
    bool CheckGambleResult(Card drawnCard)
    {
        //check the type of gamble first to know if the drawn card is correct
        if (gambleActivityType == LinkActivityEnum.LinkGamble)
        {
            //the first condition before the && is just being used for test. If the gamble type is LikActivity, the logic should generate a pool of all the cards with links
            if (cardForComparison.jigsawEffect != null && cardForComparison.jigsawEffect.outputLink == drawnCard.jigsawEffect.inputLink)
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
    //determines the next values to be shown after choosing to reveal
    //int paramter is to determine what message should be shown, will differ based on success and failure
    //int parameter is based on drawIndex, it's the the card currently opened's index
    void SetCostAndRewards(int resultVariants)
    {
        int ticketCost;
        int ticketReward;

        //at first instance of gamble without any guesses done yet
        if (resultVariants == 1)
        {
            ticketCost = Random.Range(1, 3);
            ticketReward = Random.Range(1, 3);
        }
        //at first success
        else if (resultVariants == 2)
        {
            //deal with ticket changes first before updating new values and text
            linkActivitiesManager.GambleResults(linkActivitiesSaveState.ticketCost, linkActivitiesSaveState.ticketRewards, true);
            ticketCost = Random.Range(2, 4);
            ticketReward = Random.Range(2, 4);
            messagesText.text = "You won!, play again for more prizes";
        }
        //at second success
        else if (resultVariants == 3)
        {
            //deal with ticket changes first before updating new values and text
            linkActivitiesManager.GambleResults(linkActivitiesSaveState.ticketCost, linkActivitiesSaveState.ticketRewards, true);
            ticketCost = Random.Range(3, 5);
            ticketReward = Random.Range(3, 5);
            messagesText.text = "You won again!, play for even more prizes";
        }
        //happens at failure
        else if (resultVariants == 0)
        {
            //deal with ticket changes first before updating new values and text
            linkActivitiesManager.GambleResults(linkActivitiesSaveState.ticketCost, linkActivitiesSaveState.ticketRewards, false);
            ticketCost = 0;
            ticketReward = 0;
            messagesText.text = "You lost, thanks for playing!";
        }
        //happens at final success
        else if (resultVariants == 4)
        {
            //deal with ticket changes first before updating new values and text
            linkActivitiesManager.GambleResults(linkActivitiesSaveState.ticketCost, linkActivitiesSaveState.ticketRewards, true);
            ticketCost = 0;
            ticketReward = 0;
            messagesText.text = "Complete victory! ... get out";
        }
        //happens at initial load from file 
        else
        {
            ticketCost = linkActivitiesSaveState.ticketCost;
            ticketReward = linkActivitiesSaveState.ticketRewards;
        }

        linkActivitiesSaveState.ticketCost = ticketCost;
        linkActivitiesSaveState.ticketRewards = ticketReward;
        costText.text = $"{ticketCost}";
        rewardText.text = $"{ticketReward}";
        UniversalSaveState.SaveLinkActivities(linkActivitiesSaveState);
    }

    //function to determine the costs and rewards of a gambling stage
    void DealCostAndReward(int ticketChange)
    {

    }

    //saving function called for every click of the gamble button and during initialize
    void SaveLinkActivityState()
    {
        UniversalSaveState.SaveLinkActivities(linkActivitiesSaveState);
    }


    //button for returning back to linkActivities if player decides to skip gamble
    public void ReturnToLinkActivitiesButton()
    {
        linkActivitiesSaveState.isStillInActivity = false;
        UniversalSaveState.SaveLinkActivities(linkActivitiesSaveState);
        gameObject.SetActive(false);
    }




}
