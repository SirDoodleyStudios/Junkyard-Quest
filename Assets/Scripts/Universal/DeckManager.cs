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
    List<Card> playerHand = new List<Card>();

    JigsawFormat jigsaw;

    //panel that shows the player had of cards
    public GameObject playerHandPanel;
    CustomHandLayout handLayout;
    public GameObject cardPrefab;

    Card instantiatedCard;

    // maybe for testing only
    //list of pools, not the cardpool itself
    public List<CardPool> cardPools = new List<CardPool>();

    //can be called by combat manager for updating texts
    public int deckCount { get; private set; }
    public int discardCount { get; private set; }

    //related to deck viewing
    //buttons that point to one method, just different lists to check
    public Button deckViewButton;
    public Button discardPileViewButton;
    //the parent of all deckview objects, target for disabling and enabling
    public GameObject deckViewUI;
    //Content gameobject, parent of all prefabs to be shown after button click
    public Transform deckScrollContent;
    //contains the deckview prefab template
    public GameObject deckViewPrefab;

    
    void Start()
    {
        //assigning cache for playerHand's layouting logic
        handLayout = playerHandPanel.GetComponent<CustomHandLayout>();

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
                    initialDeck.Add(card);
                }
            }

        }
        //assigns the same methodfor deck viewing on each button
        deckViewButton.onClick.AddListener(() => DeckCollectionView(deckViewButton));
        discardPileViewButton.onClick.AddListener(() => DeckCollectionView(discardPileViewButton));
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
    public void DrawCards(int drawCount)
    {
        //serves as counter when receiving draw count value
        int drawtemp = 0;        

        foreach (Card deckCard in battleDeck)
        {           

            //for keeping count of current hand and cache
            int currentHand = playerHand.Count;           
            //ensures that player hand max is only 10
            //discards drawn cards when count goes over 10
            if(playerHand.Count >= 10)
            {
                discardPile.Add(deckCard);
                discardCount = discardPile.Count;
            }
            else
            {
                GameObject enabledCard = playerHandPanel.transform.GetChild(currentHand).gameObject;
                playerHand.Add(deckCard);                
                //assigns card to Display Function
                enabledCard.GetComponent<Display>().card = deckCard;                
                //assigns card to Effect Loader Funtion
                enabledCard.GetComponent<EffectLoader>().card = deckCard;
                //setactive must be called after the the GetComponents Display and EffectLoader
                //this is because Display function will start at OnEnable
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                ///create a logic that warps position of card gameObject from deck first then enable it then show animation going from deck to hand
                enabledCard.SetActive(true);
                handLayout.ActivateRearrange(playerHand.Count);
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
            DrawCards(remainingDraw);
        }
    }

    // can only be called when card is discarded via combat
    public void DiscardCards(GameObject discardedCardObject)
    {
        Card discardedCard = discardedCardObject.GetComponent<Display>().card;
        //for moving the card to discard and removing from player hand
        //Card discardedCard = discardedPrefab.GetComponent<Display>().card; not needed since combat manager will only be sending the Card
        discardPile.Add(discardedCard);
        playerHand.Remove(discardedCard);
        //for disabling the prefab and moving them to back row, functionality moved to combatmanager
        //discardedPrefab.SetActive(false);
        //discardedPrefab.transform.SetAsLastSibling();
        discardCount = discardPile.Count;
        discardedCardObject.SetActive(false);
        handLayout.ActivateRearrange(playerHand.Count);
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

        foreach (Card deckCard in deckCheck)
        {
            bool hasNoDisabledPrefabs = true;
            deckViewPrefab.GetComponent<Display>().card = deckCard;

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

            }
            if (hasNoDisabledPrefabs)
            {
                Instantiate(deckViewPrefab, deckScrollContent);
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

    //work on card logic bug


}
