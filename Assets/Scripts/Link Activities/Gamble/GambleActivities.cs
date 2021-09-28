using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
        //draw index will start at 2 because that is the position of the first draw
        drawIndex = 2;
    }

    //initializing function
    public void InitializeGambleActivity(LinkActivityEnum gambleType)
    {
        //fetch the deck
        currentDeck = cameraUIScript.FetchDeck();
        //get random card
        baseCard = currentDeck[Random.Range(0, currentDeck.Count-1)];

        //remove the chosen card so that it doesn't get drawn anymore
        currentDeck.Remove(baseCard);
        firstDraw = currentDeck[Random.Range(0, currentDeck.Count - 1)];
        currentDeck.Remove(firstDraw);
        secondDraw = currentDeck[Random.Range(0, currentDeck.Count - 1)];
        currentDeck.Remove(secondDraw);
        thirdDraw = currentDeck[Random.Range(0, currentDeck.Count - 1)];

        //assign the cards on the card object already
        baseCardObj.GetComponent<Display>().card = baseCard;
        firstDrawObj.GetComponent<Display>().card = firstDraw;
        secondDrawObj.GetComponent<Display>().card = secondDraw;
        thirdDrawObj.GetComponent<Display>().card = thirdDraw;

        //reveal the base card
        baseCardObj.GetComponentInParent<Image>().enabled = false;
        baseCardObj.SetActive(true);
    }

    //function to reveal the drawn card
    public void RevealCardButton()
    {
        //the card to be revelealed
        //dictated by the drawIndex
        //the card object itself is childed under a holder image that will serve card back before revealing the card
        //disable the image of the cardback and enable the whole card object
        GameObject showcaseCard = cardShowcasePanel.transform.GetChild(drawIndex).gameObject;
        showcaseCard.GetComponent<Image>().enabled = false;
        showcaseCard.transform.GetChild(0).gameObject.SetActive(true);
        drawIndex++;

    }




}
