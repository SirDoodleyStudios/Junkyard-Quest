using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFunctions : BaseUnitFunctions
{
    //delegate for calling all methods on scripts inside the player prefab
    public delegate void D_StartPlayerUpdates();
    public event D_StartPlayerUpdates d_StartPlayerUpdates;
    
    [SerializeField]
    int maxCreativity;
    [SerializeField]
    public int currCreativity;


    //only player units will have creativity
    public Slider CreativitySlider;    
    public Text CreativityText;

    //for installing abilities in ability slots
    


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
    //called by StartTurn event from combat manager
    //activates all functions related to the start of turn
    public void StartTurnUpdates(int? i = null)
    {

    }

    //expect to receive negative ints for costs and positive ints for gains
    public void AlterPlayerCreativity(int creativityValue)
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
