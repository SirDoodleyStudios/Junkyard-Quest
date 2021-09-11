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

    //used by MaterialDrafting check parameters
    List<CraftingMaterialSO> craftingMaterials;

    //used by GearDrafting check parameters
    List<GearSO> gears;

    //used for blueprintDrafting check parameters
    List<BluePrintSO> blueprints;


    public void Awake()
    {
        rewardImage = gameObject.GetComponent<Image>();
        rewardsManager = transform.parent.GetComponent<RewardsManager>();

    }

    //called by rewards manager
    //only used for setting what type of reward will this object contain
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
        //if not scaprs, set the parameter at 0
        else
        {
            scrapsValue = 0;
        }

        //else if(rewardKey == CombatRewards.CardDraft)
        //{
        //    scrapsValue = 0;

        //    ////extract the player and class for the pools
        //    //universalInfo = UniversalSaveState.LoadUniversalInformation();
        //    ////contains the deck directories gameObject
        //    //deckPoolDirectories = transform.parent.parent.GetChild(0).GetComponent<DeckPools>();

        //    //cardPoolIndex = new List<int>();
        //    ////total card pools if we add the class and player decks, this is necessary because carddraft will combine the decks and pick from indices of combined decks
        //    ////the indices are created here so that we can store it and n
        //    //int deckPoolTotal = deckPoolDirectories.GetPlayerPool(universalInfo.chosenPlayer).Count + deckPoolDirectories.GetClassPool(universalInfo.chosenClass).Count;
        //    //for (int i = 0; 3 > i; i++)
        //    //{
        //    //    //prevents an index from repeating
        //    //    int tempInt = Random.Range(0, deckPoolTotal);
        //    //    if (!cardPoolIndex.Contains(tempInt))
        //    //    {
        //    //        cardPoolIndex.Add(tempInt);
        //    //    }
        //    //    else
        //    //    {
        //    //        i--;
        //    //    }          

        //    //}

        //}           
       
    }
    //gets called by manager if loaded reward is cardDraft
    public void PreLoadCardDraft(List<int> preDraft)
    {
        cardPoolIndex = new List<int>();
        cardPoolIndex = preDraft;
    }
    //gets called by manager if loaded reward is material
    public void PreLoadMaterialDraft(List<CraftingMaterialSO> preDraft)
    {
        craftingMaterials = new List<CraftingMaterialSO>();
        craftingMaterials = preDraft;
    }
    //gets called by manager if loaded reward is gear
    public void PreLoadGearDraft(List<GearSO> preDraft)
    {
        gears = new List<GearSO>();
        gears = preDraft;
    }
    //gets called by the manager if loaded reward is vlueprit
    public void PreLoadBlueprintDraft(List<BluePrintSO> predraft)
    {
        blueprints = new List<BluePrintSO>();
        blueprints = predraft;
    }


    //This is called by buttons assigned from editor
    //reward objects are buttons
    public void InstantiateChoiceWindow()
    {
        //set the current object at index 3 sibling so that universal Header is remains as sibling 4 so that we can access universalUI while choosing from draft
        GameObject rewardWindow = Instantiate(rewardPrefab, transform.parent.parent);
        //setting it as index 3 will allow the header universalUI to still be accessed when choosing draft
        //mighrated to the Drafting Scripts instead
        //rewardWindow.transform.SetSiblingIndex(3);

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
        else if (rewardEnum == CombatRewards.Material)
        {
            MaterialDrafting materialDrafting = rewardWindow.transform.GetComponent<MaterialDrafting>();
            materialDrafting.objectOriginIndex = transform.GetSiblingIndex();
            materialDrafting.InitializeMaterialChoices(craftingMaterials);
        }
        else if (rewardEnum == CombatRewards.Gear)
        {
            GearDrafting gearDrafting = rewardWindow.transform.GetComponent<GearDrafting>();
            gearDrafting.objectOriginIndex = transform.GetSiblingIndex();
            gearDrafting.InitializeGearChoices(gears);
        }
        else if (rewardEnum == CombatRewards.Blueprint)
        {
            BlueprintDrafting blueprintDrafting = rewardWindow.transform.GetComponent<BlueprintDrafting>();
            blueprintDrafting.objectOriginIndex = transform.GetSiblingIndex();
            blueprintDrafting.InitializeBlueprintChoices(blueprints);

        }

        //functionality migrated to the rewards manager and activated after picking the draft
        //disables the choice after clicking it
        //gameObject.SetActive(false);

        //lets the manager know that a reward object has been clicked
        rewardsManager.RewardSelected(rewardEnum, scrapsValue);

    }






}
