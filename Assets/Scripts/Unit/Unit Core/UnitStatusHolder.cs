
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//THIS WILL BE ATTACHED TO UNIT PREFAB AND IT CONTAINS ALL THE STATUS EFFECTS ON A UNIT
public class UnitStatusHolder : MonoBehaviour
{
    //universal list that contains all statuses that can be applied on unit
    Dictionary<CardMechanics, int> usageStatusDict = new Dictionary<CardMechanics, int>();

    //Dictionay of statuses that is currently applied on unit
    Dictionary<CardMechanics, int> turnStatusDict = new Dictionary<CardMechanics, int>();

    //maybe only a list of int references are needed?
    List<CardMechanics> existingStatus = new List<CardMechanics>();
    Dictionary<CardMechanics, GameObject> existingStatusDict = new Dictionary<CardMechanics, GameObject>();

    public static List<Sprite> statusIconSprites = new List<Sprite>();

    //holds the statuses in player prefab
    public GameObject statusPanel;

    //creates an empty gameobject when statuses are added
    public GameObject statusPrefab;

    public void Start()
    {
        //usageStatusDict.Add(CardMechanics.Confused, Confused);
        //usageStatusDict.Add(CardMechanics.Forceful, Forceful);

        //statusStacks.Add(Confused);
        //statusStacks.Add(Forceful);
    }

    public void AlterStatusStack(CardMechanics enumKey, int stack)
    {
        //statusDictionary[enumKey] = statusDictionary[enumKey] + stack;

        //CarMechanics index 40+ are turn statuses
        //turn and usage status are separated for decrementing function later
        //checks first if unit already has status, if so, simply add it in dictionary
        //if status already exists, just increment stacks
        //this function will receive stacks to add or subtract
        if ((int)enumKey >= 40)
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
        else
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
    }

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

        if (decreaseStackDict.Count != 0)
        {
            foreach (KeyValuePair<CardMechanics, int> decreaseStack in decreaseStackDict)
            {
                AlterStatusStack(decreaseStack.Key, -1);
            }
        }

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


    //For updating the icons and stack numbers in UI
    public void StatusVisualsUpdater(CardMechanics enumKey, int stack)
    {

        //existingStatusDict is a dictionary that contains gameobjects under status panel with cardMechanics as its key
        //if the mechanic key is in dict, instantiate the gameObject for the icon image then assign stack text
        if (!existingStatusDict.ContainsKey(enumKey))
        {            
            GameObject instantiatedIcon = Instantiate(statusPrefab, statusPanel.transform);
            existingStatusDict.Add(enumKey, instantiatedIcon);

            Image statusImage = instantiatedIcon.GetComponent<Image>();
            statusImage.sprite = Resources.Load<Sprite>($"UnitStatus/{enumKey}");

            //Gets only child text
            TextMeshProUGUI stackText = instantiatedIcon.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            stackText.text = stack.ToString();
        }
        //Destroy initialized Icons if stack becomes 0
        else if (existingStatusDict.ContainsKey(enumKey) && stack <= 0)
        {
            Destroy(existingStatusDict[enumKey]);
            existingStatusDict.Remove(enumKey);
            
        }
        //if mechanic key already exists, only update the stack text
        else if (existingStatusDict.ContainsKey(enumKey))
        {
            //gets the gameobject of passed mechanic key
            TextMeshProUGUI stackText = existingStatusDict[enumKey].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            stackText.text = stack.ToString();
        }

    }

    //These Calculators are going to vary depending if a mechanicis added or subrtacted
    public int DamageModifierCalculator(int baseDamage)
    {
        //if Forceful is in tags, increase damage by 30%
        if (usageStatusDict.ContainsKey(CardMechanics.Forceful))
        {
            int total = Mathf.FloorToInt(baseDamage * 1.3f);    
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
        int total = 5;
        return total;
    }


}
