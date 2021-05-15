using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardObject : MonoBehaviour
{

    //reference to the holder
    RewardsManager rewardsManager;

    //contains the reward prefab to be executed
    public GameObject rewardPrefab;
    Image rewardImage;
    CombatRewards rewardEnum;

    //only used when the reward is scaps
    int scrapsValue;

    //currently only used for CardDrafting check parameters
    //UniversalInformation universalInfo;
    //DeckPools deckPoolDirectories;
    List<int> cardPoolIndex;


    public void Awake()
    {
        rewardImage = gameObject.GetComponent<Image>();
        rewardsManager = transform.parent.GetComponent<RewardsManager>();

    }

    //called by rewards manager
    public void AssignReward(CombatRewards rewardKey, GameObject reward, int scraps)
    {
        rewardEnum = rewardKey;
        rewardPrefab = reward;
        //assign image from Resources, the text shoud be ezactly the same with enum
        rewardImage.sprite = Resources.Load<Sprite>($"Rewards/{rewardKey}");

        //save the scrapsvalue
        if(rewardKey == CombatRewards.Scraps)
        {
            scrapsValue = scraps;
        }

        else if(rewardKey == CombatRewards.CardDraft)
        {
            scrapsValue = 0;

            ////extract the player and class for the pools
            //universalInfo = UniversalSaveState.LoadUniversalInformation();
            ////contains the deck directories gameObject
            //deckPoolDirectories = transform.parent.parent.GetChild(0).GetComponent<DeckPools>();

            //cardPoolIndex = new List<int>();
            ////total card pools if we add the class and player decks, this is necessary because carddraft will combine the decks and pick from indices of combined decks
            ////the indices are created here so that we can store it and n
            //int deckPoolTotal = deckPoolDirectories.GetPlayerPool(universalInfo.chosenPlayer).Count + deckPoolDirectories.GetClassPool(universalInfo.chosenClass).Count;
            //for (int i = 0; 3 > i; i++)
            //{
            //    //prevents an index from repeating
            //    int tempInt = Random.Range(0, deckPoolTotal);
            //    if (!cardPoolIndex.Contains(tempInt))
            //    {
            //        cardPoolIndex.Add(tempInt);
            //    }
            //    else
            //    {
            //        i--;
            //    }          

            //}

        }           
       
    }

    public void PreLoadCardDraft(List<int> preDraft)
    {
        cardPoolIndex = new List<int>();
        cardPoolIndex = preDraft;
    }


    //This is called by buttons assigned from editor
    //reward objects are buttons
    public void InstantiateChoiceWindow()
    {
        //instantiate the scrap drafting object under the canvas object
        GameObject rewardWindow = Instantiate(rewardPrefab, transform.parent.parent);
        if (rewardEnum == CombatRewards.Scraps)
        {
            ScrapsDrafting scrapsDrafting = rewardWindow.transform.GetComponent<ScrapsDrafting>();
            scrapsDrafting.objectOriginIndex = transform.GetSiblingIndex();
            scrapsDrafting.InitializeScrapValue(scrapsValue);
        }
        //instantiate the card drafting object under the canvas
        else if (rewardEnum == CombatRewards.CardDraft)
        {
            UniversalInformation universalInfo = UniversalSaveState.LoadUniversalInformation();

            CardDrafting cardDrafting = rewardWindow.transform.GetComponent<CardDrafting>();
            cardDrafting.objectOriginIndex = transform.GetSiblingIndex();
            cardDrafting.InitializeDraftPool(universalInfo.chosenPlayer, universalInfo.chosenClass);
            cardDrafting.StartCardDraft(cardPoolIndex);
        }

        //functionality migrated to the rewards manager and activated after picking the draft
        //disables the choice after clicking it
        //gameObject.SetActive(false);

        //lets the manager know that a reward object has been clicked
        rewardsManager.RewardSelected(rewardEnum, scrapsValue);

    }






}
