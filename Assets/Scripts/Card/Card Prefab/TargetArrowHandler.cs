using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArrowHandler : MonoBehaviour
{
    public GameObject targettingArrowParent;

    List<GameObject> arrowDots = new List<GameObject>();

    public void Awake()
    {
        //first child is the base and the biggest
        //might reverse


        foreach (Transform dot in targettingArrowParent.transform)
        {
            arrowDots.Add(dot.gameObject);
        }
        //makes the smallest dot which is nearest to arrow head, the first object in list
        arrowDots.Reverse();
    }

    public void dynamicPositionArrow(Vector2 mousePos)
    {
        arrowDots[1].transform.position = mousePos;
    }
}
