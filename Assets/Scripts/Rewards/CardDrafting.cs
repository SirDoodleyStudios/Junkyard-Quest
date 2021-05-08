using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CardDrafting : MonoBehaviour
{
    //the holder of choice cards
    public GameObject choiceHolder;
    GridLayoutGroup gridLayout;
    RectTransform parentCanvas;
    

    //reference to the Deck Directory Holder
    public DeckPools deckPoolDirectories;
    List<Card> poolCards = new List<Card>();
    private void Awake()
    {
        //first child is the holder of cards
        parentCanvas = transform.parent.GetComponent<RectTransform>();
        gridLayout = choiceHolder.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(parentCanvas.rect.width * .13416815742f, parentCanvas.rect.height * .34280604133f);

        //also assigns the cell size of the holder to the rect sizes of the cards themselves
        foreach(Transform card in choiceHolder.transform)
        {
            RectTransform tempRect= card.gameObject.GetComponent<RectTransform>();
            tempRect.sizeDelta = new Vector2(parentCanvas.rect.width * .13416815742f, parentCanvas.rect.height * .34280604133f);
        }
        
    }

    public void InitializeDraftPool(ChosenPlayer playerKey, ChosenClass classKey)
    {
        poolCards.AddRange(deckPoolDirectories.GetClassPool(classKey));
        poolCards.AddRange(deckPoolDirectories.GetPlayerPool(playerKey));

    }

    //enables the Drafting Window
    public void StartCardDraft()
    {
        //iterates 3 timesand assign a random card from the pool, it won't repeat

        List<int> indices = new List<int>();
        //store the card displays so that we can resize the fonts after enabling the holder and children
        List<Display> draftedDisplay = new List<Display>();
        List<CardDescriptionLayout> draftedPopups = new List<CardDescriptionLayout>();
        for (int i = 0; indices.Count < 3; i++)
        {
            int tempInt = Random.Range(0, poolCards.Count - 1);
            if (!indices.Contains(tempInt))
            {
                indices.Add(tempInt);

                //calls the factory to instantiate a copy of the base card SO
                Card tempCard = Instantiate(poolCards[tempInt]);
                tempCard.effectText = CardTagManager.GetCardEffectDescriptions(tempCard);

                Display cardDisplay = choiceHolder.transform.GetChild(i).gameObject.GetComponent<Display>();
                cardDisplay.card = tempCard;
                draftedDisplay.Add(cardDisplay);

                CardDescriptionLayout cardPopups = choiceHolder.transform.GetChild(i).gameObject.GetComponent<CardDescriptionLayout>();
                draftedPopups.Add(cardPopups);

            }
            else
            {
                i--;
            }
        }

        gameObject.SetActive(true);

        //foreach(Transform card in choiceHolder.transform)
        //{
        //    card.gameObject.SetActive(true);
        //}

        foreach (Display display in draftedDisplay)
        {
            display.FontResize();
        }
        foreach (CardDescriptionLayout popups in draftedPopups)
        {
            popups.ResizePopups();
        }

    }

    //called by DragNDrop after click
    // loads the current deck, adds the card then save immediately
 
    public void AddtoDeck(Card card)
    {
        UniversalInformation universalInfo = UniversalSaveState.LoadUniversalInformation();
        universalInfo.currentDeck.Add(card.enumCardName);
        UniversalSaveState.SaveUniversalInformation(universalInfo);
        gameObject.SetActive(false);

        //return Toggle overworld after picking a card
        SceneManager.LoadScene("OverWorldScene");
    }
}
