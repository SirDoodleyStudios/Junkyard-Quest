using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPools : MonoBehaviour
{
    public List<Card> neutralPool = new List<Card>();

    public List<Card> arlenPool = new List<Card>();
    public List<Card> cedricPool = new List<Card>();
    public List<Card> francinePool = new List<Card>();
    public List<Card> tillyPool = new List<Card>();
    public List<Card> princessPool = new List<Card>();

    public List<Card> warriorPool = new List<Card>();
    public List<Card> roguePool = new List<Card>();
    public List<Card> magePool = new List<Card>();

    public List<Card> GetPlayerPool(ChosenPlayer playerKey)
    {
        List<Card> tempList = new List<Card>();
        switch (playerKey)
        {
            case ChosenPlayer.Arlen:
                tempList = arlenPool;
                break;
            case ChosenPlayer.Cedric:
                tempList = cedricPool;
                break;
            case ChosenPlayer.Francine:
                tempList = francinePool;
                break;
            case ChosenPlayer.Tilly:
                tempList = tillyPool;
                break;
            case ChosenPlayer.Princess:
                tempList = princessPool;
                break;
            default:
                break;
        }
        return tempList;
    }

    public List<Card> GetClassPool(ChosenClass classKey)
    {
        List<Card> tempList = new List<Card>();
        switch (classKey)
        {
            case ChosenClass.Warrior:
                tempList = warriorPool;
                break;
            case ChosenClass.Rogue:
                tempList = roguePool;
                break;
            case ChosenClass.Mage:
                tempList = magePool;
                break;
            default:
                break;
        }
        return tempList;
    }
}
