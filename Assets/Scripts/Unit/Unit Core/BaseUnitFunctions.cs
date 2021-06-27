﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaseUnitFunctions:MonoBehaviour
{
    //death event delegate
    public delegate void D_Death();
    public D_Death d_death;

    //Contains  Unit scriptableobject
    //public Unit unit;

    public int maxHP;
    public int currHP;
    public int maxCreativity;
    public int currCreativity;
    public int defaultDraw;
    public int currDraw;
    public int block;   //made public for certain abilities



    //Hit counter for cards that need the hit count per turn like steadyImprovement
    protected int hitsCounter;

    //for showing HP values in UI
    public Slider HPSlider;
    //public Text HPText;
    public TextMeshProUGUI HPText;

    public Slider CreativitySlider;
    //public Text CreativityText;
    public TextMeshProUGUI CreativityText;

    //for showing block values in UI
    public GameObject blockBackground;
    //public Text blockText;
    public TextMeshProUGUI blockText;

    //The UnitStatusHolder attached to this unit
    UnitStatusHolder unitStatusHolder;

    //cache for the holder's scripr
    //used in Enemy Functions
    public EnemyAIManager enemyAIManager;




    public void Start()
    {
        ///////////////////////////////override this to start with combatManager
        //This should not be here and must be called from Combatmanager because the stats should only be available once we load the UniversalInformation
        //InitializeStats();
        unitStatusHolder = gameObject.GetComponent<UnitStatusHolder>();
        enemyAIManager = transform.parent.gameObject.GetComponent<EnemyAIManager>();
    }

    //called at the begining of combatManager
    public void BaseUnitStatsInitialize()
    {
        InitializeStats();
    }

    //Initialize HP for all units
    public virtual void InitializeStats()
    {
        //this line is moved to the inheritted classes
        //maxHP = unit.HP;
        //currHP = maxHP;
        //curent creativity overhaul will make creativity start at 0 and then make ways to generate
        //generate by turn or by kill or by card
        //currCreativity = maxCreativity;
        currCreativity = 0;

        currDraw = defaultDraw;

        HPSlider.maxValue = maxHP;
        HPSlider.minValue = 0;
        HPSlider.value = currHP;
        HPText.text = $"{currHP}/{maxHP}";

        CreativitySlider.maxValue = maxCreativity;
        CreativitySlider.minValue = 0;
        CreativitySlider.value = currCreativity;
        CreativityText.text = $"{currCreativity}/{maxCreativity}";

    }

    //Function for updating HP sliders
    public virtual void SliderValueUpdates()
    {
        HPSlider.value = currHP;
        HPText.text = $"{currHP}/{maxHP}";

        CreativitySlider.value = currCreativity;
        CreativityText.text = $"{currCreativity}/{maxCreativity}";


    }


    //Method for changing HP and block values
    //inheritted classes has these for updating Unit files
    public virtual void TakeDamage(int damageValue)
    {
        //gets difference of currentHP and damage/heal value, prevents negatives

        int modifiedValue = unitStatusHolder.DamageTakingModifierCalculator(damageValue);
        
        int HPdamage = block - modifiedValue;
        block = Mathf.Max(block - modifiedValue, 0);
        if (HPdamage < 0)
        {
            //might make it so that it reaches negative int so that overkill
            //currHP = Mathf.Max(currHP - Mathf.Abs(HPdamage), 0);
            //currHP = HPdamage;
            currHP = currHP - Mathf.Abs(HPdamage);
            SliderValueUpdates();
        }
        ShowBlock();

        //Migrated to enemy Functions
        ////at death
        ////WILL CHANGE THIS TO NOT IMMEDIATELY KILL FOR OVERKILL MECHANIC
        //if (currHP <= 0)
        //{
        //    //calls the enemyCounterIdentifier that calls an event when all enemies are gone
        //    enemyAIManager.RegisterEnemyKill();

        //    //Comment out for Testing
        //    gameObject.SetActive(false);

        //    //nothing in death delegate yet
        //    //d_death();
        //}


    }

    //for healing a unit
    //inheritted classes has these for updating Unit files
    public virtual void HealHealth(int healValue)
    {
        currHP = Mathf.Min(currHP + healValue, maxHP);
    }


    //Method for updating block
    public virtual void GainBlock(int blockValue)
    {
        block = block + blockValue;
        ShowBlock();

    }

    //used when ending unit turn
    public virtual void RemoveBlock()
    {
        block = 0;
        ShowBlock();
    }

    //Method to show or hide block depending if block has value or not
    public virtual void ShowBlock()
    {
        if(block == 0)
        {
            blockText.text = block.ToString();
            blockBackground.SetActive(false);
        }
        else if (block > 0)
        {
            blockText.text = block.ToString();
            blockBackground.SetActive(true);
        }
    }

    public virtual void AlterCreativity(int creativityValue)
    {
        //prevents from exeeding max
        if (currCreativity + creativityValue > maxCreativity)
        {
            currCreativity = Mathf.Min(maxCreativity, currCreativity + creativityValue);
        }
        //prevents from going below 0
        else if (currCreativity + creativityValue < 0)
        {
            currCreativity = Mathf.Max(0, currCreativity + creativityValue);
        }
        //change is within 0 to max range
        else
        {
            currCreativity += creativityValue;
        }
        SliderValueUpdates();

    }

}
