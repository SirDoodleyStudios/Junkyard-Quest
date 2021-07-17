using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryViewer : MonoBehaviour
{
    //the gameObject itself, used for switching content viewers
    //assigned in editor
    public GameObject blueprintUIParent;
    public GameObject materialUIParent;
    public GameObject gearUIParent;
    //tab buttons
    public Button blueprintTabButton;
    public Button materialTabButton;
    public Button gearTabButton;
    //the scroll content viewers
    public GameObject blueprintContent;
    public GameObject materialContent;
    public GameObject gearContent;
    Transform blueprintContentTrans;
    Transform materialContentTrans;
    Transform gearContentTrans;
    //reference prefbas
    public GameObject materialPrefabReference;
    public GameObject blueprintPrefabReference;
    public GameObject gearPrefabReference;
    //reference SOs
    public BluePrintSO referenceBlueprintSO;
    public CraftingMaterialSO referenceCraftingMaterialSO;
    public GearSO referenceGearSO;

    private void Awake()
    {
        blueprintContentTrans = blueprintContent.transform;
        materialContentTrans = materialContent.transform;
        gearContentTrans = gearContent.transform;
    }


    //called as awake to populate the view screens in inventory prefab
    public void InitializeInventoryView(List<AllGearTypes> blueprints, List<CraftingMaterialWrapper> materials, List<GearWrapper> gear)
    {
        //clear the blueprint and material lists first so that doubling items dosent occur
        List<BluePrintSO> blueprintSOList = new List<BluePrintSO>();
        List<CraftingMaterialSO> craftingMaterialSOList = new List<CraftingMaterialSO>();
        List<GearSO> gearSOList = new List<GearSO>();

        //for populating the blueprints list
        foreach (AllGearTypes blueprint in blueprints)
        {
            BluePrintSO tempBlueprintSO = Instantiate(referenceBlueprintSO);
            tempBlueprintSO.blueprint = blueprint;
            tempBlueprintSO.bluePrintSprite = Resources.Load<Sprite>($"Blueprints/{blueprint}");
            UniversalFunctions.AssignUniqueBlueprintValues(tempBlueprintSO);
            blueprintSOList.Add(tempBlueprintSO);
        }

        //for populating the materials list
        foreach (CraftingMaterialWrapper material in materials)
        {
            CraftingMaterialSO tempMaterialSO = Instantiate(referenceCraftingMaterialSO);
            tempMaterialSO.materialType = material.materialType;
            tempMaterialSO.materialPrefix = material.materialPrefix;
            tempMaterialSO.materialEffects.AddRange(material.materialEffects);
            craftingMaterialSOList.Add(tempMaterialSO);
        }

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

        //for actually populating the screens
        PopulateBlueprintContent(blueprintSOList);
        PopulateMaterialContent(craftingMaterialSOList);
        PopulateGearContent(gearSOList);

        //default first screen is blueprint
        SwitchToBlueprintsButton();
    }

    void PopulateBlueprintContent(List<BluePrintSO> blueprintSOList)
    {
        for (int i = 0; blueprintSOList.Count > i; i++)
        {
            //if the prefab is already existing, if it is, it sould always be disabled already
            //to check if there are children, under bluePrintContent, use childCount and i comparison
            //the foreach above ensures that all blueprint objects are at the beginning so if the current object checked is not a blueprint, we instantiate a new one
            if (blueprintContentTrans.childCount - 1 >= i && blueprintContentTrans.GetChild(i).gameObject != null)
            {
                GameObject blueprintObject = blueprintContentTrans.GetChild(i).gameObject;
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
                GameObject blueprintObject = Instantiate(blueprintPrefabReference, blueprintContentTrans);
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
            if (materialContentTrans.childCount - 1 >= i && materialContentTrans.GetChild(i).gameObject != null)
            {
                GameObject materialObject = materialContentTrans.GetChild(i).gameObject;
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
                GameObject materialObject = Instantiate(materialPrefabReference, materialContentTrans);
                CraftingMaterialSOHolder materialSOHolder = materialObject.GetComponent<CraftingMaterialSOHolder>();
                //assign accordingly with the SO in list
                //enable the option
                materialObject.SetActive(true);
                //CraftingMaterialSO instantiatedMatSO = Instantiate(materialSOList[i]);
                materialSOHolder.ShowMaterialInViewer(craftingMaterialSOList[i]);
            }
        }
    }
    //for populating the gear screen
    void PopulateGearContent(List<GearSO> gearSOList)
    {
        for (int i = 0; gearSOList.Count -1 >= i; i++)
        {
            if (gearContentTrans.childCount - 1 >= i && gearContentTrans.GetChild(i).gameObject != null)
            {
                GameObject gearObject = gearContentTrans.GetChild(i).gameObject;
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
                GameObject gearObject = Instantiate(gearPrefabReference, gearContentTrans);
                GearSOHolder gearSOHolder = gearObject.GetComponent<GearSOHolder>();
                //assign accordingly with the SO in list
                //enable the option
                gearObject.SetActive(true);
                gearSOHolder.InitializeGearPrefab(gearSOList[i]);
            }
        }
    }

    //buttons for switching views on the inventory viewer
    //the inventoryViewer parent's child 0 is materials and 1 is blueprint by default
    public void SwitchToMaterialsButton()
    {
        blueprintUIParent.transform.SetAsFirstSibling();
        gearUIParent.transform.SetAsFirstSibling();
        materialTabButton.interactable = false;
        blueprintTabButton.interactable = true;
        gearTabButton.interactable = true;
    }
    public void SwitchToBlueprintsButton()
    {
        materialUIParent.transform.SetAsFirstSibling();
        gearUIParent.transform.SetAsFirstSibling();
        materialTabButton.interactable = true;
        blueprintTabButton.interactable = false;
        gearTabButton.interactable = true;
    }
    public void SwitchToGearButton()
    {
        materialUIParent.transform.SetAsFirstSibling();
        blueprintUIParent.transform.SetAsFirstSibling();
        materialTabButton.interactable = true;
        blueprintTabButton.interactable = true;
        gearTabButton.interactable = false;
    }

    //disables the UI and all prefabs in the content viewers
    public void CloseInventoryButton()
    {
        foreach (Transform blueprintTrans in blueprintContentTrans)
        {
            blueprintTrans.gameObject.SetActive(false);
        }
        foreach (Transform materialTrans in materialContentTrans)
        {
            materialTrans.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }


}
