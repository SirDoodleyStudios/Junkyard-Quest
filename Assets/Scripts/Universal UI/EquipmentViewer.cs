using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentViewer : MonoBehaviour
{
    //reference for the OverWorld Manager
    //used only for saving equipment changes
    //assigned in editor, only assigned in the OverWorld Scene
    public OverworldManager overworldManager;

    //delegate for disabling the inventory prefabs when the proceed button is clicked
    public delegate void D_DisableInventoryPrefabs();
    public event D_DisableInventoryPrefabs d_DisableInventoryPrefabs;

    //delegate for making all gears drag and droppable, only called in overworld scene
    public delegate void D_MakeGearsDragNDroppable();
    public event D_MakeGearsDragNDroppable d_MakeGearsDragNDroppable;

    //for initiating dragNDrops, basically giving them all the references that they need
    public delegate void D_InitiateEquipmentDragNDrops(GameObject parentObject, GameObject draggingSpace, InventoryOnDropCatcher inventoryOnDrop, float canvasScale);
    public event D_InitiateEquipmentDragNDrops d_InitiateEquipmentDragNDrops;

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
    //transform for GearSlot Parent
    public Transform equippedContentTrans;
    //reference for the instantiated gear
    public GameObject gearPrefabReference;

    //will contain the equipped gear SO
    GearSO[] equippedGearSOList = new GearSO[6];

    //will contain the gearSOHolder of each slot
    GearSOHolder[] equippedGearSOHolders = new GearSOHolder[6];

    //will contain the generated GearSOs from the universalInformation sent by CameraUIScript
    List<GearSO> gearSOList = new List<GearSO>();

    //universalInfo passed by CameraUIScript
    UniversalInformation universalInfo;

    //TEST SCRIPT TO ACTIVATE THE DRAGNDROPS IN THE INVENTORY SIDE
    public Canvas canvas;
    public GameObject equipmentViewer;
    public GameObject draggingSpace;
    //dont need anymore
    //public GameObject inventoryContent;

    //identifier that makes the delegate for activating DragNDroppable gears when in overworld
    bool isGearManagementEnebled;

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
    public void InitiateEquipment(GearWrapper[] equippedGear, List<GearWrapper> inventoryGear, UniversalInformation universalInfo)
    {
        //assign the universal info given by the CameraScriptUI
        this.universalInfo = universalInfo;

        //clear the list first to prevent duplicating
        gearSOList.Clear();
        //for populating the gearList
        foreach (GearWrapper gearWrapper in inventoryGear)
        {
            gearSOList.Add(ConvertWrapperToGearSO(gearWrapper));
        }

        //for populating the equipList
        //the counting limit is because the eqip slots are always 6
        for (int i =0; 5 >= i; i++)
        {
            //make the element null first to align with the null logics
            equippedGearSOList[i] = null;
            //convert the wrappers and assign them to the proper indexes in the equipment array\
            //assign null if there is no equipment
            //the if condition is to make a slot null if there is no effect in the list of the wrapper since it generates a default class instead of null at instantiation
            if (equippedGear[i].gearEffects.Count != 0 && equippedGear[i] != null)
            {
                equippedGearSOList[i] = ConvertWrapperToGearSO(equippedGear[i]);
            }
            else
            {
                equippedGearSOList[i] = null;
            }

        }

        //assign inventory and slot gearSO to prefabs
        PopulateGearContent(gearSOList);
        //will only be called from overworld
        //enables all gear drags and droppables
        if (isGearManagementEnebled)
        {
            d_MakeGearsDragNDroppable?.Invoke();
        }
    }

    //called by overworld to CameraUI
    //makes all gears in inventory and slots possible for drag
    public void MakeGearsDragNDroppable()
    {
        isGearManagementEnebled = true;

    }

    //helper function that will generte a GearSO from a wrapper
    GearSO ConvertWrapperToGearSO(GearWrapper gearWrapper)
    {
        GearSO tempGearSO = Instantiate(referenceGearSO);
        tempGearSO.gearEffects.AddRange(gearWrapper.gearEffects);
        tempGearSO.gearClassifications = gearWrapper.gearClassifications;
        tempGearSO.gearSetBonus = gearWrapper.gearSetBonus;
        tempGearSO.gearType = gearWrapper.gearType;
        return tempGearSO;
    }

    void PopulateGearContent(List<GearSO> gearSOList)
    {
        //for populating the gear prefabs in the equipmen slots
        for (int i = 0; 5 >= i; i++)
        {
            //if the gearslot is not null, it has a gear equipped in slot
            if (equippedGearSOList[i] != null)
            {
                //actual gearSlot under the equippedContentTrans
                Transform gearSlotTrans = equippedContentTrans.GetChild(i);
                // 4 children under a slot object means that a gear prefab is already put under it during equipment management
                if (gearSlotTrans.childCount == 4)
                {
                    GameObject equippedGear = gearSlotTrans.GetChild(3).gameObject;
                    GearSOHolder gearSOHolder = equippedGear.GetComponent<GearSOHolder>();

                    gearSOHolder.InitializeGearPrefab(equippedGearSOList[i]);
                    //for updating the bool equip identifier of the slot itself
                    EquippedGearSlot gearSlot = gearSlotTrans.GetComponent<EquippedGearSlot>();
                    gearSlot.InitiateGearSlot();

                    equippedGear.SetActive(true);

                }
                //if trans does not have 4 children, it means that it doesn't have a gear Prefab under it yet and must be initialized since the element is not null
                else
                {
                    //instantiate under the slot object
                    GameObject gearObject = Instantiate(gearPrefabReference, gearSlotTrans);
                    GearSOHolder gearSOHolder = gearObject.GetComponent<GearSOHolder>();
                    //assign accordingly with the SO in list
                    //enable the option

                    gearSOHolder.InitializeGearPrefab(equippedGearSOList[i]);
                    //assign all references needed by EquipmentDragNDrop
                    gearObject.GetComponent<EquipmentDragNDrop>().InitiateEquipmentDragNDrop(equipmentViewer, draggingSpace, inventoryOnDropCatcher, canvas.scaleFactor);
                    //makes the gearslot equipped identifier true
                    EquippedGearSlot gearSlot = gearSlotTrans.GetComponent<EquippedGearSlot>();
                    gearSlot.InitiateGearSlot();

                    gearObject.SetActive(true);
                }
            }
            //if the element in array is null, it means that there is no equipped gear
            else
            {
                //actual gearSlot under the equippedContentTrans
                Transform gearSlotTrans = equippedContentTrans.GetChild(i);
                //if the slot has an object assigned to it, send it to the inventory for use later instead of destroying
                if (gearSlotTrans.childCount == 4)
                {
                    //the prefab will aways be disabled at first open
                    gearSlotTrans.GetChild(3).SetParent(inventoryContentTrans);
                }
            }
        }

        //for populating the Gear preafabs in the inventory
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
                //assign all references needed by EquipmentDragNDrop
                gearObject.GetComponent<EquipmentDragNDrop>().InitiateEquipmentDragNDrop(equipmentViewer, draggingSpace, inventoryOnDropCatcher, canvas.scaleFactor);
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



    ////primes all of the gears in inventory to give them references on governing objects
   // now called when first instantiating the prefab
    //void InitiateDragNDrops()
    //{
    //    foreach (Transform trans in inventoryContent.transform)
    //    {
    //        trans.GetComponent<EquipmentDragNDrop>().InitiateEquipmentDragNDrop(equipmentViewer, draggingSpace, inventoryOnDropCatcher, canvas.scaleFactor);
    //    }

    //}

    //function to move gearSO from inventory to slot
    public void MoveGearSOToSlot(GearSO gearSO, int slotIndex)
    {
        //makes sure that the function only works when the gear to be moved is from inventory to slot
        if (gearSOList.Contains(gearSO))
        {
            //remove from the inventory list
            gearSOList.Remove(gearSO);
            //assign the gearSO to slot depending on the index passed
            equippedGearSOList[slotIndex] = gearSO;
        }

    }
    //function to move gearSO from slot to Inventory
    public void MoveGearSOToInventory(GearSO gearSO, int slotIndex)
    {
        //if (equippedGearSOList[slotIndex] == gearSO)
        //{

        //}
        //add the gearSO back to the inventory
        gearSOList.Add(gearSO);
        //make the slot null
        equippedGearSOList[slotIndex] = null;

    }
    //test debug Log for gearSOMovement
    void TESTDEBUGLOG()
    {
        Debug.Log("GearSOList");
        foreach (GearSO gear in gearSOList)
        {
            Debug.Log($"{gear.gearSetBonus} {gear.gearType}");
        }
        Debug.Log("GearSlotList");
        for (int i = 0; 5 >= i; i++)
        {
            if (equippedGearSOList[i] == null)
            {
                Debug.Log("null");
            }
            else
            {
                Debug.Log($"{equippedGearSOList[i].gearSetBonus} {equippedGearSOList[i].gearType}");
            }

        }
    }

    //close the window UI
    public void FinishEquipmentManagementButton()
    {
        //update the gearWrappers in the universalInfo
        //universalInfo.gearWrapperList = UniversalFunctions.ConvertGearSOListToWrapper(gearSOList);
        //universalInfo.equippedGears = UniversalFunctions.ConvertMaterialSOListToWrapperArray(equippedGearSOList);
        //UniversalSaveState.SaveUniversalInformation(universalInfo);

        List<GearWrapper> gearSOListWrappers = UniversalFunctions.ConvertGearSOListToWrapper(gearSOList);
        GearWrapper[] equippedGearSOListWrappers = UniversalFunctions.ConvertMaterialSOListToWrapperArray(equippedGearSOList);
        //this function is only called when in the overWorldScene, overWorldManager is only assigned in overWorld Scene
        if (overworldManager != null)
        {
            overworldManager.SaveEquipmentInOverWorld(equippedGearSOListWrappers, gearSOListWrappers);
        }
        TESTDEBUGLOG();

        //migrated to overWorld maanger
        //updates the universalInfo in the universalUI
        cameraUIScript.UpdateUniversalInfo();

        //disables all gear Prefabs in inventory
        //works like if delegate is not null, invoke delegate event
        d_DisableInventoryPrefabs?.Invoke();

        //disable the equipmentViewer itself
        gameObject.SetActive(false);
    }


}
