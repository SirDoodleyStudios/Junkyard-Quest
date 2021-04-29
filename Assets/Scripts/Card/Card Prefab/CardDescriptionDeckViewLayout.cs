using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardDescriptionDeckViewLayout : MonoBehaviour/*, IPointerEnterHandler, IPointerExitHandler*/
{

    //parent empty object of popup decriptions and it's RectTransform cache
    public GameObject descriptionLayoutHolder;
    RectTransform descriptionRect;
    List<RectTransform> popupPosList = new List<RectTransform>();
    //contains the prefab of description GameObject
    public GameObject popupPrefab;
    //constant space between popus
    const float padding = 10;
    //text of actual description
    TextMeshProUGUI uiText;
    //canvasgroup cache for making it invisible during activating coroutine
    //this will prevent stuttering of popup visuals
    CanvasGroup canvasGroup;


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
        //cache of canvasgroup
        //gets the last child of card prefab which is the tag description holder
        canvasGroup = gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject.GetComponent<CanvasGroup>();

    }
    //on enable assign descriptions
    void OnEnable()
    {
        //cache the SO's card list
        tagsListinCard = gameObject.GetComponent<Display>().card.cardTags;
        //always clear this prefab's list of tags cuz it will change depending on what card is assigned to it during draw
        if (tagsList != null)
        {
            tagsList.Clear();
        }
        //actual assignment of string to popup text objects
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
        //descriptionLayoutHolder.SetActive(true);
        StartCoroutine(OrderPopups());

    }

    public void DisablePopups()
    {
        isInCardCollider = false;
        //make popups invisible when hover is ended
        canvasGroup.alpha = 0;
        //descriptionLayoutHolder.SetActive(false);
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
                //will allow popup tags to start at multiplier's height
                //can be added with decimal multiplier to adjust where the popup descriptions are
                float nextPosY = -descriptionRect.rect.height * .8f;
                //0f means that the popups will start on level with the top of the cards because popup anchors are in top right corner of card
                //float nextPosY = 0f;


                //for assigining positions of popups based on it's order

                for (int i = 0; tagsList.Count > i; i++)
                {
                    RectTransform popupDescRect = popupPosList[i];

                    //enables children as well, so that we can recycle popup gameObjects
                    popupDescRect.gameObject.SetActive(true);
                    //this frame yield is needed so that UI positions and sizes are set first before getting them
                    yield return new WaitForEndOfFrame();

                    //determines whether popups spawn on left or right
                    float nextPosX;
                    //deck view has 5 columns, this determines which card the column is in
                    int tempIndex;
                    if (gameObject.transform.GetSiblingIndex()>= 5)
                    {
                        tempIndex = transform.GetSiblingIndex();

                        for (int j = 1; tempIndex >= 5; j++)
                        {
                            tempIndex -= j*5;
                        }

                    }
                    else
                    {
                        tempIndex = transform.GetSiblingIndex();
                    }

                    //if card is at the 3 to 5 slot, show the tags in the left
                    //This is the only difference between the combat deck layout
                    if (2 <= tempIndex)
                    {
                        //card width and popup width
                        nextPosX = -530;
                    }
                    //if not, spawn on right
                    else
                    {
                        nextPosX = 10;
                    }

                    popupDescRect.anchoredPosition = new Vector2(nextPosX, nextPosY);

                    //will keep adding to nextPosY so that later popus are placed heigher
                    nextPosY = nextPosY + popupDescRect.rect.height + padding;
                    //alternatively, we can start with popups at heighr position then go down for additionals
                    //nextPosY = nextPosY - popupDescRect.rect.height - padding;


                }
                //descriptionLayoutHolder.SetActive(true);
                //makes popups visible during hover
                canvasGroup.alpha = 1f;



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
