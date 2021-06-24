using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JigsawLinkAlterer : MonoBehaviour
{

    //the buttons here are the alterers, assigned in script because they will be accessing the same function
    public Button inputUp;
    public Button inputDown;
    public Button outputUp;
    public Button outputDown;

    //the sample display of the to be forged card
    //assigned in editor
    public GameObject forgedCardPrefab;
    Display forgedCardDisplay;
    Card forgedCardCard;
    //sets the initial links
    JigsawLink inputLink;
    JigsawLink outputLink;
    Image jigsawImage;

    //cmeraUIScript that contains the deck update function
    //assigned in editor
    public CameraUIScript cameraUIScript;

    void Start()
    {
        //assigns listeners to the buttons in the jigsawLink reassign UI
        inputUp.onClick.AddListener(() => RespecJigsawLinks(inputUp));
        inputDown.onClick.AddListener(() => RespecJigsawLinks(inputDown));
        outputUp.onClick.AddListener(() => RespecJigsawLinks(outputUp));
        outputDown.onClick.AddListener(() => RespecJigsawLinks(outputDown));


        //sets the initial links
        //inputLink = forgedCardCard.jigsawEffect.inputLink;
        //outputLink = forgedCardCard.jigsawEffect.outputLink;


    }

    //calls the display in the cardPrefab here to change the visuals of the image
    public void RespecJigsawLinks(Button button)
    {
        if(button == inputUp)
        {
            //for up buttons
            //if JigsawLink is already index 2, go to 0
            if (inputLink == (JigsawLink)2)
            {
                inputLink = 0;
            }
            else
            {
                inputLink++;
            }
            forgedCardDisplay.JigsawImageReroll(inputLink, outputLink);

        }
        else if (button == inputDown)
        {
            //for down buttons
            //if JigsawLink is already 0, go to 2
            if (inputLink == 0)
            {
                inputLink = (JigsawLink)2;
            }
            else
            {
                inputLink--;
            }
            forgedCardDisplay.JigsawImageReroll(inputLink, outputLink);

        }
        else if (button == outputUp)
        {
            if (outputLink == (JigsawLink)2)
            {
                outputLink = 0;
            }
            else
            {
                outputLink++;
            }
            forgedCardDisplay.JigsawImageReroll(inputLink, outputLink);
        }
        else if (button == outputDown)
        {
            if (outputLink == 0)
            {
                outputLink = (JigsawLink)2;
            }
            else
            {
                outputLink--;
            }
            forgedCardDisplay.JigsawImageReroll(inputLink, outputLink);
        }


    }

    //functin called by the forge manager once forge button is clicked in the forge manager
    public void InitialAlterState(Card card)
    {
        forgedCardDisplay = forgedCardPrefab.GetComponent<Display>();
        forgedCardPrefab.GetComponent<Display>().card = card;
        forgedCardCard = forgedCardPrefab.GetComponent<Display>().card;
        inputLink = forgedCardCard.jigsawEffect.inputLink;
        outputLink = forgedCardCard.jigsawEffect.outputLink;
        forgedCardPrefab.SetActive(true);
    }

    //called after pressing the ok button once forging is acceptable
    //saves the card list and returns the card prefab and choosing panel to disabled
    public void BackToForgeButton()
    {
        JigsawFormat forgedJigsaw = forgedCardCard.jigsawEffect;
        forgedJigsaw.inputLink = inputLink;
        forgedJigsaw.outputLink = outputLink;
        forgedJigsaw.jigsawImage = forgedCardDisplay.JigsawImageReroll(inputLink, outputLink).sprite;
        //forgedCardDisplay.JigsawImageReroll(inputLink, outputLink);
        forgedCardPrefab.SetActive(false);
        cameraUIScript.UpdateCurrentDeck(forgedCardCard, true);
        gameObject.SetActive(false);
    }
}
