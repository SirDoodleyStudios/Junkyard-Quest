using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseMaterialEffectUI : MonoBehaviour
{
    //reference to crafting manager for calling the assignSlotMaterialFunction
    public CraftingManager craftingManager;

    //prefab that will showcase the chosen material
    public GameObject materialShowcase;

    //the passed CraftingMaterialSOHolder script
    CraftingMaterialSOHolder materialSOHolder;

    //function called for initializing the choose Effect UI
    //receives the SO holder in the material choice prefab
    public void StartMaterialEffectChoice(CraftingMaterialSOHolder craftingMaterialSOHolder)
    {
        materialShowcase.SetActive(true);
        materialShowcase.GetComponent<CraftingMaterialSOHolder>().TransferToChooseEffectUI(craftingMaterialSOHolder);
        materialSOHolder = craftingMaterialSOHolder;

        //makes the materialChoice prefab's box collider disabled so that they won't activate the update logic in crafting manager
        materialShowcase.GetComponent<BoxCollider2D>().enabled = false;

    }

    //Buttons for sending which index of material effect is to be taken in account
    public void EffectButton1()
    {
        craftingManager.AssignChosenMaterialToSlot(materialSOHolder, 0);
    }
    public void EffectButton2()
    {
        craftingManager.AssignChosenMaterialToSlot(materialSOHolder, 1);
    }
    public void EffectButton3()
    {
        craftingManager.AssignChosenMaterialToSlot(materialSOHolder, 2);
    }



}
