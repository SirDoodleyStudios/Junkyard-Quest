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
    public Image displayCardType;

    RectTransform cardRect;

    //indentifier if the card is already instantiated so the the font resize function doesnt trigger again
    //mostly used for non combat card display
    bool isInstantiated;

    //this is the main canvas where the holder and 
    RectTransform canvasRect;

    private void Awake()
    {
        cardRect = gameObject.GetComponent<RectTransform>();
        canvasRect = transform.parent.parent.gameObject.GetComponent<RectTransform>();
        if (transform.parent.parent.gameObject.GetComponent<CardDrafting>() != null)
        {
            Debug.Log(cardRect.rect.width);
        }
    }
    private void OnEnable()
    {


        //For displaying values in scriptable object to card prefab
        displayCardName.text = card.cardName;
        displayEffect.text = card.effectText;
        displayEnergyCost.text = card.energyCost.ToString();
        displayArtwork.sprite = card.artwork;
        displayCardType.sprite = Resources.Load<Sprite>($"CardType/{card.cardType}");

        //Original Jigsaw Define
        //displayJigsawText.text = card.attachmentText;        
        //if(card.attachmentImage != null)
        //{
        //    displayJigsawImage.sprite = card.attachmentImage;
        //}

        //Using Randomizer Test Function

        if (card.jigsawEffect != null)
        {
            displayJigsawImage.sprite = card.jigsawEffect.jigsawImage;
            displayJigsawText.text = card.jigsawEffect.jigsawDescription;

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

    //called by the instantiating script, this allows the font size to adapt after the card is instantiated
    public void FontResize()
    {
        if (!isInstantiated)
        {
            isInstantiated = true;
            //the multiplier is derived from dividing an optimal fontsize for a screen size and dividing it with the card width, this will make it consistently appealing for all screen sizes
            //float multiplier = .0665474137f;
            //this multiplier is from default font in cmbat card 14/ default combat card width 240
            float multiplier = .0583333333f;
            displayCardName.fontSize = (cardRect.rect.width * multiplier) + 1;
            displayEffect.fontSize = cardRect.rect.width * multiplier;
            displayEnergyCost.fontSize = cardRect.rect.width * multiplier+2;
            displayJigsawText.fontSize = cardRect.rect.width * multiplier;

        }
    }

}
