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

    //will contain the equipped gear SO
    GearSO[] equippedGearSOList = new GearSO[6];

    //will contain the gearSOHolder of each slot
    GearSOHolder[] equippedGearSOHolders = new GearSOHolder[6];

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
    public void InitiateEquipment(UniversalInformation universalInfo)
    {


        //populate the internal GearSOList
        GearWrapper[] tempWrapperList = universalInfo.equippedGears;
        for (int i = 0; tempWrapperList.Length - 1 >= i; i++)
        {
            GearWrapper tempGearWrapper = tempWrapperList[i];
            //if the index of the equipped gear is null, add a null in the equippedGearSOList as well
            //the checking here is for the material count because when the list is loaded directly from universalInfo, null becomes an empty GearWrapper instead
            if (tempGearWrapper.gearEffects.Count == 0)
            {
                equippedGearSOList[i] = null;
            }
            //if not null, create a proper SO
            else
            {
                GearSO tempGearSO = Instantiate(referenceGearSO);
                tempGearSO.gearClassifications = tempGearWrapper.gearClassifications;
                tempGearSO.gearSetBonus = tempGearWrapper.gearSetBonus;
                tempGearSO.gearType = tempGearWrapper.gearType;
                tempGearSO.gearEffects = tempGearWrapper.gearEffects;
                equippedGearSOList[i] = tempGearSO;

            }

            PopulateGearPrefab(equippedGearSOList[i], i);
        }

        //this is test
        InitiateDragNDrops();

    }

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

    void InitiateDragNDrops()
    {
        foreach (Transform trans in testInventoryContent.transform)
        {
            trans.GetComponent<EquipmentDragNDrop>().InitiateEquipmentDragNDrop(testEquipmentViewer, testdraggingSpace, inventoryOnDropCatcher, testCanvas.scaleFactor);
        }
    }


}
