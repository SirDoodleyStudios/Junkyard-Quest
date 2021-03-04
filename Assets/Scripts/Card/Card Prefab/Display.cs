using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Display : MonoBehaviour
{
    public Card card;

    //public Text displayCardName;
    //public Text displayEffect;
    ////public Text displayLeftEffect;
    ////public Text displayRightEffect;
    //public Text displayJigsawText;

    //public Text displayEnergyCost;
    ////public Text displayCreativityCost;

    public TextMeshProUGUI displayCardName;
    public TextMeshProUGUI displayEffect;
    //public Text displayLeftEffect;
    //public Text displayRightEffect;
    public TextMeshProUGUI displayJigsawText;

    public TextMeshProUGUI displayEnergyCost;


    public Image displayArtwork;
    public Image displayJigsawImage;

    private void Start()
    {
    }
    private void OnEnable()
    {
        //For displaying values in scriptable object to card prefab
        displayCardName.text = card.cardName;
        displayEffect.text = card.effect;
        //displayLeftEffect.text = card.leftEffect;
        //displayRightEffect.text = card.rightEffect;
        displayJigsawText.text = card.attachmentText;
        displayEnergyCost.text = card.energyCost.ToString();
        //displayCreativityCost.text = card.creativityCost.ToString();
        displayArtwork.sprite = card.artwork;
        if(card.attachmentImage != null)
        {
            displayJigsawImage.sprite = card.attachmentImage;
        }


        //Determines Card layer from CardType enum
        //9 is offense, 10 is util, 11 is ability
        //switch (card.cardType)
        //{
        //    case CardType.Offense:
        //        gameObject.layer = 9;
        //        break;

        //    case CardType.Utility:
        //        gameObject.layer = 10;
        //        break;

        //    case CardType.Ability:
        //        gameObject.layer = 11;
        //        break;
        //}
    }

}
