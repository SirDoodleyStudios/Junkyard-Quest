using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaterialSlot : MonoBehaviour
{
    //assigned in editor
    public Image materialImage;
    public TextMeshProUGUI materialNameText;
    public TextMeshProUGUI materialEffectText;
    public AllMaterialEffects materialEffect;

    //function called after a material is chosen
    //the CraftingMaterialSOHolder parameter is taken from the material prefab
    //the int parameter which effect index is to be chosen from the holder's effectList
    public void AssignMaterial(CraftingMaterialSOHolder materialHolder, int materialEffectIndex)
    {
        materialImage.sprite = materialHolder.materialImage.sprite;
        materialNameText.text = materialHolder.materialName.text;
        materialEffect = materialHolder.effectsList[materialEffectIndex];
        materialEffectText.text = CardTagManager.GetMaterialEffectDescription(materialHolder.effectsList[materialEffectIndex]);

    }


}
