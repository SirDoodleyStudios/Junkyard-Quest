using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkCollisionIdentifier : MonoBehaviour
{
    public GameObject actualLink;
    public GameObject collidingLink;
    public CircleGenerator circleGenerator;
    public bool isToBeDestroyed { get; set; }

    private void Awake()
    {
        actualLink = gameObject;
        circleGenerator = gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<CircleGenerator>();
        circleGenerator.d_DestroyLinks += DestroyLink;
        isToBeDestroyed = false;
    }
    //if upon instantiation, a collision is detected, set the identifier bool as true
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collided");
        collidingLink = collision.gameObject;
    }

    public void DestroyLink()
    {

        if (isToBeDestroyed == true)
        {
            Destroy(gameObject);
        }
    }

}


