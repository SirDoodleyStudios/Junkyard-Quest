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

        //show the jigsaw image only if the card has a jigsawEffect, clear the sprites if not
        if (card.jigsawEffect != null)
        {
            displayJigsawImage.sprite = card.jigsawEffect.jigsawImage;
            displayJigsawText.text = card.jigsawEffect.jigsawDescription;

        }
        else
        {
            displayJigsawImage.sprite = null;
            displayJigsawText.text = null;
        }

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

    //called by ForgeManager, reassign the jigsaw image depending on the chosen input and output links
    // only affects the visuals when this is called, visual of actual card prefab is only a holder for the link reroll window
    // will return the determined sprite back to jigsaw alterer so that we can save it
    public Image JigsawImageReroll(JigsawLink inputLink, JigsawLink outputLink)
    {
        displayJigsawImage.sprite = Resources.Load<Sprite>($"Jigsaw/{inputLink}2{outputLink}");
        return displayJigsawImage;

    }

}
