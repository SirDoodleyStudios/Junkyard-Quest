using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeActivities : MonoBehaviour
{
    //references assigned in editor
    //content viewers, these objects are actually the child objects of the viewports that contains the rectTransform
    public GameObject offerContent;
    Transform offerTrans;
    RectTransform offerRect;
    public GameObject inventoryContent;
    Transform inventoryTrans;
    RectTransform inventoryRect;
    //reference prefab objects
    public GameObject referenceGearPrefab;
    public GearSO referenceGearSO;
    public GameObject referenceMaterialPrefab;
    public CraftingMaterialSO referenceMaterialSO;
    public GameObject referenceBlueprintPrefab;
    public BluePrintSO referenceBlueprintSO;

    //placeHolder for the inventory lists to be populated

    private void Awake()
    {
        offerTrans = offerContent.transform;
        offerRect = offerContent.GetComponent<RectTransform>();
        inventoryTrans = inventoryContent.transform;
        inventoryRect = inventoryContent.GetComponent<RectTransform>();
    }


    //initialize trade
    //called by LinkActivitiesManager to determine what kind of trade is needed
    //parameter is always a trade type, must only be called during trades
    public void InitializeTradeActivities(LinkActivityEnum tradeType, UniversalInformation universalInfo)
    {
        //populate the offer object
        PopulateOfferContent(tradeType);

        if (tradeType == LinkActivityEnum.BlueprintTrade)
        {
            List<AllGearTypes> blueprints = universalInfo.bluePrints;
            List<BluePrintSO> blueprintSOList = new List<BluePrintSO>();
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
            List<CraftingMaterialWrapper> materials = universalInfo.craftingMaterialWrapperList;
            List<CraftingMaterialSO> craftingMaterialSOList = new List<CraftingMaterialSO>();
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
            List<GearWrapper> gear = universalInfo.gearWrapperList;
            List<GearSO> gearSOList = new List<GearSO>();
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
    }

    //creates the randokmized item based on the trade type
    void PopulateOfferContent(LinkActivityEnum tradeType)
    {
        if (tradeType == LinkActivityEnum.BlueprintTrade)
        {
            BluePrintSO tempBlueprint = Instantiate(referenceBlueprintSO);

            //assign the blueprint values
            AllGearTypes randomizedBlueprint = UniversalFunctions.GetRandomEnum<AllGearTypes>();
            tempBlueprint.blueprint = randomizedBlueprint;
            tempBlueprint.bluePrintSprite = Resources.Load<Sprite>($"Blueprints/{randomizedBlueprint}");
            //send the blueprint ot the AssignBluprintValues to fill out the vecttor and allowable type list
            tempBlueprint = UniversalFunctions.AssignUniqueBlueprintValues(tempBlueprint);
            //instantiate a blueprintPrefab and assign put it in the offer panel
            GameObject offerObj = Instantiate(referenceBlueprintPrefab, offerTrans);
            offerObj.GetComponent<BluePrintSOHolder>().blueprintSO = tempBlueprint;
            offerObj.SetActive(true);


        }
        else if (tradeType == LinkActivityEnum.MaterialTrade)
        {

        }
        else if (tradeType == LinkActivityEnum.GearTrade)
        {

        }
        else
        {

        }
    }

    void PopulateBlueprintContent(List<BluePrintSO> blueprintSOList)
    {
        for (int i = 0; blueprintSOList.Count > i; i++)
        {
            //if the prefab is already existing, if it is, it sould always be disabled already
            //to check if there are children, under bluePrintContent, use childCount and i comparison
            //the foreach above ensures that all blueprint objects are at the beginning so if the current object checked is not a blueprint, we instantiate a new one
            if (inventoryTrans.childCount - 1 >= i && inventoryTrans.GetChild(i).gameObject != null)
            {
                GameObject blueprintObject = inventoryTrans.GetChild(i).gameObject;
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
                GameObject blueprintObject = Instantiate(referenceBlueprintPrefab, inventoryTrans);
                BluePrintSOHolder bluePrintSOHolder = blueprintObject.GetComponent<BluePrintSOHolder>();
                //assign accordingly with the SO in list
                bluePrintSOHolder.blueprintSO = blueprintSOList[i];
                //enable the option
                blueprintObject.SetActive(true);
            }
        }
    }

    void PopulateMaterialContent(List<CraftingMaterialSO> craftingMaterialSOList)
    {
        for (int i = 0; craftingMaterialSOList.Count > i; i++)
        {
            //if the prefab is already existing, if it is, it sould always be disabled already
            //to check if there are children, under bluePrintContent, use childCount and i comparison
            //the foreach above ensures that all blueprint objects are at the beginning so if the current object checked is not a blueprint, we instantiate a new one
            if (inventoryTrans.childCount - 1 >= i && inventoryTrans.GetChild(i).gameObject != null)
            {
                GameObject materialObject = inventoryTrans.GetChild(i).gameObject;
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
                GameObject materialObject = Instantiate(referenceMaterialPrefab, inventoryTrans);
                CraftingMaterialSOHolder materialSOHolder = materialObject.GetComponent<CraftingMaterialSOHolder>();
                //assign accordingly with the SO in list
                //enable the option
                materialObject.SetActive(true);
                //CraftingMaterialSO instantiatedMatSO = Instantiate(materialSOList[i]);
                materialSOHolder.ShowMaterialInViewer(craftingMaterialSOList[i]);
            }
        }
    }

    void PopulateGearContent(List<GearSO> gearSOList)
    {
        for (int i = 0; gearSOList.Count - 1 >= i; i++)
        {
            if (inventoryTrans.childCount - 1 >= i && inventoryTrans.GetChild(i).gameObject != null)
            {
                GameObject gearObject = inventoryTrans.GetChild(i).gameObject;
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
                GameObject gearObject = Instantiate(referenceGearPrefab, inventoryTrans);
                GearSOHolder gearSOHolder = gearObject.GetComponent<GearSOHolder>();
                //assign accordingly with the SO in list
                //enable the option
                gearObject.SetActive(true);
                gearSOHolder.InitializeGearPrefab(gearSOList[i]);
            }
        }
    }
}

