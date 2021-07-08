using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    //will contain all available bluprints for crafting
    List<AllGearTypes> blueprints = new List<AllGearTypes>();
    //will contain all generated blueprintsSO from blueprints
    List<BluePrintSO> blueprintSOList = new List<BluePrintSO>();

    //will contain all materilSOs available for use
    //will only be instanstiated at start because crafting UI ends after a successful craft
    List<CraftingMaterialSO> materialSOList = new List<CraftingMaterialSO>();

    //SO instantiated
    public CraftingMaterialSO craftingMaterial;

    //assigned from editor
    public Image bluePrintImage;
    public GameObject materialSlotsPanel;
    //will be used for instantiating the blueprints
    public BluePrintSO referenceBluePrintSO;
    //will be used for instantiating materials
    public CraftingMaterialSO referenceMaaterialSO;

    //the content view for blueprints
    public GameObject craftingChoiceUI;
    public GameObject choiceContentViewer;
    //reference prefabs of blueprint and material
    public GameObject blueprintReference;
    public GameObject materialReference;
    //the Grid layout group of the scroll content viewer, will be used to set the grid size depending if material or blueprit is to be chosen
    public GridLayoutGroup contentGridLayoutGroup;

    UniversalInformation universalInfo;

    //identifier to check whether the player is choosing, will enable all functions in Update
    bool isChoosing;
    //will be used for processing the objects chosen during update
    enum choosingContent { blueprint, material}
    choosingContent content;

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
            tempBlueprint.materialSlotPositions = AssignMaterialSlotPositions(blueprint);
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
            foreach(AllMaterialEffects effect in universalInfo.materialEffects)
            {
                tempEffectsList.Add(effect);
            }
            materialSOList.Add(tempMaterial);
        }

    }

    //function to open the blueprint content viewer too choose a blueprint to craft with
    public void ChooseBluePrintButton()
    {
        //enables the viewer gameObject itself
        craftingChoiceUI.SetActive(true);
        //enable the blueprint contentviewer
        choiceContentViewer.SetActive(true);
        contentGridLayoutGroup.cellSize = new Vector2(250, 250);
        contentGridLayoutGroup.constraintCount = 4;

        //generate choices depending on how many blueprints are in blueprint list
        //if an there is already an existing prefab in the blueprint choices, enable that then just assign the bluprintSO
        Transform blueprintContentTrans = choiceContentViewer.transform;
        for (int i = 0; blueprintSOList.Count > i; i++)
        {
            //if the prefab is already existing, if it is, it sould always be disabled already
            //to check if there are children, under bluePrintContent, use childCount and i comparison
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
        isChoosing = true;
        content = choosingContent.blueprint;

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

    }

    //helper function to determine the desired positions of material slotes depending on blueprint
    //all positions here are just created from edito to see what looks good with a given image then listed here
    //the position order is always counted from top to bottom
    List<Vector2> AssignMaterialSlotPositions(AllGearTypes blueprint)
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
        craftingChoiceUI.SetActive(true);
        //enable the blueprint contentviewer
        choiceContentViewer.SetActive(true);
        contentGridLayoutGroup.cellSize = new Vector2(1000, 250);
        contentGridLayoutGroup.constraintCount = 1;

        content = choosingContent.material;
        //materialContentViewer.SetActive(true);
        choiceContentViewer.SetActive(true);
        //decode the mterial wrapper list in universalInfo back to SO
        foreach (CraftingMaterialSO materialSO in materialSOList)
        {
            GameObject tempMaterial = Instantiate(materialReference, choiceContentViewer.transform);
            tempMaterial.GetComponent<CraftingMaterialSOHolder>().craftingMaterialSO = materialSO;
            tempMaterial.SetActive(true);

        }
        ///////////////////////HERE//////////////////////////

    }

    //logic for going back from choice panel
    //assigned in editor
    public void EndChoiceButton()
    {
        if (content == choosingContent.blueprint)
        {
            //disables all choices under the content viewer
            foreach (Transform blueprintTrans in choiceContentViewer.transform)
            {
                GameObject blueprintObject = blueprintTrans.gameObject;
                blueprintObject.SetActive(false);
            }
            //enable the blueprint contentviewer
            choiceContentViewer.SetActive(false);
            //enables the viewer gameObject itself
        }

        craftingChoiceUI.SetActive(false);
        isChoosing = false;

    }

    //going back to rest UI
    public void EndCraftingButton()
    {
        //disables the Crafting UI itself
        transform.parent.gameObject.SetActive(false);
    }

    //go back to crafting UI
    public void EndChoosingButton()
    {
        //disables the content viewer
        craftingChoiceUI.SetActive(false);
        isChoosing = false;
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
            if (pointedObject.collider != null)
            {
                //if is choosing, follow the blueprint aor material list logic
                //update is for choosing a an option in blueprint and material choice, will only turn true if the buttons to choose are clicked
                if (isChoosing)
                {
                    GameObject chosenPrefab = pointedObject.collider.gameObject;
                    //logic for choice if choosing for blueprint
                    if (content == choosingContent.blueprint)
                    {
                        //disables all choices under the content viewer
                        foreach (Transform blueprintTrans in choiceContentViewer.transform)
                        {
                            GameObject blueprintObject = blueprintTrans.gameObject;
                            blueprintObject.SetActive(false);
                        }
                        //enable the blueprint contentviewer
                        choiceContentViewer.SetActive(false);
                        //enables the viewer gameObject itself
                        craftingChoiceUI.SetActive(false);

                        //send the SO assigned for the blueprint prefab
                        ShowBluePrint(chosenPrefab.GetComponent<BluePrintSOHolder>().blueprintSO);

                    }
                    //logic for choosing materials
                    else if (content == choosingContent.material)
                    {

                    }

                    //disables identifier
                    isChoosing = false;
                }
                //if not choosing, only monitor clicks on the material slot object
                //if we got here, it means that we have interacted with the collider of the mteril slot and that only since they're the only objects that has colliders in blueprint UI
                else
                {

                    ChooseMaterialForSlot();


                }

            }

        }
        
    }

}
