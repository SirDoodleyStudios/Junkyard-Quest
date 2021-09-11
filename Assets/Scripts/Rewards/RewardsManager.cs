using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class RewardsManager : MonoBehaviour
{
    //assigned in editor
    public CameraUIScript cameraUIScript;

    //reward gameObject Prefabs
    public GameObject cardDraftPrefab;
    public GameObject scrapGainPrefab;
    public GameObject materialDraftPrefab;
    public CraftingMaterialSO materialSO;
    public GameObject gearDraftPrefab;
    public GearSO gearSO;
    public GameObject blueprintDraftPrefab;
    public BluePrintSO blueprintSO;

    //loaded and saved universal Info Instance
    //same for RewardsSaveState
    UniversalInformation universalInfo;
    //RewardsSaveState rewardsSaveState;

    //this is a holder class for the list<int> for cardDrafts
    CardDraftListWrapper cardDraftListWrapper;
    //this is a holder class for the List<CraftingMaterialWrapper> for materialDrafts
    MaterialDraftListWrapper materialDraftListWrapper;
    //this is a holder class for the List<GearWrapper> for gearDrating
    GearDraftListWrapper gearDraftListWrapper;
    //holder for List<AllGearTypes> since blueprints don't have wrappers and just needs the blueprint type key enum
    BlueprintDraftListWrapper blueprintDraftListWrapper;

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

    private void Awake()
    {
        //calls initialization of descriptions first for card draft window
        CardTagManager.InitializeTextDescriptionDictionaries();

        universalInfo = UniversalSaveState.LoadUniversalInformation();
        cameraUIScript.InitiateUniversalUIInfoData(universalInfo);
        cameraUIScript.AssignUIObjects(universalInfo);
    }

    // Start is called before the first frame update
    //must remain in start because for some reason, the rewardObjects' sprite won't render first
    void Start()
    {
        rewardsRepository.Add(CombatRewards.CardDraft, cardDraftPrefab);
        rewardsRepository.Add(CombatRewards.Abilities, cardDraftPrefab);
        rewardsRepository.Add(CombatRewards.Scraps, scrapGainPrefab);
        rewardsRepository.Add(CombatRewards.Material, materialDraftPrefab);
        rewardsRepository.Add(CombatRewards.Gear, gearDraftPrefab);
        rewardsRepository.Add(CombatRewards.Blueprint, blueprintDraftPrefab);
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

        //sets the universalInfo's scene to rewards to properly set it as reward, since if this is just at the start of the run, it will be set as PlayerSelectScreen
        universalInfo.scene = SceneList.Rewards;
    }

    //for spawning the choices in the rewards panel
    //this will just generate a list
    void GenerateRewardsList()
    {
        //called at the beginning of the run, has a fixed reward count and reward types
        if (universalInfo.scene == SceneList.PlayerSelectScreen)
        {
            //list is fixed
            rewardsList.Add(CombatRewards.CardDraft);
            rewardsList.Add(CombatRewards.CardDraft);
            rewardsList.Add(CombatRewards.CardDraft);
            rewardsList.Add(CombatRewards.Material);
            rewardsList.Add(CombatRewards.Blueprint);
            //test material, gear is the real 6th starting kit
            //rewardsList.Add(CombatRewards.Material);
            rewardsList.Add(CombatRewards.Gear);
        }

        else if (universalInfo.nodeActivity == NodeActivityEnum.Combat)
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
                rewardsList.Add(CombatRewards.Material);
            }
        }
        //called when going in a boon node
        else if (universalInfo.nodeActivity == NodeActivityEnum.Boon)
        {
            //always get 1 card and 1 scraps reward
            rewardsList.Add(CombatRewards.CardDraft);
            rewardsList.Add(CombatRewards.Scraps);

            //randomly generate rewards
            int randomPercent = UnityEngine.Random.Range(1, 100);

            //20% chance to get no bonus loot, so we start at 20 base
            //20% chance of getting additional card
            if (21 <= randomPercent && randomPercent <= 40)
            {
                rewardsList.Add(CombatRewards.CardDraft);
            }
            //20% of getting additional scraps
            else if (41 <= randomPercent && randomPercent <= 60)
            {
                rewardsList.Add(CombatRewards.Scraps);
            }
            //10% chance of getting a material
            else if (61 <= randomPercent && randomPercent <= 70)
            {
                rewardsList.Add(CombatRewards.Material);
            }
            //10% chance of getting a blueprint
            else if (71 <= randomPercent && randomPercent <= 80)
            {

            }
            //10% chance of getting a gear
            else if (81 <= randomPercent && randomPercent <= 90)
            {

            }
            //10% chance of getting an ability
            else if (81 <= randomPercent && randomPercent <= 90)
            {
                rewardsList.Add(CombatRewards.Abilities);
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
        //temporary list to hold all the lists inside rewardsSaveState so that we can match indices
        //for cardDraft
        List<List<int>> cardDrafts = new List<List<int>>(); ;
        //int counter t6o determine what list to load during CardDraftLoading
        int cardListCounter = 0;

        //for MaterialDraft
        //this is the converted list holder of the saved materialWrappers to SOs
        List<List<CraftingMaterialSO>> materialSODrafts = new List<List<CraftingMaterialSO>>();
        int materialListCounter = 0;

        //for GearDraft
        //this is the convertef list holder of the saved gearWrappersto SOs
        List<List<GearSO>> gearSODrafts = new List<List<GearSO>>();
        int gearListCounter = 0;

        //for blueprintDraft
        List<List<BluePrintSO>> blueprintSODrafts = new List<List<BluePrintSO>>();
        int blueprintListCounter = 0;

        //Load the RewardsSaveState immediately if isLoadedfromFile
        if (isLoadedFromFile)
        {
            rewardsSaveState = UniversalSaveState.LoadRewardsState();

            //decode the CardDraftListWrapper into a list of lists
            CardDraftListWrapper tempCardDraftList = rewardsSaveState.cardDraftListWrapper;
            cardDrafts.Add(tempCardDraftList.possibleCardDraft1);
            cardDrafts.Add(tempCardDraftList.possibleCardDraft2);
            cardDrafts.Add(tempCardDraftList.possibleCardDraft3);
            cardDrafts.Add(tempCardDraftList.possibleCardDraft4);
            cardDrafts.Add(tempCardDraftList.possibleCardDraft5);
            cardDrafts.Add(tempCardDraftList.possibleCardDraft6);

            //decode the MaterialDraftListWrapper into a list of lists
            MaterialDraftListWrapper tempMaterialDraftList = rewardsSaveState.materialDraftListWrapper;
            List<List<CraftingMaterialWrapper>> tempMaterialWrapperListHolder = new List<List<CraftingMaterialWrapper>>();
            tempMaterialWrapperListHolder.Add(tempMaterialDraftList.possibleMaterialDraft1);
            tempMaterialWrapperListHolder.Add(tempMaterialDraftList.possibleMaterialDraft2);
            tempMaterialWrapperListHolder.Add(tempMaterialDraftList.possibleMaterialDraft3);
            tempMaterialWrapperListHolder.Add(tempMaterialDraftList.possibleMaterialDraft4);
            tempMaterialWrapperListHolder.Add(tempMaterialDraftList.possibleMaterialDraft5);
            tempMaterialWrapperListHolder.Add(tempMaterialDraftList.possibleMaterialDraft6);
            //iterate through the wrapperList to convert their elements into SOs
            foreach (List<CraftingMaterialWrapper> tempWrapperList in tempMaterialWrapperListHolder)
            {
                //temporary list that will hold the converted SO List
                List<CraftingMaterialSO> tempSOList = new List<CraftingMaterialSO>();
                foreach (CraftingMaterialWrapper tempWrapper in tempWrapperList)
                {
                    //construct SO from the wrapper
                    CraftingMaterialSO tempMaterialSO = Instantiate(materialSO);
                    tempMaterialSO.materialPrefix = tempWrapper.materialPrefix;
                    tempMaterialSO.materialType = tempWrapper.materialType;
                    tempMaterialSO.materialEffects.AddRange(tempWrapper.materialEffects);
                    tempSOList.Add(tempMaterialSO);
                }
                materialSODrafts.Add(tempSOList);
            }

            //decode the GearDraftListWrapper into list of lists
            GearDraftListWrapper tempGearDraftList = rewardsSaveState.gearDraftListWrapper;
            List<List<GearWrapper>> tempWrapperListsHolder = new List<List<GearWrapper>>();
            tempWrapperListsHolder.Add(tempGearDraftList.possibleGearDraft1);
            tempWrapperListsHolder.Add(tempGearDraftList.possibleGearDraft2);
            tempWrapperListsHolder.Add(tempGearDraftList.possibleGearDraft3);
            tempWrapperListsHolder.Add(tempGearDraftList.possibleGearDraft4);
            tempWrapperListsHolder.Add(tempGearDraftList.possibleGearDraft5);
            tempWrapperListsHolder.Add(tempGearDraftList.possibleGearDraft6);
            //iterate through the wrapperList to convert their elements into SOs
            foreach (List<GearWrapper> tempWrapperList in tempWrapperListsHolder)
            {
                //temporary list that will hold the converted SO List
                List<GearSO> tempSOList = new List<GearSO>();
                foreach (GearWrapper tempWrapper in tempWrapperList)
                {
                    //construct SO from the wrapper
                    GearSO tempGearSO = Instantiate(gearSO);
                    tempGearSO.gearType = tempWrapper.gearType;
                    tempGearSO.gearSetBonus = tempWrapper.gearSetBonus;
                    tempGearSO.gearEffects.AddRange(tempWrapper.gearEffects);
                    tempSOList.Add(tempGearSO);
                }
                gearSODrafts.Add(tempSOList);
            }
            
            //decode the blueprintDraftListWrapper into list of lists
            BlueprintDraftListWrapper tempBlueprintDraftList = rewardsSaveState.blueprintDraftListWrapper;
            List<List<AllGearTypes>> tempBlueprintTypeListHolder = new List<List<AllGearTypes>>();
            tempBlueprintTypeListHolder.Add(tempBlueprintDraftList.possibleBlueprintDraft1);
            tempBlueprintTypeListHolder.Add(tempBlueprintDraftList.possibleBlueprintDraft2);
            tempBlueprintTypeListHolder.Add(tempBlueprintDraftList.possibleBlueprintDraft3);
            tempBlueprintTypeListHolder.Add(tempBlueprintDraftList.possibleBlueprintDraft4);
            tempBlueprintTypeListHolder.Add(tempBlueprintDraftList.possibleBlueprintDraft5);
            tempBlueprintTypeListHolder.Add(tempBlueprintDraftList.possibleBlueprintDraft6);

            foreach (List<AllGearTypes> tempBlueprintTypeList in tempBlueprintTypeListHolder)
            {
                //temporarily hold the generated bluePrint SO
                List<BluePrintSO> tempSOList = new List<BluePrintSO>();
                foreach (AllGearTypes blueprintKey in tempBlueprintTypeList)
                {
                    BluePrintSO instantiatedBlueprint = Instantiate(blueprintSO);
                    instantiatedBlueprint.blueprint = blueprintKey;
                    //assign the blueprint values
                    instantiatedBlueprint.bluePrintSprite = Resources.Load<Sprite>($"Blueprints/{blueprintKey}");
                    //send the blueprint ot the AssignBluprintValues to fill out the vecttor and allowable type list
                    instantiatedBlueprint = UniversalFunctions.AssignUniqueBlueprintValues(instantiatedBlueprint);
                    tempSOList.Add(instantiatedBlueprint);
                }
                blueprintSODrafts.Add(tempSOList);
            }
            
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
                else if (universalInfo.nodeActivity == NodeActivityEnum.Rival)
                {
                    baseScraps = 100;
                    overkillMultiplier = 5;
                }
                //for boon nodes
                else if (universalInfo.nodeActivity == NodeActivityEnum.Boon)
                {
                    baseScraps = UnityEngine.Random.Range(30, 70);
                    overkillMultiplier = 0;
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


            //if cardDraft, assign 0 scraps and send a list of ints for the indices to be used for the randomized card draft
            else if (rewardsList[i] == CombatRewards.CardDraft)
            {
                if (choiceObject.activeSelf)
                {
                    rewardObject.AssignReward(rewardsList[i], rewardsRepository[rewardsList[i]], 0);
                    //if not loaded fom file, perform randomization of indices from scratch
                    if (isLoadedFromFile == false)
                    {
                        //contains the deck directories gameObject
                        DeckPools deckPoolDirectories = transform.parent.GetChild(0).GetComponent<DeckPools>();

                        List<int> cardPoolIndex = new List<int>();
                        //total card pools if we add the class and player decks, this is necessary because carddraft will combine the decks and pick from indices of combined decks
                        //the indices are created here so that we can store it and n
                        int deckPoolTotal = deckPoolDirectories.GetPlayerPool(universalInfo.chosenPlayer).Count + deckPoolDirectories.GetClassPool(universalInfo.chosenClass).Count;
                        for (int j = 0; 3 > j; j++)
                        {
                            //prevents an index from repeating
                            int tempInt = UnityEngine.Random.Range(0, deckPoolTotal);
                            if (!cardPoolIndex.Contains(tempInt))
                            {
                                cardPoolIndex.Add(tempInt);
                            }
                            else
                            {
                                j--;
                            }

                        }

                        //send the preloaded draft to reward object
                        rewardObject.PreLoadCardDraft(cardPoolIndex);

                        //create instance of the cardDraft wrapper class then insert the generated list to the wrapper
                        if (cardDraftListWrapper == null)
                        {
                            cardDraftListWrapper = new CardDraftListWrapper();
                            cardDraftListWrapper.AssignList(cardPoolIndex);
                        }
                        else
                        {
                            cardDraftListWrapper.AssignList(cardPoolIndex);
                        }

                    }
                    //if loaded from file
                    else
                    {
                        //send the preloaded draft to reward object
                        rewardObject.PreLoadCardDraft(cardDrafts[cardListCounter]);
                        //increase counter so that the next loaded CardDraft will take the next list
                        cardListCounter++;
                    }

                }

            }


            //for materialReward
            else if (rewardsList[i] == CombatRewards.Material)
            {
                if (choiceObject.activeSelf)
                {
                    //assign the reward type
                    rewardObject.AssignReward(rewardsList[i], rewardsRepository[rewardsList[i]], 0);

                    if (!isLoadedFromFile)
                    {
                        //temporary holder of the generated MaterialSO
                        List<CraftingMaterialSO> tempMatSOList = new List<CraftingMaterialSO>();
                        //generate two random materials as choices
                        for (int j = 0; 1 >= j; j++)
                        {
                            //randomize ints for the components of the material being built
                            CraftingMaterialSO instantiatedMatSO = Instantiate(materialSO);

                            instantiatedMatSO.materialType = UniversalFunctions.GetRandomEnum<AllMaterialTypes>();
                            //material prefix should not have "Normal" which is index 0 in AllMaterialPrefixes enum
                            //reiterates until the randomized prefix is not Normal anymore
                            AllMaterialPrefixes materialPrefix;
                            do
                            {
                                materialPrefix = UniversalFunctions.GetRandomEnum<AllMaterialPrefixes>();
                            }
                            while ((int)materialPrefix == 0);
                            instantiatedMatSO.materialPrefix = materialPrefix;
                            //for randomizing the material effects
                            for (int k = 0; 1 >= k; k++)
                            {
                                AllMaterialEffects materialEffect;
                                //prevents repeat of material Effect by rerolling the material Effect enum if the SO's material Effect List already contains the randomized effect
                                //the materialEffect < 100 condition is for preventing set bonuses that are in the 100+ spot of the enum are not taken during randomization
                                do
                                {
                                    materialEffect = UniversalFunctions.GetRandomEnum<AllMaterialEffects>();
                                }
                                while (instantiatedMatSO.materialEffects.Contains(materialEffect) || (int)materialEffect >= 100);
                                instantiatedMatSO.materialEffects.Add(materialEffect);
                            }
                            //add generated materialSO to temporary list
                            tempMatSOList.Add(instantiatedMatSO);
                        }
                        //preload the generated objects
                        rewardObject.PreLoadMaterialDraft(tempMatSOList);

                        //convert the generated SOs to Wrappers
                        List<CraftingMaterialWrapper> wrappersToBeSaved = UniversalFunctions.ConvertCraftingMaterialSOListToWrapper(tempMatSOList);

                        //create instance of the materialDraft wrapper class then insert the generated list to the wrapper
                        if (materialDraftListWrapper == null)
                        {
                            materialDraftListWrapper = new MaterialDraftListWrapper();
                            materialDraftListWrapper.AssignList(wrappersToBeSaved);
                        }
                        else
                        {
                            materialDraftListWrapper.AssignList(wrappersToBeSaved);
                        }
                    }
                    else
                    {
                        //send the preloaded draft to reward object
                        rewardObject.PreLoadMaterialDraft(materialSODrafts[materialListCounter]);
                        //increase counter so that the next loaded CardDraft will take the next list
                        materialListCounter++;
                    }

                }
            }

            else if (rewardsList[i] == CombatRewards.Gear)
            {
                if (choiceObject.activeSelf)
                {
                    //assign the reward type
                    rewardObject.AssignReward(rewardsList[i], rewardsRepository[rewardsList[i]], 0);

                    if (!isLoadedFromFile)
                    {
                        //temporary holder of the generated MaterialSO
                        List<GearSO> tempGearSOList = new List<GearSO>();
                        //generate two random materials as choices
                        for (int j = 0; 1 >= j; j++)
                        {
                            //randomize ints for the components of the material being built
                            GearSO instantiatedGearSO = Instantiate(gearSO);

                            instantiatedGearSO.gearType = UniversalFunctions.GetRandomEnum<AllGearTypes>();
                            //normal gears found in world are all normal prefix
                            instantiatedGearSO.gearSetBonus = AllMaterialPrefixes.Normal;

                            //for randomizing the gear effects
                            for (int k = 0; 1 >= k; k++)
                            {
                                AllMaterialEffects materialEffect;
                                //prevents repeat of material Effect by rerolling the material Effect enum if the SO's material Effect List already contains the randomized effect
                                //the materialEffect < 100 condition is for preventing set bonuses that are in the 100+ spot of the enum are not taken during randomization
                                do
                                {
                                    materialEffect = UniversalFunctions.GetRandomEnum<AllMaterialEffects>();
                                }
                                while (instantiatedGearSO.gearEffects.Contains(materialEffect) || (int)materialEffect >= 100);
                                instantiatedGearSO.gearEffects.Add(materialEffect);
                            }
                            //add generated materialSO to temporary list
                            tempGearSOList.Add(instantiatedGearSO);
                        }
                        //preload the generated objects
                        rewardObject.PreLoadGearDraft(tempGearSOList);

                        //convert the generated SOs to Wrappers
                        List<GearWrapper> wrappersToBeSaved = UniversalFunctions.ConvertGearSOListToWrapper(tempGearSOList);

                        //create instance of the materialDraft wrapper class then insert the generated list to the wrapper
                        if (gearDraftListWrapper == null)
                        {
                            gearDraftListWrapper = new GearDraftListWrapper();
                            gearDraftListWrapper.AssignList(wrappersToBeSaved);
                        }
                        else
                        {
                            gearDraftListWrapper.AssignList(wrappersToBeSaved);
                        }
                    }
                    else
                    {
                        //send the preloaded draft to reward object
                        rewardObject.PreLoadGearDraft(gearSODrafts[gearListCounter]);
                        //increase counter so that the next loaded draft will take the next list
                        gearListCounter++;
                    }
                }

            }

            else if (rewardsList[i] == CombatRewards.Blueprint)
            {
                if (choiceObject.activeSelf)
                {
                    //assign the reward type
                    rewardObject.AssignReward(rewardsList[i], rewardsRepository[rewardsList[i]], 0);

                    if (!isLoadedFromFile)
                    {
                        //will contain the randomized oprion to be saved
                        List<BluePrintSO> tempBlueprintSOList = new List<BluePrintSO>();
                        List<AllGearTypes> tempBlueprintKeyList = new List<AllGearTypes>();

                        //list that wioll take note of radomly generated blueprints so that we can prevent repeating of options
                        // the initial list will contain only blueprints that are not in the player's inventory
                        List<AllGearTypes> repeatPreventer = new List<AllGearTypes>();
                        repeatPreventer = UniversalFunctions.SearchAvailableBlueprints(universalInfo.bluePrints);
                        for (int l = 0; 1 >= l; l++)
                        {
                            BluePrintSO tempBlueprint = Instantiate(blueprintSO);
                            //call function to randomly get blueprint enums\
                            //will prevent from repeating options

                            //do
                            //{
                            //    randomizedBlueprint = UniversalFunctions.GetRandomEnum<AllGearTypes>();
                            //}
                            //while (repeatPreventer.Contains(randomizedBlueprint));
                            //add the generated indedx to prevent a repeat
                            //repeatPreventer.Add(randomizedBlueprint);

                            //get a random bluepint from the list then remove the selected blueprint to prevent repetition
                            int randomizedIndex = UnityEngine.Random.Range(0, repeatPreventer.Count);
                            AllGearTypes randomizedBlueprint = repeatPreventer[randomizedIndex];
                            repeatPreventer.RemoveAt(randomizedIndex);


                            //assign the blueprint values
                            tempBlueprint.blueprint = randomizedBlueprint;
                            tempBlueprint.bluePrintSprite = Resources.Load<Sprite>($"Blueprints/{randomizedBlueprint}");
                            //send the blueprint ot the AssignBluprintValues to fill out the vecttor and allowable type list
                            tempBlueprint = UniversalFunctions.AssignUniqueBlueprintValues(tempBlueprint);
                            //add to the list to be loaded in reward object
                            tempBlueprintSOList.Add(tempBlueprint);
                            tempBlueprintKeyList.Add(randomizedBlueprint);

                            //this is activated when all blueprints are already taken, immediately break
                            if (repeatPreventer.Count == 0)
                            {
                                break;
                            }
                        }
                        rewardObject.PreLoadBlueprintDraft(tempBlueprintSOList);

                        //create instance of the materialDraft wrapper class then insert the generated list to the wrapper
                        if (blueprintDraftListWrapper == null)
                        {
                            blueprintDraftListWrapper = new BlueprintDraftListWrapper();
                            blueprintDraftListWrapper.AssignList(tempBlueprintKeyList);
                        }
                        else
                        {
                            blueprintDraftListWrapper.AssignList(tempBlueprintKeyList);
                        }
                    }
                    else
                    {

                        //send the preloaded draft to reward object
                        rewardObject.PreLoadBlueprintDraft(blueprintSODrafts[blueprintListCounter]);
                        //increase counter so that the next loaded draft will take the next list
                        blueprintListCounter++;
                    }
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

        //only save the cardDraftListWrapper if from rewardObjects are not loaded
        //DONT REMEMBER WHY WE ONLY SAVE IF THE GENERATION IS NOT FROM SAVE FILE
        if (!isLoadedFromFile)
        {
            rewardsSaveState.cardDraftListWrapper = cardDraftListWrapper;
            rewardsSaveState.materialDraftListWrapper = materialDraftListWrapper;
            rewardsSaveState.gearDraftListWrapper = gearDraftListWrapper;
            rewardsSaveState.blueprintDraftListWrapper = blueprintDraftListWrapper;
        }

        UniversalSaveState.SaveRewardsState(rewardsSaveState);
    }

    //override when a Rewards.json exist in the files
    void LoadRewardsFromFile()
    {
        RewardsSaveState rewardsSaveState = UniversalSaveState.LoadRewardsState();

        rewardsList = rewardsSaveState.rewardsList;
        rewardsAvailabilityList = rewardsSaveState.rewardsAvailabilityList;
        claimedRewardCounter = rewardsSaveState.claimRewardCounter;

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
            BackToOerworldButton();
        }

    }
    //Function for going back to Overworld
    //called when button is pressed or if all reward options are taken
    public void BackToOerworldButton()
    {
        //delete the file before going back to overworld
        File.Delete(Application.persistentDataPath + "/Rewards.json");
        SceneManager.LoadScene("OverworldScene");
    }

}

//this is a classwrapper that will hold the preloaded CardDraft Lists
[Serializable]
public class CardDraftListWrapper
{
    public List<int> possibleCardDraft1;
    public List<int> possibleCardDraft2;
    public List<int> possibleCardDraft3;
    public List<int> possibleCardDraft4;
    public List<int> possibleCardDraft5;
    public List<int> possibleCardDraft6;

    public void AssignList(List<int> draftList)
    {
        //everytim AssignList is called, the List<int> parametr will be assigned on empty Lists
        if (possibleCardDraft1 == null)
        {
            possibleCardDraft1 = draftList;
        }
        else if(possibleCardDraft2 == null)
        {
            possibleCardDraft2 = draftList;
        }
        else if (possibleCardDraft3 == null)
        {
            possibleCardDraft3 = draftList;
        }
        else if (possibleCardDraft4 == null)
        {
            possibleCardDraft4 = draftList;
        }
        else if (possibleCardDraft5 == null)
        {
            possibleCardDraft5 = draftList;
        }
        else if (possibleCardDraft6 == null)
        {
            possibleCardDraft6 = draftList;
        }
    }    

}

[Serializable]
public class MaterialDraftListWrapper
{
    public List<CraftingMaterialWrapper> possibleMaterialDraft1;
    public List<CraftingMaterialWrapper> possibleMaterialDraft2;
    public List<CraftingMaterialWrapper> possibleMaterialDraft3;
    public List<CraftingMaterialWrapper> possibleMaterialDraft4;
    public List<CraftingMaterialWrapper> possibleMaterialDraft5;
    public List<CraftingMaterialWrapper> possibleMaterialDraft6;

    public void AssignList(List<CraftingMaterialWrapper> draftList)
    {
        //convert to craftingMaterialSO first

        //everytime AssignList is called, the List< parametr will be assigned on empty Lists
        if (possibleMaterialDraft1 == null)
        {
            possibleMaterialDraft1 = draftList;
        }
        else if (possibleMaterialDraft2 == null)
        {
            possibleMaterialDraft2 = draftList;
        }
        else if (possibleMaterialDraft3 == null)
        {
            possibleMaterialDraft3 = draftList;
        }
        else if (possibleMaterialDraft4 == null)
        {
            possibleMaterialDraft4 = draftList;
        }
        else if (possibleMaterialDraft5 == null)
        {
            possibleMaterialDraft5 = draftList;
        }
        else if (possibleMaterialDraft6 == null)
        {
            possibleMaterialDraft6 = draftList;
        }
    }
}

[Serializable]
public class GearDraftListWrapper
{
    public List<GearWrapper> possibleGearDraft1;
    public List<GearWrapper> possibleGearDraft2;
    public List<GearWrapper> possibleGearDraft3;
    public List<GearWrapper> possibleGearDraft4;
    public List<GearWrapper> possibleGearDraft5;
    public List<GearWrapper> possibleGearDraft6;

    public void AssignList(List<GearWrapper> draftList)
    {
        //convert to craftingMaterialSO first

        //everytime AssignList is called, the List< parametr will be assigned on empty Lists
        if (possibleGearDraft1 == null)
        {
            possibleGearDraft1 = draftList;
        }
        else if (possibleGearDraft2 == null)
        {
            possibleGearDraft2 = draftList;
        }
        else if (possibleGearDraft3 == null)
        {
            possibleGearDraft3 = draftList;
        }
        else if (possibleGearDraft4 == null)
        {
            possibleGearDraft4 = draftList;
        }
        else if (possibleGearDraft5 == null)
        {
            possibleGearDraft5 = draftList;
        }
        else if (possibleGearDraft6 == null)
        {
            possibleGearDraft6 = draftList;
        }
    }

}

[Serializable]
public class BlueprintDraftListWrapper
{
    public List<AllGearTypes> possibleBlueprintDraft1;
    public List<AllGearTypes> possibleBlueprintDraft2;
    public List<AllGearTypes> possibleBlueprintDraft3;
    public List<AllGearTypes> possibleBlueprintDraft4;
    public List<AllGearTypes> possibleBlueprintDraft5;
    public List<AllGearTypes> possibleBlueprintDraft6;

    public void AssignList(List<AllGearTypes> draftList)
    {
        if (possibleBlueprintDraft1 == null)
        {
            possibleBlueprintDraft1 = draftList;
        }
        else if (possibleBlueprintDraft2 == null)
        {
            possibleBlueprintDraft2 = draftList;
        }
        else if (possibleBlueprintDraft3 == null)
        {
            possibleBlueprintDraft3 = draftList;
        }
        else if (possibleBlueprintDraft4 == null)
        {
            possibleBlueprintDraft4 = draftList;
        }
        else if (possibleBlueprintDraft5 == null)
        {
            possibleBlueprintDraft5 = draftList;
        }
        else if (possibleBlueprintDraft6 == null)
        {
            possibleBlueprintDraft6 = draftList;
        }
    }
}