using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    List<Card> initialDeck = new List<Card>();

    List<Card> battleDeck = new List<Card>();
    List<Card> discardPile = new List<Card>();
    List<Card> consumePile = new List<Card>();
    public List<Card> playerHandList { get; private set; } = new List<Card>();

    JigsawFormat jigsaw;

    //panel that shows the player had of cards
    public GameObject playerHandPanel;
    PlayerHand playerHandScript;
    CustomHandLayout handLayout;
    public GameObject cardPrefab;

    Card instantiatedCard;

    // maybe for testing only
    //list of pools, not the cardpool itself
    public List<CardPool> cardPools = new List<CardPool>();

    //can be called by combat manager for updating texts
    public int deckCount { get; private set; }
    public int discardCount { get; private set; }
    public int consumeCount { get; private set; }

    //related to deck viewing
    //buttons that point to one method, just different lists to check
    public Button deckViewButton;
    public Button discardPileViewButton;
    public Button consumePileViewButton;

    //the parent of all deckview objects, target for disabling and enabling
    public GameObject deckViewUI;
    //Content gameobject, parent of all prefabs to be shown after button click
    public Transform deckScrollContent;
    //contains the deckview prefab template
    public GameObject deckViewPrefab;

    //for test only, randomly apply jigsaw effects to instantiated cards///////////////
    public List<JigsawFormat> testJigsawList = new List<JigsawFormat>();



    void Start()
    {
        //assigning cache for playerHand's layouting logic
        handLayout = playerHandPanel.GetComponent<CustomHandLayout>();

        //assigning cache for the playerHand script
        playerHandScript = playerHandPanel.GetComponent<PlayerHand>();

        //must depend on chosen character and class

        //look through pools first, LOGIC MIGHT CHANGE
        foreach (CardPool pool in cardPools)
        {
            //duplicates each card 3 times, for test only
            foreach(Card card in pool.listOfCards)
            {
                for (int i = 0; 2 >= i; i++)
                {
                    instantiatedCard = Instantiate(card);
                    instantiatedCard.effectText = CardTagManager.GetCardEffectDescriptions(instantiatedCard.enumCardName);

                    //test script for randomly assigning jigsawFormat to Cards
                    //test script for randomly assigning jigsaws to cards FOR TEST USE ONLY /////////////////////////////////////////////////////
                    if (instantiatedCard.cardType == CardType.Offense || instantiatedCard.cardType == CardType.Utility)
                    {
                        JigsawFormat instantiatedJigsaw = Instantiate(testJigsawList[Random.Range(0, testJigsawList.Count)]);
                        instantiatedCard.jigsawEffect = instantiatedJigsaw;
                        JigsawFormat assignedJigsaw = instantiatedCard.jigsawEffect;

                        assignedJigsaw.inputLink = (JigsawLink)Random.Range(0, 2);
                        assignedJigsaw.outputLink = (JigsawLink)Random.Range(0, 2);

                        assignedJigsaw.jigsawDescription = CardTagManager.GetJigsawDescriptions(assignedJigsaw.enumJigsawName);
                        assignedJigsaw.jigsawImage = CardTagManager.DetermineJigsawImage(assignedJigsaw.inputLink, assignedJigsaw.outputLink);
                    }


                    //initialDeck.Add(card);
                    initialDeck.Add(instantiatedCard);

                }
            }

        }
        //assigns the same methodfor deck viewing on each button
        deckViewButton.onClick.AddListener(() => DeckCollectionView(deckViewButton));
        discardPileViewButton.onClick.AddListener(() => DeckCollectionView(discardPileViewButton));
        consumePileViewButton.onClick.AddListener(() => DeckCollectionView(consumePileViewButton));
        InitializeBattleDeck();


    }

    //first state of player deck every start of battle
    public void InitializeBattleDeck()
    {
        battleDeck = initialDeck;
        deckCount = battleDeck.Count;
        discardCount = discardPile.Count;
        Shuffle(battleDeck);
    }


    //drawcount is sent by combatmanager
    //return int is the remaining deck cards
    public IEnumerator DrawCards(int drawCount)
    {
        //serves as counter when receiving draw count value
        int drawtemp = 0;
        //time differential between each draw
        float lagTime = .1f;

        //sets playerHand state to DrawPhase so that DragNDrop logics wont work while drawing
        playerHandScript.StateChanger(CombatState.DrawPhase);

        foreach (Card deckCard in battleDeck)
        {           

            //for keeping count of current hand and cache
            int currentHand = playerHandList.Count;           
            //ensures that player hand max is only 10
            //discards drawn cards when count goes over 10
            if(playerHandList.Count >= 10)
            {
                discardPile.Add(deckCard);
                discardCount = discardPile.Count;
            }
            else
            {
                GameObject enabledCard = playerHandPanel.transform.GetChild(currentHand).gameObject;
                playerHandList.Add(deckCard);                
                //assigns card to Display Function
                enabledCard.GetComponent<Display>().card = deckCard;                
                //assigns card to Effect Loader Funtion
                enabledCard.GetComponent<EffectLoader>().card = deckCard;
                //setactive must be called after the the GetComponents Display and EffectLoader
                //this is because Display function will start at OnEnable
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                ///create a logic that warps position of card gameObject from deck first then enable it then show animation going from deck to hand
                ///

                //enabledCard.SetActive(true);
                //yield return new WaitForSeconds(.01f);

                handLayout.ActivateRearrange(playerHandList.Count, enabledCard);
                yield return new WaitForSeconds(lagTime);
                //calls the event in playerHand to make the set positions of cards after tweening its fixed final positions
                playerHandScript.FixCardPositions();


            }

            drawtemp++;
            //stops loop when deck runs out
            //stops loop when draw count runs out
            if (battleDeck.Count - drawtemp == 0 
                || drawtemp >= drawCount)
            {
                break;
            }

        }
        //at end of draw, DragNDrop should be albe to work now
        playerHandScript.StateChanger(CombatState.PlayerTurn);

        //ensures that when deck count is 0, only remaining cards are removed then calls the reset
        if (battleDeck.Count - drawtemp == 0)
        {
            battleDeck.RemoveRange(0, drawtemp);
            DeckReset(drawCount - drawtemp);
        }
        //just removes card from deck if draw is less than deck
        else if (battleDeck.Count > drawtemp)
        {
            //this line removes from deck
            battleDeck.RemoveRange(0, drawCount);
            deckCount = battleDeck.Count;
        }

    }



    //called by draw function when deck count runs out
    //receives the remaining drawcount and passes it back when it calls the draw function again after moving discard to deck and shuffling
    public void DeckReset(int remainingDraw)
    {

        battleDeck.AddRange(discardPile);
        discardPile.Clear();
        deckCount = battleDeck.Count;
        discardCount = discardPile.Count;
        Shuffle(battleDeck);
        if (remainingDraw != 0)
        {
            StartCoroutine(DrawCards(remainingDraw));
            //DrawCards(remainingDraw);
        }
    }

    // can only be called when card is discarded via combat
    public IEnumerator DiscardCards(GameObject discardedCardObject)
    {
        playerHandScript.StateChanger(CombatState.DrawPhase);
        //time differential between each rearrange
        float lagTime = .2f;

        Card discardedCard = discardedCardObject.GetComponent<Display>().card;
        //for moving the card to discard and removing from player hand
        //Card discardedCard = discardedPrefab.GetComponent<Display>().card; not needed since combat manager will only be sending the Card
        discardPile.Add(discardedCard);
        playerHandList.Remove(discardedCard);
        //for disabling the prefab and moving them to back row, functionality moved to combatmanager
        //discardedPrefab.SetActive(false);
        //discardedPrefab.transform.SetAsLastSibling();
        discardCount = discardPile.Count;

        handLayout.ActivateRearrange(playerHandList.Count, discardedCardObject);
        //disable is here to make way for rearrange position because rearange function turns card to enabled
        discardedCardObject.SetActive(false);
        yield return new WaitForSeconds(lagTime);
        //calls the event in playerHand to make the set positions of cards after tweening its fixed final positions
        playerHandScript.FixCardPositions();

        playerHandScript.StateChanger(CombatState.PlayerTurn);

        //playerHandScript.ResetToDeckPosition();
    }
    //for consumed cards, similar to discard but cards in consume pile will not be drawn upon
    public IEnumerator ConsumeCards(GameObject consumedCardObject)
    {
        playerHandScript.StateChanger(CombatState.DrawPhase);
        //time differential between each rearrange
        float lagTime = .2f;
        Card consumedCard = consumedCardObject.GetComponent<Display>().card;
        consumePile.Add(consumedCard);
        playerHandList.Remove(consumedCard);

        //for updating Consume UI
        consumeCount = consumePile.Count;

        handLayout.ActivateRearrange(playerHandList.Count, consumedCardObject);
        //disable is here to make way for rearrange position because rearange function turns card to enabled
        consumedCardObject.SetActive(false);
        yield return new WaitForSeconds(lagTime);
        //calls the event in playerHand to make the set positions of cards after tweening its fixed final positions
        playerHandScript.FixCardPositions();

        playerHandScript.StateChanger(CombatState.PlayerTurn);

        //playerHandScript.ResetToDeckPosition();
    }

    //Just for Plain rearrangement without any discard or consume
    public IEnumerator PlainRearrange(GameObject rearrangedCard)
    {
        DragNDrop dragNDrop = rearrangedCard.GetComponent<DragNDrop>();

        playerHandScript.StateChanger(CombatState.DrawPhase);
        float lagTime = .2f;
        handLayout.ActivateRearrange(playerHandList.Count, rearrangedCard);
        yield return new WaitForSeconds(lagTime);
        playerHandScript.FixCardPositions();
        playerHandScript.StateChanger(CombatState.PlayerTurn);

        dragNDrop.ResetSortingCanvasAndCollider();
    }

    public void AlterHandCreative(Card creativeCard, bool isRemoving)
    {
        //Combatmanager sends the card to be removed and a bool whether it's a return to deck or a remove from deck from cancelling creative

        if (isRemoving)
        {
            playerHandList.Remove(creativeCard);
        }
        else
        {
            playerHandList.Add(creativeCard);
        }
    }

    //public void DiscardAll()
    //{

    //    foreach (Transform playerHandChild in playerHandPanel.transform)
    //    {
    //        GameObject currPrefab = playerHandChild.gameObject;

    //        if(currPrefab.activeSelf == true)
    //        {
    //            //for moving the card to discard and removing from player hand
    //            Card discardedCard = currPrefab.GetComponent<Display>().card;
    //            discardPile.Add(discardedCard);
    //            playerHand.Remove(discardedCard);
    //            currPrefab.SetActive(false);
    //        }
    //    }
    //    discardCount = discardPile.Count;

    //}

    //single method for viewing a card collection in view
    public void DeckCollectionView(Button button)
    {
        deckViewUI.SetActive(true);
        List<Card> deckCheck = new List<Card>();

        //assigns which deck pile is to be inspected depending on which button is pressed
        if(button == deckViewButton)
        {
            //deckCheck.AddRange(battleDeck);
            deckCheck = battleDeck;
            //add default sorting, players should not be able to see order of cards
        }
        else if (button == discardPileViewButton)
        {
            //deckCheck.AddRange(discardPile);
            deckCheck = discardPile;
        }
        else if (button == consumePileViewButton)
        {
            deckCheck = consumePile;
        }

        //actual logic to show each card in UI one by one

        foreach (Card deckCard in deckCheck)
        {
            bool hasNoDisabledPrefabs = true;
            deckViewPrefab.GetComponent<Display>().card = deckCard;

            //checks the scroll contents if there are already instantiated card prefabs that can be recycled
            foreach(Transform content in deckScrollContent)
            {
                GameObject disabledPrefabs = content.gameObject;
                if (!disabledPrefabs.activeSelf)
                {
                    disabledPrefabs.GetComponent<Display>().card = deckCard;
                    disabledPrefabs.SetActive(true);
                    hasNoDisabledPrefabs = false;
                    break;
                }
            //if no card prefab can be recycled, instantiate a new one
            }
            if (hasNoDisabledPrefabs)
            {
                GameObject instantiatedPrefab = Instantiate(deckViewPrefab, deckScrollContent);
                instantiatedPrefab.GetComponent<Display>().card = deckCard;
                instantiatedPrefab.SetActive(true);
            }
        }
    }

    public void BackFromDeckView()
    {
        foreach (Transform content in deckScrollContent)
        {
            content.gameObject.SetActive(false);
        }
        deckViewUI.SetActive(false);
    }


    //LOGIC MUST BE CREATED THAT DOES NOT THROW AN ERROR IF DRAW COUNT IS BIGGER THAN DECK COUNT
    void Shuffle<Card>(List<Card> list)
    {
        //only shuffles if deck count is not 0
        if(battleDeck.Count != 0)
        {
            System.Random random = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                int k = random.Next(n);
                n--;
                Card temp = list[k];
                list[k] = list[n];
                list[n] = temp;
            }
        }

    }





}
