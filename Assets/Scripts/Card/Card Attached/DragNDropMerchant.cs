using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DragNDropMerchant : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    CardDescriptionLayout cardDescriptionLayout;

    //Reference Objects for the prices
    public GameObject scrapsValueObj;
    public TextMeshProUGUI scrapsValueText;
    public int scrapsValueInt;
    private void Awake()
    {
        SetScrapsValue();
    }
    private void Start()
    {
        //calls activate and deactivate popup methods in cardDescriptionLayout
        cardDescriptionLayout = gameObject.GetComponent<CardDescriptionLayout>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        cardDescriptionLayout.EnablePopups();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardDescriptionLayout.DisablePopups();
    }

    //called at awake
    public void SetScrapsValue()
    {
        int scrapsValueInt = Random.Range(45, 71);
        scrapsValueText.text = $"{scrapsValueInt}";
    }
}
