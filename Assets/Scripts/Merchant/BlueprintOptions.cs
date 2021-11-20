using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintOptions : MonoBehaviour
{
    //assigned in editor
    //the manager script
    public MerchantManager merchantManager;
    //the scroll content
    public GameObject blueprintContent;
    Transform blueprintContentTrans;
    //reference for instantiating the blueprint
    public GameObject blueprintReference;
    public BluePrintSO referenceBluePrintSO;
    //reference prefab for the priceTag
    public GameObject priceTagPrefab;
    //reference to UI viewers, this is so that Update Function is disabled when viewing the UniversalUIs
    public GameObject inventoryViewer;
    public GameObject deckViewer;
    public GameObject gearViewer;

    //internal merchantSaveState
    MerchantSaveState merchantSaveState;   

    //dictionary that will hold the existing blueprints and their respective costs
    Dictionary<BluePrintSO, int> blueprintNCost = new Dictionary<BluePrintSO, int>();

    //for mouse pointing and clicking
    Vector2 PointRay;
    Ray ray;
    RaycastHit2D pointedObject;

    //called by merchant manager when initializing everything at the beginning
    //bool parameter indicates if the optios to be loaded are for file
    public MerchantSaveState InitiateBlueprintOptions(UniversalInformation universalInfo, MerchantSaveState merchantSave, bool isLoadedFromFile)
    {
        //assign the transform of the content object
        blueprintContentTrans = blueprintContent.transform;

        //the internal merchantSaveState file
        merchantSaveState = merchantSave;

        //when loaded from file
        if (isLoadedFromFile)
        {
            //list of AllGearTypes to be converted to BlueprintSOs
            for (int i = 0; merchantSave.blueprintOptions.Count - 1 >= i; i++)
            {
                //create the SO
                BluePrintSO tempBlueprint = Instantiate(referenceBluePrintSO);
                AllGearTypes blueprint = merchantSaveState.blueprintOptions[i];
                tempBlueprint.blueprint = blueprint;
                tempBlueprint.bluePrintSprite = Resources.Load<Sprite>($"Blueprints/{blueprint}");

                //add to dictionary
                blueprintNCost.Add(tempBlueprint, merchantSaveState.blueprintOptionCosts[i]);
            }
        }

        else
        {
            //list that wioll take note of radomly generated blueprints so that we can prevent repeating of options
            // the initial list will contain only blueprints that are not in the player's inventory
            List<AllGearTypes> repeatPreventer = new List<AllGearTypes>();
            repeatPreventer = UniversalFunctions.SearchAvailableBlueprints(universalInfo.bluePrints);

            //randomize blueprints that will show up in options
            for (int i = 0; 3 >= i; i++)
            {
                BluePrintSO tempBlueprint = Instantiate(referenceBluePrintSO);
                //call function to randomly get blueprint enums\
                //will prevent from repeating options
                //AllGearTypes randomizedBlueprint;
                //do
                //{
                //    randomizedBlueprint = UniversalFunctions.GetRandomEnum<AllGearTypes>();
                //}
                //while (repeatPreventer.Contains(randomizedBlueprint));
                ////add the generated indedx to prevent a repeat
                //repeatPreventer.Add(randomizedBlueprint);

                //get a random bluepint from the list then remove the selected blueprint to prevent repetition
                int randomizedIndex = UnityEngine.Random.Range(0, repeatPreventer.Count);
                AllGearTypes randomizedBlueprint = repeatPreventer[randomizedIndex];
                repeatPreventer.RemoveAt(randomizedIndex);

                //assign the blueprint values
                tempBlueprint.blueprint = randomizedBlueprint;
                tempBlueprint.bluePrintSprite = Resources.Load<Sprite>($"Blueprints/{randomizedBlueprint}");
                //send the blueprint ot the AssignBluprintValues to fill out the vecttor and allowable type list
                tempBlueprint = UniversalFunctions.AssignUniqueBlueprintValues(tempBlueprint);

                //randomize a cost
                int randomizedCost = Random.Range(70, 100);

                //add to list
                blueprintNCost.Add(tempBlueprint, randomizedCost);

                //add to merchantSaveState for returning to merchantManager
                merchantSaveState.blueprintOptions.Add(randomizedBlueprint);
                merchantSaveState.blueprintOptionCosts.Add(randomizedCost);

                //this is activated when all blueprints are already taken, immediately break
                if (repeatPreventer.Count == 0)
                {
                    break;
                }
            }
        }

        return merchantSaveState;

    }

    //function used to populate the blueprint content with blueprint objects
    public void ViewBlueprintOptions(bool isTicketUsed)
    {
        //multiplier to price depending if the discount ticket is activated
        float priceMultiplier;
        if (isTicketUsed)
        {
            //get discount of 50% if ticket is used
            priceMultiplier = .8f;
        }
        else
        {
            priceMultiplier = 1;
        }

        //generate choices depending on how many blueprints are in blueprint list
        //if an there is already an existing prefab in the blueprint choices, enable that then just assign the bluprintSO

        foreach (KeyValuePair<BluePrintSO, int> BNC in blueprintNCost)
        {
            bool hasNoDisabledPrefabs = true;

            foreach (Transform content in blueprintContentTrans)
            {
                GameObject disabledPrefabs = content.gameObject;
                if (!disabledPrefabs.activeSelf)
                {
                    BluePrintSOHolder blueprintSOHolder = disabledPrefabs.GetComponent<BluePrintSOHolder>();
                    blueprintSOHolder.blueprintSO = BNC.Key;
                    //create the priceTag prefab then instantiate it under the blueprint then set the price
                    GameObject priceTagObj = content.GetChild(content.childCount - 1).gameObject;
                    PriceTag priceTag = priceTagObj.GetComponent<PriceTag>();
                    priceTag.SetPriceTag(Mathf.RoundToInt(BNC.Value*priceMultiplier));

                    //assign accordingly with the SO in list
                    //enable the option
                    disabledPrefabs.SetActive(true);
                    //deactivates the identifier so that no new prefabs are instantiated for this iteration
                    hasNoDisabledPrefabs = false;
                    break;
                }
            }

            if (hasNoDisabledPrefabs)
            {
                //instantiate under material content
                GameObject blueprintObject = Instantiate(blueprintReference, blueprintContentTrans);
                BluePrintSOHolder blueprintSOHolder = blueprintObject.GetComponent<BluePrintSOHolder>();
                blueprintSOHolder.blueprintSO = BNC.Key;

                //create the priceTag prefab then instantiate it under the card then set the price
                GameObject priceTagObj = Instantiate(priceTagPrefab, blueprintObject.transform);
                PriceTag priceTag = priceTagObj.GetComponent<PriceTag>();
                priceTag.SetPriceTag(Mathf.RoundToInt(BNC.Value * priceMultiplier));

                //assign accordingly with the SO in list
                //enable the option
                blueprintObject.SetActive(true);
            }
        }
    }

    //update
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
                    if (selectedObject.CompareTag("Blueprint"))
                    {
                        //The material Object's priceTag
                        PriceTag blueprintPrice = selectedTrans.GetChild(selectedTrans.childCount - 1).GetComponent<PriceTag>();
                        //check if player has enough scraps
                        if (merchantManager.CheckScraps(blueprintPrice.priceTag))
                        {
                            //the blueprint Object's SO Holder
                            BluePrintSOHolder blueprintSOHolder = selectedObject.GetComponent<BluePrintSOHolder>();
                            blueprintNCost.Remove(blueprintSOHolder.blueprintSO);
                            //update the change to the merchantSaveState
                            UpdateBlueprintMaterialSaveState();
                            merchantManager.AddBoughtBlueprint(blueprintSOHolder.blueprintSO);
                            //disable the prefab
                            selectedObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    //called when the merchantSaveState is going to be updatd after buying an option
    void UpdateBlueprintMaterialSaveState()
    {
        List<AllGearTypes> tempBlueprint = new List<AllGearTypes>();
        List<int> tempBlueprintCostList = new List<int>();

        foreach (KeyValuePair<BluePrintSO, int> BNC in blueprintNCost)
        {
            tempBlueprint.Add(BNC.Key.blueprint);
            tempBlueprintCostList.Add(BNC.Value);
        }
        merchantSaveState.blueprintOptions = tempBlueprint;
        merchantSaveState.blueprintOptionCosts = tempBlueprintCostList;
    }

    //back button that disables the UI and saves the merchantSaveState and UniversalInformaetion forreal
    public void BlueprintBackButton()
    {
        //disables all children prefab of cards
        foreach (Transform content in blueprintContentTrans)
        {
            content.gameObject.SetActive(false);
        }
        //disable the CardOptionUI
        gameObject.SetActive(false);

        //updates the options in the current instance of merchantSaveState
        UpdateBlueprintMaterialSaveState();
        merchantManager.UpdateMerchantSaveState(merchantSaveState);
    }

}
