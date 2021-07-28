using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentViewer : MonoBehaviour
{
    //assigned in editor
    //contains the gameObjects for equipment slots
    public GameObject equipmentSlotPanel;
    Transform equipmentSlotPanelTrans;
    //reference SO for gear
    public GearSO referenceGearSO;
    //onDropCatcher for inventory
    public InventoryOnDropCatcher inventoryOnDropCatcher;
    //transform for inventory for instantiating the gearPrefabs here
    public Transform inventoryContentTrans;
    //reference for the instantiated gear
    public GameObject gearPrefabReference;

    //will contain the equipped gear SO
    GearSO[] equippedGearSOList = new GearSO[6];

    //will contain the gearSOHolder of each slot
    GearSOHolder[] equippedGearSOHolders = new GearSOHolder[6];

    //will contain the generated GearSOs from the universalInformation sent by CameraUIScript
    List<GearSO> gearSOList = new List<GearSO>();

    private void Awake()
    {
        equipmentSlotPanelTrans = equipmentSlotPanel.transform;
        //the 5 parameter is because currently we have 6 equipment slots
        for (int i = 0; 5 >= i; i++)
        {
            equippedGearSOHolders[i] = equipmentSlotPanelTrans.GetChild(i).GetComponent<GearSOHolder>();
        }
    }

    //called by cameraScriptUI, universalInfo parameter from the UI button press
    public void InitiateEquipment(GearWrapper[] equippedGear, List<GearWrapper> inventoryGear)
    {


        //populate the equipped SO list
        //GearWrapper[] tempWrapperList = universalInfo.equippedGears;
        //for (int i = 0; tempWrapperList.Length - 1 >= i; i++)
        //{
        //    GearWrapper tempGearWrapper = tempWrapperList[i];
        //    //if the index of the equipped gear is null, add a null in the equippedGearSOList as well
        //    //the checking here is for the material count because when the list is loaded directly from universalInfo, null becomes an empty GearWrapper instead
        //    if (tempGearWrapper.gearEffects.Count == 0)
        //    {
        //        equippedGearSOList[i] = null;
        //    }
        //    //if not null, create a proper SO
        //    else
        //    {
        //        GearSO tempGearSO = Instantiate(referenceGearSO);
        //        tempGearSO.gearClassifications = tempGearWrapper.gearClassifications;
        //        tempGearSO.gearSetBonus = tempGearWrapper.gearSetBonus;
        //        tempGearSO.gearType = tempGearWrapper.gearType;
        //        tempGearSO.gearEffects = tempGearWrapper.gearEffects;
        //        equippedGearSOList[i] = tempGearSO;

        //    }

        //    //PopulateGearPrefab(equippedGearSOList[i], i);
        //}

        //for populating the gearList
        foreach (GearWrapper gearWrapper in inventoryGear)
        {
            GearSO tempGearSO = Instantiate(referenceGearSO);
            tempGearSO.gearEffects.AddRange(gearWrapper.gearEffects);
            tempGearSO.gearClassifications = gearWrapper.gearClassifications;
            tempGearSO.gearSetBonus = gearWrapper.gearSetBonus;
            tempGearSO.gearType = gearWrapper.gearType;
            gearSOList.Add(tempGearSO);
        }


        //assign inventory gearSO to prefabs
        PopulateGearContent(gearSOList);
        //assigns references to all instantiated objects
        InitiateDragNDrops();

    }

    void PopulateGearContent(List<GearSO> gearSOList)
    {
        for (int i = 0; gearSOList.Count - 1 >= i; i++)
        {
            if (inventoryContentTrans.childCount - 1 >= i && inventoryContentTrans.GetChild(i).gameObject != null)
            {
                GameObject gearObject = inventoryContentTrans.GetChild(i).gameObject;
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
                GameObject gearObject = Instantiate(gearPrefabReference, inventoryContentTrans);
                GearSOHolder gearSOHolder = gearObject.GetComponent<GearSOHolder>();
                //assign accordingly with the SO in list
                //enable the option
                gearObject.SetActive(true);
                gearSOHolder.InitializeGearPrefab(gearSOList[i]);
            }
        }
    }

    //Wont need this anymore
    //the GearSO parameter contains the equippedGear  
    public void PopulateGearPrefab(GearSO equippedGearSO, int index)
    {
        //the checking here is null because that's what we actually send internally
        if (equippedGearSO != null)
        {
            equippedGearSOHolders[index].InitializeGearPrefab(equippedGearSO);
        }
        else
        {
            switch (index)
            {
                case 0:
                    equippedGearSOHolders[index].InitializeEmptyGearPrefab(AllGearClassifications.MainHand);
                    break;
                case 1:
                    equippedGearSOHolders[index].InitializeEmptyGearPrefab(AllGearClassifications.OffHand);
                    break;
                case 2:
                    equippedGearSOHolders[index].InitializeEmptyGearPrefab(AllGearClassifications.Helm);
                    break;
                case 3:
                    equippedGearSOHolders[index].InitializeEmptyGearPrefab(AllGearClassifications.Armor);
                    break;
                case 4:
                    equippedGearSOHolders[index].InitializeEmptyGearPrefab(AllGearClassifications.Belt);
                    break;
                case 5:
                    equippedGearSOHolders[index].InitializeEmptyGearPrefab(AllGearClassifications.Trinket);
                    break;
                default:
                    break;
            }
        }

    }

    //TEST SCRIPT TO ACTIVATE THE DRAGNDROPS IN THE INVENTORY SIDE
    public Canvas testCanvas;
    public GameObject testEquipmentViewer;
    public GameObject testdraggingSpace;
    public GameObject testInventoryContent;

    //primes all of the gears in inventory to give them references on governing objects
    void InitiateDragNDrops()
    {
        foreach (Transform trans in testInventoryContent.transform)
        {
            trans.GetComponent<EquipmentDragNDrop>().InitiateEquipmentDragNDrop(testEquipmentViewer, testdraggingSpace, inventoryOnDropCatcher, testCanvas.scaleFactor);
        }
    }


}
