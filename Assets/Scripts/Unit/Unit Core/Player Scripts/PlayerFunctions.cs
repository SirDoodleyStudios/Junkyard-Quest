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

    //this is reference for the universalUI that get's updated when there are changes to player stats
    //assigned in editor
    public CameraUIScript cameraUIScript;

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
        //so that the assigned playerUnit SO is just a copy of the one in resources
        PlayerUnit unitCopy = Instantiate(unit);
        playerUnit = unitCopy;
    }


    public override void InitializeStats()
    {
        //Copies HP, Creativity, and Draw from scriptable Object first
        maxHP = playerUnit.HP;
        currHP = playerUnit.currHP;
        currCreativity = playerUnit.currCreativity;
        maxCreativity = playerUnit.creativity;
        defaultDraw = playerUnit.draw;
        base.InitializeStats();              

        //initializes creativity stats for players only
        defaultEnergy = playerUnit.energy;
        currEnergy = defaultEnergy;

    }

    //called by StartTurn event from combat manager
    //activates all functions related to the start of turn
    public void PlayerTurn()
    {
        base.RemoveBlock();
        gameObject.GetComponent<AbilityManager>().EnableAbilities();
    }

    public override void TakeDamage(int damageValue)
    {
        base.TakeDamage(damageValue);
        //update the playerUnit HP attachement
        playerUnit.currHP = currHP;
    }

    public override void HealHealth(int healValue)
    {
        base.HealHealth(healValue);
        playerUnit.currHP = currHP;
    }

    //expect to receive negative ints for costs and positive ints for gains
    //migrated to BaseUnitFunctions because enemies will use creativity as well
    //creativity, called by combatManager delegate during start of turn
    public override void AlterCreativity(int creativityValue)
    {
        ////prevents from exeeding max
        //if (currCreativity + creativityValue > maxCreativity)
        //{
        //    currCreativity = Mathf.Min(maxCreativity, currCreativity + creativityValue);
        //}
        ////prevents from going below 0
        //else if (currCreativity + creativityValue < 0)
        //{
        //    currCreativity = Mathf.Max(0, currCreativity + creativityValue);
        //}
        ////change is within 0 to max range
        //else
        //{
        //    currCreativity += creativityValue;
        //}
        //SliderValueUpdates();

        base.AlterCreativity(creativityValue);

    }



    //Function for updating Sliders
    public override void SliderValueUpdates()
    {
        //updates HP sliders
        base.SliderValueUpdates();

        //updates creativity sliders
        //migrated to BaseUnitFunctions
        //CreativitySlider.value = currCreativity;
        //CreativityText.text = $"{currCreativity}/{maxCreativity}";

        //updates the HP and creativity in UI
        cameraUIScript.UpdateUIObjectsHP(currHP, maxHP);
        cameraUIScript.UpdateUIObjectsCretivity(currCreativity, maxCreativity);
    }
    //value parameter is used to increase or decrease currEnergy by value
    public void AlterEnergy(int value)
    {
        currEnergy += value;
    }


}
