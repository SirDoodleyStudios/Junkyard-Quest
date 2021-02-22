
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS WILL BE ATTACHED TO UNIT PREFAB AND IT CONTAINS ALL THE STATUS EFFECTS ON A UNIT
public class UnitStatusHolder : MonoBehaviour
{
    //universal list that contains all statuses that can be applied on unit
    Dictionary<CardMechanics, int> usageStatusDict = new Dictionary<CardMechanics, int>();

    //Dictionay of statuses that is currently applied on unit
    Dictionary<CardMechanics, int> turnStatusDict = new Dictionary<CardMechanics, int>();

    //maybe only a list of int references are needed?
    List<int> statusStacks = new List<int>();

    public static List<Sprite> statusIconSprites = new List<Sprite>();




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
            }
            else
            {
                turnStatusDict[enumKey] = turnStatusDict[enumKey] + stack;
            }
        }
        else
        {
            if (!usageStatusDict.ContainsKey(enumKey))
            {
                usageStatusDict.Add(enumKey, stack);
            }
            else
            {
                usageStatusDict[enumKey] = usageStatusDict[enumKey] + stack;
            }
        }

        StatusVisualsUpdater();
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




    public void StatusVisualsUpdater()
    {

    }
}
