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
    List<EnemyActionFormat> actionDeck = new List<EnemyActionFormat>();
    //the list of EAFormats that the enemy has in hand
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
    EnemyActionFormat intendedAction;
    GameObject intendedActionHolder;

    List<EnemyActionFormat> intendedActions = new List<EnemyActionFormat>();
    List<GameObject> intendedActionHolders = new List<GameObject>();


    public override void InitializeStats()
    {
        //Copies HP, Creativity, and Draw from scriptable Object first
        maxHP = enemyUnit.HP;
        maxCreativity = enemyUnit.Creativity;
        defaultDraw = enemyUnit.draw;
        base.InitializeStats();

        //List of EnemyActionFormats in enemyUnit, add range so that the original scriptableObject is not affected when moving stuff
        actionDeck.AddRange(enemyUnit.actionList);

        //instantiates card prefabs
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


    //Function for updating Sliders
    public override void SliderValueUpdates()
    {
        //updates HP sliders
        base.SliderValueUpdates();
    }

    public void EnemyPrepare()
    {

        //if (actionHand.Count <= 0)
        //{
        //    EnemyDrawHand();
        //}
        //EnemyCastIntent();

        //if (actionHand.Count <= 0)
        //{
        //    EnemyDrawHand2();
        //}
        //EnemyCastIntent2();

        

        if (actionPanel.transform.GetChild(0).gameObject.activeSelf == false)
        {
            EnemyDrawHand2();
        }
        EnemyCastIntent2();
    }

    public void EnemyAct()
    {

        //base.RemoveBlock();
        //foreach (EnemyActionFormat intendedAction in intendedActions)
        //{
        //    for (int i = 0; intendedActions.IndexOf(intendedAction) >= i; i++)
        //    {
        //        EnemyActionFactory.GetEnemyActionEffect(intendedActions[i].enumEnemyAction).InitializeEnemyAction(enemyUnit, gameObject);
        //        //EnemyActionFactory.GetEnemyActionEffect(intendedAction.enumEnemyAction).InitializeEnemyAction(enemyUnit, gameObject);
        //    }
        //    //intendedActionHolder.SetActive(false);
        //    //intendedActionHolder.transform.SetAsLastSibling(); 

        //    intendedActionHolders[intendedActions.IndexOf(intendedAction)].SetActive(false);
        //    //intendedActionHolders[intendedActions.IndexOf(intendedAction)].transform.SetAsLastSibling();

        //    //actionHand.Remove(intendedAction);
        //    //actionDeck.Add(intendedAction);
        //    Debug.Log($"index is {intendedActions.IndexOf(intendedAction)}");


        //}

        //actionDeck.AddRange(intendedActions);
        //intendedActions.Clear();

        //removes block first before acting
        base.RemoveBlock();
        //iterates through the intent panel and activate their effects
        foreach (Transform intentTransforms in intentPanel.transform)
        {
            //activates the first EnemyActionIcon first then carries over to the next before activating the second effect just like in creative mode
            for (int i = 0; intentTransforms.GetSiblingIndex() >= i; i++)
            {
                EnemyActionFormat actionFormat = intentPanel.transform.GetChild(i).GetComponent<EnemyActionIcon>().enemyAction;
                EnemyActionFactory.GetEnemyActionEffect(actionFormat.enumEnemyAction).InitializeEnemyAction(enemyUnit, gameObject);

            }
            intentTransforms.gameObject.SetActive(false);
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

    public void EnemyDrawHand()
    {

        for(int i = 0; enemyUnit.draw > i; i++)
        {
            
            EnemyActionFormat tempAction = actionDeck[Random.Range(0, actionDeck.Count)];
            //actionHand.Add(tempAction);
            //actionDeck.Remove(tempAction);

            //for assigning sprites and enabling action slot gameObject
            //foreach (GameObject action in actionSlotObjects)
            //{
            //    if (action.activeSelf == false)
            //    {
            //        Image tempActionImage = action.GetComponent<Image>();
            //        tempActionImage.sprite = actionIconsDict[tempAction.actionType];
            //        action.SetActive(true);
            //        break;

            //    }
            //}

            //for assigning sprites and enabling action slot gameObject
            //iterates through the action panel to enable action slots and assign sprites
            foreach (Transform actionSlot in actionPanel.transform)
            {
                //cache for the child transform's gameObject
                GameObject actionObject = actionSlot.gameObject;
                //iterates till it finds a disabled slot, enables it then breaks
                if (actionObject.activeSelf == false)
                {
                    //at draw, moves actions from pool to hand by draw times
                    actionHand.Add(tempAction);
                    actionDeck.Remove(tempAction);

                    Image tempActionImage = actionObject.GetComponent<Image>();
                    //tempActionImage.sprite = actionIconsDict[tempAction.actionType];      //dictionaryApproach
                    //CONSIDER USING SCRIPTABLE OBJECTS INSTEAD OF RESOURCE FOLDER
                    tempActionImage.sprite = Resources.Load<Sprite>($"EnemyActionIcon/{tempAction.actionType}");
                    actionObject.SetActive(true);
                    actionSlotObjects.Add(actionObject);


                    break;
                }
            }
        } 
    }

    public void EnemyCastIntent()
    {
        bool isNoMoreLinks = false;

        //display intent first
        //foreach (GameObject intent in intentSlots)
        foreach (Transform intentTransform in intentPanel.transform)
        {
            GameObject intent = intentTransform.gameObject;
            if (intent.activeSelf == false)
            {
                //cache for intent object's image
                Image tempIntentImage = intent.GetComponent<Image>();

                //picks a random EnemyActionformat from enemy hand if intents are empty
                //tries to pick a linkable action if not empty
                //actions are added to intendedActionsList
                int actionIntentIndexMatcher = new int();
                if (intendedActions.Count <= 0)
                {
                    actionIntentIndexMatcher = Random.Range(0, actionHand.Count);
                    isNoMoreLinks = false;
                }
                else 
                {
                    //checks each remaining card in in actionHand if there are linkables
                    foreach (EnemyActionFormat action in actionHand)
                    {
                        //checks the last element of the link if its output link matches the elements iterated through
                        if (action.inputLink == intendedActions[intendedActions.Count - 1].outputLink)
                        {
                            actionIntentIndexMatcher = actionHand.IndexOf(action);
                            isNoMoreLinks = false;
                            break;
                        }
                        //if we got here as final, there are no links
                        isNoMoreLinks = true;

                    }

                }

                //if searching for linkable failed and if the last search drained the actionHand
                if (isNoMoreLinks == true || actionHand.Count <= 0)
                {
                    break;
                }
                else
                {
                    //Adds the EnemyActionFormat that was randomized or deemed linkable by the else above
                    //this list contains all EnemyActionFormats to be executed later
                    intendedActions.Add(actionHand[actionIntentIndexMatcher]);
                    //instantly removes from actionhand when picked as linkable
                    actionHand.RemoveAt(actionIntentIndexMatcher);
                    //This list contains all gameObjects under intent panel to be enabled or disabled later on
                    intendedActionHolders.Add(intent);




                    //assigns image to intent slot and enables the intent slot
                    //tempIntentImage.sprite = actionIconsDict[intendedActions[intendedActions.Count - 1].actionType];      //dictionary approach
                    //CONSIDER USING SCRIPTABLE OBJECTS INSTEAD OF RESOURCE FOLDER
                    tempIntentImage.sprite = Resources.Load<Sprite>($"EnemyActionIcon/{intendedActions[intendedActions.Count - 1].actionType}");
                    intent.SetActive(true);

                    //disables the gameObject that holds the action then sets as last so that the Matcher still matches the indices of actionSlotObjects and actionHand
                    actionSlotObjects[actionIntentIndexMatcher].SetActive(false);
                    actionSlotObjects[actionIntentIndexMatcher].transform.SetAsLastSibling();
                    actionSlotObjects.RemoveAt(actionIntentIndexMatcher);
                }





                
            }
        }


    }
    public void EnemyDrawHand2()
    {
        //defaults the draw to enemy's EnemyActionFormat draw stat
        for(int i = 0; enemyUnit.draw > i; i++)
        {
            EnemyActionFormat tempAction = actionDeck[Random.Range(0, actionDeck.Count)];
            //for assigning sprites and enabling action slot gameObject
            //iterates through the action panel to enable action slots and assign sprites
            foreach (Transform actionSlot in actionPanel.transform)
            {
                //cache for the child transform's gameObject
                GameObject actionObject = actionSlot.gameObject;
                TextMeshProUGUI actionObjectNumberText = actionObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
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
                List<GameObject> tempList = new List<GameObject>();               
                foreach (Transform actionTransform in actionPanel.transform)
                {
                    if (actionTransform.gameObject.activeSelf)
                    {
                        tempList.Add(actionTransform.gameObject);
                    }
                    
                }
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
}

