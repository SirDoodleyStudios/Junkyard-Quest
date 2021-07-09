using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaterialSlot : MonoBehaviour
{
    //assigned in editor
    public Image materialImage;
    public TextMeshProUGUI materialName;
    public TextMeshProUGUI materialEffect;

    //function called after a material is chosen
    //the CraftingMaterialSOHolder parameter is taken from the material prefab
    public void AssignMaterial(CraftingMaterialSOHolder materialHolder)
    {
        materialImage.sprite = materialHolder.materialImage.sprite;
        materialName.text = CardTagManager.GetMaterialEffectDescription(materialHolder.chosenEffect);
    }


}
