using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


//MAKE SURE THAT THE CARD PREFAB IS  ANCHOR PRESET IS AT MIDDLE LEFT
public class CustomHandLayout : MonoBehaviour
{
    //reference for playerHand in same object
    public PlayerHand playerHand;

    //public delegate void D_FixOriginalPositions();
    //public event D_FixOriginalPositions d_FixOriginalPositions;
    //saved from last rearrange call so that hoverRearrange can use it
    int lastHandCount;
    //list of card prefab neighbors to be rearranged back to original position once card is unhovered
    List<GameObject> neighborHoverList = new List<GameObject>();
    //bool that identifies whether a card is being hovered on, used for overriding delay from postion tweens
    bool isHovering;

    //bool that identifies wheter the cards in hand are tweening
    bool isTweening;

    RectTransform handRect;
    float oddIncrement;
    float evenIncrement;

    Vector2 pointAodd;
    Vector2 pointAeven;
    Vector2 pointBodd;
    Vector2 pointBeven;
    Vector2 pointCodd;
    Vector2 pointCeven;

    float centerXodd;
    float centerXeven;
    float centerYodd;
    float centerYeven;
    float radiusodd;
    float radiuseven;

    public void Awake()
    {

        //tweeing initialize, needs to be called before any DOTween
        DOTween.Init(true, true, LogBehaviour.Default);
        DOTween.SetTweensCapacity(2000, 50);
    }

    public void Start()
    {
        handRect = gameObject.GetComponent<RectTransform>();
        //division nodes depending on the card hand count
        //for odd, max is 9, then 10 for even, added one in the dividers so that the max cards won't go out of bounds
        oddIncrement = handRect.rect.width / 10;
        evenIncrement = handRect.rect.width / 11;
        //Assignment of Vector points
        float yVariable = 4f;
        pointAodd = new Vector2(oddIncrement, handRect.rect.height / (2 * yVariable));
        pointAeven = new Vector2(evenIncrement, handRect.rect.height / (2 * yVariable));
        pointBodd = new Vector2((9 * oddIncrement), handRect.rect.height / (2 * yVariable));
        pointBeven = new Vector2((10 * evenIncrement), handRect.rect.height / (2 * yVariable));
        pointCodd = new Vector2 (5 * oddIncrement, 0);
        pointCeven = new Vector2 (5 * evenIncrement, 0);
        //Immediate assignment of center X
        centerXodd = (pointAodd.x + pointBodd.x) / 2;
        centerXeven = (pointAeven.x + pointBeven.x) / 2;
        //formulas for geting the raddi
        OddEquationGenerator();
        EvenEquationGenerator();

    }

    public void ActivateRearrange(int handCount, GameObject cardPrefab)
    {
        //playerHand.StateChanger(CombatState.DrawPhase);
        //to be used by hoverRearrange
        lastHandCount = handCount;
        //checks if even
        if(handCount%2 == 0)
        {
            StartCoroutine(RearrangeEvenHand(handCount, cardPrefab));
        }
        //cheks if odd
        else if (handCount%2 == 1)
        {
            StartCoroutine(RearrangeOddHand(handCount, cardPrefab));
        }

    }

    //called by hovered dragNDrop
    //direction is either 1 or -1, it will determine where the neighbors will move
    //1 is from pointerEnter and -1 is from pointerExit
    public void HoverRearrange(int hoveredIndex)
    {
        isHovering = true;
        if (lastHandCount % 2 == 0)
        {
            StartCoroutine(HoverEvenRearrange(lastHandCount, hoveredIndex, 1));
        }
        //cheks if odd
        else if (lastHandCount % 2 == 1)
        {
            StartCoroutine(HoverOddRearrange(lastHandCount, hoveredIndex, 1));
        }
    }
    //called by the hovered DragNDrop
    public void UnHoverRearrange(int hoveredIndex)
    {
        isHovering = false;
        if (lastHandCount % 2 == 0)
        {
            StartCoroutine(HoverEvenRearrange(lastHandCount, hoveredIndex, -1));
        }
        //cheks if odd
        else if (lastHandCount % 2 == 1)
        {
            StartCoroutine(HoverOddRearrange(lastHandCount, hoveredIndex, -1));
        }
    }

    IEnumerator RearrangeOddHand(int handCount, GameObject cardPrefab)
    {
        //playerHand.StateChanger(CombatState.Tweening);
        float lagTime = .1f;
        //temp index is the last index of the card hand
        int tempIndex = handCount - 1;
        Transform cardTransform = gameObject.transform;
        for (int i = 0; tempIndex >= i; i++ )
        {


            Transform childTransform = cardTransform.GetChild(i);
            RectTransform cardRect = childTransform.GetComponent<RectTransform>();
            float x = (oddIncrement * ((5 - tempIndex / 2) + i));
            float y = YPositionOddFormula((oddIncrement * ((5 - tempIndex / 2) + i)));
            //cardRect.anchoredPosition = new Vector2((oddIncrement * ((5 - tempIndex / 2) + i)), YPositionOddFormula((oddIncrement * ((5 - tempIndex / 2) + i))));
            //Tween here
            //activates the card prefab at the very beginning of the drawhand
            //yield return new WaitForSeconds(lagTime);
            cardPrefab.SetActive(true);
            childTransform.DORotateQuaternion(Quaternion.Euler(0, 0, -RotationAngleCalculatorEven(x, y)), lagTime);
            cardRect.DOAnchorPos(new Vector2(x, y), lagTime, false);


            //cardRect.anchoredPosition = new Vector2(x, y);

            //sets perfect rotation angles depending on index
            //childTransform.rotation = Quaternion.Euler(0, 0, -RotationAngleCalculatorOdd(x, y));
        }
        //d_FixOriginalPositions();
        yield return null;

        //playerHand.StateChanger(CombatState.PlayerTurn);

    }

    IEnumerator RearrangeEvenHand(int handCount, GameObject cardPrefab)
    {
        //playerHand.StateChanger(CombatState.Tweening);
        float lagTime = .1f;
        int tempIndex = handCount - 1;
        Transform cardTransform = gameObject.transform;
        for (int i = 0; tempIndex >= i; i++)
        {


            Transform childTransform = cardTransform.GetChild(i);
            RectTransform cardRect = childTransform.GetComponent<RectTransform>();
            float x = (evenIncrement * ((5 - (tempIndex - 1) / 2) + i));
            float y = YPositionEvenFormula((evenIncrement * ((5 - (tempIndex - 1) / 2) + i)));
            //cardRect.anchoredPosition = new Vector2((evenIncrement * ((5 - (tempIndex -1) / 2) + i)), YPositionEvenFormula((evenIncrement * ((5 - (tempIndex - 1) / 2) + i))));

            //activates the card prefab at the very beginning of the drawhand
            //yield return new WaitForSeconds(lagTime);
            cardPrefab.SetActive(true);
            childTransform.DORotateQuaternion(Quaternion.Euler(0, 0, -RotationAngleCalculatorEven(x, y)), lagTime);
            cardRect.DOAnchorPos(new Vector2(x, y), lagTime, false);


            //cardRect.anchoredPosition = new Vector2(x, y);

            //sets perfect rotation angles depending on index
            //childTransform.rotation = Quaternion.Euler(0, 0, -RotationAngleCalculatorEven(x, y));

        }
        //d_FixOriginalPositions();
        yield return null;
        //playerHand.StateChanger(CombatState.PlayerTurn);

    }
    //rearrange the hovered card's neighbors bly splayig them away from hovered card
    //direction is either 1 or -1, it will determine where the neighbors will move
    //1 is from pointerEnter and -1 is from pointerExit
    IEnumerator HoverOddRearrange(int handCount, int hoverIndex, int direction)
    {
        float lagTime = .1f;
        float splayRate = .7f;
        //temp index is the last index of the card hand
        int tempIndex = handCount - 1;
        Transform cardTransform = gameObject.transform;
        //gets the leftmost card but not past 0
        int minNeighbor = Mathf.Max(0, hoverIndex - 2);
        //gets the rightmost card but not past the current hand count
        int maxNeighbor = Mathf.Min(tempIndex, hoverIndex + 2);
        for (int i = minNeighbor; maxNeighbor >= i; i++)
        {
            Transform childTransform = cardTransform.GetChild(i);
            RectTransform cardRect = childTransform.GetComponent<RectTransform>();

            //the zooming mechanic is called here
            //originally in DragNDrop
            if (hoverIndex - i == 0)
            {
                //DORewind makes sure that other tween effects are terminated when hovering quickly to other objects
                //childTransform.DORewind(false);
                //childTransform.localScale = new Vector3(1.4f, 1.4f, childTransform.localScale.z);
                //cardRect.anchoredPosition = new Vector2(cardRect.anchoredPosition.x, 90);
                //childTransform.rotation = Quaternion.Euler(0, 0, 0);

                CardZoomFunction(childTransform, cardRect);

            }

            //only calculates neighbor indexes and not the hovered card itself
            //if (hoverIndex - i!=0)
            else
            {
                float x = cardRect.anchoredPosition.x - (oddIncrement * splayRate / (hoverIndex - i) * direction);
                float y = YPositionOddFormula(x);
                //cardRect.anchoredPosition = new Vector2((oddIncrement * ((5 - tempIndex / 2) + i)), YPositionOddFormula((oddIncrement * ((5 - tempIndex / 2) + i))));
                //Tween here
                //activates the card prefab at the very beginning of the drawhand
                //yield return new WaitForSeconds(lagTime);
                childTransform.DORotateQuaternion(Quaternion.Euler(0, 0, -RotationAngleCalculatorEven(x, y)), lagTime);
                cardRect.DOAnchorPos(new Vector2(x, y), lagTime, false);
            }

        }
        //d_FixOriginalPositions();
        yield return null;



    }
    //rearrange the hovered card's neighbors bly splayig them away from hovered card
    //direction is either 1 or -1, it will determine where the neighbors will move
    //1 is from pointerEnter and -1 is from pointerExit
    IEnumerator HoverEvenRearrange(int handCount, int hoverIndex, int direction)
    {
        float lagTime = .1f;
        float splayRate = .7f;
        //temp index is the last index of the card hand
        int tempIndex = handCount - 1;
        Transform cardTransform = gameObject.transform;
        //gets the leftmost card but not past 0
        int minNeighbor = Mathf.Max(0, hoverIndex - 2);
        //gets the rightmost card but not past the current hand count
        int maxNeighbor = Mathf.Min(tempIndex, hoverIndex + 2);
        for (int i = minNeighbor; maxNeighbor >= i; i++)
        {
            Transform childTransform = cardTransform.GetChild(i);
            RectTransform cardRect = childTransform.GetComponent<RectTransform>();

            //the zooming mechanic is called here
            //originally in DragNDrop
            if (hoverIndex - i == 0)
            {
                ////DORewind makes sure that other tween effects are terminated when hovering quickly to other objects
                //childTransform.DORewind(false);
                //childTransform.localScale = new Vector3(1.4f, 1.4f, childTransform.localScale.z);
                //cardRect.anchoredPosition = new Vector2(cardRect.anchoredPosition.x, 30);
                //childTransform.rotation = Quaternion.Euler(0, 0, 0);

                CardZoomFunction(childTransform, cardRect);

            }
            //only calculates neighbor indexes and not the hovered card itself
            else
            {                


                //neighbor game objects are added to a list so that we can change their positions back to normal once hocvering is done
                GameObject childTransformObject = cardTransform.GetChild(i).gameObject;
                neighborHoverList.Add(childTransformObject);

                float x = cardRect.anchoredPosition.x - (evenIncrement * splayRate / (hoverIndex - i) * direction);
                float y = YPositionEvenFormula(x);
                //cardRect.anchoredPosition = new Vector2((oddIncrement * ((5 - tempIndex / 2) + i)), YPositionOddFormula((oddIncrement * ((5 - tempIndex / 2) + i))));
                //Tween here
                //activates the card prefab at the very beginning of the drawhand
                //yield return new WaitForSeconds(lagTime);
                childTransform.DORotateQuaternion(Quaternion.Euler(0, 0, -RotationAngleCalculatorEven(x, y)), lagTime);
                cardRect.DOAnchorPos(new Vector2(x, y), lagTime, false);


            }
        }
        //d_FixOriginalPositions();
        yield return null;
    }

    public void CardZoomFunction(Transform childTransform, RectTransform cardRect)
    {
        //DORewind makes sure that other tween effects are terminated when hovering quickly to other objects
        childTransform.DORewind(false);
        childTransform.localScale = new Vector3(1.4f, 1.4f, childTransform.localScale.z);
        cardRect.anchoredPosition = new Vector2(cardRect.anchoredPosition.x, 90);
        childTransform.rotation = Quaternion.Euler(0, 0, 0);
    }
    




    //actual equation for finding Y position based on determined X
    float YPositionOddFormula(float x)
    {
        float y = Mathf.Sqrt(Mathf.Pow(radiusodd, 2) - Mathf.Pow((x - centerXodd), 2)) + centerYodd;
        return y;
    }

    float YPositionEvenFormula(float x)
    {
        float y = Mathf.Sqrt(Mathf.Pow(radiuseven, 2) - Mathf.Pow((x - centerXeven), 2)) + centerYeven;
        return y;
    }

    //For finding the card hand layout arc
    void OddEquationGenerator()
    {
        float ACBisectorX = (pointAodd.x + pointCodd.x) / 2;
        float ACBisectorY = (pointAodd.y + pointCodd.y) / 2;

        float perpGradient = Mathf.Pow(((pointAodd.y - pointCodd.y) / (pointAodd.x - pointCodd.x)), -1);
        centerYodd = perpGradient * (centerXodd - ACBisectorX) + ACBisectorY;

        radiusodd = Mathf.Sqrt(Mathf.Pow((pointCodd.x - centerXodd), 2) + Mathf.Pow((pointCodd.y - centerYodd), 2));


    }

    void EvenEquationGenerator()
    {
        float ACBisectorX = (pointAeven.x + pointCeven.x) / 2;
        float ACBisectorY = (pointAeven.y + pointCeven.y) / 2;

        float perpGradient = Mathf.Pow(((pointAeven.y - pointCeven.y) / (pointAeven.x - pointCeven.x)), -1);
        centerYeven = perpGradient * (centerXeven - ACBisectorX) + ACBisectorY;

        radiuseven = Mathf.Sqrt(Mathf.Pow((pointCeven.x - centerXeven), 2) + Mathf.Pow((pointCeven.y - centerYeven), 2));
    }

    float RotationAngleCalculatorOdd(float x, float y)
    {

        //float angleRad = Mathf.Atan(Mathf.Abs((y - centerYodd)/(x - centerXodd)));
        //float angleDeg = angleRad * (180 / Mathf.PI);
        //return angleDeg;

        float perpGrad = Mathf.Pow(((y - centerYodd) / (x - centerXodd)), -1);
        //float yIntercept = y - (perpGrad * x);
        float angleDeg = Mathf.Atan(perpGrad) * Mathf.Rad2Deg;
        //float angleDeg = Mathf.Atan(perpGrad);
        return angleDeg;

    }
    float RotationAngleCalculatorEven(float x, float y)
    {
        //float angleRad = Mathf.Atan(Mathf.Abs((y - centerYeven)/(x - centerXeven)));  
        //float angleDeg = angleRad * (180 / Mathf.PI);
        //return angleDeg;


        float perpGrad = Mathf.Pow(((y - centerYeven) / (x - centerXeven)), -1);
        //float yIntercept = y - (perpGrad * x);
        float angleDeg = Mathf.Atan(perpGrad) * Mathf.Rad2Deg;
        //float angleDeg = Mathf.Atan(perpGrad);
        return angleDeg;

    }

}
