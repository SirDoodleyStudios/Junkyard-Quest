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
            //extract the player and class for the pools
            UniversalInformation universalInfo = UniversalSaveState.LoadUniversalInformation();

            CardDrafting cardDrafting = rewardWindow.transform.GetComponent<CardDrafting>();
            cardDrafting.objectOriginIndex = transform.GetSiblingIndex();
            cardDrafting.InitializeDraftPool(universalInfo.chosenPlayer, universalInfo.chosenClass);
            cardDrafting.StartCardDraft();
        }

        //functionality migrated to the rewards manager and activated after picking the draft
        //disables the choice after clicking it
        //gameObject.SetActive(false);

        //lets the manager know that a reward object has been clicked
        rewardsManager.RewardSelected(rewardEnum, scrapsValue);

    }






}
