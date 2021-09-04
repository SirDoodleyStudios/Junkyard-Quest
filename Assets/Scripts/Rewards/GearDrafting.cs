using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearDrafting : MonoBehaviour
{
    //this int is to identify from what object index it came from
    public int objectOriginIndex;
    //the holder of choice cards
    public GameObject choiceHolder;
    GridLayoutGroup gridLayout;
    RectTransform parentCanvas;
}
