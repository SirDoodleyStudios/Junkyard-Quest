using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PriceTag : MonoBehaviour
{
    public int priceTag;
    //the price tag's transform
    RectTransform tagRect;

    public TextMeshProUGUI scrapValueText;
    private void Awake()
    {
        tagRect = gameObject.GetComponent<RectTransform>();
        //price tag size will differ depending on screen resoulution
        tagRect.sizeDelta = new Vector2 (Screen.width * .10f, Screen.height * .05f);
        //place the price tag under the object 
        tagRect.anchoredPosition = new Vector2(0, -Screen.height*.05f);

    }
    //called by the OptionsScript when enabling or instantiating the item
    public void SetPriceTag(int sentPrice)
    {
        priceTag = sentPrice;
        scrapValueText.text = $"{sentPrice}";
    }
}
