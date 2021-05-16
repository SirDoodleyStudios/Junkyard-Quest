
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CardDrafting : MonoBehaviour
{
    //this int is to identify from what object index it came from
    public int objectOriginIndex;
    //the holder of choice cards
    public GameObject choiceHolder;
    GridLayoutGroup gridLayout;
    RectTransform parentCanvas;

    //Skil buttoon
    Button skipButton;


    //reference to the Deck Directory Holder
    DeckPools deckPoolDirectories;
    List<Card> poolCards = new List<Card>();
    private void Awake()
    {
        //the skip button is always the last child under the cardDrafting object
        skipButton = transform.GetChild(transform.childCount - 1).GetComponent<Button>();
        //assign the button for skipCard
        skipButton.onClick.AddListener(() => SkipCard());

        //the DeckPool directory is located at index 0 of the canvas
        deckPoolDirectories = transform.parent.GetChild(0).GetComponent<DeckPools>();

        //first child is the holder of cards
        parentCanvas = transform.parent.GetComponent<RectTransform>();
        gridLayout = choiceHolder.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(parentCanvas.rect.width * .13416815742f, parentCanvas.rect.height * .34280604133f);

        //also assigns the cell size of the holder to the rect sizes of the cards themselves
        foreach (Transform card in choiceHolder.transform)
        {
            RectTransform tempRect = card.gameObject.GetComponent<RectTransform>();
            tempRect.sizeDelta = new Vector2(parentCanvas.rect.width * .13416815742f, parentCanvas.rect.height * .34280604133f);
        }

    }

    public void InitializeDraftPool(ChosenPlayer playerKey, ChosenClass classKey)
    {
        poolCards.AddRange(deckPoolDirectories.GetClassPool(classKey));
        poolCards.AddRange(deckPoolDirectories.GetPlayerPool(playerKey));

    }

    //enables the Drafting Window
    //an int array is sent as parameter and will contain the indices of cards so that it's predetermined at reward object creation
    public void StartCardDraft(List<int> indices)
    {
        //iterates 3 timesand assign a random card from the pool, it won't repeat

        //List<int> indices = new List<int>();
        //store the card displays so that we can resize the fonts after enabling the holder and children
        List<Display> draftedDisplay = new List<Display>();
        List<CardDescriptionLayout> draftedPopups = new List<CardDescriptionLayout>();

        foreach (int index in indices)
        {
            //calls the factory to instantiate a copy of the base card SO
            Card tempCard = Instantiate(poolCards[index]);
            tempCard.effectText = CardTagManager.GetCardEffectDescriptions(tempCard);

            //children indices are determined by the int list index as well
            Display cardDisplay = choiceHolder.transform.GetChild(indices.IndexOf(index)).gameObject.GetComponent<Display>();
            cardDisplay.card = tempCard;
            draftedDisplay.Add(cardDisplay);

            CardDescriptionLayout cardPopups = choiceHolder.transform.GetChild(indices.IndexOf(index)).gameObject.GetComponent<CardDescriptionLayout>();
            draftedPopups.Add(cardPopups);
        }

        //for (int i = 0; indices.Count < 3; i++)
        //{
        //    //this is used for accessing the idex of the saved pool
        //    int tempInt = Random.Range(0, poolCards.Count - 1);
        //    if (!indices.Contains(tempInt))
        //    {
        //        indices.Add(tempInt);

        //        //calls the factory to instantiate a copy of the base card SO
        //        Card tempCard = Instantiate(poolCards[tempInt]);
        //        tempCard.effectText = CardTagManager.GetCardEffectDescriptions(tempCard);

        //        Display cardDisplay = choiceHolder.transform.GetChild(i).gameObject.GetComponent<Display>();
        //        cardDisplay.card = tempCard;
        //        draftedDisplay.Add(cardDisplay);

        //        CardDescriptionLayout cardPopups = choiceHolder.transform.GetChild(i).gameObject.GetComponent<CardDescriptionLayout>();
        //        draftedPopups.Add(cardPopups);

        //    }
        //    else
        //    {
        //        i--;
        //    }
        //}

        //gameObject.SetActive(true);

        //enable the choices one by one
        foreach (Transform card in choiceHolder.transform)
        {
            card.gameObject.SetActive(true);
        }

        //resizing for the correct display and popups
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


        //calls rewardManager to disable to reward object
        RewardsManager rewardManager = transform.parent.GetChild(2).GetComponent<RewardsManager>();
        rewardManager.ClaimReward(objectOriginIndex);

        Destroy(gameObject);


    }

    //skip card button
    void SkipCard()
    {
        RewardsManager rewardManager = transform.parent.GetChild(2).GetComponent<RewardsManager>();
        rewardManager.ClaimReward(objectOriginIndex);
        Destroy(gameObject);
    }

    //calls the manager to incite countdown for closing rewards scene
    //private void OnDestroy()
    //{
    //    //index 2 of the canvas panel is always the rewards manager
    //    RewardsManager rewardManager = transform.parent.GetChild(2).GetComponent<RewardsManager>();
    //    rewardManager.ClaimReward(objectOriginIndex);
    //}
}
