using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//MAKE SURE THAT THE CARD PREFAB IS  ANCHOR PRESET IS AT MIDDLE LEFT
public class CustomHandLayout : MonoBehaviour
{
    public delegate void D_FixOriginalPositions();
    public event D_FixOriginalPositions d_FixOriginalPositions;

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

    public void Start()
    {
        handRect = gameObject.GetComponent<RectTransform>();
        //division nodes depending on the card hand count
        //for odd, max is 9, then 10 for even, added one in the dividers so that the max cards won't go out of bounds
        oddIncrement = handRect.rect.width / 10;
        evenIncrement = handRect.rect.width / 11;
        //Assignment of Vector points
        float yVariable = 3f;
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

    public void ActivateRearrange(int handCount)
    {
        //checks if even
        if(handCount%2 == 0)
        {
            StartCoroutine(RearrangeEvenHand(handCount));
        }
        //cheks if odd
        else if (handCount%2 == 1)
        {
            StartCoroutine(RearrangeOddHand(handCount));
        }
    }

    public void HoverRearrange()
    {

    }

    IEnumerator RearrangeOddHand(int handCount)
    {   
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
            cardRect.anchoredPosition = new Vector2(x, y);
            //sets perfect rotation angles depending on index
            childTransform.rotation = Quaternion.Euler(0, 0, -RotationAngleCalculatorOdd(x, y));
        }
        yield return null;
        d_FixOriginalPositions();
    }

    IEnumerator RearrangeEvenHand(int handCount)
    {
        int tempIndex = handCount - 1;
        Transform cardTransform = gameObject.transform;
        for (int i = 0; tempIndex >= i; i++)
        {
            Transform childTransform = cardTransform.GetChild(i);
            RectTransform cardRect = childTransform.GetComponent<RectTransform>();
            float x = (evenIncrement * ((5 - (tempIndex - 1) / 2) + i));
            float y = YPositionEvenFormula((evenIncrement * ((5 - (tempIndex - 1) / 2) + i)));
            //cardRect.anchoredPosition = new Vector2((evenIncrement * ((5 - (tempIndex -1) / 2) + i)), YPositionEvenFormula((evenIncrement * ((5 - (tempIndex - 1) / 2) + i))));
            cardRect.anchoredPosition = new Vector2(x, y);
            //sets perfect rotation angles depending on index
            childTransform.rotation = Quaternion.Euler(0, 0, -RotationAngleCalculatorEven(x, y));

        }
        yield return null;
        d_FixOriginalPositions();
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
