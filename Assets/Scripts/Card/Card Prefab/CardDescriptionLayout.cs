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
    //to hold the card's tagls list so that the SO's info doesnt get affected
    List<CardMechanics> tagsList = new List<CardMechanics>();
    //existing popups instantiated for the card
    List<GameObject> existingDescObjList = new List<GameObject>();
    //cached list of this card SO's tags list
    List<CardMechanics> tagsListinCard = new List<CardMechanics>();

    public bool isInCardCollider;

    //cache once
    void Awake()
    {

    }
    //on enable assign descriptions
    void OnEnable()
    {
        tagsListinCard = gameObject.GetComponent<Display>().card.cardTags;
        if (tagsList != null)
        {
            tagsList.Clear();
        }
        AssignDescriptions();

    }
    //everytim the card is disabled, it's discarded or out of use
    private void OnDisable()
    {

    }

    void AssignDescriptions()
    {
        //takes card SO list of tags and imports to card tag list here
        tagsList.AddRange(tagsListinCard);
        foreach (CardMechanics tag in tagsList)
        {
            //this allows recycling of game objects, we just enable them if it already exists
            if (existingDescObjList.Count >= tagsList.Count)
            {
                //gets gameobject from existingList based on how many elements there are in the tagslist
                RectTransform popupDesc = existingDescObjList[tagsList.IndexOf(tag)].GetComponent<RectTransform>();
                uiText = popupDesc.GetComponent<TextMeshProUGUI>();
                if (gameObject.GetComponent<Display>().card.cardTags != null)
                {
                    uiText.text = CardTagManager.GetCardTagDescriptions(tag);
                }                    

            }
            //create popup fields
            else
            {
                //create gameObject if the count of existing is lacking then set them to inactive
                //the popup prefab is inactive in the prefab itself
                GameObject popupDesc = Instantiate(popupPrefab, descriptionLayoutHolder.transform);

                //records the gameObject as added for recycling
                existingDescObjList.Add(popupDesc);
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
    }

    //These will be called by DragNDrop since they have the onpointer enter and onexitevents
    public void EnablePopups()
    {
        isInCardCollider = true;
        descriptionLayoutHolder.SetActive(true);
        StartCoroutine(OrderPopups());
    }

    public void DisablePopups()
    {
        isInCardCollider = false;
        descriptionLayoutHolder.SetActive(false);
        foreach (GameObject obj in existingDescObjList)
        {
            obj.SetActive(false);
        }
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
                //yield return null;

                //sets position of card description
                //foreach (RectTransform popupDescRect in popupPosList)
                //{
                //    //enables children as well, so that we can recycle popup gameObjects
                //    popupDescRect.gameObject.SetActive(true);

                //    yield return null;
                //    popupDescRect.anchoredPosition = new Vector2(10, nextPosY);
                //    nextPosY = nextPosY + popupDescRect.rect.height + padding;

                //}

                for (int i = 0; tagsList.Count > i; i++)
                {
                    RectTransform popupDescRect = popupPosList[i];

                    //enables children as well, so that we can recycle popup gameObjects
                    popupDescRect.gameObject.SetActive(true);

                    yield return null;
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
