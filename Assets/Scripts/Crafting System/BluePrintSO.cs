using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "BluePrint", menuName = "BluePrint")]

public class BluePrintSO : ScriptableObject
{
    //the blueprint type
    public AllGearTypes blueprint;

    //Contains the blueprint image
    public Sprite bluePrintSprite;

    //contains the material type to be accepted
    //must synchronize with materialSLotPositions
    public List<AllMaterialTypes> acceptableMaterialTypes = new List<AllMaterialTypes>();

    //will contain list of coordinates for wach mterial Slot instantiated by MaterialSlotManager
    public List<Vector2> materialSlotPositions = new List<Vector2>();

    //will contain the classification of gear
    public AllGearClassifications gearClassifications;

}
