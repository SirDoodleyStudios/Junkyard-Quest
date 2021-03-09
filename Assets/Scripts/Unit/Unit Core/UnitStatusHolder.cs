
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//THIS WILL BE ATTACHED TO UNIT PREFAB AND IT CONTAINS ALL THE STATUS EFFECTS ON A UNIT
public class UnitStatusHolder : MonoBehaviour
{

    //dictionary of applied statuses on unit
    Dictionary<CardMechanics, int> usageStatusDict = new Dictionary<CardMechanics, int>();
    Dictionary<CardMechanics, int> turnStatusDict = new Dictionary<CardMechanics, int>();
    Dictionary<CardMechanics, int> consumeUsageStatusDict = new Dictionary<CardMechanics, int>();
    Dictionary<CardMechanics, int> consumeTurnStatusDict = new Dictionary<CardMechanics, int>();
    Dictionary<CardMechanics, int> stackAlterByCountDict = new Dictionary<CardMechanics, int>();

    //identifiers for the counting events in BaseCardEffect
    public bool isHitCounting { get; set; }

    //maybe only a list of int references are needed?
    List<CardMechanics> existingStatus = new List<CardMechanics>();
    Dictionary<CardMechanics, GameObject> existingStatusDict = new Dictionary<CardMechanics, GameObject>();

    public static List<Sprite> statusIconSprites = new List<Sprite>();

    //holds the statuses in player prefab
    public GameObject statusPanel;

    //creates an empty gameobject when statuses are added
    public GameObject statusPrefab;



    //Dictionary that stores all status and stacks to be applied o unit next turn
    Dictionary<CardMechanics, int> nextTurnStackDict = new Dictionary<CardMechanics, int>();


    public void Start()
    {

        isHitCounting = false;
        //usageStatusDict.Add(CardMechanics.Confused, Confused);
        //usageStatusDict.Add(CardMechanics.Forceful, Forceful);

        //statusStacks.Add(Confused);
        //statusStacks.Add(Forceful);
    }

    public void StatusUpdateForNewTurn()
    {
        ActivateUniqueStatusEffects();
        ConsumeTurnStackUpdate();
        TurnStatusUpdater();
        //clears counting logic from last turn
        stackAlterByCountDict.Clear();
        isHitCounting = false;
    }

    //STACK AMOUNT ALTERERS/////////////////////

    public void AlterStatusStack(CardMechanics enumKey, int stack)
    {
        //statusDictionary[enumKey] = statusDictionary[enumKey] + stack;

        //CarMechanics index 70+ are turn statuses
        //turn and usage status are separated for decrementing function later
        //checks first if unit already has status, if so, simply add it in dictionary
        //if status already exists, just increment stacks
        //this function will receive stacks to add or subtract

        //for usage stacks
        if ((int)enumKey >= 40 && (int)enumKey <= 69)
        {
            if (!usageStatusDict.ContainsKey(enumKey))
            {
                usageStatusDict.Add(enumKey, stack);
                StatusVisualsUpdater(enumKey, stack);
            }
            else
            {
                //after adding or subtracting, calls visual updater
                usageStatusDict[enumKey] = usageStatusDict[enumKey] + stack;
                StatusVisualsUpdater(enumKey, usageStatusDict[enumKey]);
                //if stack becomes 0 at the end, remove from the usageStatusDict
                if (usageStatusDict[enumKey] <= 0)
                {
                    usageStatusDict.Remove(enumKey);
                }
            }

        }
        // for turn stacks
        else if ((int)enumKey >= 70 && (int)enumKey <= 99)
        {
            if (!turnStatusDict.ContainsKey(enumKey))
            {
                turnStatusDict.Add(enumKey, stack);
                StatusVisualsUpdater(enumKey, stack);
            }
            else
            {
                //after adding or subtracting, calls visual updater
                turnStatusDict[enumKey] = turnStatusDict[enumKey] + stack;
                StatusVisualsUpdater(enumKey, turnStatusDict[enumKey]);
                //if stack becomes 0 at the end, remove from the turnStatusDict
                if (turnStatusDict[enumKey] <= 0)
                {
                    turnStatusDict.Remove(enumKey);
                }
            }
            
        }
        //for consume usage stacks
        else if ((int)enumKey >= 100 && (int)enumKey <= 129)
        {
            if (!consumeUsageStatusDict.ContainsKey(enumKey))
            {
                consumeUsageStatusDict.Add(enumKey, stack);
                StatusVisualsUpdater(enumKey, stack);
            }
            else
            {
                //after adding or subtracting, calls visual updater
                consumeUsageStatusDict[enumKey] = consumeUsageStatusDict[enumKey] + stack;
                StatusVisualsUpdater(enumKey, consumeUsageStatusDict[enumKey]);
                //if stack becomes 0 at the end, remove from the usageStatusDict
                if (consumeUsageStatusDict[enumKey] <= 0)
                {
                    consumeUsageStatusDict.Remove(enumKey);
                }
            }
        }
        //for consume turn stacks
        else if ((int)enumKey >= 130)
        {
            if (!consumeTurnStatusDict.ContainsKey(enumKey))
            {
                consumeTurnStatusDict.Add(enumKey, stack);
                StatusVisualsUpdater(enumKey, stack);
            }
            else
            {
                //after adding or subtracting, calls visual updater
                consumeTurnStatusDict[enumKey] = consumeTurnStatusDict[enumKey] + stack;
                StatusVisualsUpdater(enumKey, consumeTurnStatusDict[enumKey]);
                //if stack becomes 0 at the end, remove from the usageStatusDict
                if (consumeTurnStatusDict[enumKey] <= 0)
                {
                    consumeTurnStatusDict.Remove(enumKey);
                }
            }
        }


    }
    //adds statuses that will be increased by other player actions
    //Called by BaseCardEffect as the actual card effect
    public void AddStatusStackToCountingDict(CardMechanics enumKey, int stackIncrement)
    {
        //add if not yet existing
        if (!stackAlterByCountDict.ContainsKey(enumKey))
        {
            stackAlterByCountDict.Add(enumKey, stackIncrement);
        }
        //increase increment stack if status is already existing
        else
        {
            stackAlterByCountDict[enumKey] += stackIncrement;
        }

    }

    //called by DealDamage() as an event for status counters that rely on hits
    public void AlterStatusStackByHitCounter()
    {
        //the stack that was defined in AddStatusStackToCountingDict() will be added to statuses everytime counter triggers are called
        foreach (KeyValuePair<CardMechanics, int> stackCount in stackAlterByCountDict)
        {
            AlterStatusStack(stackCount.Key, stackCount.Value);
        }
    }




    //Called when start of player turn is triggered
    public void TurnStatusUpdater()
    {
        //list to contain enumKeys to be removed once stack goes 0
        List<CardMechanics> removeStackList = new List<CardMechanics>();
        //list to contain enumkeys just to be decreased
        Dictionary<CardMechanics, int> decreaseStackDict = new Dictionary<CardMechanics, int>();
        foreach (KeyValuePair<CardMechanics, int> tagStack in turnStatusDict)
        {
            int tempStack = tagStack.Value - 1;
            if (tempStack <= 0)
            {
                removeStackList.Add(tagStack.Key);
            }
            else
            {
                decreaseStackDict.Add(tagStack.Key, tagStack.Value);
            }
            //tempstack sent is negative cuz we removing stacks
            //AlterStatusStack(tagStack.Key, -tempStack);
            //StatusVisualsUpdater(tagStack.Key, tempStack);
        }
        //for statuses that still has stacks
        if (decreaseStackDict.Count != 0)
        {
            foreach (KeyValuePair<CardMechanics, int> decreaseStack in decreaseStackDict)
            {
                AlterStatusStack(decreaseStack.Key, -1);
            }
        }
        // not even sure why we need this but it works
        if (removeStackList.Count != 0)
        {
            foreach (CardMechanics enumKey in removeStackList)
            {
                AlterStatusStack(enumKey, -1);
            }
        }

        decreaseStackDict.Clear();
        removeStackList.Clear();

    }

    //for statuses that removes all stacks at turn reset
    public void ConsumeTurnStackUpdate()
    {
        //holder dictionary, before altering the real thing
        Dictionary<CardMechanics, int> removeStackDict = new Dictionary<CardMechanics, int>();

        //reduces the whole stack to 0 at start turn
        foreach (KeyValuePair<CardMechanics, int> tagStack in consumeTurnStatusDict)
        {
            removeStackDict.Add(tagStack.Key, tagStack.Value);

        }

        foreach (KeyValuePair<CardMechanics, int> tagStack in removeStackDict)
        {
            AlterStatusStack(tagStack.Key, -tagStack.Value);
        }
    }



    //Called by Calculators only
    //stack accepted is the value to be incremented to existing stack, can be negative if decrease increment
    public void UsageStatusUpdater(CardMechanics enumKey, int stackAlter)
    {
        if (usageStatusDict.ContainsKey(enumKey))
        {
            AlterStatusStack(enumKey, stackAlter);

        }
    }

    //Called by Calculators only
    //stack accepted must always be the existing stack of the status and deplete it to 0
    public void ConsumeUsageStatusUpdater(CardMechanics enumKey, int stackAlter)
    {
        if (consumeUsageStatusDict.ContainsKey(enumKey))
        {
            AlterStatusStack(enumKey, stackAlter);
        }
    }


    //VISUALS UPDATER///////////////

    //For updating the icons and stack numbers in UI
    public void StatusVisualsUpdater(CardMechanics enumKey, int stack)
    {

        //existingStatusDict is a dictionary that contains gameobjects under status panel with cardMechanics as its key
        //if the mechanic key is in dict, instantiate the gameObject for the icon image then assign stack text
        if (!existingStatusDict.ContainsKey(enumKey))
        {            
            GameObject instantiatedIcon = Instantiate(statusPrefab, statusPanel.transform);
            existingStatusDict.Add(enumKey, instantiatedIcon);


            //CONSIDER USING SCRIPTABLE OBJECTS INSTEAD OF RESOURCE FOLDER
            Image statusImage = instantiatedIcon.GetComponent<Image>();
            statusImage.sprite = Resources.Load<Sprite>($"UnitStatus/{enumKey}");

            //Gets the first child which is the stack text
            TextMeshProUGUI stackText = instantiatedIcon.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            stackText.text = stack.ToString();

            //Calls statusIconDescription to assign the text upon instatiation then sends the text to it
            StatusIconDescription statusIconDesc = instantiatedIcon.GetComponent<StatusIconDescription>();
            statusIconDesc.AssignIconDescription(CardTagManager.GetCardTagDescriptions(enumKey));

        }

        if (existingStatusDict.ContainsKey(enumKey) && !existingStatusDict[enumKey].activeSelf)
        {
            //enables the status icon
            existingStatusDict[enumKey].SetActive(true);
            //Gets the first child which is the stack text
            TextMeshProUGUI stackText = existingStatusDict[enumKey].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            stackText.text = stack.ToString();

        }
        //Destroy initialized Icons if stack becomes 0
        else if (existingStatusDict.ContainsKey(enumKey) && stack <= 0)
        {
            //Destroy(existingStatusDict[enumKey]);
            //existingStatusDict.Remove(enumKey);

            existingStatusDict[enumKey].SetActive(false);

            
        }
        //if mechanic key already exists, only update the stack text
        else if (existingStatusDict.ContainsKey(enumKey))
        {
            //gets the gameobject of passed mechanic key
            TextMeshProUGUI stackText = existingStatusDict[enumKey].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            stackText.text = stack.ToString();
        }

    }


    //CALCULATORS//////////////////////


    //These Calculators are going to vary depending if a mechanicis added or subrtacted
    public int DamageDealingModifierCalculator(int baseDamage)
    {
        //if Forceful is in tags, increase damage by 30%
        if (usageStatusDict.ContainsKey(CardMechanics.Forceful))
        {
            int total = Mathf.FloorToInt(baseDamage * 1.3f);
            AlterStatusStack(CardMechanics.Forceful, -1);
            return total;
        }
        //if there are no cardd tags to affect calculations, just return base damage
        else
        {
            return baseDamage;
        }        
    }

    public int DamageTakingModifierCalculator(int baseDamage)
    {
        if (turnStatusDict.ContainsKey(CardMechanics.Shocked))
        {
            int total = baseDamage + turnStatusDict[CardMechanics.Shocked];
            return total;
        }
        //if there are no cardd tags to affect calculations, just return base damage
        else
        {
            return baseDamage;
        }

    }

    public int BlockModifierCalculator(int baseBlock)
    {
        int total;
        if (usageStatusDict.ContainsKey(CardMechanics.Feeble))
        {
            total = Mathf.FloorToInt(baseBlock * .7f);
            return total;
        }
        else
        {
            return baseBlock;
        }
    }

    //Unique Status LOGICS//////////////////////
    // this function is for activating statuses that does something at start Turn
    void ActivateUniqueStatusEffects()
    {
        //for applying Forceful based on GenerateForceful Stacks
        if (consumeTurnStatusDict.ContainsKey(CardMechanics.GenerateForceful))
        {
            AlterStatusStack(CardMechanics.Forceful, consumeTurnStatusDict[CardMechanics.GenerateForceful]);
        }
    }

    //called by cards that needs status checking, returns the current status stack
    public int StatusStackChecker(CardMechanics enumKey)
    {
        //returns current stack or 0 if none
        if (consumeTurnStatusDict.ContainsKey(enumKey))
        {
            return consumeTurnStatusDict[enumKey];
        }

        //first checking is if existing so that only one check is needed if the status to be checked does not exist
        if (existingStatusDict.ContainsKey(enumKey))
        {
            //usage status
            if ((int)enumKey >= 40 && (int)enumKey <= 69)
            {
                return usageStatusDict[enumKey];
            }
            //turn status
            else if ((int)enumKey >= 70 && (int)enumKey <= 99)
            {
                return turnStatusDict[enumKey];
            }
            //consume usage status
            else if ((int)enumKey >= 100 && (int)enumKey <= 129)
            {
                return consumeUsageStatusDict[enumKey];
            }
            //consume turn status
            else if ((int)enumKey >= 130)
            {
                return consumeTurnStatusDict[enumKey];
            }
            //should not happen
            else
            {
                Debug.Log("Status Checker logic bug");
                return 0;
            }
        }
        //immediately returns 0 when status to be checked doesnt exist
        else
        {
            return 0;
        }

    }
}
