using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    //the universal Header UI, assigned in editor
    public CameraUIScript cameraUIScript;
    //initialize the header and deck viewers
    private void Awake()
    {
        universalInfo = UniversalSaveState.LoadUniversalInformation();
        //will initialize the factory for getting cards using allcard enum
        CardSOFactory.InitializeCardSOFactory(universalInfo.chosenPlayer, universalInfo.chosenClass);
        //initializes the deck viewr
        CardTagManager.InitializeTextDescriptionDictionaries();
        cameraUIScript.GenerateDeck(universalInfo);
        cameraUIScript.AssignUIObjects(universalInfo);

        //default remaining actions is 3, might get changed with gear modifiers
        remainingActions = 3;

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
    }
}
