using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeLinkIdentifier : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CircleGenerator circleGenerator;
    public bool isToBeDestroyed { get; set; }

    public List<GameObject> linkedOuterNodes = new List<GameObject>();
    public List<GameObject> linkedInnerNodes = new List<GameObject>();
    //contains the pari of nodes and link objects
    //key is the node linked to, value is the link object itself
    public Dictionary<GameObject, GameObject> pairOuterNodeLink = new Dictionary<GameObject, GameObject>();
    public Dictionary<GameObject, GameObject> pairInnerNodeLink = new Dictionary<GameObject, GameObject>();

    //bool identifier if node is available for clicking
    public bool isClickable;
    //bool identifier if node is currently selected
    public bool isSelected;

    //child 1 is the node area of the node prefab which contains the actual node sprite
    Image nodeImage;
    //node's color
    Color originalColor;
    Color stateColor;

    private void Awake()
    {
        nodeImage = gameObject.transform.GetChild(1).GetComponent<Image>();
        originalColor = Color.white;
        circleGenerator = gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<CircleGenerator>();
        circleGenerator.d_DestroyOverworldObjects += DestroyNode;
        isToBeDestroyed = false;
        isClickable = false;
        isSelected = false;
    }
    //called by an event during circle nodes generation
    public void DestroyNode()
    {
        if (isToBeDestroyed == true)
        {
            Debug.Log($"Destroyed Node in {gameObject.transform.parent.gameObject.transform.GetSiblingIndex()}");
            Destroy(gameObject);
        }
    }
    //called by update in OverworldManager, indicates if the node is avalable for click
    public void MakeNodeClickable()
    {
        isClickable = true;
        //color clickable nodes to red
        nodeImage.color = Color.red;

    }
    public void MakeNodeSelected()
    {
        nodeImage.color = Color.green;
        isSelected = true;
    }
    public void MakeNodeUnselected()
    {
        nodeImage.color = originalColor;
        isSelected = false;
        isClickable = false;

    }

    //effects on node when hovered
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isClickable)
        {
            stateColor = nodeImage.color;
            nodeImage.color = Color.blue;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {

        if (isClickable)
        {
            nodeImage.color = stateColor;
        }
    }


}
