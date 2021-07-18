using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingManager : MonoBehaviour
{
    //Rest Manager reference for calling the remaining actions update when a successful craft is done
    //assigned in editor
    public RestManager restManager;

    //delegate function that is used to alter a material slot's collider
    public delegate void D_MaterialSlotColliderAlterer(bool b);
    public event D_MaterialSlotColliderAlterer d_MaterialSlotColliderAlterer;

    //delegate for mass clearing of material slots when a blueprint is chosen
    public delegate void D_MaterialSlotDataClearer();
    public event D_MaterialSlotDataClearer d_MaterialSlotDataClearer;

    //will contain all available bluprints for crafting
    List<AllGearTypes> blueprints = new List<AllGearTypes>();
    //will contain all generated blueprintsSO from blueprints
    List<BluePrintSO> blueprintSOList = new List<BluePrintSO>();

    //will contain all materilSOs available for use
    //will only be instanstiated at start because crafting UI ends after a successful craft
    List<CraftingMaterialSO> materialSOList = new List<CraftingMaterialSO>();
    //separate list that will contain the materialSOs currently in a slot
    List<CraftingMaterialSO> materialSOListSlotted = new List<CraftingMaterialSO>();
    //list that contains all the materialSLot objects
    //used for easily checking on all materialSlots when checking if all slots are occupied
    List<MaterialSlot> materialSlotScripts = new List<MaterialSlot>();

    //assigned from editor
    public Image bluePrintImage;
    public TextMeshProUGUI blueprintName;
    public GameObject materialSlotsPanel;
    //will be used for instantiating the blueprints
    public BluePrintSO referenceBluePrintSO;
    //will be used for instantiating materials
    public CraftingMaterialSO referenceMaterialSO;
    //for instantiating gears
    public GearSO referenceGearSO;
    //button that will trigger the crafting will be made interactable or not depending if all slots are slotted or not
    //the button is default interactable false
    public Button craftButton;

    //the content view for blueprints
    public GameObject craftingUI;
    public GameObject craftingChoiceUIBlueprint;
    public GameObject craftingChoiceUIMaterial;
    public GameObject blueprintContentViewer;
    public GameObject materialContentViewer;
    //reference prefabs of blueprint, material and gear
    public GameObject blueprintReference;
    public GameObject materialReference;
    //the Grid layout group of the scroll content viewer, will be used to set the grid size depending if material or blueprit is to be chosen
    public GridLayoutGroup blueprintGridLayoutGroup;
    public GridLayoutGroup materialGridLayoutGroup;
    //pop-up UI for choosing the effect of the clicked material in content viewer
    public GameObject chooseMaterialEffectUI;
    public ChooseMaterialEffectUI chooseMaterialEffectUIScript;
    //popup to showcase the Crafted Gear
    public GameObject craftedGearShowcasePanel;
    public GearSOHolder craftedGearSOHolder;

    //the load file
    UniversalInformation universalInfo;

    //identifier to check whether the player is choosing materialSlot, material/blueprint, effect, will enable all functions in Update
    //replaced the isChoosing bool identifier and choosingContent
    enum choosingMode { overview, blueprint, material, materialEffect}
    choosingMode choiceEnum;
    //bool isChoosing;
    //will be used for processing the objects chosen during update
    //enum choosingContent { none, blueprint, material}
    //choosingContent chhoiceEnum;

    //holder for the materialSlot prefab to be configured after choosing a material
    GameObject currentMaterialSlot;
    //holder for the current blueprint/gearType chosen
    BluePrintSO chosenBlueprint;




    //for mouse pointing and clicking
    Vector2 PointRay;
    Ray ray;
    RaycastHit2D pointedObject;

    private void Awake()
    {
        universalInfo = UniversalSaveState.LoadUniversalInformation();
        blueprints.AddRange(universalInfo.bluePrints);

        //generate a BluePrintSO for each AllGearTypes in universalInfo
        foreach (AllGearTypes blueprint in blueprints)
        {
            BluePrintSO tempBlueprint = Instantiate(referenceBluePrintSO);
            tempBlueprint.blueprint = blueprint;
            tempBlueprint.bluePrintSprite = Resources.Load<Sprite>($"Blueprints/{blueprint}");
            //send the blueprint ot the AssignBluprintValues to fill out the cevtor and allowable type list
            tempBlueprint = UniversalFunctions.AssignUniqueBlueprintValues(tempBlueprint);
            //add to list
            blueprintSOList.Add(tempBlueprint);
        }
        //automaticaly shows the first blueprint as default
        ShowBluePrint(blueprintSOList[0]);

        //the wrapper for materials should be decoded in the mono scripts
        foreach(CraftingMaterialWrapper wrapper in universalInfo.craftingMaterialWrapperList)
        {
            CraftingMaterialSO tempMaterial = Instantiate(referenceMaterialSO);
            tempMaterial.materialType = wrapper.materialType;
            tempMaterial.materialPrefix = wrapper.materialPrefix;
            //for ddecoding the material
            List<AllMaterialEffects> tempEffectsList = new List<AllMaterialEffects>();
            foreach(AllMaterialEffects effect in wrapper.materialEffects)
            {
                tempEffectsList.Add(effect);
            }
            tempMaterial.materialEffects = tempEffectsList;
            materialSOList.Add(tempMaterial);
        }
    }

    //function to open the blueprint content viewer too choose a blueprint to craft with
    public void ChooseBluePrintButton()
    {
        //enables the viewer gameObject itself
        craftingChoiceUIBlueprint.SetActive(true);
        //enable the blueprint contentviewer
        blueprintContentViewer.SetActive(true);
        blueprintGridLayoutGroup.cellSize = new Vector2(864, 250);
        blueprintGridLayoutGroup.constraintCount = 2;

        Transform blueprintContentTrans = blueprintContentViewer.transform;

        //generate choices depending on how many blueprints are in blueprint list
        //if an there is already an existing prefab in the blueprint choices, enable that then just assign the bluprintSO

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
                GameObject blueprintObject = Instantiate(blueprintReference, blueprintContentTrans);
                BluePrintSOHolder bluePrintSOHolder = blueprintObject.GetComponent<BluePrintSOHolder>();
                //assign accordingly with the SO in list
                bluePrintSOHolder.blueprintSO = blueprintSOList[i];
                //enable the option
                blueprintObject.SetActive(true);
            }
        }

        //enables blueprint logics in update
        //isChoosing = true;
        choiceEnum = choosingMode.blueprint;
        //disables all materialSlot colliders
        d_MaterialSlotColliderAlterer(false);

        //clears all materialSlots first then returns the materials back in materialSOList
        d_MaterialSlotDataClearer();
        materialSOList.AddRange(materialSOListSlotted);
        materialSOListSlotted.Clear();

    }

    //to be called to show the blueprint
    //holds logic to assign the positions of material slots
    //calls are to be received by awake for initial choice, then from another function that chooses a blueprint from a grid list
    void ShowBluePrint(BluePrintSO bluePrintSO)
    {
        //assigns chosen blueprint for the script variable
        chosenBlueprint = bluePrintSO;

        //disable all material slots first before refreshing
        foreach (Transform materialSlot in materialSlotsPanel.transform)
        {
            materialSlot.gameObject.SetActive(false);
        }
        materialSlotScripts.Clear();
        bluePrintImage.sprite = bluePrintSO.bluePrintSprite;
        blueprintName.text = $"{bluePrintSO.blueprint}";

        //check list of positions in SO and assign them to prefab for materialSlots in materialSlots panel
        //the count will also apply for the material types available
        for (int i = 0; bluePrintSO.materialSlotPositions.Count > i; i++)
        {
            //materialSlotsPanel.SetActive(true);
            GameObject materialSlotPrefab = materialSlotsPanel.transform.GetChild(i).gameObject;
            RectTransform materialSlotRect = materialSlotPrefab.GetComponent<RectTransform>();
            MaterialSlot materialSlotScript = materialSlotPrefab.GetComponent<MaterialSlot>();
            //sets the position of the materialSLot prefab
            materialSlotRect.localPosition = bluePrintSO.materialSlotPositions[i];
            //assigns the materialType that this slot will accept
            materialSlotScript.allowableType = bluePrintSO.acceptableMaterialTypes[i];
            //assigns each MaterialSlot prefab in the materialSlotObjects List for processing later
            materialSlotScripts.Add(materialSlotScript);
            materialSlotPrefab.SetActive(true);
        }

        EndChoiceButton();
    }

    //called when a material slot is clicked in blueprint UI
    void ChooseMaterialForSlot()
    {
        craftingChoiceUIMaterial.SetActive(true);
        //enable the blueprint contentviewer
        materialContentViewer.SetActive(true);
        materialGridLayoutGroup.cellSize = new Vector2(864, 250);
        materialGridLayoutGroup.constraintCount = 2;

        choiceEnum = choosingMode.material;

        Transform materialContentTrans = materialContentViewer.transform;

        //will store the acceptableMaterial from the currentMaterialSlotChoice
        AllMaterialTypes acceptableMaterial = currentMaterialSlot.GetComponent<MaterialSlot>().allowableType;
        //separate temporary list that will serve as the filtered list for the materialSOList
        List<CraftingMaterialSO> tempMaterialSOList = new List<CraftingMaterialSO>();
        foreach (CraftingMaterialSO originalSO in materialSOList)
        {
            if(originalSO.materialType == acceptableMaterial)
            {
                tempMaterialSOList.Add(originalSO);
            }
        }

        //decode the mterial wrapper list in universalInfo back to SO
        for (int i = 0; tempMaterialSOList.Count > i; i++)
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
                materialSOHolder.ShowMaterialInViewer(tempMaterialSOList[i]);
            }
            //if there is no blueprint Prefab under the content, instantiate a new one
            else
            {
                //instantiate under material content
                GameObject materialObject = Instantiate(materialReference, materialContentTrans);
                CraftingMaterialSOHolder materialSOHolder = materialObject.GetComponent<CraftingMaterialSOHolder>();
                //assign accordingly with the SO in list
                //enable the option
                materialObject.SetActive(true);
                //CraftingMaterialSO instantiatedMatSO = Instantiate(materialSOList[i]);
                materialSOHolder.ShowMaterialInViewer(tempMaterialSOList[i]);
            }
        }
        //disables all materialSlot colliders
        d_MaterialSlotColliderAlterer(false);
        
    }


    //Function to assign the chosen material to the slot
    //called by the effectChoice buttons in the material Prefab
    //the int parameter which effect index is to be chosen from the holder's effectList
    public void AssignChosenMaterialToSlot(CraftingMaterialSO materialSO, int chosenEffectIndex)
    {
        chooseMaterialEffectUI.SetActive(false);
        //for assigning the objects on material slot
        MaterialSlot materialSlot = currentMaterialSlot.GetComponent<MaterialSlot>();
        materialSlot.AssignMaterial(materialSO, chosenEffectIndex);
        EndChoiceButton();
        //remove the material from list and move to the Slotted list
        materialSOListSlotted.Add(materialSO);
        materialSOList.Remove(materialSO);

    }


    //logic for going back from choice panel
    //assigned in editor
    public void EndChoiceButton()
    {
        if (choiceEnum == choosingMode.blueprint)
        {
            //disables all choices under the content viewer
            foreach (Transform blueprintTrans in blueprintContentViewer.transform)
            {
                GameObject blueprintObject = blueprintTrans.gameObject;
                blueprintObject.SetActive(false);
            }
            //disable the blueprint contentviewer
            blueprintContentViewer.SetActive(false);
            //enables the viewer gameObject itself
            craftingChoiceUIBlueprint.SetActive(false);
        }
        else if (choiceEnum == choosingMode.material)
        {
            foreach (Transform materialTrans in materialContentViewer.transform)
            {
                GameObject materialObject = materialTrans.gameObject;
                materialObject.SetActive(false);
            }
            //disable the material contentviewer
            materialContentViewer.SetActive(false);
            //enables the viewer gameObject itself
            craftingChoiceUIMaterial.SetActive(false);
            //resets the current material Slot
            currentMaterialSlot = null;
        }

        //sets the identifier values to default to avoid accidental triggers in update
        choiceEnum = choosingMode.overview;
        //isChoosing = false;
        d_MaterialSlotColliderAlterer(true);
        //enables or disables the craft button
        CheckCraftGear();
    }


    //going back to rest UI
    public void EndCraftingButton()
    {
        if (materialSOListSlotted.Count != 0)
        {
            materialSOList.AddRange(materialSOListSlotted);
            materialSOListSlotted.Clear();
        }
        //disables the Crafting UI itself
        craftingUI.SetActive(false);
    }

    //checks all materialSLots then makes the craft button interactable
    public void CheckCraftGear()
    {
        int materialSlotCounter = 0;
        foreach (MaterialSlot materialSlotScript in materialSlotScripts)
        {
            //checks if the slot has an assigned material
            //if yes add to counter, if no, break loop
            if(materialSlotScript.craftingMaterialSO != null)
            {
                materialSlotCounter++;
            }
            else
            {
                break;
            }
        }

        //if the counter reached the materialSlotScripts count, it means that all materialSlots has a craftingMaterialSO then make the button interactable
        if (materialSlotCounter == materialSlotScripts.Count)
        {
            craftButton.interactable = true;
        }
        else
        {
            craftButton.interactable = false;
        }
    }
    //proceeds to crafting
    public void CraftGearButton()
    {

        d_MaterialSlotDataClearer();
        //the slotted list gets cleared so that the used materials are permanently removed
        materialSOListSlotted.Clear();
        EndCraftingButton();

        //instantiate the gear then showcase it to the shocase UI
        GearSO craftedGear = Instantiate(referenceGearSO);

        //get gear type and classification from the blueprintSO
        craftedGear.gearType = chosenBlueprint.blueprint;
        craftedGear.gearClassifications = chosenBlueprint.gearClassifications;


        //bool identifier to check if the materials usedin the gear are the same prefix
        //will start as true then becomes false if the current material being checked's prefix doesn't match the previous
        bool isSameMaterialType = true;

        //scans the materialSlots and get their chosen effects
        for (int i = 0; materialSlotScripts.Count - 1 >= i; i++)
        {
            //adds the effect in the gear effectList
            craftedGear.gearEffects.Add(materialSlotScripts[i].materialEffect);

            //if the material is the first one being checked, skip the bool logic
            //will only keep checking if the identifier is still true so that we don't always check when there's no need if it's already false
            if (isSameMaterialType == true)
            {
                if (i == 0)
                {
                    //set the firs prefix as the final if the logic pushes through
                    craftedGear.gearSetBonus = materialSlotScripts[0].materialPrefix;
                }
                else
                {
                    //if the current and previous material prefix is not the same, make the final prefix Normal
                    if (materialSlotScripts[i - 1].materialPrefix != materialSlotScripts[i].materialPrefix)
                    {
                        isSameMaterialType = false;
                        craftedGear.gearSetBonus = AllMaterialPrefixes.Normal;
                    }
                }
            }
        }

        //determines a bonus effect if all all materials share the same prefix
        if (isSameMaterialType == true)
        {
            switch (craftedGear.gearSetBonus)
            {
                case AllMaterialPrefixes.Normal:
                    break;
                case AllMaterialPrefixes.Sturdy:
                    craftedGear.gearEffects.Add(AllMaterialEffects.Sturdy);
                    break;
                case AllMaterialPrefixes.Mysterious:
                    craftedGear.gearEffects.Add(AllMaterialEffects.Mysterious);
                    break;
                case AllMaterialPrefixes.Fancy:
                    craftedGear.gearEffects.Add(AllMaterialEffects.Fancy);
                    break;
                default:
                    break;
            }
        }

        craftedGearShowcasePanel.SetActive(true);
        craftedGearSOHolder.InitializeGearPrefab(craftedGear);

        //saves to universalInfo, restmanager has the saving function
        //updates materials and gear in the save file
        restManager.UpdateMaterialAndGearList(materialSOList, craftedGear);
        restManager.UpdateRemainingActions();

    }
    //proceed button for the Created Gear Scene, just for disabling the UiI and clearing the showcase prefab
    public void EndGearShowcaseButton()
    {
        craftedGearSOHolder.ClearGearPrefab();
        craftedGearShowcasePanel.SetActive(false);
    }


    //for clicking choices when picking a blueprint or material
    private void Update()
    {
        PointRay = Input.mousePosition;
        ray = Camera.main.ScreenPointToRay(PointRay);
        pointedObject = Physics2D.GetRayIntersection(ray);

        //if the clicked object is a card and has a collider
        if (Input.GetMouseButtonDown(0))
        {

        //update is for choosing a an option in blueprint and material choice, will only turn true if the buttons to choose are clicked

            if(choiceEnum == choosingMode.overview)
            {
                if(pointedObject.collider != null)
                {
                    //assign the chosen prefab slot as the current slot
                    //the parent is the actual materialSlot since the clickable part is only a child
                    currentMaterialSlot = pointedObject.collider.gameObject.transform.parent.gameObject;
                    if (currentMaterialSlot.GetComponent<MaterialSlot>().craftingMaterialSO != null)
                    {
                        //rejoins Crfting SO in the list if there is a material in the slot then clears the MaterialSlot object
                        MaterialSlot materialSlot = currentMaterialSlot.GetComponent<MaterialSlot>();
                        materialSOList.Add(materialSlot.craftingMaterialSO);
                        materialSOListSlotted.Remove(materialSlot.craftingMaterialSO);
                        materialSlot.ClearMaterialSlot();
                    }
                    ChooseMaterialForSlot();
                }

            }
            //logic for choice if choosing for blueprint
            else if (choiceEnum == choosingMode.blueprint)
            {


                if(pointedObject.collider != null)
                {
                    GameObject chosenPrefab = pointedObject.collider.gameObject;

                    //send the SO assigned for the blueprint prefab
                    ShowBluePrint(chosenPrefab.GetComponent<BluePrintSOHolder>().blueprintSO);
                }

            }
            //logic for choosing materials
            else if (choiceEnum == choosingMode.material)
            {
                if (pointedObject.collider != null)
                {
                    GameObject chosenMaterial = pointedObject.collider.gameObject;
                    //sends the CraftingMaterialHolder to the prefab inside the chooseEffectUI in the script
                    //ActivateChooseMaterialEffect(pointedObject.collider.gameObject.GetComponent<CraftingMaterialSOHolder>());
                    chooseMaterialEffectUI.SetActive(true);

                    chooseMaterialEffectUIScript.StartMaterialEffectChoice(chosenMaterial.GetComponent<CraftingMaterialSOHolder>().craftingMaterialSO);
                    //this is to prevent misclicks in the materialContent viewer after effectChoiceUI becomes active
                    materialContentViewer.SetActive(false);

                }
            }            
            
        }

        //TEST ON THE LIST REMOVE THT LEAVES A NULL
        if (Input.GetMouseButtonDown(1))
        {
            materialSOList.RemoveRange(Random.Range(0, materialSOList.Count - 1), 1);
        }
        
    }

}
