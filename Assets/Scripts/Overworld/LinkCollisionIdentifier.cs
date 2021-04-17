using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkCollisionIdentifier : MonoBehaviour
{
    public GameObject actualLink;
    public List<GameObject> collidingLinks;
    public List<GameObject> collidingNodes;
    public CircleGenerator circleGenerator;
    public bool isToBeDestroyed { get; set; }
    //this contains the nodes that the link is linking
    public GameObject innerNode;
    public GameObject outerNode;
    //identifier if traversed already, traversed links will not have any activities
    public bool isTraversed;
    //image of the link that will be darkened if traversed already
    Image linkImage;
    


    private void Awake()
    {
        actualLink = gameObject;
        circleGenerator = gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<CircleGenerator>();
        circleGenerator.d_DestroyLinks += DestroyLink;
        circleGenerator.d_RemoveLinkAsCollider += RemoveLinkAsCollider;
        isToBeDestroyed = false;
        // the image to be modified is in the child of the link prefab
        linkImage = gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
    }
    //upon instantiate access from circleGenerator to add the nodes in the link

    //if upon instantiation, a collision is detected, set the identifier bool as true
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Link")
        {
            //collidingLink = collision.gameObject;
            collidingLinks.Add(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Node")
        {
            //collidingNode = collision.gameObject;
            collidingNodes.Add(collision.gameObject);
        }

    }
    //function called by overworld manager when clicking a clickable node
    //makes the link path traversed, meanin, it will not have activities anymore
    public void MakeLinkTraversed()
    {
        isTraversed = true;
        linkImage.color = new Color(.5f, .5f, .5f);
    }

    //function called to access the inner and outer nodes of a link and then remove respective references to each other
    //called by CircleGenerator when destroying collider links
    //calls the inner and outer nodes' NodeLinkIdentifier then trigger a function there that removes sent gameObjects from their link list
    public void RemoveNodeReferences()
    {
        NodeLinkIdentifier innerIdent = innerNode.GetComponent<NodeLinkIdentifier>();
        NodeLinkIdentifier outerIdent = outerNode.GetComponent<NodeLinkIdentifier>();

        innerIdent.RemoveOuterNodeLinkReference(outerNode);
        outerIdent.RemoveInnerNodeLinkReference(innerNode);
    }

    //event called from circle generator that removes the passed parameter from this link's list of linkColliders
    public void RemoveLinkAsCollider(GameObject collidingLink)
    {
        if (collidingLinks.Contains(collidingLink))
        {
            collidingLinks.Remove(collidingLink);
        }
    }

    //destroy links that are determined for destroying
    //will be true if it was considered as a collider link
    public void DestroyLink()
    {

        if (isToBeDestroyed == true)
        {
            //Debug.Log($"Destroyed Link linking {innerNode.transform.GetSiblingIndex()} in {innerNode.transform.parent.gameObject.transform.GetSiblingIndex()}" +
            //    $" and {outerNode.transform.GetSiblingIndex()} in {outerNode.transform.parent.transform.GetSiblingIndex()}");
            Destroy(gameObject);
        }
    }

}


