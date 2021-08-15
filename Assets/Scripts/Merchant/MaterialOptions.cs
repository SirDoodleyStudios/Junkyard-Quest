using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MaterialOptions : MonoBehaviour
{
    //assigned in editor
    //reference to merchantManager
    public MerchantManager merchantManager;
    //trans of the options content viewer
    public Transform materialContentTrans;
    //material prefab base reference for instantiating
    public GameObject materialPrefabReference;
    public CraftingMaterialSO referenceCraftingMaterialSO;
    //prefab for the priceTag
    public GameObject priceTagPrefab;
    //reference to UI viewers, this is so that Update Function is disabled when viewing the UniversalUIs
    public GameObject inventoryViewer;
    public GameObject deckViewer;
    public GameObject gearViewer;

    //internal merchantSaveStte received and then edited to be sent back to merchantManager for saving
    MerchantSaveState merchantSaveState;

    ////list of MaterialSOs converted from the wrapper list in merchantSaveState
    //List<CraftingMaterialSO> materialSOList = new List<CraftingMaterialSO>();
    ////List per material Cost, must synchronize with materialSOList
    //List<int> materialCostList = new List<int>();
    //dictionary that contains the material and corresponding cost
    Dictionary<CraftingMaterialSO, int> materialNCost = new Dictionary<CraftingMaterialSO, int>();

    //for mouse pointing and clicking
    Vector2 PointRay;
    Ray ray;
    RaycastHit2D pointedObject;

    //Initialization
    //bool parameter to determine if the initialization is fresh or from file
    public MerchantSaveState InitiateMaterialOptions(UniversalInformation universalInfo, MerchantSaveState merchantSave, bool isLoadedFromFile)
    {
        //assigns the saveState may it be empty or loaded
        merchantSaveState = merchantSave;
        if (isLoadedFromFile)
        {
            //list of materialSOs converted to wrappers
            for(int i = 0; merchantSave.materialOptions.Count - 1 >= i; i++)
            {
                //the wrapper to be converted
                CraftingMaterialWrapper CMW = merchantSave.materialOptions[i];
                //construct SO from the wrapper
                CraftingMaterialSO tempMaterialSO = Instantiate(referenceCraftingMaterialSO);
                tempMaterialSO.materialPrefix = CMW.materialPrefix;
                tempMaterialSO.materialType = CMW.materialType;
                tempMaterialSO.materialEffects.AddRange(CMW.materialEffects);

                //add the SO and cost in the dictionary
                materialNCost.Add(tempMaterialSO, merchantSave.materialOptionCosts[i]);
            }
        }
        else
        {
            //randomly create materials
            //default 6 materials in options
            for (int i = 0; 5 >= i; i++)
            {
                //randomize ints for the components of the material being built
                CraftingMaterialSO tempMaterialSO = Instantiate(referenceCraftingMaterialSO);
                tempMaterialSO.materialType = UniversalFunctions.GetRandomEnum<AllMaterialTypes>();
                //material prefix should not have "Normal" which is index 0 in AllMaterialPrefixes enum
                //reiterates until the randomized prefix is not Normal anymore
                AllMaterialPrefixes materialPrefix;
                do
                {
                    materialPrefix = UniversalFunctions.GetRandomEnum<AllMaterialPrefixes>();
                }
                while ((int)materialPrefix == 0);
                tempMaterialSO.materialPrefix = materialPrefix;

                for (int j = 0; 1 >= j; j++)
                {
                    AllMaterialEffects materialEffect;
                    //prevents repeat of material Effect by rerolling the material Effect enum if the SO's material Effect List already contains the randomized effect
                    //the materialEffect < 100 condition is for preventing set bonuses that are in the 100+ spot of the enum are not taken during randomization
                    do
                    {
                        materialEffect = UniversalFunctions.GetRandomEnum<AllMaterialEffects>();
                    }
                    while (tempMaterialSO.materialEffects.Contains(materialEffect) || (int)materialEffect >= 100);
                    tempMaterialSO.materialEffects.Add(materialEffect);
                }
                //randomize card scraps cost
                int materialValueInt = UnityEngine.Random.Range(80, 100);
                materialNCost.Add(tempMaterialSO, materialValueInt);

                //populate the merchantSaveState with the randomly generated 
                CraftingMaterialWrapper instantiatedCMW = new CraftingMaterialWrapper(tempMaterialSO);
                merchantSaveState.materialOptions.Add(instantiatedCMW);
                merchantSaveState.materialOptionCosts.Add(materialValueInt);
            }
        }
        return merchantSaveState;
    }

    //public void ViewMaterialOptionsOld()
    //{
    //    for (int i = 0; materialNCost.Count > i; i++)
    //    {
    //        //if the prefab is already existing, if it is, it sould always be disabled already
    //        //to check if there are children, under bluePrintContent, use childCount and i comparison
    //        //the foreach above ensures that all blueprint objects are at the beginning so if the current object checked is not a blueprint, we instantiate a new one
    //        if (materialContentTrans.childCount - 1 >= i && materialContentTrans.GetChild(i).gameObject != null)
    //        {
    //            GameObject materialObject = materialContentTrans.GetChild(i).gameObject;
    //            Transform materialTrans = materialObject.transform;
    //            CraftingMaterialSOHolder materialSOHolder = materialObject.GetComponent<CraftingMaterialSOHolder>();

    //            //create the priceTag prefab then instantiate it under the card then set the price
    //            GameObject priceTagObj = materialTrans.GetChild(materialTrans.childCount - 1).gameObject;
    //            PriceTag priceTag = priceTagObj.GetComponent<PriceTag>();
    //            priceTag.SetPriceTag(materialCostList[i]);

    //            //assign accordingly with the SO in list
    //            //enable the option
    //            materialObject.SetActive(true);
    //            //CraftingMaterialSO instantiatedMatSO = Instantiate(materialSOList[i]);
    //            materialSOHolder.ShowMaterialInViewer(materialSOList[i]);
    //        }
    //        //if there is no blueprint Prefab under the content, instantiate a new one
    //        else
    //        {
    //            //instantiate under material content
    //            GameObject materialObject = Instantiate(materialPrefabReference, materialContentTrans);
    //            CraftingMaterialSOHolder materialSOHolder = materialObject.GetComponent<CraftingMaterialSOHolder>();

    //            //create the priceTag prefab then instantiate it under the card then set the price
    //            GameObject priceTagObj = Instantiate(priceTagPrefab, materialObject.transform);
    //            PriceTag priceTag = priceTagObj.GetComponent<PriceTag>();
    //            priceTag.SetPriceTag(materialCostList[i]);

    //            //assign accordingly with the SO in list
    //            //enable the option
    //            materialObject.SetActive(true);
    //            //CraftingMaterialSO instantiatedMatSO = Instantiate(materialSOList[i]);
    //            materialSOHolder.ShowMaterialInViewer(materialSOList[i]);
    //        }
    //    }
    //}

    public void ViewMaterialOptions()
    {
        foreach(KeyValuePair<CraftingMaterialSO, int> MNC in materialNCost)
        {
            bool hasNoDisabledPrefabs = true;            

            foreach (Transform content in materialContentTrans)
            {
                GameObject disabledPrefabs = content.gameObject;
                if (!disabledPrefabs.activeSelf)
                {
                    CraftingMaterialSOHolder materialSOHolder = disabledPrefabs.GetComponent<CraftingMaterialSOHolder>();

                    //create the priceTag prefab then instantiate it under the card then set the price
                    GameObject priceTagObj = content.GetChild(content.childCount - 1).gameObject;
                    PriceTag priceTag = priceTagObj.GetComponent<PriceTag>();
                    priceTag.SetPriceTag(MNC.Value);

                    //assign accordingly with the SO in list
                    //enable the option
                    disabledPrefabs.SetActive(true);
                    //CraftingMaterialSO instantiatedMatSO = Instantiate(materialSOList[i]);
                    materialSOHolder.ShowMaterialInViewer(MNC.Key);
                    //deactivates the identifier so that no new prefabs are instantiated for this iteration
                    hasNoDisabledPrefabs = false;
                    break;
                }            
            }

            if (hasNoDisabledPrefabs)
            {
                //instantiate under material content
                GameObject materialObject = Instantiate(materialPrefabReference, materialContentTrans);
                CraftingMaterialSOHolder materialSOHolder = materialObject.GetComponent<CraftingMaterialSOHolder>();

                //create the priceTag prefab then instantiate it under the card then set the price
                GameObject priceTagObj = Instantiate(priceTagPrefab, materialObject.transform);
                PriceTag priceTag = priceTagObj.GetComponent<PriceTag>();
                priceTag.SetPriceTag(MNC.Value);

                //assign accordingly with the SO in list
                //enable the option
                materialObject.SetActive(true);
                //CraftingMaterialSO instantiatedMatSO = Instantiate(materialSOList[i]);
                materialSOHolder.ShowMaterialInViewer(MNC.Key);
            }
        }
    }

    //used for choosing options
    private void Update()
    {

        PointRay = Input.mousePosition;
        ray = Camera.main.ScreenPointToRay(PointRay);
        pointedObject = Physics2D.GetRayIntersection(ray);

        if (Input.GetMouseButtonDown(0))
        {
            //checkes first if the universalUIs are enabled, Update shouldn't work if they are
            if (!inventoryViewer.activeSelf && !deckViewer.activeSelf && !gearViewer.activeSelf)
            {
                if (pointedObject.collider != null)
                {
                    GameObject selectedObject = pointedObject.collider.gameObject;
                    Transform selectedTrans = selectedObject.transform;
                    if (selectedObject.CompareTag("Material"))
                    {
                        //The material Object's priceTag
                        PriceTag materialPrice = selectedTrans.GetChild(selectedTrans.childCount - 1).GetComponent<PriceTag>();

                        //check if player has enough scraps
                        if (merchantManager.CheckScraps(materialPrice.priceTag))
                        {
                            //the material Object's SO Holder
                            CraftingMaterialSOHolder materialSOHolder = selectedObject.GetComponent<CraftingMaterialSOHolder>();
                            materialNCost.Remove(materialSOHolder.craftingMaterialSO);
                            //update the change to the merchantSaveState
                            UpdateMaterialOptionsSaveState();
                            merchantManager.AddBoughtMaterial(materialSOHolder.craftingMaterialSO);
                            //disable the prefab
                            selectedObject.SetActive(false);
                        }
                    }
                }
            }

            else
            {
                Debug.Log("no material");
            }

        }
    }
    //called when the merchantSaveState is going to be updatd after buying an option
    void UpdateMaterialOptionsSaveState()
    {
        List<CraftingMaterialSO> tempMaterialSOList = new List<CraftingMaterialSO>();
        List<int> tempMaterialCostList = new List<int>();

        foreach (KeyValuePair<CraftingMaterialSO, int> CMW in materialNCost)
        {
            tempMaterialSOList.Add(CMW.Key);
            tempMaterialCostList.Add(CMW.Value);
        }
        //convert the materialSOList to wrappers first before saving
        List<CraftingMaterialWrapper> tempMaterialWrapperList = UniversalFunctions.ConvertCraftingMaterialSOListToWrapper(tempMaterialSOList);
        merchantSaveState.materialOptions = tempMaterialWrapperList;
        merchantSaveState.materialOptionCosts = tempMaterialCostList;
    }

    //back button that disables the UI and saves the merchantSaveState and UniversalInformaetion forreal
    public void MaterialBackButton()
    {
        //disables all children prefab of cards
        foreach (Transform content in materialContentTrans)
        {
            content.gameObject.SetActive(false);
        }
        //disable the CardOptionUI
        gameObject.SetActive(false);

        //updates the card related options in the current instance of merchantSaveState
        UpdateMaterialOptionsSaveState();
        merchantManager.UpdateMerchantSaveState(merchantSaveState);
    }

}
