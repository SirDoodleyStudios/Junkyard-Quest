using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Player Unit", menuName = "Player Unit")]
public class PlayerUnit : Unit
{
    //in Base
    //public int HP;
    //public int Creativity;
    //only playerUnits use this
    public int currHP;
    public int energy;
    public ChosenPlayer chosenPlayer;
}
