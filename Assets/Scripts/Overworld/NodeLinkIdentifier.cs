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
    //the node activity attached to this
    public NodeActivityEnum nodeActivityEnum;

    //child 1 is the node area of the node prefab which contains the actual node sprite
    Transform nodeAreaTrans;
    Image nodeArea;
    //node's color
    Color originalColor;
    Color stateColor;
    //the activity icon of the node itself
    Image nodeActivityIcon;
    bool isActivityDone;

    private void Awake()
    {
        //Child 1 of the node prefab is the nodeArea, dont mess with the prefab
        nodeAreaTrans = gameObject.transform.GetChild(1);
        //for the node Area only
        nodeArea = nodeAreaTrans.GetComponent<Image>();
        originalColor = Color.white;
        //for the node icon, child 0 is the node Icon image, it's the only chlid
        nodeActivityIcon = nodeAreaTrans.GetChild(0).gameObject.GetComponent<Image>();

        //assigns the NodeAvtivity immediately
        //random only for now, will incorporate logic later
        //6 max index is because NodeActivityEnum has 6 elements, make sure to change this if there re changes to enums

        if (!UniversalSaveState.isMapInitialized)
        {
            nodeActivityEnum = (NodeActivityEnum)Random.Range(0, 6);
            AssignNodeIconImage();
        }


        circleGenerator = gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<CircleGenerator>();
        circleGenerator.d_DestroyNodes += DestroyNode;
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
    //called by this script during first initialize
    //called by loading function during next exectuions
    public void AssignNodeIconImage()
    {
        nodeActivityIcon.sprite = Resources.Load<Sprite>($"NodeIcon/{nodeActivityEnum}");
    }

    //called by update in OverworldManager, indicates if the node is avalable for click
    public void MakeNodeClickable()
    {
        isSelected = false;
        isClickable = true;
        //color clickable nodes to red
        nodeArea.color = Color.red;

    }
    public void MakeNodeSelected()
    {
        nodeArea.color = Color.green;
        isSelected = true;
        isClickable = false;
    }
    public void MakeNodeUnselected()
    {
        nodeArea.color = originalColor;
        isSelected = false;
        isClickable = false;

    }

    //called by LinkCollisionIdentifier when destroying a link, receives a gameObject to be removed from inner and outer Node references
    //function in LinkCollisionIdentifier that calls these functions are called from CircleGenerator
    public void RemoveInnerNodeLinkReference(GameObject node)
    {
        linkedInnerNodes.Remove(node);
        pairInnerNodeLink.Remove(node);
    }
    public void RemoveOuterNodeLinkReference(GameObject node)
    {
        linkedOuterNodes.Remove(node);
        pairOuterNodeLink.Remove(node);
    }

    //effects on node when hovered
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isClickable)
        {
            stateColor = nodeArea.color;
            nodeArea.color = Color.blue;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {

        if (isClickable)
        {
            nodeArea.color = stateColor;
        }
    }


}
