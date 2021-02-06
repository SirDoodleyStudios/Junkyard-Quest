using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{

    List<Card> playerDeck = new List<Card>();
    List<Card> discardPile = new List<Card>();
    List<Card> consumePile = new List<Card>();
    List<Card> playerHand = new List<Card>();

    JigsawFormat jigsaw;

    //panel that shows the player had of cards
    public GameObject playerHandPanel;
    public GameObject cardPrefab;

    Card instantiatedCard;

    // maybe for testing only
    public List<CardPool> cardPools = new List<CardPool>();

    //can be called by combat manager for updating texts
    public int deckCount { get; private set; }
    public int discardCount { get; private set; }




    void Start()
    {
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
                    playerDeck.Add(card);
                }
            }

        }

        deckCount = playerDeck.Count;
        discardCount = discardPile.Count;
        Shuffle(playerDeck);

    }


    //drawcount is sent by combatmanager
    //return int is the remaining deck cards
    public void DrawCards(int drawCount)
    {
        //serves as counter when receiving draw count value
        int drawtemp = 0;        

        foreach (Card deckCard in playerDeck)
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
                enabledCard.SetActive(true);
            }

            drawtemp++;
            //stops loop when deck runs out
            //stops loop when draw count runs out
            if (playerDeck.Count - drawtemp == 0 
                || drawtemp >= drawCount)
            {
                break;
            }

        }

        //ensures that when deck count is 0, only remaining cards are removed then calls the reset
        if (playerDeck.Count - drawtemp == 0)
        {
            playerDeck.RemoveRange(0, drawtemp);
            DeckReset(drawCount - drawtemp);
        }
        //just removes card from deck if draw is less than deck
        else if (playerDeck.Count > drawtemp)
        {
            //this line removes from deck
            playerDeck.RemoveRange(0, drawCount);
            deckCount = playerDeck.Count;
        }

    }


    //called by draw function when deck count runs out
    //receives the remaining drawcount and passes it back when it calls the draw function again after moving discard to deck and shuffling
    public void DeckReset(int remainingDraw)
    {
        playerDeck.AddRange(discardPile);
        discardPile.Clear();
        deckCount = playerDeck.Count;
        discardCount = discardPile.Count;
        Shuffle(playerDeck);
        if (remainingDraw != 0)
        {
            DrawCards(remainingDraw);
        }
    }

    // can only be called when card is discarded via combat
    public void DiscardCards(Card discardedCard)
    {
        //for moving the card to discard and removing from player hand
        //Card discardedCard = discardedPrefab.GetComponent<Display>().card; not needed since combat manager will only be sending the Card
        discardPile.Add(discardedCard);
        playerHand.Remove(discardedCard);
        //for disabling the prefab and moving them to back row, functionality moved to combatmanager
        //discardedPrefab.SetActive(false);
        //discardedPrefab.transform.SetAsLastSibling();
        discardCount = discardPile.Count;
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


    //LOGIC MUST BE CREATED THAT DOES NOT THROW AN ERROR IF DRAW COUNT IS BIGGER THAN DECK COUNT
    void Shuffle<Card>(List<Card> list)
    {
        //only shuffles if deck count is not 0
        if(playerDeck.Count != 0)
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
