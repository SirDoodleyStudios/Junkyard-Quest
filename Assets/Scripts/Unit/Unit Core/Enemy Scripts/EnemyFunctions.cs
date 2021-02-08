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

    public List<Sprite> actionIconSprites = new List<Sprite>();
    public Dictionary<EnemyActionType, Sprite> actionIconsDict = new Dictionary<EnemyActionType, Sprite>();

    //for enemy action hand
    public List<GameObject> actionSlots;

    //for action intent panel
    public GameObject intentPanel;
    public List<GameObject> intentSlots;

    //EnemyActionformat from enemy hand, intendedAction becomes the actual action at player's end turn
    EnemyActionFormat intendedAction;


    public override void InitializeStats()
    {
        //Copies HP, Creativity, and Draw from scriptable Object first
        maxHP = enemyUnit.HP;
        maxCreativity = enemyUnit.Creativity;
        defaultDraw = enemyUnit.draw;
        base.InitializeStats();

        //List of EnemyActionFormats in enemyUnit
        actionDeck = enemyUnit.actionList;

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
        EnemyActionFactory.GetEnemyActionEffect(intendedAction.enumEnemyAction).InitializeEnemyAction(enemyUnit);
        EnemyActionFactory.GetEnemyActionEffect(intendedAction.enumEnemyAction).CardEffectActivate(gameObject);
    }

    public void EnemyDrawHand()
    {
        for(int i = 0; enemyUnit.draw > i; i++)
        {
            //at draw, moves actions from pool to hand by draw times
            EnemyActionFormat tempAction = actionDeck[Random.Range(0, actionDeck.Count)];
            actionHand.Add(tempAction);
            actionDeck.Remove(tempAction);
            foreach (GameObject action in actionSlots)
            {
                if (action.activeSelf == false)
                {
                    Image tempActionImage = action.GetComponent<Image>();
                    tempActionImage.sprite = actionIconsDict[tempAction.actionType];
                    action.SetActive(true);
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
                EnemyActionFormat tempIntendedAction = actionHand[Random.Range(0, actionHand.Count)];
                intendedAction = tempIntendedAction;
                //assigns image to intent slot and enables the intent slot
                tempIntentImage.sprite = actionIconsDict[intendedAction.actionType];
                intent.SetActive(true);
                //since an action from hand has been intended, remove it from hand lista and add it back to deck
                actionHand.Remove(tempIntendedAction);
                actionDeck.Add(tempIntendedAction);
                
                break;
            }
        }
    }

}
