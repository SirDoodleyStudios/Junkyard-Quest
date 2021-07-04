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

    //SO instantiated
    public CraftingMaterialSO craftingMaterial;

    //assigned from editor
    public Image bluePrintImage;
    public GameObject materialSlotsPanel;
    //will be used for instantiating the blueprints
    public BluePrintSO referenceBluePrintSO;

    //the content view for blueprints
    public GameObject craftingChoiceUI;
    public GameObject blueprintContentViewer;
    public GameObject materialContentViewer;
    //reference prefabs of blueprint and material
    public GameObject blueprintReference;
    public GameObject materialReference;

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
    }

    //function to open the blueprint content viewer too choose a blueprint to craft with
    public void ChooseBluePrintButton()
    {
        //enables the viewer gameObject itself
        craftingChoiceUI.SetActive(true);
        //enable the blueprint contentviewer
        blueprintContentViewer.SetActive(true);

        //generate choices depending on how many blueprints are in blueprint list
        //if an there is already an existing prefab in the blueprint choices, enable that then just assign the bluprintSO
        Transform blueprintContentTrans = blueprintContentViewer.transform;
        for (int i = 0; blueprintSOList.Count > i; i++)
        {
            //if the prefab is already existing, if it is, it sould always be disabled already
            if (blueprintContentTrans.GetChild(i).gameObject != null)
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

    //logic for going back from choice panel
    //assigned in editor
    public void EndChoiceButton()
    {
        if (content == choosingContent.blueprint)
        {
            //disables all choices under the content viewer
            foreach (Transform blueprintTrans in blueprintContentViewer.transform)
            {
                GameObject blueprintObject = blueprintTrans.gameObject;
                blueprintObject.SetActive(false);
            }
            //enable the blueprint contentviewer
            blueprintContentViewer.SetActive(false);
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

    //for clicking choices when picking a blueprint or material
    private void Update()
    {
        //update is for choosing a an option in blueprint and material choice, will only turn true if the buttons to choose are clicked
        if (isChoosing)
        {
            PointRay = Input.mousePosition;
            ray = Camera.main.ScreenPointToRay(PointRay);
            pointedObject = Physics2D.GetRayIntersection(ray);

            //if the clicked object is a card and has a collider
            if (Input.GetMouseButtonDown(0))
            {

                if (pointedObject.collider != null)
                {

                    GameObject chosenPrefab = pointedObject.collider.gameObject;
                    //logic for choice if choosing for blueprint
                    if (content == choosingContent.blueprint)
                    {
                        //disables all choices under the content viewer
                        foreach (Transform blueprintTrans in blueprintContentViewer.transform)
                        {
                            GameObject blueprintObject = blueprintTrans.gameObject;
                            blueprintObject.SetActive(false);
                        }
                        //enable the blueprint contentviewer
                        blueprintContentViewer.SetActive(false);
                        //enables the viewer gameObject itself
                        craftingChoiceUI.SetActive(false);

                        //send the SO assigned for the blueprint prefab
                        ShowBluePrint(chosenPrefab.GetComponent<BluePrintSOHolder>().blueprintSO);

                    }

                    //disables identifier
                    isChoosing = false;
                }

            }
        }
    }

}
