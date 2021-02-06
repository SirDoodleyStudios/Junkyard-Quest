using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardPool", menuName = "CardPool")]
public class CardPool : ScriptableObject
{
    public List<Card> listOfCards;
}
