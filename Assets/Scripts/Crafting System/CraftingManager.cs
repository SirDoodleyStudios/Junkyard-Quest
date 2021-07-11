using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //assigned from editor
    public Image bluePrintImage;
    public GameObject materialSlotsPanel;
    //will be used for instantiating the blueprints
    public BluePrintSO referenceBluePrintSO;
    //will be used for instantiating materials
    public CraftingMaterialSO referenceMaaterialSO;

    //the content view for blueprints
    public GameObject craftingChoiceUIBlueprint;
    public GameObject craftingChoiceUIMaterial;
    public GameObject blueprintContentViewer;
    public GameObject materialContentViewer;
    //reference prefabs of blueprint and material
    public GameObject blueprintReference;
    public GameObject materialReference;
    //the Grid layout group of the scroll content viewer, will be used to set the grid size depending if material or blueprit is to be chosen
    public GridLayoutGroup blueprintGridLayoutGroup;
    public GridLayoutGroup materialGridLayoutGroup;
    //pop-up UI for choosing the effect of the clicked material in content viewer
    public GameObject chooseMaterialEffectUI;
    public ChooseMaterialEffectUI chooseMaterialEffectUIScript;

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

    //separate list that will contain the materialSOs currently in a slot
    List<CraftingMaterialSO> materialSOListSlotted = new List<CraftingMaterialSO>();


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
            tempBlueprint.materialSlotPositions = AssignUniqueBlueprintValues(blueprint);
            //add to list
            blueprintSOList.Add(tempBlueprint);
        }
        //automaticaly shows the first blueprint as default
        ShowBluePrint(blueprintSOList[0]);

        //the wrapper for materials should be decoded in the mono scripts
        foreach(CraftingMaterialWrapper wrapper in universalInfo.craftingMaterialWrapperList)
        {
            CraftingMaterialSO tempMaterial = Instantiate(referenceMaaterialSO);
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
        blueprintGridLayoutGroup.cellSize = new Vector2(250, 250);
        blueprintGridLayoutGroup.constraintCount = 4;

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

    }

    //to be called to show the blueprint
    //holds logic to assign the positions of material slots
    //calls are to be received by awake for initial choice, then from another function that chooses a blueprint from a grid list
    void ShowBluePrint(BluePrintSO bluePrintSO)
    {
        //disable all material slots first before refreshing
        foreach (Transform materialSlot in materialSlotsPanel.transform)
        {
            materialSlot.gameObject.SetActive(false);
        }

        bluePrintImage.sprite = bluePrintSO.bluePrintSprite;

        //check list of positions in SO and assign them to prefab for materialSlots in materialSlots panel
        for (int i = 0; bluePrintSO.materialSlotPositions.Count > i; i++)
        {
            //materialSlotsPanel.SetActive(true);

            RectTransform materialSlotRect = materialSlotsPanel.transform.GetChild(i).GetComponent<RectTransform>();
            materialSlotRect.localPosition = bluePrintSO.materialSlotPositions[i];
            materialSlotsPanel.transform.GetChild(i).gameObject.SetActive(true);
        }

        EndChoiceButton();
    }

    //helper function to determine the desired positions of material slotes depending on blueprint
    //all positions here are just created from edito to see what looks good with a given image then listed here
    //the position order is always counted from top to bottom
    List<Vector2> AssignUniqueBlueprintValues(AllGearTypes blueprint)
    {

        List<Vector2> materialSlotPositions = new List<Vector2>();

        switch (blueprint)
        {
            case AllGearTypes.Sword:
                materialSlotPositions.Add(new Vector2(-312, 86.5f));
                materialSlotPositions.Add(new Vector2(-312, -165));
                break;
            case AllGearTypes.Axe:
                materialSlotPositions.Add(new Vector2(-204, 105));
                materialSlotPositions.Add(new Vector2(-333, -173));
                break;
            case AllGearTypes.Shield:
                break;
            case AllGearTypes.Hammer:
                break;
            case AllGearTypes.Greatsword:
                break;
            case AllGearTypes.Spear:
                break;
            default:
                break;
        }
        return materialSlotPositions;
    }

    //called when a material slot is clicked in blueprint UI
    void ChooseMaterialForSlot()
    {
        craftingChoiceUIMaterial.SetActive(true);
        //enable the blueprint contentviewer
        materialContentViewer.SetActive(true);
        materialGridLayoutGroup.cellSize = new Vector2(1000, 250);
        materialGridLayoutGroup.constraintCount = 1;

        choiceEnum = choosingMode.material;

        Transform materialContentTrans = materialContentViewer.transform;



        //decode the mterial wrapper list in universalInfo back to SO
        for (int i = 0; materialSOList.Count > i; i++)
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
                materialSOHolder.ShowMaterialInViewer(materialSOList[i]);
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
                materialSOHolder.ShowMaterialInViewer(materialSOList[i]);
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
    }


    //going back to rest UI
    public void EndCraftingButton()
    {
        //disables the Crafting UI itself
        transform.parent.gameObject.SetActive(false);
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
                        materialSOList.Remove(materialSlot.craftingMaterialSO);
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

                    //clears all materialSlots first then returns the materials back in materialSOList
                    d_MaterialSlotDataClearer();
                    materialSOList.AddRange(materialSOListSlotted);
                    materialSOListSlotted.Clear();


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
