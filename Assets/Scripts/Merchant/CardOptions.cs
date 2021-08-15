using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardOptions : MonoBehaviour
{
    //assigned in editor
    //card prefab to instantiate
    public GameObject cardObjectPrefab;
    //reference MerchantManager
    public MerchantManager merchantManager;
    //the content view
    public Transform cardOptionsContent;
    //used contains the deck pools
    public DeckPools deckPools;
    //The price tag prefab to be attached to the items on sale
    public GameObject priceTagPrefab;

    //generated at initiate function, should be random
    List<Card> playerCardPool = new List<Card>();
    List<Card> classCardPool = new List<Card>();

    //will contain the actual list of cards generated for merchant
    //List<Card> cardList = new List<Card>();
    //contains the scrapValue of each card in the cardList
    List<int> cardListCosts = new List<int>();
    //dictionary that holds the Card and it's corresponding cost
    Dictionary<Card, int> CardNCosts = new Dictionary<Card, int>();

    //current merchantSaveState to be altered and sent back to MerchantManager once player closes the Option UI
    MerchantSaveState merchantSaveState;

    //initiate the available card Options
    //called by MerchantManager at the beginning
    //if Merchant.json exists, the list is sent automatically from the Merchant file, if not, proceed to randomization here
    //bool parameter determines whether the card list will come from file or randomized here
    //called at the beginning of merchantManager to initiate the possible contents of the card options
    //the returned merchantSaveState is used solely for gathering the initial generated options and populating the file immediately
    public MerchantSaveState InitiateCardOptions(UniversalInformation universalInfo, MerchantSaveState merchantSave, bool isLoadedFromFile)
    {
        //stores the chosen player and class pools
        ChosenPlayer chosenPlayer = universalInfo.chosenPlayer;
        ChosenClass chosenClass = universalInfo.chosenClass;
        //initializes theCardSO factory
        CardSOFactory.InitializeCardSOFactory(chosenPlayer, chosenClass);
        //generate the card pools
        classCardPool = deckPools.GetClassPool(chosenClass);
        playerCardPool = deckPools.GetPlayerPool(chosenPlayer);
        //assign the received merchantSaveState to be altered and sent back to the MerchantManager when saving
        merchantSaveState = merchantSave;

        //if loaded from file, just assign the merchantSaveStateParameter
        if (isLoadedFromFile)
        {
            //initiatie the CardSOFactory
            CardSOFactory.InitializeCardSOFactory(chosenPlayer, chosenClass);

            int optionsCount = merchantSaveState.cardOptions.Count;
            //decrypt the merchantSaveState here and convert it to the CardNCost Dictionary
            for (int i = 0; optionsCount - 1 >= i; i++ )
            {
                Card instantiatedCard = CardSOFactory.GetCardSO(merchantSave.cardOptions[i].cardEnum);
                instantiatedCard.effectText = CardTagManager.GetCardEffectDescriptions(instantiatedCard);
                CardNCosts.Add(instantiatedCard, merchantSaveState.cardOptionCosts[i]);
            }
        }
        //if not loaded, use default loading
        else
        {
            //this internal int list is going to be used during randomizing so that no card repeats in options
            //the ints to be stored here are indices of the card pool
            List<int> repeatPreventer = new List<int>();

            //generate 5 player cards
            //NOT USED FOR NOW BECAUSE THERE AREN'T ANY ARLEN CARDS YET, ALIGN WITH THE CLASS CARD LOGIC
            //for (int i = 0; 4 >= i; i++)
            //{
            //    int randomizedIndex = Random.Range(0, playerCardPool.Count);
            //    cardList.Add(playerCardPool[randomizedIndex]);
            //}

            //generate 5 class cards
            for (int i = 0; 4 >= i; i++)
            {
                //the do while makes it so that if a duplicate Card is generated from the randomizer, it loops back and randomizes a new one until the card to be added becomes unique
                int randomizedIndex;
                Card instantiatedCard;
                do
                {
                    randomizedIndex = Random.Range(0, classCardPool.Count);
                    instantiatedCard = Instantiate(classCardPool[randomizedIndex]);
                }
                while (repeatPreventer.Contains(randomizedIndex));
                //add the cardpool index
                repeatPreventer.Add(randomizedIndex);

                instantiatedCard.effectText = CardTagManager.GetCardEffectDescriptions(instantiatedCard);
                //randomize card scraps cost
                int scrapsValueInt = Random.Range(45, 71);
                CardNCosts.Add(instantiatedCard, scrapsValueInt);

                //gather the randomly generated options and populate the merchantSaveState for the fresh initialize phase
                CardAndJigsaWrapper instantiatedCJW = new CardAndJigsaWrapper(instantiatedCard);
                merchantSaveState.cardOptions.Add(instantiatedCJW);
                merchantSaveState.cardOptionCosts.Add(scrapsValueInt);
            }
        }
        return merchantSaveState;

    }

    //shows the randomly generated options for cards

    public void ViewCardOptions(MerchantSaveState currSaveState)
    {
        //assigns the merchantSaveState from the merchantManager
        merchantSaveState = currSaveState;

        //actual logic to show each card in UI one by one

        foreach (KeyValuePair<Card, int> cardNCost in CardNCosts)
        {
            bool hasNoDisabledPrefabs = true;
            cardObjectPrefab.GetComponent<Display>().card = cardNCost.Key;

            //sets the size of each cell in the content holder depending on the size of screen
            //the numbers are calculated to get the exact amount needed
            //I just generated the cards on the screen wtih a sixe that I liked with a fixed 100,143.75 cell size then divided the screen sizes from them
            GridLayoutGroup gridLayout = cardOptionsContent.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(Screen.width * .13440860215f, Screen.height * .34389952153f);
            gridLayout.spacing = new Vector2(Screen.width * .0237247924f, Screen.height * .04219409282f);

            //checks the scroll contents if there are already instantiated card prefabs that can be recycled
            foreach (Transform content in cardOptionsContent)
            {
                GameObject disabledPrefabs = content.gameObject;
                if (!disabledPrefabs.activeSelf)
                {
                    disabledPrefabs.GetComponent<Display>().card = cardNCost.Key;
                    disabledPrefabs.SetActive(true);
                    hasNoDisabledPrefabs = false;

                    //price tag is just referenced since it should be existing under the card prefab at this point
                    GameObject priceTagObj = content.GetChild(content.childCount - 1).gameObject;
                    PriceTag priceTag = priceTagObj.GetComponent<PriceTag>();
                    priceTag.SetPriceTag(cardNCost.Value);

                    //Price is now in the priceTag script
                    //the dragNDrpMerchant holds the scrap value
                    //DragNDropMerchant dragNDropMerchant = disabledPrefabs.GetComponent<DragNDropMerchant>();
                    //dragNDropMerchant.SetScrapsValue(cardNCost.Value);
                    break;
                }
                //if no card prefab can be recycled, instantiate a new one
            }

            if (hasNoDisabledPrefabs)
            {
                GameObject instantiatedPrefab = Instantiate(cardObjectPrefab, cardOptionsContent);
                RectTransform instantiatedRect = instantiatedPrefab.GetComponent<RectTransform>();
                Display instantiatedDisplay = instantiatedPrefab.GetComponent<Display>();
                CardDescriptionLayout instantiatedPopups = instantiatedPrefab.GetComponent<CardDescriptionLayout>();
                instantiatedRect.sizeDelta = new Vector2(Screen.width * .13440860215f, Screen.height * .34389952153f);
                instantiatedDisplay.card = cardNCost.Key;

                //create the priceTag prefab then instantiate it under the card then set the price
                GameObject priceTagObj = Instantiate(priceTagPrefab, instantiatedPrefab.transform);
                PriceTag priceTag = priceTagObj.GetComponent<PriceTag>();
                priceTag.SetPriceTag(cardNCost.Value);
                

                //DragNDropMerchant dragNDropMerchant = instantiatedPrefab.GetComponent<DragNDropMerchant>();
                //dragNDropMerchant.SetScrapsValue(cardNCost.Value);

                //sets the prefixed sizes of the card's fons
                instantiatedDisplay.FontResize();
                instantiatedPopups.ResizePopups();
                instantiatedPrefab.SetActive(true);
            }


        }
    }

    //function called to remove the bought card from the card list
    public void RemoveCardFromOptionList(Card card)
    {
        //cardList.Remove(card);
        CardNCosts.Remove(card);
    }

    //back button
    public void CardOptionBackButton()
    {
        //disables all children prefab of cards
        foreach (Transform content in cardOptionsContent)
        {
            content.gameObject.SetActive(false);
        }
        //disable the CardOptionUI
        gameObject.SetActive(false);

        //updates the card related options in the current instance of merchantSaveState
        UpdateCardOptionsSaveState();
        merchantManager.UpdateMerchantSaveState(merchantSaveState);

    }

    //helper function
    //retrieves the current cardList and their costs and separates them to two lists so that it can be saved in the merchantSaveState file
    //called when exiting from the card Options UI
    public void UpdateCardOptionsSaveState()
    {
        //List holders
        List<CardAndJigsaWrapper> tempCJWList = new List<CardAndJigsaWrapper>();
        List<int> tempCostList = new List<int>();

        //converts the current dictionary from Card,int to CJW,int
        foreach (KeyValuePair<Card, int> cardNCost in CardNCosts)
        {
            CardAndJigsaWrapper CJW = new CardAndJigsaWrapper(cardNCost.Key);
            tempCJWList.Add(CJW);
            tempCostList.Add(cardNCost.Value);
        }
        //assign the listst in the merchantSaveState instance
        merchantSaveState.cardOptions = tempCJWList;
        merchantSaveState.cardOptionCosts = tempCostList;
    }

}
