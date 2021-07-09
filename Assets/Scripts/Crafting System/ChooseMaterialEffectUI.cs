using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseMaterialEffectUI : MonoBehaviour
{
    //prefab that will showcase the chosen material
    public GameObject materialShowcase;

    //function called for initializing the choose Effect UI
    //receives the SO holder in the material choice prefab
    public void StartMaterialEffectChoice(CraftingMaterialSOHolder craftingMaterialSOHolder)
    {
        materialShowcase.SetActive(true);
        materialShowcase.GetComponent<CraftingMaterialSOHolder>().TransferToChooseEffectUI(craftingMaterialSOHolder);
    }
}
