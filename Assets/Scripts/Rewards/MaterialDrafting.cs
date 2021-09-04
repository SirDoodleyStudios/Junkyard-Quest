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

    private void Awake()
    {
        //first child is the holder of cards
        parentCanvas = transform.parent.GetComponent<RectTransform>();
        gridLayout = choiceHolder.GetComponent<GridLayoutGroup>();
        //standard material prefab size is 864, 250
        gridLayout.cellSize = new Vector2(Screen.width*.45f, Screen.height* .231481481f);
    }

    //called by RewardObject to show material choices
    public void InitializeMaterialChoices()
    {
        
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
