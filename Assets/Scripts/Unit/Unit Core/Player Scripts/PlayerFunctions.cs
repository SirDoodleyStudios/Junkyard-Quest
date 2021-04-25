using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFunctions : BaseUnitFunctions
{
    //attached player unit scriptable object
    public PlayerUnit playerUnit;

    //delegate for calling all methods on scripts inside the player prefab
    public delegate void D_StartPlayerUpdates();
    public event D_StartPlayerUpdates d_StartPlayerUpdates;   

    //only applicable to player
    public int defaultEnergy;
    public int currEnergy;

    //migrated to BaseUnitFunctions
    //int maxCreativity;
    //public int currCreativity;
    //public int defaultDraw;
    //public int currDraw;
    //public Slider CreativitySlider;    
    //public Text CreativityText;

    //for installing abilities in ability slots


    //for loading the playerFunction loaded as the playerUnit
    //called by combatManager during combatStart after loading
    public void LoadPlayerUnitFromFile(PlayerUnit unit)
    {
        playerUnit = unit;
    }


    public override void InitializeStats()
    {
        //Copies HP, Creativity, and Draw from scriptable Object first
        maxHP = playerUnit.HP;
        maxCreativity = playerUnit.Creativity;
        defaultDraw = playerUnit.draw;
        base.InitializeStats();              

        //initializes creativity stats for players only
        defaultEnergy = playerUnit.energy;
        currEnergy = defaultEnergy;
        Debug.Log("player function");
    }

    //called by StartTurn event from combat manager
    //activates all functions related to the start of turn
    public void PlayerTurn()
    {
        base.RemoveBlock();
        gameObject.GetComponent<AbilityManager>().EnableAbilities();
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

    public void AlterEnergy(int value)
    {
        currEnergy += value;
    }



}
