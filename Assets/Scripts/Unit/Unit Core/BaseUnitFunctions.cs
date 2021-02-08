using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUnitFunctions:MonoBehaviour
{
    //Contains  Unit scriptableobject
    //public Unit unit;

    protected int maxHP;
    protected int currHP;
    public int maxCreativity;
    public int currCreativity;
    public int defaultDraw;
    public int currDraw;
    protected int block;

    //for showing HP values in UI
    public Slider HPSlider;
    public Text HPText;

    public Slider CreativitySlider;
    public Text CreativityText;

    //for showing block values in UI
    public GameObject blockBackground;
    public Text blockText;




    public void Start()
    {
        InitializeStats();
    }

    //Initialize HP for all units
    public virtual void InitializeStats()
    {
        //this line is moved to the inheritted classes
        //maxHP = unit.HP;
        currHP = maxHP;
        currCreativity = maxCreativity;
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
        //Creativity slider is in playerfunctions
    }


    //Method for changing HP and block values
    public virtual void TakeDamage(int damageValue)
    {
        //gets difference of currentHP and damage/heal value, prevents negatives
        
        
        
        int HPdamage = block - damageValue;
        block = Mathf.Max(block - damageValue, 0);
        if (HPdamage < 0)
        {
            currHP = Mathf.Max(currHP - Mathf.Abs(HPdamage), 0);
            SliderValueUpdates();
        }
        ShowBlock();

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


}
