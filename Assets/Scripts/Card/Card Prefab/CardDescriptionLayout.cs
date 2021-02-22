using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardDescriptionLayout : MonoBehaviour/*, IPointerEnterHandler, IPointerExitHandler*/
{
    //contains card tag descriptions
    CardTagManager cardTagManager;
    //parent empty object of popup decriptions and it's RectTransform cache
    public GameObject descriptionLayoutHolder;
    RectTransform descriptionRect;
    List<RectTransform> popupPosList = new List<RectTransform>();
    //contains the prefab of description GameObject
    public GameObject popupPrefab;
    //constant space between popus
    const float padding = 10;

    TextMeshProUGUI uiText;

    //formula not yet done but me bored
    //public is for testing only
    List<CardMechanics> tagsList = new List<CardMechanics>();

    public bool isInCardCollider;

    void Start()
    {
        //takes card SO list of tags and imports to card tag list here
        tagsList.AddRange(gameObject.GetComponent<Display>().card.cardTags);
        foreach (CardMechanics tag in tagsList)
        {
            //create popup fields
            GameObject popupDesc = Instantiate(popupPrefab, descriptionLayoutHolder.transform);
            RectTransform popupDescRect = popupDesc.GetComponent<RectTransform>();


            uiText = popupDesc.GetComponent<TextMeshProUGUI>();
            if (gameObject.GetComponent<Display>().card.cardTags != null)
            {
                uiText.text = CardTagManager.GetCardTagDescriptions(tag);
            }
            

            //popupDesc.GetComponent<Text>().text = CardTagManager.GetCardTagDescriptions(tag);

            popupPosList.Add(popupDescRect);

        }

    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    isInCardCollider = true;
    //    StartCoroutine(OrderPopups());

    //}


    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    isInCardCollider = false;
    //    descriptionLayoutHolder.SetActive(false);
    //}

    //These will be called by DragNDrop since they have the onpointer enter and onexitevents
    public void EnablePopups()
    {
        isInCardCollider = true;
        StartCoroutine(OrderPopups());
    }

    public void DisablePopups()
    {
        isInCardCollider = false;
        descriptionLayoutHolder.SetActive(false);
    }


    IEnumerator OrderPopups()
    {       
        //counts as timer for hover
        for (float timer = 0; timer <= 5; timer += Time.deltaTime)
        {
            //if timer is past .5 seconds, popups appear then immediately breakes
            //also checks if the hover bool checker is true before popup appears
            if (timer >= .5f && isInCardCollider == true)
            {
                //Cache for a rectTransform's height
                descriptionRect = descriptionLayoutHolder.GetComponent<RectTransform>();
                float nextPosY = -descriptionRect.rect.height;
                descriptionLayoutHolder.SetActive(true);

                //waits for next frame because rectTransform updates will only apply on next frame
                yield return null;

                foreach (RectTransform popupDescRect in popupPosList)
                {
                    popupDescRect.anchoredPosition = new Vector2(10, nextPosY);
                    nextPosY = nextPosY + popupDescRect.rect.height + padding;
                }

                yield break;
            }
            //if pointer is not hovered anymore, immediately stop countdown and break
            else if (isInCardCollider == false)
            {
                yield break;
            }

            //makes sure that countdown keeps going in real time every iteration check
            yield return null;
        }



    }



}
