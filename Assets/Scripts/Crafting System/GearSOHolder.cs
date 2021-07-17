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
    //will contain all altered effect text slots
    List<TextMeshProUGUI> effectsList = new List<TextMeshProUGUI>();
    //name of gear
    public TextMeshProUGUI gearName;
    public Image gearIcon;

    //contains the SO
    public GearSO gearSO;

    //initiates the gear prefab
    public void InitializeGearPrefab(GearSO tempGearSO)
    {
        gearSO = tempGearSO;
        gearIcon.sprite = Resources.Load<Sprite>($"Gear/{gearSO.gearSetBonus} {gearSO.gearType}");
        gearName.text = $"{gearSO.gearSetBonus} {gearSO.gearType}";

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

}
