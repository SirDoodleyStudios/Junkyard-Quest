using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkCollisionIdentifier : MonoBehaviour
{
    public GameObject actualLink;
    public GameObject collidingLink;
    public GameObject collidingNode;
    public CircleGenerator circleGenerator;
    public bool isToBeDestroyed { get; set; }
    //this contains the nodes that the link is linking
    public GameObject innerNode;
    public GameObject outerNode;

    //identifier if a node is clickable


    private void Awake()
    {
        actualLink = gameObject;
        circleGenerator = gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<CircleGenerator>();
        circleGenerator.d_DestroyOverworldObjects += DestroyLink;
        isToBeDestroyed = false;
    }
    //upon instantiate access from circleGenerator to add the nodes in the link

    //if upon instantiation, a collision is detected, set the identifier bool as true
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collided");
        if (collision.gameObject.tag == "Link")
        {
            collidingLink = collision.gameObject;
        }
        else if (collision.gameObject.tag == "Node")
        {
            collidingNode = collision.gameObject;
        }

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

    //destroy links that are determined for destroying
    //will be true if it was considered as a collider link
    public void DestroyLink()
    {

        if (isToBeDestroyed == true)
        {
            Debug.Log($"Destroyed Link linking {innerNode.transform.GetSiblingIndex()} in {innerNode.transform.parent.gameObject.transform.GetSiblingIndex()}" +
                $" and {outerNode.transform.GetSiblingIndex()} in {outerNode.transform.parent.transform.GetSiblingIndex()}");
            Destroy(gameObject);
        }
    }

}


