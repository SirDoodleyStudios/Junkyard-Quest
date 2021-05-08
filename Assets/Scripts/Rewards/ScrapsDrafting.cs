using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrapsDrafting : MonoBehaviour
{
    //the holder of choice cards
    public GameObject choiceHolder;
    GridLayoutGroup gridLayout;
    RectTransform parentCanvas;

    private void Awake()
    {
        //first child is the holder of cards
        parentCanvas = transform.parent.GetComponent<RectTransform>();
        gridLayout = choiceHolder.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(parentCanvas.rect.width * .13416815742f, parentCanvas.rect.height * .34280604133f);

        //also assigns the cell size of the holder to the rect sizes of the cards themselves
        foreach (Transform card in choiceHolder.transform)
        {
            //resizing the choice object itself
            RectTransform tempRect = card.gameObject.GetComponent<RectTransform>();
            tempRect.sizeDelta = new Vector2(parentCanvas.rect.width * .13416815742f, parentCanvas.rect.height * .34280604133f);

            //assigning text depending on scrap Value
        }



    }

    //should be called by rewards manager
    public void InitializeScrapValue(int scraps)
    {
        //child 6 of each choice is the Effect texf
        GameObject scrapObject = choiceHolder.transform.GetChild(0).gameObject;
        GameObject HPObject = choiceHolder.transform.GetChild(1).gameObject;
        GameObject CreativityObject = choiceHolder.transform.GetChild(2).gameObject;


        TextMeshProUGUI scrapGainText = scrapObject.transform.GetChild(6).gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI HPGainText = HPObject.transform.GetChild(6).gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI CreativityGainText = CreativityObject.transform.GetChild(6).gameObject.GetComponent<TextMeshProUGUI>();

        //assign text based on Scrap value
        //scraps choice is enabled always
        scrapObject.SetActive(true);
        scrapGainText.text = $"Gain {scraps} scraps";
        //HP gain option only available if scraps/10 is 1 or more
        //will most likely happen all the time
        if (Mathf.Floor(scraps/10) >= 1)
        {
            HPObject.SetActive(true);
            HPGainText.text = $"Gain +{Mathf.Floor(scraps / 10)} HP";
        }
        //Creativity can only increased if scraps reward are 50 or more
        if (Mathf.Floor(scraps / 50) >= 1)
        {
            CreativityObject.SetActive(true);
            CreativityGainText.text = $"Gain +{Mathf.Floor(scraps / 50)} Creativity";
        }

    }



}
