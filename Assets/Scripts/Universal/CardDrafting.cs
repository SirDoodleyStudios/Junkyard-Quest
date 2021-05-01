using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDrafting : MonoBehaviour
{
    GameObject choiceHolder;

    private void Start()
    {
        //first child is the holder of cards
        choiceHolder = transform.GetChild(0).gameObject;


    }
}
