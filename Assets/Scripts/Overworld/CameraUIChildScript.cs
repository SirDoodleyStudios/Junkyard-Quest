using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUIChildScript : MonoBehaviour
{
    CameraUIScript parentScript;
    RectTransform parentRect;
    RectTransform childRect;


    public void Start()
    {
        parentScript = gameObject.transform.parent.gameObject.GetComponent<CameraUIScript>();
        parentRect = transform.parent.gameObject.GetComponent<RectTransform>();
        childRect = gameObject.GetComponent<RectTransform>();
        parentScript.d_ResizeCameraUI += RepositionChild;


    }
    //used for determining the size based on calculated orthographic from overwolrd manager
    //determines the position of the child depending on the index of the object
    public void RepositionChild(float orthoOffset)
    {
        childRect.sizeDelta = new Vector2(orthoOffset, orthoOffset);

        //index and order of the UI object
        float padding = parentRect.rect.height*.01f;
        int index = transform.GetSiblingIndex();
        //if the child is the first one, adds another padding space to create a gap grom the top
        if (index == 0)
        {
            float yPositon = 0f - padding;
            childRect.anchoredPosition = new Vector2(0, yPositon);
        }
        else
        {
            float yPositon = 0f - padding - orthoOffset;
            childRect.anchoredPosition = new Vector2(0, (yPositon * index)-padding);
        }


    }

}
