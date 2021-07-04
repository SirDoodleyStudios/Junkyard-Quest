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

    void Awake()
    {
        blueprintName.text = $"{blueprintSO.blueprint}";
        blueprintSprite.sprite = blueprintSO.bluePrintSprite;
    }
}
