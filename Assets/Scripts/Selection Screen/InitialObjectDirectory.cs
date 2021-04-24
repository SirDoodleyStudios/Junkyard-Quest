using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this will contain the basic scriptable object card per player and per class
public class InitialObjectDirectory : MonoBehaviour
{
    public List<Card> arlenBasic = new List<Card>();
    public List<Card> cedricBasic = new List<Card>();
    public List<Card> francineBasic = new List<Card>();
    public List<Card> tillyBasic = new List<Card>();
    public List<Card> princessBasic = new List<Card>();

    public List<Card> warriorBasic = new List<Card>();
    public List<Card> rogueBasic = new List<Card>();
    public List<Card> mageBasic = new List<Card>();

    public PlayerUnit arlenUnit;
    public PlayerUnit cedricUnit;
    public PlayerUnit francineUnit;
    public PlayerUnit tillyUnit;
    public PlayerUnit princessUnit;

}
