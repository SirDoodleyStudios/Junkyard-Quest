using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    //contains the list of sprites for each attack typem this is manually inserted in inspector
    public List<Sprite> actionIconSprites = new List<Sprite>();
    //Dictionary to identify what sprite will be used depending on the action type of the enemy action
    public Dictionary<EnemyActionType, Sprite> actionIconsDict = new Dictionary<EnemyActionType, Sprite>();

    //for action intent panel
    public GameObject intentPanel;
    //list that contains the gameObjects under the intent panel
    public List<GameObject> intentSlots;

    //EnemyActionformat from enemy hand, intendedAction becomes the actual action at player's end turn
    //intendedActionHolder is the gameObject to enable and disable that contains the indededAction
    EnemyActionFormat intendedAction;
    GameObject intendedActionHolder;


    public override void InitializeStats()
    {
        //Copies HP, Creativity, and Draw from scriptable Object first
        maxHP = enemyUnit.HP;
        maxCreativity = enemyUnit.Creativity;
        defaultDraw = enemyUnit.draw;
        base.InitializeStats();

        //List of EnemyActionFormats in enemyUnit, add range so that the original scriptableObject is not affected when moving stuff
        actionDeck.AddRange(enemyUnit.actionList);

        //assigning icon sprites to actiontypenums
        actionIconsDict.Add(EnemyActionType.Offense, actionIconSprites[0]);
        actionIconsDict.Add(EnemyActionType.Block, actionIconSprites[1]);
        actionIconsDict.Add(EnemyActionType.Enhance, actionIconSprites[2]);
        actionIconsDict.Add(EnemyActionType.Debilitate, actionIconSprites[3]);
        actionIconsDict.Add(EnemyActionType.Summon, actionIconSprites[4]);
        actionIconsDict.Add(EnemyActionType.Special, actionIconSprites[5]);

    }


    //Function for updating Sliders
    public override void SliderValueUpdates()
    {
        //updates HP sliders
        base.SliderValueUpdates();
    }

    public void EnemyPrepare()
    {
        if (actionHand.Count <= 0)
        {
            EnemyDrawHand();
        }
        EnemyCastIntent();


    }

    public void EnemyAct()
    {
        EnemyActionFactory.GetEnemyActionEffect(intendedAction.enumEnemyAction).InitializeEnemyAction(enemyUnit, gameObject);
        //EnemyActionFactory.GetEnemyActionEffect(intendedAction.enumEnemyAction).InitializeEnemyAction(enemyUnit);
        //EnemyActionFactory.GetEnemyActionEffect(intendedAction.enumEnemyAction).CardEffectActivate(gameObject);
        intendedActionHolder.SetActive(false);
        actionHand.Remove(intendedAction);
        actionDeck.Add(intendedAction);

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
                    tempActionImage.sprite = actionIconsDict[tempAction.actionType];
                    actionObject.SetActive(true);
                    actionSlotObjects.Add(actionObject);
                    break;
                }
            }
        } 
    }

    public void EnemyCastIntent()
    {
        //display intent first
        foreach (GameObject intent in intentSlots)
        {
            if (intent.activeSelf == false)
            {
                //cache for intent object's image
                Image tempIntentImage = intent.GetComponent<Image>();
                //picks a random EnemyActionformat from enemy hand, intendedAction becomes the actual action at player's end turn
                int actionIntentIndexMatcher = Random.Range(0, actionHand.Count);

                //caching of inteded intended action is so that we can remove it from list of actionHand and preserve the actual intended action for execution later
                //EnemyActionFormat tempIntendedAction = actionHand[actionIntentIndexMatcher];
                //intendedAction = tempIntendedAction;

                intendedAction = actionHand[actionIntentIndexMatcher];
                intendedActionHolder = intent;

                //assigns image to intent slot and enables the intent slot
                tempIntentImage.sprite = actionIconsDict[intendedAction.actionType];
                intent.SetActive(true);

                //disables the gameObject that holds the action then sets as last so that the Matcher sill matches the indices of actionSlotObjects and actionHand
                actionSlotObjects[actionIntentIndexMatcher].SetActive(false);
                Debug.Log(actionSlotObjects[actionIntentIndexMatcher].activeSelf);
                actionPanel.transform.GetChild(actionIntentIndexMatcher).gameObject.transform.SetAsLastSibling();
                //actionSlotObjects[actionIntentIndexMatcher].transform.SetAsLastSibling();

                //since an action from hand has been intended, remove it from hand list and add it back to deck
                //actionHand.Remove(tempIntendedAction);
                //actionDeck.Add(tempIntendedAction);
                
                break;
            }
        }
    }

}
