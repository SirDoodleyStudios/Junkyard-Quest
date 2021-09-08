using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearDrafting : MonoBehaviour
{
    //this int is to identify from what object index it came from
    public int objectOriginIndex;
    //the holder of choice cards
    public GameObject choiceHolder;
    GridLayoutGroup gridLayout;
    RectTransform parentCanvas;

    //skip button reference
    Button skipButton;

    //reference for the Universal UI script
    public CameraUIScript cameraUIScript;

    private void Awake()
    {
        //set the current object at index 4 sibling so that universal Header is last sibling
        transform.SetSiblingIndex(4);
        //find the UniversalUI whic is always the last sibling of under the canvas
        //curently, we use child 5 as the last sibling under canvas
        cameraUIScript = transform.parent.GetChild(5).GetChild(0).GetComponent<CameraUIScript>();

        //the skip button is always the last child under the cardDrafting object
        skipButton = transform.GetChild(transform.childCount - 1).GetComponent<Button>();
        //assign the button for skipCard
        skipButton.onClick.AddListener(() => SkipGear());

        //first child is the holder of cards
        parentCanvas = transform.parent.GetComponent<RectTransform>();
        gridLayout = choiceHolder.GetComponent<GridLayoutGroup>();
        //standard material prefab size is 864, 325
        gridLayout.cellSize = new Vector2(Screen.width * .45f, Screen.height * .300925926f);
    }

    //called by RewardObject to show material choices
    //this list must only contain 2 gears
    public void InitializeGearChoices(List<GearSO> gears)
    {
        for (int i = 0; 1 >= i; i++)
        {
            GameObject gearObj = choiceHolder.transform.GetChild(i).gameObject;
            GearSOHolder gearSOHolder = gearObj.GetComponent<GearSOHolder>();
            gearSOHolder.InitializeGearPrefab(gears[i]);
            gearObj.SetActive(true);
        }
    }

    //add the material in inventory, called by the dragNDrop
    public void AddToInventory(GearSO addedSO)
    {
        UniversalInformation universalInfo = UniversalSaveState.LoadUniversalInformation();
        //generate the wrapper
        GearWrapper gearWrapper = new GearWrapper(addedSO);
        universalInfo.gearWrapperList.Add(gearWrapper);
        //update universalUI
        cameraUIScript.UpdateGearInventory(gearWrapper, true);

        UniversalSaveState.SaveUniversalInformation(universalInfo);
        cameraUIScript.UpdateUniversalInfo();

        //calls rewardManager to disable to reward object
        RewardsManager rewardManager = transform.parent.GetChild(2).GetComponent<RewardsManager>();
        rewardManager.ClaimReward(objectOriginIndex);

        Destroy(gameObject);
    }

    void SkipGear()
    {
        //Used when skip is for abandoning the choice instead of just closing the draftWindow
        //RewardsManager rewardManager = transform.parent.GetChild(2).GetComponent<RewardsManager>();
        //rewardManager.ClaimReward(objectOriginIndex);
        Destroy(gameObject);
    }
}
