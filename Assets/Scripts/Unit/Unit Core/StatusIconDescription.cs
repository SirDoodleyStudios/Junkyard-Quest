using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

//ACTS AS A HOLDER FOR ICON STATUS DESCRIPTION
public class StatusIconDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI descriptionText;

    bool isInIconCollider;

    public void Start()
    {

    }

    //triggered upon instantiating status Icon GameObject
    public void AssignIconDescription(string statusDescription)
    {
        descriptionText.text = statusDescription;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isInIconCollider = true;
        StartCoroutine(OrderPopups());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isInIconCollider = false;
        descriptionText.gameObject.SetActive(false);
    }

    IEnumerator OrderPopups()
    {
        //counts as timer for hover
        for (float timer = 0; timer <= 5; timer += Time.deltaTime)
        {
            //if timer is past .5 seconds, popups appear then immediately breakes
            //also checks if the hover bool checker is true before popup appears
            if (timer >= .5f && isInIconCollider == true)
            {
                descriptionText.gameObject.SetActive(true);
                yield break;
            }
            //if pointer is not hovered anymore, immediately stop countdown and break
            else if (isInIconCollider == false)
            {
                yield break;
            }

            //makes sure that countdown keeps going in real time every iteration check
            yield return null;
        }



    }
}
