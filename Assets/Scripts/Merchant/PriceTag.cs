using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PriceTag : MonoBehaviour
{
    public int priceTag;

    public TextMeshProUGUI scrapValueText;
    private void Awake()
    {
        //the TMPro child is at child 1
        //scrapValueText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }
    //called by the OptionsScript when enabling or instantiating the item
    public void SetPriceTag(int sentPrice)
    {
        priceTag = sentPrice;
        scrapValueText.text = $"{sentPrice}";
    }
}
