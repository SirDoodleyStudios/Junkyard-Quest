using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChooseMaterialEffectUI : MonoBehaviour
{
    //reference to crafting manager for calling the assignSlotMaterialFunction
    public CraftingManager craftingManager;

    //prefab that will showcase the chosen material
    public GameObject materialShowcase;

    //the passed CraftingMaterialSOHolder script
    //this is the originalSO
    CraftingMaterialSO materialSO;

    //assigned in editor, the effect buttons and texts
    public Button effectButon1;
    public Button effectButon2;
    public Button effectButon3;
    public TextMeshProUGUI effectText1;
    public TextMeshProUGUI effectText2;
    public TextMeshProUGUI effectText3;

    //function called for initializing the choose Effect UI
    //receives the SO holder in the material choice prefab
    public void StartMaterialEffectChoice(CraftingMaterialSO craftingMaterialSO)
    {
        //CraftingMaterialSO craftingMaterialSO = Instantiate(craftingMaterialSO);

        materialShowcase.SetActive(true);
        materialShowcase.GetComponent<CraftingMaterialSOHolder>().TransferToChooseEffectUI(craftingMaterialSO);
        //the original is still recorded because this is used for removal later
        materialSO = craftingMaterialSO;

        //makes the materialChoice prefab's box collider disabled so that they won't activate the update logic in crafting manager
        materialShowcase.GetComponent<BoxCollider2D>().enabled = false;

    }

    //Buttons for sending which index of material effect is to be taken in account
    public void EffectButton1()
    {
        craftingManager.AssignChosenMaterialToSlot(materialSO, 0);
        ClearShowcasePrefab();
    }
    public void EffectButton2()
    {
        craftingManager.AssignChosenMaterialToSlot(materialSO, 1);
        ClearShowcasePrefab();
    }
    public void EffectButton3()
    {
        craftingManager.AssignChosenMaterialToSlot(materialSO, 2);
        ClearShowcasePrefab();
    }

    //for clearing the text and interactability of effects after choosing one
    void ClearShowcasePrefab()
    {
        effectButon1.interactable = false;
        effectButon2.interactable = false;
        effectButon3.interactable = false;
        effectText1.text = "";
        effectText2.text = "";
        effectText3.text = "";
    }

}
