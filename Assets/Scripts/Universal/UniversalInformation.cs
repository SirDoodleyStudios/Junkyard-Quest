using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

//This is the wrapper class that will contain all information needed to be accessed in all scenes

[Serializable]
public class UniversalInformation
{
    public List<Card> universalDeck;
    public int currentHP;
    public int scraps;


}
