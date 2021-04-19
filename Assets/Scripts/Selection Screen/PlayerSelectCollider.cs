using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerSelectCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Transform playerObjectTrans;
    SelectionManager selectionManager;
    RectTransform objectRect;
    BoxCollider2D objectCollider;
    //indicator if the onject itself is highlighted
    bool isChosen;
    //int indicator that is crosschecked with what was clicked in the collider in SelectionManager
    int index;

    //child of gameObject that holds the selection aura
    Image selectionAuraImage;
    Color originalColor;
    void Start()
    {
        playerObjectTrans = gameObject.transform;
        //takes the selection manager script of the holder's parent
        selectionManager = playerObjectTrans.parent.parent.gameObject.GetComponent<SelectionManager>();
        selectionManager.d_PlayerChosenEvent += UnChoosePlayer;
        //for calling the slection aura image child of the gameObject
        selectionAuraImage = playerObjectTrans.GetChild(0).gameObject.GetComponent<Image>();
        originalColor = selectionAuraImage.color;
        
        isChosen = false;
        index = playerObjectTrans.GetSiblingIndex();

        //sets the collder equal to object's size
        objectCollider = gameObject.GetComponent<BoxCollider2D>();
        objectRect = gameObject.GetComponent<RectTransform>();
        objectCollider.size = objectRect.rect.size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //gets the selection aura image and activates it
        if (!isChosen)
        {
            selectionAuraImage.enabled = true;
            //hover is pink
            selectionAuraImage.color = new Color(1f, .25f, 1f, 1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //gets the selection aura image and activates it
        if (!isChosen)
        {
            selectionAuraImage.enabled = false;
            //default is blue
            selectionAuraImage.color = originalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //when clicked, set to blue
        selectionAuraImage.color = originalColor;
        selectionManager.isSomethingChosen = true;
        isChosen = true;
        //calls the unchoose event to unchoose the other objects
        selectionManager.UnchooseActivate();
    }

    public void UnChoosePlayer(int tempInt)
    {
        //only removes highlight and isChosen indicator if the object clicked is not the chosen
        if (isChosen && index != tempInt)
        {
            selectionAuraImage.enabled = false;
            isChosen = false;
        }
    }
}
