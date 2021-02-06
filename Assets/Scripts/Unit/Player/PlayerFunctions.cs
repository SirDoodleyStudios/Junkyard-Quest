﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFunctions : BaseUnitFunctions
{

    
    [SerializeField]
    int maxCreativity;
    [SerializeField]
    public int currCreativity;


    //only player units will have creativity
    public Slider CreativitySlider;    
    public Text CreativityText;


    public override void InitializeStats()
    {
        //Calls HP Initialization
        base.InitializeStats();

        //initializes creativity stats for players only
        maxCreativity = unit.Creativity;
        currCreativity = maxCreativity;
        CreativitySlider.maxValue = maxCreativity;
        CreativitySlider.minValue = 0;
        CreativitySlider.value = currCreativity;
        CreativityText.text = $"{currCreativity}/{maxCreativity}";

    }

    public void ReduceCreativity(int creativityValue)
    {


        currCreativity -= creativityValue;
        SliderValueUpdates();

    }



    //Function for updating Sliders
    public override void SliderValueUpdates()
    {
        //updates HP sliders
        base.SliderValueUpdates();

        //updates creativity sliders
        CreativitySlider.value = currCreativity;
        CreativityText.text = $"{currCreativity}/{maxCreativity}";

    }
}
