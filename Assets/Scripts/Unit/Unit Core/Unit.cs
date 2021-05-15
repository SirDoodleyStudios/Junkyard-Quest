using System;
using UnityEngine;

[Serializable]
//[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
public class Unit : ScriptableObject
{
    public int HP;
    public int Creativity;
    public int draw;
    //also added for Enemy Units now to preserve them during save
    public int currHP;



}
