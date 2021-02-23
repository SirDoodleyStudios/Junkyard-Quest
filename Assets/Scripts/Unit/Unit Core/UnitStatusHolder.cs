
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


    //Turn Status Stacks
    int Confused = new int();


    //Usage Status Stacks
    int Forceful = new int();

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

        if ((int)enumKey >= 40)
        {
            if (!turnStatusDict.ContainsKey(enumKey))
            {
                turnStatusDict.Add(enumKey, stack);
                StatusVisualsUpdater(enumKey, stack);
            }
            else
            {
                turnStatusDict[enumKey] = turnStatusDict[enumKey] + stack;
                StatusVisualsUpdater(enumKey, turnStatusDict[enumKey]);
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
                usageStatusDict[enumKey] = usageStatusDict[enumKey] + stack;
                StatusVisualsUpdater(enumKey, usageStatusDict[enumKey]);
            }
            
        }
    }

    public void TurnStatusUpdater()
    {
        List<CardMechanics> tempList = new List<CardMechanics>();
        foreach (KeyValuePair<CardMechanics, int> tagStack in turnStatusDict)
        {
            int tempStack = tagStack.Value - 1;
            if (tempStack <= 0)
            {
                tempList.Add(tagStack.Key);
            }
            StatusVisualsUpdater(tagStack.Key, tempStack);
        }

        if (tempList.Count != 0)
        {
            foreach (CardMechanics enumKey in tempList)
            {
                turnStatusDict.Remove(enumKey);
            }
        }
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


    public int DamageModifierCalculator(int baseDamage)
    {
        int total = 5;
        return total;
    }

    public int BlockModifierCalculator(int baseBlock)
    {
        int total = 5;
        return total;
    }


}
