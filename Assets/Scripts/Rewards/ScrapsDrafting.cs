using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrapsDrafting : MonoBehaviour
{
    //this int is to identify from what object index it came from
    public int objectOriginIndex;
    //the holder of choice cards
    public GameObject choiceHolder;
    GridLayoutGroup gridLayout;
    RectTransform parentCanvas;
    int scrapsValue;
    //reference for the Universal UI script
    public CameraUIScript cameraUIScript;

    private void Awake()
    {
        //set the current object at index 3 sibling so that universal Header is remains as sibling 4 so that we can access universalUI while choosing from draft
        transform.SetSiblingIndex(3);
        //find the UniversalUI whic is always the last sibling of under the canvas
        //curently, we use child 4 as the last sibling under canvas
        cameraUIScript = transform.parent.GetChild(4).GetChild(0).GetComponent<CameraUIScript>();


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
    //all values are in here, no reason to add value in the draft objects
    public void InitializeScrapValue(int scraps)
    {
        scrapsValue = scraps;

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

    //called by the dragNdrop
    public void ClaimScraps(int index)
    {
        //load the universalInformation and change it with values to save
        UniversalInformation universalInfo = UniversalSaveState.LoadUniversalInformation();

        switch (index)
        {
            case 0:
                universalInfo.scraps += scrapsValue;
                //update the UniversalUI to reflect change
                cameraUIScript.UpdateUIObjectsActiveScraps(universalInfo.scraps + scrapsValue);
                break;
            case 1:
                universalInfo.playerStats.HP += (int)Mathf.Floor(scrapsValue / 10);
                universalInfo.playerStats.currHP += (int)Mathf.Floor(scrapsValue / 10);
                //update the UniversalUI to reflect change
                cameraUIScript.UpdateUIObjectsHP(universalInfo.playerStats.currHP, universalInfo.playerStats.HP);
                break;
            case 2:
                universalInfo.playerStats.creativity += (int)Mathf.Floor(scrapsValue / 50);
                //update the UniversalUI to reflect change
                cameraUIScript.UpdateUIObjectsCretivity(universalInfo.playerStats.currCreativity, universalInfo.playerStats.creativity);
                break;
            default:
                break;
        }

        //save after changing
        UniversalSaveState.SaveUniversalInformation(universalInfo);
        cameraUIScript.UpdateUniversalInfo();

        //index 2 of the canvas panel is always the rewards manager
        RewardsManager rewardManager = transform.parent.GetChild(2).GetComponent<RewardsManager>();
        rewardManager.ClaimReward(objectOriginIndex);

        //destroy after picking
        Destroy(gameObject);
    }
    //calls the manager to incite countdown for closing rewards scene
    //private void OnDestroy()
    //{
    //    //index 2 of the canvas panel is always the rewards manager
    //    RewardsManager rewardManager = transform.parent.GetChild(2).GetComponent<RewardsManager>();
    //    rewardManager.ClaimReward(objectOriginIndex);
    //}



}
