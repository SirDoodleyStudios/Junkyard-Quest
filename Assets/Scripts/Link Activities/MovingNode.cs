using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingNode : MonoBehaviour
{
    RectTransform objectRect;
    private void Awake()
    {
        //tweeing initialize, needs to be called before any DOTween
        DOTween.Init(true, true, LogBehaviour.Default);
        DOTween.SetTweensCapacity(20000, 100);
        objectRect = gameObject.GetComponent<RectTransform>();
    }

    //called by external script to make the node move
    public void MovePlayerNode(Vector3 destination)
    {
        objectRect.DOAnchorPos(destination, .05f, false);
    }

}
