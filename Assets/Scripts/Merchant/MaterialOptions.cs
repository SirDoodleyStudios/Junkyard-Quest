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

    //internal merchantSaveStte received and then edited to be sent back to merchantManager for saving
    MerchantSaveState merchantSaveState;

    //list of MaterialSOs converted from the wrapper list in merchantSaveState
    List<CraftingMaterialSO> materialSOList = new List<CraftingMaterialSO>();
    //List per material Cost, must synchronize with materialSOList
    List<int> materialCostList = new List<int>();

    //for mouse pointing and clicking
    Vector2 PointRay;
    Ray ray;
    RaycastHit2D pointedObject;

    //Initialization
    //bool parameter to determine if the initialization is fresh or from file
    public void InitiateMaterialOptions(UniversalInformation universalInfo, MerchantSaveState merchantSave, bool isLoadedFromFile)
    {
        if (isLoadedFromFile)
        {

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
                materialSOList.Add(tempMaterialSO);
                //randomize card scraps cost
                int materialValueInt = UnityEngine.Random.Range(80, 100);
                materialCostList.Add(materialValueInt);
            }

        }
    }

    public void ViewMaterialOptions()
    {
        for (int i = 0; materialSOList.Count > i; i++)
        {
            //if the prefab is already existing, if it is, it sould always be disabled already
            //to check if there are children, under bluePrintContent, use childCount and i comparison
            //the foreach above ensures that all blueprint objects are at the beginning so if the current object checked is not a blueprint, we instantiate a new one
            if (materialContentTrans.childCount - 1 >= i && materialContentTrans.GetChild(i).gameObject != null)
            {
                GameObject materialObject = materialContentTrans.GetChild(i).gameObject;
                Transform materialTrans = materialObject.transform;
                CraftingMaterialSOHolder materialSOHolder = materialObject.GetComponent<CraftingMaterialSOHolder>();

                //create the priceTag prefab then instantiate it under the card then set the price
                GameObject priceTagObj = materialTrans.GetChild(materialTrans.childCount - 1).gameObject;
                PriceTag priceTag = priceTagObj.GetComponent<PriceTag>();
                priceTag.SetPriceTag(materialCostList[i]);

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
                GameObject materialObject = Instantiate(materialPrefabReference, materialContentTrans);
                CraftingMaterialSOHolder materialSOHolder = materialObject.GetComponent<CraftingMaterialSOHolder>();

                //create the priceTag prefab then instantiate it under the card then set the price
                GameObject priceTagObj = Instantiate(priceTagPrefab, materialObject.transform);
                PriceTag priceTag = priceTagObj.GetComponent<PriceTag>();
                priceTag.SetPriceTag(materialCostList[i]);

                //assign accordingly with the SO in list
                //enable the option
                materialObject.SetActive(true);
                //CraftingMaterialSO instantiatedMatSO = Instantiate(materialSOList[i]);
                materialSOHolder.ShowMaterialInViewer(materialSOList[i]);
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
            if (pointedObject.collider != null)
            {
                GameObject selectedObject = pointedObject.collider.gameObject;
                if (selectedObject.CompareTag("Material"))
                {
                    Debug.Log("Clicked material");
                }

            }
            else
            {
                Debug.Log("no material");
            }

        }
    }

    //called when a material prefab is clicked, calls merchantManager
    void BuyMaterial()
    {

    }

}
