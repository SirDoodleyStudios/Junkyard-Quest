using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BluePrintSOHolder : MonoBehaviour
{
    //set from craftingmanager during instantiation
    public BluePrintSO blueprintSO;
    //assigned in editor
    public TextMeshProUGUI blueprintName;
    public Image blueprintSprite;

    //will contain texts that will tell player what material types are needed
    public GameObject materialTextPanel;

    void Awake()
    {
        blueprintName.text = $"{blueprintSO.blueprint}";
        blueprintSprite.sprite = blueprintSO.bluePrintSprite;

        for (int i = 0; blueprintSO.acceptableMaterialTypes.Count - 1 >= i; i++)
        {
            //the gameobject with the text is a child
            GameObject materialText = materialTextPanel.transform.GetChild(i).GetChild(0).gameObject;
            materialText.GetComponent<TextMeshProUGUI>().text = $"Acceptable material for {GetGearPart(blueprintSO.blueprint, i+1)}: {blueprintSO.acceptableMaterialTypes[i]}";
        }
    }

    //function for getting the gear part for the material slot.
    //this is all for context flavor and to make the text longer
    string GetGearPart(AllGearTypes gearType, int slotIndex)
    {
        //temporarily has a default value because not all switch cases are assigned with a value yet
        string gearPart = "";
        switch (gearType)
        {
            case AllGearTypes.Sword:
                switch (slotIndex)
                {
                    case 1:
                        gearPart = "blade";
                        break;
                    case 2:
                        gearPart = "hilt";
                        break;
                    default:
                        break;
                }
                break;
            case AllGearTypes.Axe:
                switch (slotIndex)
                {
                    case 1:
                        gearPart = "head";
                        break;
                    case 2:
                        gearPart = "haft";
                        break;
                    default:
                        break;
                }
                break;
            case AllGearTypes.Shield:
                break;
            case AllGearTypes.Hammer:
                break;
            case AllGearTypes.Greatsword:
                break;
            case AllGearTypes.Spear:
                break;
            default:
                break;
        }
        return gearPart;
    }
}
