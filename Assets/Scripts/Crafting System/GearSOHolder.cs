using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GearSOHolder : MonoBehaviour
{
    //assigned in editor
    //holds the texts for active effects
    public GameObject gearEffectsPanel;
    //name of gear
    public TextMeshProUGUI gearName;
    //parent of gear name, will be disabled when initiated as empty
    public GameObject gearNameObject;
    public TextMeshProUGUI gearClassification;
    public Image gearIcon;

    //contains the SO
    public GearSO gearSO;

    //the gameObjects to be disabled when the prefab is initialized as empty


    //initiates the gear prefab
    public void InitializeGearPrefab(GearSO tempGearSO)
    {
        //this check is to make sure that effects panel and gear name is enabled, can be disabled if prefab was previously tagged as empty
        if (!gearEffectsPanel.activeSelf && gearNameObject.activeSelf)
        {
            gearNameObject.SetActive(true);
            gearEffectsPanel.SetActive(true);
        }

        gearSO = tempGearSO;
        gearIcon.sprite = Resources.Load<Sprite>($"Gear/{gearSO.gearSetBonus} {gearSO.gearType}");
        gearName.text = $"{gearSO.gearSetBonus} {gearSO.gearType}";
        gearClassification.text = $"{tempGearSO.gearClassifications}";

        for (int i = 0; gearSO.gearEffects.Count - 1 >= i; i++)
        {
            TextMeshProUGUI effectText = gearEffectsPanel.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
            effectText.text = CardTagManager.GetMaterialEffectDescription(gearSO.gearEffects[i]);
        }

    }

    //clears the prefab
    //just used for removing the texts in the prefab so that unwanted text is prevented in next craft
    public void ClearGearPrefab()
    {
        foreach (Transform effectTrans in gearEffectsPanel.transform)
        {
            effectTrans.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    //public void InitializeEmptyGearPrefab(AllGearClassifications tempGearClassification)
    //{
    //    gearEffectsPanel.SetActive(false);
    //    gearIcon.sprite = Resources.Load<Sprite>($"Gear/Empty {tempGearClassification}");
    //    gearClassification.text = $"{tempGearClassification}";
    //    gearNameObject.SetActive(false);
    //}

    //used by EquipmentViewer, this will give the gear prefab an empty variant of the equipment slot
    //these are just for changing visuals
    public void MakeGearPrefabEmpty()
    {

    }
    //used by EquipmentViewer, this will make a gear slot equipped
    public void EquipGearPrefab()
    {

    }

}
