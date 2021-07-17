using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RestManager : MonoBehaviour
{
    UniversalInformation universalInfo;
    //the panel that contains all rest action buttons, assigned in editor
    public GameObject actionsPanel;

    //remaining actions that the player can take during rest
    int remainingActions;

    //possible health recovery amount
    int healthRecovery;
    //possible creativity recovery ammount
    int creativeRecovery;
    //text to edit at start because of varying HP and Creativity values
    //assigned in editor
    public TextMeshProUGUI restText;
    public TextMeshProUGUI meditateText;
    public TextMeshProUGUI remainingActionsCountText;

    //gameObject reference for the CraftingUI
    public GameObject craftingUI;

    //the universal Header UI, assigned in editor
    public CameraUIScript cameraUIScript;
    //initialize the header and deck viewers
    private void Awake()
    {
        //sets the cell size of action buttons based on aspect ratio
        //constt multipliers are predetermind and precalculated
        actionsPanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Screen.width/4.26666667f, Screen.height/3.6f);
        actionsPanel.GetComponent<GridLayoutGroup>().spacing = new Vector2(Screen.height / 36, Screen.height / 36f);

        universalInfo = UniversalSaveState.LoadUniversalInformation();
        //will initialize the factory for getting cards using allcard enum
        CardSOFactory.InitializeCardSOFactory(universalInfo.chosenPlayer, universalInfo.chosenClass);
        //initializes the deck viewr
        CardTagManager.InitializeTextDescriptionDictionaries();
        cameraUIScript.GenerateDeck(universalInfo);
        cameraUIScript.AssignUIObjects(universalInfo);

        //default remaining actions is 3, might get changed with gear modifiers
        remainingActions = universalInfo.remainingRestActions;

    }

    private void Start()
    {
        //automatically assigns 20% of max health as the health recovery amount
        healthRecovery = Mathf.FloorToInt(universalInfo.playerStatsWrapper.HP * .2f);
        //auomatically assigns 10% of max creativity as the creativity bonus next turn
        creativeRecovery = Mathf.FloorToInt(universalInfo.playerStatsWrapper.Creativity * .1f);
        //assign texts
        restText.text = $"Rest: Heal {healthRecovery} HP";
        meditateText.text = $"Meditate: Gain {creativeRecovery} creativity at the start of next combat";
        //remaining actions text
        remainingActionsCountText.text = $"{remainingActions}";
    }

    public void RestButton()
    {
        //prevents the HP from being recovered beyond max
        int tempHP = Mathf.Min((universalInfo.playerStats.currHP += healthRecovery), universalInfo.playerStats.HP);
        universalInfo.playerStats.currHP = tempHP;
        universalInfo.wornOutCount = 0;
        cameraUIScript.AssignUIObjects(universalInfo);
        UpdateRemainingActions();
    }

    public void MeditateButton()
    {
        //prevents th creativity from gowing beyond mac
        int tempCreativity = Mathf.Min((universalInfo.playerStats.currCreativity += creativeRecovery), universalInfo.playerStats.creativity);
        universalInfo.playerStats.currCreativity = tempCreativity;
        cameraUIScript.AssignUIObjects(universalInfo);
        UpdateRemainingActions();
    }

    public void CraftingButton()
    {
        craftingUI.SetActive(true);
    }
    //called by crafting list after a successful craft, this will update the universalInformation of the restmanager with the correct material and gear list before being saved
    public void UpdateMaterialAndGearList(List<CraftingMaterialSO> materialList, GearSO gearSO)
    {
        universalInfo.craftingMaterialSOList = materialList;
        //we can't add an element directly in the universalInfo becasue static scripts can't have it's own instance
        List<GearSO> gearList = universalInfo.gearList;
        gearList.Add(gearSO);
        universalInfo.gearList = gearList;
    }

    //function to disable all action buttons when remaining actions count is 0
    public void UpdateRemainingActions()
    {
        remainingActions--;
        remainingActionsCountText.text = $"{remainingActions}";
        if (remainingActions == 0)
        {
            foreach (Transform actionTrans in actionsPanel.transform)
            {
                //checks if object is active first
                if (actionTrans.gameObject.activeSelf)
                {
                    actionTrans.GetComponent<Button>().interactable = false;
                }
            }
        }
        universalInfo.remainingRestActions = remainingActions;

        //each action will save to universal info now
        UniversalSaveState.SaveUniversalInformation(universalInfo);

        //will update the universal UI's current copy of universalInformation
        cameraUIScript.UpdateUniversalInfo();
    }

    //used to go back to overworld
    public void GoBackToOverworldButton()
    {
        //sets the remaining actions back to 3 when exiting
        universalInfo.remainingRestActions = 3;
        UniversalSaveState.SaveUniversalInformation(universalInfo);
        SceneManager.LoadScene("OverWorldScene");
    }
}
