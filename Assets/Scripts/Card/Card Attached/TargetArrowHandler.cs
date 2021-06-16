using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArrowHandler : MonoBehaviour
{
    //dots and arrowhead holder
    public GameObject targettingArrowParent;
    //list of all dots gameObject
    List<GameObject> arrowDots = new List<GameObject>();
    //list of dots Rect Transforms
    List<RectTransform> arrowDotsRect = new List<RectTransform>();

    //for calculating dot positions
    float basePosRefY;
    float basePosRefX;
    float newPosY;
    float newPosX;

    //for orienting arrowhead
    GameObject arrowHead;
    RectTransform arrowHeadRect;

    public void Awake()
    {
        //first child is the base and the biggest
        //might reverse


        foreach (Transform dot in targettingArrowParent.transform)
        {
            arrowDots.Add(dot.gameObject);
        }
        //reverse list so that last object ehich is arrowhead is rendered last
        arrowDots.Reverse();
        //cache firs object in reversed list as the arrowhead
        arrowHead = arrowDots[0];
        arrowHeadRect = arrowHead.GetComponent<RectTransform>();

        //cache dots' rectTransforms for faster disabling later
        foreach (GameObject dot in arrowDots)
        {
            arrowDotsRect.Add(dot.GetComponent<RectTransform>());
        }




    }

    public void DynamicPositionArrow(Vector2 mousePos)
    {
        basePosRefY = targettingArrowParent.transform.position.y;
        basePosRefX = targettingArrowParent.transform.position.x;
        newPosY = mousePos.y - basePosRefY;
        newPosX = mousePos.x - basePosRefX;

        //for adjusting arrow head's angle in reference with parent position
        //cache these boi
        arrowHead.transform.position = mousePos;
        arrowHeadRect.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(arrowHeadRect.anchoredPosition.x, arrowHeadRect.anchoredPosition.y) * Mathf.Rad2Deg);

        arrowDots[1].transform.position = new Vector3((newPosX * .86f) + basePosRefX, (newPosY * .86f) + basePosRefY, 0f);
        arrowDots[2].transform.position = new Vector3((newPosX * .75f) + basePosRefX, (newPosY * .75f) + basePosRefY, 0f);
        arrowDots[3].transform.position = new Vector3((newPosX * .64f) + basePosRefX, (newPosY * .64f) + basePosRefY, 0f);
        arrowDots[4].transform.position = new Vector3((newPosX * .53f) + basePosRefX, (newPosY * .53f) + basePosRefY, 0f);
        arrowDots[5].transform.position = new Vector3((newPosX * .42f) + basePosRefX, (newPosY * .42f) + basePosRefY, 0f);
        arrowDots[6].transform.position = new Vector3((newPosX * .32f) + basePosRefX, (newPosY * .32f) + basePosRefY, 0f);
        arrowDots[7].transform.position = new Vector3((newPosX * .23f) + basePosRefX, (newPosY * .23f) + basePosRefY, 0f);
        arrowDots[8].transform.position = new Vector3((newPosX * .16f) + basePosRefX, (newPosY * .16f) + basePosRefY, 0f);
        arrowDots[9].transform.position = new Vector3((newPosX * .1f) + basePosRefX, (newPosY * .1f) + basePosRefY, 0f);
        arrowDots[10].transform.position = new Vector3((newPosX * .05f) + basePosRefX, (newPosY * .05f) + basePosRefY, 0f);
    }

    public void EnableArrow()
    {
        targettingArrowParent.SetActive(true);
    }

    public void DisableArrow()
    {
        targettingArrowParent.SetActive(false);
        foreach (RectTransform dots in arrowDotsRect)
        {
            dots.anchoredPosition = new Vector3(0, 0, 0);
        }
    }

}
