using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkCollisionIdentifier : MonoBehaviour
{
    public GameObject actualLink;
    public GameObject collidingLink;
    public CircleGenerator circleGenerator;
    public bool isToBeDestroyed { get; set; }
    //this contains the nodes that the link is linking
    public GameObject innerNode { get; set; }
    public GameObject outerNode { get; set; }

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
        collidingLink = collision.gameObject;
    }

    //destroy links that are determined for destroying
    //will be true if it was considered as a collider link
    public void DestroyLink()
    {

        if (isToBeDestroyed == true)
        {
            Destroy(gameObject);
        }
    }

}


