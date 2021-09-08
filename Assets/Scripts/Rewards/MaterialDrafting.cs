using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialDrafting : MonoBehaviour
{
    //this int is to identify from what object index it came from
    public int objectOriginIndex;
    //the holder of choice cards
    public GameObject choiceHolder;
    GridLayoutGroup gridLayout;
    RectTransform parentCanvas;
    //skip button reference
    Button skipButton;

    //reference for the Universal UI script
    public CameraUIScript cameraUIScript;

    private void Awake()
    {
        //set the current object at index 4 sibling so that universal Header is last sibling
        transform.SetSiblingIndex(4);
        //find the UniversalUI whic is always the last sibling of under the canvas
        //curently, we use child 5 as the last sibling under canvas
        cameraUIScript = transform.parent.GetChild(5).GetChild(0).GetComponent<CameraUIScript>();

        //the skip button is always the last child under the cardDrafting object
        skipButton = transform.GetChild(transform.childCount - 1).GetComponent<Button>();
        //assign the button for skipCard
        skipButton.onClick.AddListener(() => SkipMaterial());

        //first child is the holder of cards
        parentCanvas = transform.parent.GetComponent<RectTransform>();
        gridLayout = choiceHolder.GetComponent<GridLayoutGroup>();
        //standard material prefab size is 864, 250
        gridLayout.cellSize = new Vector2(Screen.width*.45f, Screen.height* .231481481f);
    }

    //called by RewardObject to show material choices
    //this list must only contain 2 materials
    public void InitializeMaterialChoices(List<CraftingMaterialSO> materials)
    {
        for (int i = 0; 1 >= i; i++)
        {
            GameObject materialObj = choiceHolder.transform.GetChild(i).gameObject;
            CraftingMaterialSOHolder materialSOHolder = materialObj.GetComponent<CraftingMaterialSOHolder>();
            materialSOHolder.ShowMaterialInViewer(materials[i]);
            materialObj.SetActive(true);
        }
    }

    //add the material in inventory, called by the dragNDrop
    public void AddToInventory(CraftingMaterialSO addedSO)
    {
        UniversalInformation universalInfo = UniversalSaveState.LoadUniversalInformation();
        //generate the wrapper
        CraftingMaterialWrapper materialWrapper = new CraftingMaterialWrapper(addedSO);
        universalInfo.craftingMaterialWrapperList.Add(materialWrapper);
        //update universalUI
        cameraUIScript.UpdateMaterialInventory(materialWrapper, true);

        UniversalSaveState.SaveUniversalInformation(universalInfo);
        cameraUIScript.UpdateUniversalInfo();

        //calls rewardManager to disable to reward object
        RewardsManager rewardManager = transform.parent.GetChild(2).GetComponent<RewardsManager>();
        rewardManager.ClaimReward(objectOriginIndex);

        Destroy(gameObject);
    }

    void SkipMaterial()
    {
        //Used when skip is for abandoning the choice instead of just closing the draftWindow
        //RewardsManager rewardManager = transform.parent.GetChild(2).GetComponent<RewardsManager>();
        //rewardManager.ClaimReward(objectOriginIndex);
        Destroy(gameObject);
    }


    //public MerchantSaveState InitiateMaterialOptions(UniversalInformation universalInfo, bool isLoadedFromFile)
    //{
    //    if (isLoadedFromFile)
    //    {
    //        //list of materialSOs converted to wrappers
    //        for (int i = 0; merchantSave.materialOptions.Count - 1 >= i; i++)
    //        {
    //            //the wrapper to be converted
    //            CraftingMaterialWrapper CMW = merchantSave.materialOptions[i];
    //            //construct SO from the wrapper
    //            CraftingMaterialSO tempMaterialSO = Instantiate(referenceCraftingMaterialSO);
    //            tempMaterialSO.materialPrefix = CMW.materialPrefix;
    //            tempMaterialSO.materialType = CMW.materialType;
    //            tempMaterialSO.materialEffects.AddRange(CMW.materialEffects);

    //            //add the SO and cost in the dictionary
    //            materialNCost.Add(tempMaterialSO, merchantSave.materialOptionCosts[i]);
    //        }
    //    }
    //    else
    //    {
    //        //randomly create materials
    //        //default 6 materials in options
    //        for (int i = 0; 5 >= i; i++)
    //        {
    //            //randomize ints for the components of the material being built
    //            CraftingMaterialSO tempMaterialSO = Instantiate(referenceCraftingMaterialSO);
    //            tempMaterialSO.materialType = UniversalFunctions.GetRandomEnum<AllMaterialTypes>();
    //            //material prefix should not have "Normal" which is index 0 in AllMaterialPrefixes enum
    //            //reiterates until the randomized prefix is not Normal anymore
    //            AllMaterialPrefixes materialPrefix;
    //            do
    //            {
    //                materialPrefix = UniversalFunctions.GetRandomEnum<AllMaterialPrefixes>();
    //            }
    //            while ((int)materialPrefix == 0);
    //            tempMaterialSO.materialPrefix = materialPrefix;

    //            for (int j = 0; 1 >= j; j++)
    //            {
    //                AllMaterialEffects materialEffect;
    //                //prevents repeat of material Effect by rerolling the material Effect enum if the SO's material Effect List already contains the randomized effect
    //                //the materialEffect < 100 condition is for preventing set bonuses that are in the 100+ spot of the enum are not taken during randomization
    //                do
    //                {
    //                    materialEffect = UniversalFunctions.GetRandomEnum<AllMaterialEffects>();
    //                }
    //                while (tempMaterialSO.materialEffects.Contains(materialEffect) || (int)materialEffect >= 100);
    //                tempMaterialSO.materialEffects.Add(materialEffect);
    //            }
    //            //randomize card scraps cost
    //            int materialValueInt = UnityEngine.Random.Range(80, 100);
    //            materialNCost.Add(tempMaterialSO, materialValueInt);

    //            //populate the merchantSaveState with the randomly generated 
    //            CraftingMaterialWrapper instantiatedCMW = new CraftingMaterialWrapper(tempMaterialSO);
    //            merchantSaveState.materialOptions.Add(instantiatedCMW);
    //            merchantSaveState.materialOptionCosts.Add(materialValueInt);
    //        }
    //    }
    //    return merchantSaveState;
    //}
}
