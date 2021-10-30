using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeActivities : MonoBehaviour
{
    //delegate event for disabling all inventory objects for trade when confirming or skipping trade
    public delegate void D_DisableInventoryObjects();
    public event D_DisableInventoryObjects d_DisableInventoryObjects;

    //references assigned in editor
    //content viewers, these objects are actually the child objects of the viewports that contains the rectTransform
    //linkActivityManager
    public LinkActivitiesManager linkActivitiesManager;
    //trade objects
    public GameObject offerContent;
    Transform offerTrans;
    RectTransform offerRect;
    //the object prefab that will be generated as the traded offer
    //can be gear, material, or blueprint
    GameObject offerObj;
    //inventory conentes
    public GameObject materialInventoryContent;
    Transform materialInventoryTrans;
    GridLayoutGroup materialInventoryGrid;
    public GameObject blueprintInventoryContent;
    Transform blueprintInventoryTrans;
    GridLayoutGroup blueprintInventoryGrid;
    public GameObject gearInventoryContent;
    Transform gearInventoryTrans;
    GridLayoutGroup gearInventoryGrid;
    //reference prefab objects
    public GameObject referenceGearPrefab;
    public GearSO referenceGearSO;
    public GameObject referenceMaterialPrefab;
    public CraftingMaterialSO referenceMaterialSO;
    public GameObject referenceBlueprintPrefab;
    public BluePrintSO referenceBlueprintSO;
    //cameraUIScript
    public CameraUIScript cameraUIScript;


    //lists that will initially hold the inventory SOs
    //will be altered when a trade is confirmed
    List<GearSO> gearSOList = new List<GearSO>();
    List<CraftingMaterialSO> craftingMaterialSOList = new List<CraftingMaterialSO>();
    List<BluePrintSO> blueprintSOList = new List<BluePrintSO>();

    //Assigned in the Initialize function
    //determined tradeType
    LinkActivityEnum tradeActivity;
    //the universalInof being used
    UniversalInformation universalInfo;
    //linkActivitySaveState being used
    LinkActivitiesSaveState linkActivitySaveState;

    //placeHolder for the inventory lists to be populated

    private void Awake()
    {
        offerTrans = offerContent.transform;
        offerRect = offerContent.GetComponent<RectTransform>();
        materialInventoryTrans = materialInventoryContent.transform;
        materialInventoryGrid = materialInventoryContent.GetComponent<GridLayoutGroup>();
        blueprintInventoryTrans = blueprintInventoryContent.transform;
        blueprintInventoryGrid = blueprintInventoryContent.GetComponent<GridLayoutGroup>();
        gearInventoryTrans = gearInventoryContent.transform;
        gearInventoryGrid = gearInventoryContent.GetComponent<GridLayoutGroup>();
    }


    //initialize trade
    //called by LinkActivitiesManager to determine what kind of trade is needed
    //parameter is always a trade type, must only be called during trades
    //receives a linkActivitiesSaveState and then returns it back to linkActivitiesManager to save the generated offer object
    public LinkActivitiesSaveState InitializeTradeActivities(LinkActivityEnum tradeType, UniversalInformation universalInfoInternal, LinkActivitiesSaveState linkActivitySaveStateInternal)
    {

        tradeActivity = tradeType;
        universalInfo = universalInfoInternal;
        linkActivitySaveState = linkActivitySaveStateInternal;

        //populate the offer object
        //will edit the LinkActivitySaveState with the new generated offer objects
        PopulateOfferContent(tradeType, linkActivitySaveStateInternal.isStillInActivity);

        if (tradeType == LinkActivityEnum.BlueprintTrade)
        {
            //enable corresponding content
            blueprintInventoryContent.SetActive(true);

            //w = 864, h = 250 in 1902x1080
            blueprintInventoryGrid.cellSize = new Vector2(Screen.width * .45f, Screen.height * .23f);

            List<AllGearTypes> blueprints = universalInfoInternal.bluePrints;
            foreach (AllGearTypes blueprint in blueprints)
            {
                BluePrintSO tempBlueprintSO = Instantiate(referenceBlueprintSO);
                tempBlueprintSO.blueprint = blueprint;
                tempBlueprintSO.bluePrintSprite = Resources.Load<Sprite>($"Blueprints/{blueprint}");
                UniversalFunctions.AssignUniqueBlueprintValues(tempBlueprintSO);
                blueprintSOList.Add(tempBlueprintSO);
            }
            PopulateBlueprintContent(blueprintSOList);
        }
        else if (tradeType == LinkActivityEnum.MaterialTrade)
        {
            //enable corresponding content
            materialInventoryContent.SetActive(true);

            //w = 864, h = 250 in 1902x1080
            materialInventoryGrid.cellSize = new Vector2(Screen.width * .45f, Screen.height * .23f);

            List<CraftingMaterialWrapper> materials = universalInfoInternal.craftingMaterialWrapperList;
            //for populating the materials list
            foreach (CraftingMaterialWrapper material in materials)
            {
                CraftingMaterialSO tempMaterialSO = Instantiate(referenceMaterialSO);
                tempMaterialSO.materialType = material.materialType;
                tempMaterialSO.materialPrefix = material.materialPrefix;
                tempMaterialSO.materialEffects.AddRange(material.materialEffects);
                craftingMaterialSOList.Add(tempMaterialSO);
            }
            PopulateMaterialContent(craftingMaterialSOList);

        }
        else if(tradeType == LinkActivityEnum.GearTrade)
        {
            //enable corresponding content
            gearInventoryContent.SetActive(true);

            //w = 864, h = 325 in 1902x1080
            gearInventoryGrid.cellSize = new Vector2(Screen.width * .45f, Screen.height * .30f);

            List<GearWrapper> gear = universalInfoInternal.gearWrapperList;
            //for populating the gearList
            foreach (GearWrapper gearWrapper in gear)
            {
                GearSO tempGearSO = Instantiate(referenceGearSO);
                tempGearSO.gearEffects.AddRange(gearWrapper.gearEffects);
                tempGearSO.gearClassifications = gearWrapper.gearClassifications;
                tempGearSO.gearSetBonus = gearWrapper.gearSetBonus;
                tempGearSO.gearType = gearWrapper.gearType;
                gearSOList.Add(tempGearSO);
            }
            PopulateGearContent(gearSOList);
        }
        else
        {

        }

        //return the now edited LinkActivitySaveState
        return linkActivitySaveState;
    }

    //creates the randokmized item based on the trade type
    void PopulateOfferContent(LinkActivityEnum tradeType, bool isLoadedFromFile)
    {
        //assign directly from file if loaded
        if (isLoadedFromFile)
        {
            if (tradeType == LinkActivityEnum.BlueprintTrade)
            {
                BluePrintSO tempBlueprint = Instantiate(referenceBlueprintSO);
                //assign the blueprint values
                AllGearTypes loadedBlueprint = linkActivitySaveState.blueprint;
                tempBlueprint.blueprint = loadedBlueprint;
                tempBlueprint.bluePrintSprite = Resources.Load<Sprite>($"Blueprints/{loadedBlueprint}");
                //send the blueprint ot the AssignBluprintValues to fill out the vecttor and allowable type list
                tempBlueprint = UniversalFunctions.AssignUniqueBlueprintValues(tempBlueprint);
                //instantiate a blueprintPrefab and assign put it in the offer panel
                offerObj = Instantiate(referenceBlueprintPrefab, offerTrans);
                offerObj.GetComponent<BluePrintSOHolder>().blueprintSO = tempBlueprint;
                offerObj.SetActive(true);
                //center the offer object and set the size
                RectTransform offerRect = offerObj.GetComponent<RectTransform>();
                offerRect.anchorMin = new Vector2(.5f, .5f);
                offerRect.anchorMax = new Vector2(.5f, .5f);
                offerRect.sizeDelta = new Vector2(Screen.width * .45f, Screen.height * .23f);

            }
            else if (tradeType == LinkActivityEnum.MaterialTrade)
            {
                //randomize ints for the components of the material being built
                CraftingMaterialSO tempMaterialSO = Instantiate(referenceMaterialSO);
                tempMaterialSO.materialType = linkActivitySaveState.craftingMaterialWrapper.materialType;
                tempMaterialSO.materialPrefix = linkActivitySaveState.craftingMaterialWrapper.materialPrefix;
                tempMaterialSO.materialEffects = linkActivitySaveState.craftingMaterialWrapper.materialEffects;

                offerObj = Instantiate(referenceMaterialPrefab, offerTrans);
                CraftingMaterialSOHolder materialSOHolder = offerObj.GetComponent<CraftingMaterialSOHolder>();
                materialSOHolder.ShowMaterialInViewer(tempMaterialSO);
                offerObj.SetActive(true);
                //center the offer object and set the size
                RectTransform offerRect = offerObj.GetComponent<RectTransform>();
                offerRect.anchorMin = new Vector2(.5f, .5f);
                offerRect.anchorMax = new Vector2(.5f, .5f);
                offerRect.sizeDelta = new Vector2(Screen.width * .45f, Screen.height * .23f);
            }
            else if (tradeType == LinkActivityEnum.GearTrade)
            {
                //randomize ints for the components of the material being built
                GearSO instantiatedGearSO = Instantiate(referenceGearSO);

                instantiatedGearSO.gearType = linkActivitySaveState.gearWrapper.gearType;
                instantiatedGearSO.gearSetBonus = linkActivitySaveState.gearWrapper.gearSetBonus;
                instantiatedGearSO.gearEffects = linkActivitySaveState.gearWrapper.gearEffects;

                offerObj = Instantiate(referenceGearPrefab, offerTrans);
                GearSOHolder gearSOHolder = offerObj.GetComponent<GearSOHolder>();
                gearSOHolder.InitializeGearPrefab(instantiatedGearSO);
                offerObj.SetActive(true);
                //center the offer object and set the size
                RectTransform offerRect = offerObj.GetComponent<RectTransform>();
                offerRect.anchorMin = new Vector2(.5f, .5f);
                offerRect.anchorMax = new Vector2(.5f, .5f);
                offerRect.sizeDelta = new Vector2(Screen.width * .45f, Screen.height * .30f);
            }
        }


        //generate new one if not loaded from file
        else
        {
            if (tradeType == LinkActivityEnum.BlueprintTrade)
            {
                BluePrintSO tempBlueprint = Instantiate(referenceBlueprintSO);

                //assign the blueprint values
                //fetch the list of blueprints not yet in player possession
                List<AllGearTypes> availableBlueprints = UniversalFunctions.SearchAvailableBlueprints(universalInfo.bluePrints);
                AllGearTypes randomizedBlueprint = availableBlueprints[Random.Range(0, availableBlueprints.Count-1)];

                tempBlueprint.blueprint = randomizedBlueprint;
                tempBlueprint.bluePrintSprite = Resources.Load<Sprite>($"Blueprints/{randomizedBlueprint}");
                //send the blueprint ot the AssignBluprintValues to fill out the vecttor and allowable type list
                tempBlueprint = UniversalFunctions.AssignUniqueBlueprintValues(tempBlueprint);
                //instantiate a blueprintPrefab and assign put it in the offer panel
                offerObj = Instantiate(referenceBlueprintPrefab, offerTrans);
                offerObj.GetComponent<BluePrintSOHolder>().blueprintSO = tempBlueprint;
                offerObj.SetActive(true);
                //center the offer object and set the size
                RectTransform offerRect = offerObj.GetComponent<RectTransform>();
                offerRect.anchorMin = new Vector2(.5f, .5f);
                offerRect.anchorMax = new Vector2(.5f, .5f);
                offerRect.sizeDelta = new Vector2(Screen.width * .45f, Screen.height * .23f);
                //save the generated offer object in linkActivitiesSaveState
                linkActivitySaveState.blueprint = randomizedBlueprint;

            }
            else if (tradeType == LinkActivityEnum.MaterialTrade)
            {
                //randomize ints for the components of the material being built
                CraftingMaterialSO tempMaterialSO = Instantiate(referenceMaterialSO);
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

                offerObj = Instantiate(referenceMaterialPrefab, offerTrans);
                CraftingMaterialSOHolder materialSOHolder = offerObj.GetComponent<CraftingMaterialSOHolder>();
                materialSOHolder.ShowMaterialInViewer(tempMaterialSO);
                offerObj.SetActive(true);
                //center the offer object and set the size
                RectTransform offerRect = offerObj.GetComponent<RectTransform>();
                offerRect.anchorMin = new Vector2(.5f, .5f);
                offerRect.anchorMax = new Vector2(.5f, .5f);
                offerRect.sizeDelta = new Vector2(Screen.width * .45f, Screen.height * .23f);
                //save the generated offer object in linkActivitiesSaveState
                linkActivitySaveState.craftingMaterialWrapper = new CraftingMaterialWrapper(tempMaterialSO);

            }
            else if (tradeType == LinkActivityEnum.GearTrade)
            {
                //randomize ints for the components of the material being built
                GearSO instantiatedGearSO = Instantiate(referenceGearSO);

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
                offerObj = Instantiate(referenceGearPrefab, offerTrans);
                GearSOHolder gearSOHolder = offerObj.GetComponent<GearSOHolder>();
                gearSOHolder.InitializeGearPrefab(instantiatedGearSO);
                offerObj.SetActive(true);
                //center the offer object and set the size
                RectTransform offerRect = offerObj.GetComponent<RectTransform>();
                offerRect.anchorMin = new Vector2(.5f, .5f);
                offerRect.anchorMax = new Vector2(.5f, .5f);
                offerRect.sizeDelta = new Vector2(Screen.width * .45f, Screen.height * .30f);
                //save the generated offer object in linkActivitiesSaveState
                linkActivitySaveState.gearWrapper = new GearWrapper(instantiatedGearSO);


            }
            else
            {
                //shouldn't happen
                Debug.Log("offerObject nulling");
                offerObj = null;
            }
        }

        //remove the DragNDrop script from the offer object 
        Destroy(offerObj.GetComponent<TradeDragNDrop>());


    }

    void PopulateBlueprintContent(List<BluePrintSO> blueprintSOList)
    {
        for (int i = 0; blueprintSOList.Count > i; i++)
        {
            //if the prefab is already existing, if it is, it sould always be disabled already
            //to check if there are children, under bluePrintContent, use childCount and i comparison
            //the foreach above ensures that all blueprint objects are at the beginning so if the current object checked is not a blueprint, we instantiate a new one
            if (blueprintInventoryTrans.childCount - 1 >= i && blueprintInventoryTrans.GetChild(i).gameObject != null)
            {
                GameObject blueprintObject = blueprintInventoryTrans.GetChild(i).gameObject;
                BluePrintSOHolder bluePrintSOHolder = blueprintObject.GetComponent<BluePrintSOHolder>();
                //assign accordingly with the SO in list
                bluePrintSOHolder.blueprintSO = blueprintSOList[i];
                //enable the option
                blueprintObject.SetActive(true);
            }
            //if there is no blueprint Prefab under the content, instantiate a new one
            else
            {
                //instantiate under bluprint content
                GameObject blueprintObject = Instantiate(referenceBlueprintPrefab, blueprintInventoryTrans);
                BluePrintSOHolder bluePrintSOHolder = blueprintObject.GetComponent<BluePrintSOHolder>();
                //assign accordingly with the SO in list
                bluePrintSOHolder.blueprintSO = blueprintSOList[i];

                //enable the option
                blueprintObject.SetActive(true);
            }
        }
        //called to assign the tradeActivities as reference script to the instantiated objects
        foreach (Transform inventory in blueprintInventoryTrans)
        {
            inventory.GetComponent<TradeDragNDrop>().AssignTradeReference(this);
        }
    }

    void PopulateMaterialContent(List<CraftingMaterialSO> craftingMaterialSOList)
    {
        for (int i = 0; craftingMaterialSOList.Count > i; i++)
        {
            //if the prefab is already existing, if it is, it sould always be disabled already
            //to check if there are children, under bluePrintContent, use childCount and i comparison
            //the foreach above ensures that all blueprint objects are at the beginning so if the current object checked is not a blueprint, we instantiate a new one
            if (materialInventoryTrans.childCount - 1 >= i && materialInventoryTrans.GetChild(i).gameObject != null)
            {
                GameObject materialObject = materialInventoryTrans.GetChild(i).gameObject;
                CraftingMaterialSOHolder materialSOHolder = materialObject.GetComponent<CraftingMaterialSOHolder>();
                //assign accordingly with the SO in list
                //enable the option
                materialObject.SetActive(true);
                //CraftingMaterialSO instantiatedMatSO = Instantiate(materialSOList[i]);
                materialSOHolder.ShowMaterialInViewer(craftingMaterialSOList[i]);
            }
            //if there is no blueprint Prefab under the content, instantiate a new one
            else
            {
                //instantiate under material content
                GameObject materialObject = Instantiate(referenceMaterialPrefab, materialInventoryTrans);
                CraftingMaterialSOHolder materialSOHolder = materialObject.GetComponent<CraftingMaterialSOHolder>();
                //assign accordingly with the SO in list
                //enable the option
                materialObject.SetActive(true);
                //CraftingMaterialSO instantiatedMatSO = Instantiate(materialSOList[i]);
                materialSOHolder.ShowMaterialInViewer(craftingMaterialSOList[i]);
            }
        }
        //called to assign the tradeActivities as reference script to the instantiated objects
        foreach (Transform inventory in materialInventoryTrans)
        {
            inventory.GetComponent<TradeDragNDrop>().AssignTradeReference(this);
        }
    }

    void PopulateGearContent(List<GearSO> gearSOList)
    {
        for (int i = 0; gearSOList.Count - 1 >= i; i++)
        {
            if (gearInventoryTrans.childCount - 1 >= i && gearInventoryTrans.GetChild(i).gameObject != null)
            {
                GameObject gearObject = materialInventoryTrans.GetChild(i).gameObject;
                GearSOHolder gearSOHolder = gearObject.GetComponent<GearSOHolder>();
                //assign accordingly with the SO in list
                //enable the option
                gearObject.SetActive(true);
                //CraftingMaterialSO instantiatedMatSO = Instantiate(materialSOList[i]);
                gearSOHolder.InitializeGearPrefab(gearSOList[i]);
            }
            //if there is no blueprint Prefab under the content, instantiate a new one
            else
            {
                //instantiate under material content
                GameObject gearObject = Instantiate(referenceGearPrefab, gearInventoryTrans);
                GearSOHolder gearSOHolder = gearObject.GetComponent<GearSOHolder>();
                //assign accordingly with the SO in list
                //enable the option
                gearObject.SetActive(true);
                gearSOHolder.InitializeGearPrefab(gearSOList[i]);
            }
        }
        //called to assign the tradeActivities as reference script to the instantiated objects
        foreach (Transform inventory in gearInventoryTrans)
        {
            inventory.GetComponent<TradeDragNDrop>().AssignTradeReference(this);
        }
    }

    //called by the traderagNDrop when a material is chosen
    public void ConfirmTrade(GameObject inventoryObj)
    {
        if (tradeActivity == LinkActivityEnum.BlueprintTrade)
        {
            //List of blueprints is easily and readily manipulated in saveState because they're just single value elements that can't repeat
            //remove the chosen inventory from trade
            AllGearTypes tradeOut = inventoryObj.GetComponent<BluePrintSOHolder>().blueprintSO.blueprint;
            universalInfo.bluePrints.Remove(tradeOut);
            cameraUIScript.UpdateBlueprintInventory(tradeOut, false);
            //add te gearSO in the offer object
            AllGearTypes tradeIn = offerObj.GetComponent<BluePrintSOHolder>().blueprintSO.blueprint;
            universalInfo.bluePrints.Add(tradeIn);
            cameraUIScript.UpdateBlueprintInventory(tradeIn, true);
        }
        else if (tradeActivity == LinkActivityEnum.MaterialTrade)
        {
            //remove the chosen inventory from trade
            CraftingMaterialSO tradeOut = inventoryObj.GetComponent<CraftingMaterialSOHolder>().craftingMaterialSO;
            craftingMaterialSOList.Remove(tradeOut);
            //add te gearSO in the offer object
            CraftingMaterialSO tradeIn = offerObj.GetComponent<CraftingMaterialSOHolder>().craftingMaterialSO;
            craftingMaterialSOList.Add(tradeIn);
            //reconstruct the wrappers for saving
            List<CraftingMaterialWrapper> materialWrappers = UniversalFunctions.ConvertCraftingMaterialSOListToWrapper(craftingMaterialSOList);
            universalInfo.craftingMaterialWrapperList = materialWrappers;
            //update the UI info
            cameraUIScript.UpdateFullMaterialInventory(materialWrappers);
        }
        else if (tradeActivity == LinkActivityEnum.GearTrade)
        {
            //remove the chosen inventory from trade
            GearSO tradeOut = inventoryObj.GetComponent<GearSOHolder>().gearSO;
            gearSOList.Remove(tradeOut);
            //add te gearSO in the offer object
            GearSO tradeIn = offerObj.GetComponent<GearSOHolder>().gearSO;
            gearSOList.Add(tradeIn);
            //reconstruct the wrappers for saving
            List<GearWrapper> gearWrappers = UniversalFunctions.ConvertGearSOListToWrapper(gearSOList);
            universalInfo.gearWrapperList = gearWrappers;
            //update the UI info
            cameraUIScript.UpdateFullGearInventory(gearWrappers);

        }
        else
        {

        }

        //save the universalInfo
        UniversalSaveState.SaveUniversalInformation(universalInfo);
        cameraUIScript.UpdateUniversalInfo();

        //close and resets the trade panel
        CloseTradePanel();
    }


    public void CloseTradePanel()
    {
        //delete offer object only
        Destroy(offerObj);

        //disable all contents and content holders
        if (d_DisableInventoryObjects!=null)
        {
            d_DisableInventoryObjects();
        }
        materialInventoryContent.SetActive(false);
        blueprintInventoryContent.SetActive(false);
        gearInventoryContent.SetActive(false);

        //disables the trade panel
        gameObject.SetActive(false);

        //call linkActivityManager to update the link saveState to false isInActivity since we're moving back to path
        linkActivitiesManager.UpdateLinkSaveState(false);
    }
}

