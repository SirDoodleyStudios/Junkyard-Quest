using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class RewardsManager : MonoBehaviour
{
    //reward gameObject Prefabs
    public GameObject cardDraftPrefab;
    public GameObject scrapGainPrefab;
    public GameObject treasureGainPrefab;

    //loaded and saved universal Info Instance
    //same for RewardsSaveState
    UniversalInformation universalInfo;
    //RewardsSaveState rewardsSaveState;

    //indicator if the rewardsscene has already been initialized, this is determined if we load or not
    public bool isRewardsSceneInitiated;

    //current list of rewards options and the availability
    //the bool list will always have 6 elements, false for non- elements
    public List<CombatRewards> rewardsList = new List<CombatRewards>();
    public List<bool> rewardsAvailabilityList = new List<bool>();

    //dictionary of rewardsKey and rewardPrefab
    public Dictionary<CombatRewards, GameObject> rewardsRepository = new Dictionary<CombatRewards, GameObject>();

    //counts if a reward has already been claimed
    int claimedRewardCounter;

    //keeps track of what rewards window is currently open, this field is used for saving and loading
    CombatRewards currentReward;

    // Start is called before the first frame update
    //must remain in start because for some reason, the rewardObjects' sprite won't render first
    void Start()
    {
        //calls initialization of descriptions first for card draft window
        CardTagManager.InitializeTextDescriptionDictionaries();

        universalInfo = UniversalSaveState.LoadUniversalInformation();

        rewardsRepository.Add(CombatRewards.CardDraft, cardDraftPrefab);
        rewardsRepository.Add(CombatRewards.Abilities, cardDraftPrefab);
        rewardsRepository.Add(CombatRewards.Scraps, scrapGainPrefab);
        //rewardsRepository.Add(CombatRewards.Treasures, treasureGainPrefab);

        //if Rewrds save file exists, go call function the LoadFromFileOverride
        if (File.Exists(Application.persistentDataPath + "/Rewards.json"))
        {
            LoadRewardsFromFile();
        }
        else
        {
            //create new save file instance
            GenerateRewardsList();

        }
    }

    //for spawning the choices in the rewards panel
    //this will just generate a list
    void GenerateRewardsList()
    {
        if (universalInfo.nodeActivity == NodeActivityEnum.Combat)
        {
            rewardsList.Add(CombatRewards.CardDraft);
            rewardsList.Add(CombatRewards.Scraps);

            //if all enemies are overkilled, add another reward
            if (universalInfo.enemyCount == universalInfo.overkills)
            {
                rewardsList.Add(CombatRewards.CardDraft);
            }
        }

        else if (universalInfo.nodeActivity == NodeActivityEnum.Rival)
        {
            rewardsList.Add(CombatRewards.CardDraft);
            rewardsList.Add(CombatRewards.Scraps);
            //no logics for these yet
            //rewardsList.Add(CombatRewards.Abilities);
            //rewardsList.Add(CombatRewards.Treasures);

            //if all enemies are overkilled, add another reward
            if (universalInfo.enemyCount == universalInfo.overkills)
            {
                rewardsList.Add(CombatRewards.Treasures);
            }
        }

        //the bool is for GenerateRewardsObjects if the command is from file or not
        GenerateRewardsObjects(false);

    }

    //for rigging the rewards objects in the panel
    //this will actually maipulate the objects
    //The bool will signify if the generation of reward objects is initiated from loading a file
    void GenerateRewardsObjects(bool isLoadedFromFile)
    {
        //stores the determined rewards list and availability list
        RewardsSaveState rewardsSaveState = new RewardsSaveState();
        //Load the RewardsSaveState immediately if isLoadedfromFile
        if (isLoadedFromFile)
        {
            rewardsSaveState = UniversalSaveState.LoadRewardsState();
        }

        //iterate through the rewardsList to one by one generate their respective rewardObjects
        for (int i = 0; rewardsList.Count > i; i++)
        {

            GameObject choiceObject = transform.GetChild(i).gameObject;
            //gets the object in holder and assign an object prefab depending on the combatRewards key enumerated in rewardsList
            RewardObject rewardObject = choiceObject.GetComponent<RewardObject>();



            //if loaded from file, simply assign the bool saved in the list based on the matched current index
            if (isLoadedFromFile)
            {
                choiceObject.SetActive(rewardsSaveState.rewardsAvailabilityList[i]);
            }
            //when generating a new Rewards scene, allways set the object to true since the initial is false
            else
            {
                choiceObject.SetActive(true);
            }



            //if the reward to be loaded is scraps, calculate the scraps amount depending on overkills
            if (rewardsList[i] == CombatRewards.Scraps)
            {
                //int base value based on activity
                //multiplier is how much gold bonus is gotten based on overkill
                int baseScraps;
                int overkillMultiplier;

                if (universalInfo.nodeActivity == NodeActivityEnum.Combat)
                {
                    baseScraps = 50;
                    overkillMultiplier = 1;
                }
                else if(universalInfo.nodeActivity == NodeActivityEnum.Rival)
                {
                    baseScraps = 100;
                    overkillMultiplier = 5;
                }
                //should be called in skirmishes only
                else
                {
                    //TESTING ONLY WHILE THERE ARE NO LOGICS FOR GETTING OVERKILLS YET
                    baseScraps = 50;

                    //real base value;
                    //baseScraps = 20;
                    overkillMultiplier = 1;
                }

                //if the reward object being loaded is false, don't initiate
                if (choiceObject.activeSelf)
                {
                    int totalScraps = baseScraps + ((universalInfo.overkills * 10) * overkillMultiplier);
                    rewardObject.AssignReward(rewardsList[i], rewardsRepository[rewardsList[i]], totalScraps);
                }

            }
            //if not scraps, just send 0 value in scraps parameter
            else
            {
                if (choiceObject.activeSelf)
                {
                    rewardObject.AssignReward(rewardsList[i], rewardsRepository[rewardsList[i]], 0);
                }

            }


        }

        //save changes to save file
        foreach (Transform rewardTrans in transform)
        {
            rewardsAvailabilityList.Add(rewardTrans.gameObject.activeSelf);
        }
        rewardsSaveState.rewardsList = rewardsList;
        rewardsSaveState.rewardsAvailabilityList = rewardsAvailabilityList;
        UniversalSaveState.SaveRewardsState(rewardsSaveState);
    }

    //override when a Rewards.json exist in the files
    void LoadRewardsFromFile()
    {
        RewardsSaveState rewardsSaveState = UniversalSaveState.LoadRewardsState();

        rewardsList = rewardsSaveState.rewardsList;
        rewardsAvailabilityList = rewardsSaveState.rewardsAvailabilityList;

        //the bool is for GenerateRewardsObjects if the command is from file or not
        GenerateRewardsObjects(true);
        //if true, it means that we have a draft window open bbefore exiting
        //FOR NOW, NO FUNCTIONALITY TO SAVE THE CARDS INSTANTIATED
        if (rewardsSaveState.isCurrentRewardEnabled)
        {
            transform.GetChild(rewardsSaveState.objectOriginIndex).GetComponent<RewardObject>().InstantiateChoiceWindow();
        }


    }


    //called when rewardObject is clicked
    public void RewardSelected(CombatRewards rewardEnum, int scraps)
    {
        
        //only updates the availability list
        foreach (Transform rewardTrans in transform)
        {
            rewardsAvailabilityList.Add(rewardTrans.gameObject.activeSelf);
        }
        RewardsSaveState rewardsSaveState = UniversalSaveState.LoadRewardsState();
        rewardsSaveState.rewardsAvailabilityList = rewardsAvailabilityList;

        //will get 0 scraps if the reward is card draft
        rewardsSaveState.isCurrentRewardEnabled = true;
        rewardsSaveState.currentReward = rewardEnum;
        rewardsSaveState.currentScraps = scraps;

    }

    //called by a reward object when it is clicked, once all reward objects are disabled, go back to overworld
    //the int parameter is the index of the clicked object that created the draft window 
    public void ClaimReward(int objectOriginIndex)
    {
        //disable the object that called the claimReward
        transform.GetChild(objectOriginIndex).gameObject.SetActive(false);

        RewardsSaveState rewardsSaveState = UniversalSaveState.LoadRewardsState();
        claimedRewardCounter++;

        rewardsSaveState.claimRewardCounter = claimedRewardCounter;
        rewardsSaveState.isCurrentRewardEnabled = false;
        rewardsSaveState.currentScraps = 0;
        rewardsSaveState.rewardsAvailabilityList[objectOriginIndex] = false;

        UniversalSaveState.SaveRewardsState(rewardsSaveState);

        //once the count matches the list count, it means that all rewards has been claimed

        if (rewardsList.Count == claimedRewardCounter)
        {
            //delete the file before going back to overworld
            File.Delete(Application.persistentDataPath + "/Rewards");
            SceneManager.LoadScene("OverworldScene");
        }

    }






}
