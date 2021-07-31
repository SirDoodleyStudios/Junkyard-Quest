﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentViewer : MonoBehaviour
{
    //delegate for disabling the inventory prefabs when the proceed button is clicked
    public delegate void D_DisableInventoryPrefabs();
    public event D_DisableInventoryPrefabs d_DisableInventoryPrefabs;

    //assigned in editor
    public CameraUIScript cameraUIScript;
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
            if (inventoryContentTrans.childCount - 1 >= i && inventoryContentTrans.GetChild(i).gameObject != null && !inventoryContentTrans.GetChild(i).gameObject.activeSelf)
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
    //public void PopulateGearPrefab(GearSO equippedGearSO, int index)
    //{
    //    //the checking here is null because that's what we actually send internally
    //    if (equippedGearSO != null)
    //    {
    //        equippedGearSOHolders[index].InitializeGearPrefab(equippedGearSO);
    //    }
    //    else
    //    {
    //        switch (index)
    //        {
    //            case 0:
    //                equippedGearSOHolders[index].InitializeEmptyGearPrefab(AllGearClassifications.MainHand);
    //                break;
    //            case 1:
    //                equippedGearSOHolders[index].InitializeEmptyGearPrefab(AllGearClassifications.OffHand);
    //                break;
    //            case 2:
    //                equippedGearSOHolders[index].InitializeEmptyGearPrefab(AllGearClassifications.Helm);
    //                break;
    //            case 3:
    //                equippedGearSOHolders[index].InitializeEmptyGearPrefab(AllGearClassifications.Armor);
    //                break;
    //            case 4:
    //                equippedGearSOHolders[index].InitializeEmptyGearPrefab(AllGearClassifications.Belt);
    //                break;
    //            case 5:
    //                equippedGearSOHolders[index].InitializeEmptyGearPrefab(AllGearClassifications.Trinket);
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //}

    //TEST SCRIPT TO ACTIVATE THE DRAGNDROPS IN THE INVENTORY SIDE
    public Canvas testCanvas;
    public GameObject equipmentViewer;
    public GameObject testdraggingSpace;
    public GameObject testInventoryContent;

    //primes all of the gears in inventory to give them references on governing objects
    void InitiateDragNDrops()
    {
        foreach (Transform trans in testInventoryContent.transform)
        {
            trans.GetComponent<EquipmentDragNDrop>().InitiateEquipmentDragNDrop(equipmentViewer, testdraggingSpace, inventoryOnDropCatcher, testCanvas.scaleFactor);
        }
    }

    //function to move gears around lists
    public void UpdateGearAssignments()
    {

    }

    //close the window UI
    public void FinishEquipmentManagementButton()
    {
        //creates a new universalInfo to update the wrappers in the list
        UniversalInformation universalInfo = UniversalSaveState.LoadUniversalInformation();
        universalInfo.gearWrapperList = UniversalFunctions.ConvertGearSOListToWrapper(gearSOList);
        //updates the universalInfo in the universalUI
        cameraUIScript.UpdateUniversalInfo();

        //clear the list of GearSOs since the initiation will populate from the currently saved universalInfo
        gearSOList.Clear();
        //disables all gear Prefabs in inventory
        d_DisableInventoryPrefabs();
        //disable the equipmentViewer itself
        gameObject.SetActive(false);
    }


}
