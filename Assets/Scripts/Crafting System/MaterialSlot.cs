using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaterialSlot : MonoBehaviour
{
    //gets the craftingManager reference
    public CraftingManager craftingManager;

    //assigned MaterialSO
    //to be used by CraftingManager for rejoining the CraftingMaterialSO to the list if a material Slot is cancelled 
    public CraftingMaterialSO craftingMaterialSO;

    //assigned in editor
    public Image materialImage;
    public TextMeshProUGUI materialNameText;
    public TextMeshProUGUI materialEffectText;
    public AllMaterialEffects materialEffect;
    //this contains the gameObject with collider that can be clicked to go to material content viewer
    public BoxCollider2D materialInteractable;

    //the material type that the slot will allow
    public AllMaterialTypes allowableType;

    private void Awake()
    {
        craftingManager = transform.parent.parent.GetComponent<CraftingManager>();

    }

    void OnEnable()
    {
        craftingManager.d_MaterialSlotColliderAlterer += AlterMaterialSlotCollider;
        craftingManager.d_MaterialSlotDataClearer += ClearMaterialSlot;
        //at the start of craft, clear everything
        ClearMaterialSlot();
    }
    private void OnDisable()
    {
        craftingManager.d_MaterialSlotColliderAlterer -= AlterMaterialSlotCollider;
        craftingManager.d_MaterialSlotDataClearer -= ClearMaterialSlot;
    }

    //function called after a material is chosen
    //the CraftingMaterialSOHolder parameter is taken from the material prefab
    //the int parameter which effect index is to be chosen from the holder's effectList
    public void AssignMaterial(CraftingMaterialSO materialSO, int materialEffectIndex)
    {
        //instantiate a copy for this instance's values
        //CraftingMaterialSO materialSO = Instantiate(materialSO);


        materialImage.sprite = Resources.Load<Sprite>($"Materials/{materialSO.materialPrefix}{materialSO.materialType}");
        materialImage.enabled = true;
        materialNameText.text = $"{materialSO.materialPrefix} {materialSO.materialType}";
        materialEffect = materialSO.materialEffects[materialEffectIndex];
        materialEffectText.text = CardTagManager.GetMaterialEffectDescription(materialSO.materialEffects[materialEffectIndex]);
        //the original is maintained for removal later
        craftingMaterialSO = materialSO;
    }

    //called by the craftingManager. This disables or enables the box collider so that it doesn't get detected during content choice UIs
    //the bool parameter determines if the materialSLot is to be enabled or disabled
    public void AlterMaterialSlotCollider(bool state)
    {
        materialInteractable.enabled = state;
    }

    //remove material details from slot
    //called when a material slot is clicked to remove the first choice before choosing again
    //also called by the change blueprint button as a reset to give way to the next blueprint to be used
    public void ClearMaterialSlot()
    {

        materialImage.sprite = null;
        materialImage.enabled = false;
        materialNameText.text = $"Material Type: {allowableType}";
        materialEffectText.text = "Material Effect";
        craftingMaterialSO = null;

    }


}
