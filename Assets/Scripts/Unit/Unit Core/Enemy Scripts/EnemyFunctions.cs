using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyFunctions : BaseUnitFunctions
{

    //delegate for calling all methods on scripts inside the player prefab
    public delegate void D_StartEnemyUpdates();
    public event D_StartEnemyUpdates d_StartEnemyUpdates;

    //attached enemyUnitScriptable
    //contains stats and list of actions
    public EnemyUnit enemyUnit;
    //the list of EAFormats that the enemy can draw
    //made public so that it can be saved in CombatSaveState
    public List<EnemyActionFormat> actionDeck = new List<EnemyActionFormat>();
    //the list of EAFormats that the enemy has in hand
    //OUTDTED
    List<EnemyActionFormat> actionHand = new List<EnemyActionFormat>();



    //for enemy action hand
    //contains the gameobjects under the actions panel
    public GameObject actionPanel;
    List<GameObject> actionSlotObjects = new List<GameObject>();
    //contains the list of sprites for each attack typem this is manually inserted in inspector\

    //Decomissioned by 
    //public List<Sprite> actionIconSprites = new List<Sprite>();
    ////Dictionary to identify what sprite will be used depending on the action type of the enemy action
    //public Dictionary<EnemyActionType, Sprite> actionIconsDict = new Dictionary<EnemyActionType, Sprite>();

    //for action intent panel
    public GameObject intentPanel;
    //list that contains the gameObjects under the intent panel
    //public List<GameObject> intentSlots;

    //holds the action prefab to be generated at initizalization, determined by draw
    public GameObject enemyActionPrefab;

    //EnemyActionformat from enemy hand, intendedAction becomes the actual action at player's end turn
    //intendedActionHolder is the gameObject to enable and disable that contains the indededAction
    //EnemyActionFormat intendedAction;
    //GameObject intendedActionHolder;

    //List<EnemyActionFormat> intendedActions = new List<EnemyActionFormat>();
    //List<GameObject> intendedActionHolders = new List<GameObject>();

    //identifier if enemy is dead and if overkilled
    //is changed by combat manager
    public bool isAlive;
    public bool isBeingOverkilled;
    public bool isOverKilled;


    public override void InitializeStats()
    {
        //Copies HP, Creativity, and Draw from scriptable Object first
        maxHP = enemyUnit.HP;
        currHP = enemyUnit.currHP;
        maxCreativity = enemyUnit.Creativity;
        defaultDraw = enemyUnit.draw;
        base.InitializeStats();

        //List of EnemyActionFormats in enemyUnit, add range so that the original scriptableObject is not affected when moving stuff
        actionDeck.AddRange(enemyUnit.actionList);

        //instantiates empty card prefabs based on the enemy draw
        for (int i = 0; enemyUnit.draw > i; i++)
        {
            GameObject freshInstantiate = Instantiate(enemyActionPrefab, actionPanel.transform);
            freshInstantiate.SetActive(false);
        }

        //assigning icon sprites to actiontypenums
        //decomissioned by Resources.Load
        //actionIconsDict.Add(EnemyActionType.Offense, actionIconSprites[0]);
        //actionIconsDict.Add(EnemyActionType.Block, actionIconSprites[1]);
        //actionIconsDict.Add(EnemyActionType.Enhance, actionIconSprites[2]);
        //actionIconsDict.Add(EnemyActionType.Debilitate, actionIconSprites[3]);
        //actionIconsDict.Add(EnemyActionType.Summon, actionIconSprites[4]);
        //actionIconsDict.Add(EnemyActionType.Special, actionIconSprites[5]);

    }

    //will determine what will enemy do next
    public void EnemyStartTurn()
    {
        //if the enemy is istill is positive HP, proceed to enemy act, if not, activate death function
        if (isBeingOverkilled)
        {
            ActivateEnemyDeath();
        }
        else
        {
            EnemyAct();
        }
    }

    public override void TakeDamage(int damageValue)
    {
        base.TakeDamage(damageValue);
        //if enemy hp reaches 0 or negative, set isbeingOverkilled
        if (currHP <= 0)
        {
            isBeingOverkilled = true;
        }
    }

    //death method to be called at the start of enemy turn if curr HP is 0 or negative
    //also called if HP reaches overkill threshhold
    public void ActivateEnemyDeath()
    {
        //this is so that only active objects are considered
        if (gameObject.activeSelf)
        {
            //overkill logic will occur when death method is called and negative current HP is less than negative of half of max HP
            if (-currHP <= -(enemyUnit.HP / 2))
            {
                isAlive = false;
                isBeingOverkilled = false;
                isOverKilled = true;

                //calls the enemyCounterIdentifier that calls an event when all enemies are gone
                //defined in BaseUnitFunctions
                //RegisterEnemyKIll parameter will determine if the kill is an overkill
                enemyAIManager.RegisterEnemyKill();

                //Comment out for Testing
                gameObject.SetActive(false);
            }
            //no overkill
            else if (-currHP >= -(enemyUnit.HP / 2))
            {
                isAlive = false;
                isBeingOverkilled = false;
                isOverKilled = false;
                enemyAIManager.RegisterEnemyKill();

                //Comment out for Testing
                gameObject.SetActive(false);
            }
        }
    }

    //Function for updating Sliders
    public override void SliderValueUpdates()
    {
        //updates HP sliders
        base.SliderValueUpdates();
    }

    public void EnemyPrepare()
    {        

        if (actionPanel.transform.GetChild(0).gameObject.activeSelf == false)
        {
            EnemyDrawHand2();
        }
        EnemyCastIntent2();
    }



    public void EnemyAct()
    {

        //removes block first before acting
        base.RemoveBlock();
        //iterates through the intent panel and activate their effects
        foreach (Transform intentTransforms in intentPanel.transform)
        {
            //FOR LINKING like Creativity
            //activates the first EnemyActionIcon first then carries over to the next before activating the second effect just like in creative mode
            //for (int i = 0; intentTransforms.GetSiblingIndex() >= i; i++)
            //{
            //    EnemyActionFormat actionFormat = intentPanel.transform.GetChild(i).GetComponent<EnemyActionIcon>().enemyAction;
            //    EnemyActionFactory.GetEnemyActionEffect(actionFormat.enumEnemyAction).InitializeEnemyAction(enemyUnit, gameObject);

            //}
            //intentTransforms.gameObject.SetActive(false);
            //for linking without any repetition
            GameObject intentObject = intentTransforms.gameObject;
            EnemyActionFormat actionFormat = intentObject.GetComponent<EnemyActionIcon>().enemyAction;
            EnemyActionFactory.GetEnemyActionEffect(actionFormat.enumEnemyAction).InitializeEnemyAction(enemyUnit, gameObject);

        }
        //for transferring enemy actions back to the actionPanel 
        //only checks index 0 because we're returning the prefabs to actionHand one by one
        while (intentPanel.transform.childCount > 0)
        {
            Transform returningAction = intentPanel.transform.GetChild(0);
            EnemyActionFormat returningActionFormat = returningAction.gameObject.GetComponent<EnemyActionIcon>().enemyAction;
            //returns the EnemyActionFormat to deckHolder
            actionDeck.Add(returningAction.gameObject.GetComponent<EnemyActionIcon>().enemyAction);
            returningAction.SetParent(actionPanel.transform);
            returningAction.SetAsLastSibling();
        }
    }

    //returning the enemyactionformat attached to the enemy intent
    public EnemyActionFormat AccessEnemyIntents(int intentIndex)
    {
        EnemyActionFormat enemyIntent = intentPanel.transform.GetChild(intentIndex).GetComponent<EnemyActionIcon>().enemyAction;
        return enemyIntent;
    }

    public void EnemyDrawHand2()
    {
        //defaults the draw to enemy's EnemyActionFormat draw stat
        //THE ACTION DECK WILL BE SAVED AND LOADED IN COMBAT SAVES
        for(int i = 0; enemyUnit.draw > i; i++)
        {
            EnemyActionFormat tempAction = actionDeck[Random.Range(0, actionDeck.Count)];
            //for assigning sprites and enabling action slot gameObject
            //iterates through the action panel to enable action slots and assign sprites
            foreach (Transform actionSlot in actionPanel.transform)
            {
                //cache for the child transform's gameObject
                //child 3 is the text object
                GameObject actionObject = actionSlot.gameObject;
                TextMeshProUGUI actionObjectNumberText = actionObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
                //iterates till it finds a disabled slot, enables it then breaks
                if (actionObject.activeSelf == false)
                {
                    //removed from deck so that an action is not repeated during draw
                    actionDeck.Remove(tempAction);

                    //Each action has the EnemyActionIcon script that contains the sprite holder and EnemyActionFormat of the action itself
                    EnemyActionIcon enemyActionIcon = actionSlot.GetComponent<EnemyActionIcon>();
                    //assigns which sprite to use from the sprite dictionary
                    //enemyActionIcon.actionIcon.sprite = actionIconsDict[tempAction.actionType];       //sprite dictionary approach
                    //CONSIDER USING SCRIPTABLE OBJECTS INSTEAD OF RESOURCE FOLDER
                    enemyActionIcon.actionIcon.sprite = Resources.Load<Sprite>($"EnemyActionIcon/{tempAction.actionType}");
                    //assigns the EnemyActionFormat from the deck to the EnemyActionIcon holder
                    enemyActionIcon.enemyAction = tempAction;
                    //assign random input and output link
                    JigsawLink tempInput = (JigsawLink)Random.Range(0, 2);
                    JigsawLink tempOutput = (JigsawLink)Random.Range(0, 2);
                    enemyActionIcon.enemyAction.inputLink = tempInput;
                    enemyActionIcon.enemyAction.outputLink = tempOutput;
                    enemyActionIcon.inputLinkImage.sprite = Resources.Load<Sprite>($"EnemyActionIcon/{tempInput}");
                    enemyActionIcon.outputLinkImage.sprite = Resources.Load<Sprite>($"EnemyActionIcon/{tempOutput}");

                    //calls local function to get respective action pattern number texts, potency for attacks and blocks, stacks for enhance and debilitates
                    actionObjectNumberText.text = ActionNumbersGet(tempAction.enumEnemyAction);
                    actionObject.SetActive(true);
                    break;
                }
            }
        }       

    }

    //This function is solely for getting text numbers to be displayed on enemy action and intents
    private string ActionNumbersGet(AllEnemyActions actions)
    {
        string numberText;
        switch (actions)
        {
            //AttackPatterns
            case AllEnemyActions.Enemy_AttackPattern1:
                if (enemyUnit.HitsPattern1 > 1)
                {
                    numberText = $"{enemyUnit.AttackPattern1}X{enemyUnit.HitsPattern1}";
                }
                else
                {
                    numberText = enemyUnit.AttackPattern1.ToString();
                }                
                break;
            case AllEnemyActions.Enemy_AttackPattern2:
                if (enemyUnit.HitsPattern2 > 1)
                {
                    numberText = $"{enemyUnit.AttackPattern2}X{enemyUnit.HitsPattern2}";
                }
                else
                {
                    numberText = enemyUnit.AttackPattern2.ToString();
                }
                break;
            case AllEnemyActions.Enemy_AttackPattern3:
                if (enemyUnit.HitsPattern3 > 1)
                {
                    numberText = $"{enemyUnit.AttackPattern3}X{enemyUnit.HitsPattern3}";
                }
                else
                {
                    numberText = enemyUnit.AttackPattern3.ToString();
                }
                break;
            //BlockPatterns
            case AllEnemyActions.Enemy_BlockPattern1:
                numberText = enemyUnit.AttackPattern1.ToString();
                break;
            case AllEnemyActions.Enemy_BlockPattern2:
                numberText = enemyUnit.AttackPattern2.ToString();
                break;
            case AllEnemyActions.Enemy_BlockPattern3:
                numberText = enemyUnit.AttackPattern3.ToString();
                break;
            //EnhancePatterns
            case AllEnemyActions.Enemy_EnhancePattern1:
                numberText = enemyUnit.EnhanceStackPattern1.ToString();
                break;
            case AllEnemyActions.Enemy_EnhancePattern2:
                numberText = enemyUnit.EnhanceStackPattern2.ToString();
                break;
            case AllEnemyActions.Enemy_EnhancePattern3:
                numberText = enemyUnit.EnhanceStackPattern3.ToString();
                break;
            //DebilitatePatterns
            case AllEnemyActions.Enemy_DebilitatePattern1:
                numberText = enemyUnit.DebilitateStackPattern1.ToString();
                break;
            case AllEnemyActions.Enemy_DebilitatePattern2:
                numberText = enemyUnit.DebilitateStackPattern2.ToString();
                break;
            case AllEnemyActions.Enemy_DebilitatePattern3:
                numberText = enemyUnit.DebilitateStackPattern3.ToString();
                break;

            default:
                numberText = "";
                break;
        }
        return numberText;
    }

    public void EnemyCastIntent2()
    {
        //this bool is for determining whether we move on from finding links
        bool isNoMoreLinks = false;
        //do is for assuring that we find an initial intent
        do
        {
            //for finding initial intent
            if (intentPanel.transform.childCount <= 0)
            {
                //only finds from actions that are enabled
                //adds the action from action panel to in a list to be transported to the intents panel
                List<GameObject> tempList = new List<GameObject>();               
                foreach (Transform actionTransform in actionPanel.transform)
                {
                    if (actionTransform.gameObject.activeSelf)
                    {
                        tempList.Add(actionTransform.gameObject);
                    }
                    
                }
                //actual action transferring line
                tempList[Random.Range(0, tempList.Count - 1)].transform.SetParent(intentPanel.transform);


            }
            //for finding available links for the initial intent
            //limit of 2 links only or 3 actions
            else if (intentPanel.transform.childCount < 3)
            {
                //checks each remaining card in in actionHand if there are linkables
                foreach (Transform actionTransform in actionPanel.transform)
                {
                    if (actionTransform.gameObject.activeSelf == true)
                    {
                        EnemyActionIcon inputIntent = actionTransform.gameObject.GetComponent<EnemyActionIcon>();
                        EnemyActionIcon outputIntent = intentPanel.transform.GetChild(intentPanel.transform.childCount - 1).gameObject.GetComponent<EnemyActionIcon>();

                        //checks the last element of the link if its output link matches the elements iterated through
                        if (inputIntent.enemyAction.inputLink == outputIntent.enemyAction.outputLink)
                        {
                            //transfers the gameObject itself to be under the intent panel
                            actionTransform.SetParent(intentPanel.transform);
                            isNoMoreLinks = false;
                            break;
                        }
                        
                    }
                    //if we get here, no more available enemyActions
                    isNoMoreLinks = true;

                }
            }
            //if 3 intent slots are picked, immediately break
            else
            {
                isNoMoreLinks = true;
            }

        //will keep iterating and finding links until there are none
        } while (isNoMoreLinks == false);

    }

    //For loading enemy intents and actions from CombatSaveState file
    public void EnemyPrepareFromFileLoad(EnemyUnitStatsWrapper enemyWrapper)
    {
        //temporary holder of decryted EnemyActionFormat for ActionPanel and IntentPanel
        List<EnemyActionFormat> actionPanelList = new List<EnemyActionFormat>();
        List<EnemyActionFormat> intentPanelList = new List<EnemyActionFormat>();




        //clear action deck firs becuse it might have been populated during initialize
        actionDeck.Clear();
        //decrypts all action format wrappers back to actul scripts
        //fill-up enemy action deck
        foreach (EnemyActionFormatWrapper enemyActionWrapper in enemyWrapper.currentActionDeck)
        {
            //Loads the a generic EnemyctioFormt to be instantiated
            EnemyActionFormat tempFormat = Instantiate(Resources.Load<EnemyActionFormat>("EnemyUnits/EnemyActionFormatBase"));

            //assigns wrapper values to EnemyActionFormt values
            tempFormat.inputLink = enemyActionWrapper.inputLink;
            tempFormat.outputLink = enemyActionWrapper.outputLink;
            tempFormat.actionType = enemyActionWrapper.actionType;
            tempFormat.enumEnemyAction = enemyActionWrapper.enumEnemyAction;

            actionDeck.Add(tempFormat);
        }

        //decrypts the actionPanelWrappers
        foreach (EnemyActionFormatWrapper enemyActionWrapper in enemyWrapper.actionPanelActions)
        {
            //Loads the a generic EnemyctioFormt to be instantiated
            EnemyActionFormat tempFormat = Instantiate(Resources.Load<EnemyActionFormat>("EnemyUnits/EnemyActionFormatBase"));

            //assigns wrapper values to EnemyActionFormt values
            tempFormat.inputLink = enemyActionWrapper.inputLink;
            tempFormat.outputLink = enemyActionWrapper.outputLink;
            tempFormat.actionType = enemyActionWrapper.actionType;
            tempFormat.enumEnemyAction = enemyActionWrapper.enumEnemyAction;

            actionPanelList.Add(tempFormat);
        }
        //ssigns actionFormts to actionPanel game objects
        for (int i = 0; enemyWrapper.actionPanelActions.Count-1 >= i; i++)
        {
            GameObject actionObject = actionPanel.transform.GetChild(i).gameObject;
            EnemyActionIcon actionIcon = actionObject.GetComponent<EnemyActionIcon>();
            //ActionIcon text is stored in a child object at 3 index of the enemyActionIconPrefab
            TextMeshProUGUI actionNumberText = actionObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
            actionIcon.enemyAction = actionPanelList[i];

            actionIcon.actionIcon.sprite = Resources.Load<Sprite>($"EnemyActionIcon/{actionPanelList[i].actionType}");
            //assign random input and output link
            actionIcon.inputLinkImage.sprite = Resources.Load<Sprite>($"EnemyActionIcon/{actionPanelList[i].inputLink}");
            actionIcon.outputLinkImage.sprite = Resources.Load<Sprite>($"EnemyActionIcon/{actionPanelList[i].outputLink}");

            actionObject.SetActive(true);
            //gets text
            actionNumberText.text = ActionNumbersGet(actionPanelList[i].enumEnemyAction);

        }

        //assign actionFormats to intentPanel game Objects
        foreach (EnemyActionFormatWrapper enemyActionWrapper in enemyWrapper.intentPanelActions)
        {
            //Loads the a generic EnemyctioFormt to be instantiated
            EnemyActionFormat tempFormat = Instantiate(Resources.Load<EnemyActionFormat>("EnemyUnits/EnemyActionFormatBase"));

            //assigns wrapper values to EnemyActionFormt values
            tempFormat.inputLink = enemyActionWrapper.inputLink;
            tempFormat.outputLink = enemyActionWrapper.outputLink;
            tempFormat.actionType = enemyActionWrapper.actionType;
            tempFormat.enumEnemyAction = enemyActionWrapper.enumEnemyAction;

            intentPanelList.Add(tempFormat);
        }
        for (int i = 0; enemyWrapper.intentPanelActions.Count - 1 >= i; i++)
        {
            GameObject instantiatedObject = Instantiate(enemyActionPrefab, intentPanel.transform);
            EnemyActionIcon instantiatedActionIcon = instantiatedObject.GetComponent<EnemyActionIcon>();
            //ActionIcon text is stored in a child object at 3 index of the enemyActionIconPrefab
            TextMeshProUGUI intentActionText = instantiatedObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
            instantiatedActionIcon.enemyAction = intentPanelList[i];

            instantiatedActionIcon.actionIcon.sprite = Resources.Load<Sprite>($"EnemyActionIcon/{intentPanelList[i].actionType}");
            //assign random input and output link
            instantiatedActionIcon.inputLinkImage.sprite = Resources.Load<Sprite>($"EnemyActionIcon/{intentPanelList[i].inputLink}");
            instantiatedActionIcon.outputLinkImage.sprite = Resources.Load<Sprite>($"EnemyActionIcon/{intentPanelList[i].outputLink}");

            //gets text
            intentActionText.text = ActionNumbersGet(intentPanelList[i].enumEnemyAction);
        }

    }
   
}

