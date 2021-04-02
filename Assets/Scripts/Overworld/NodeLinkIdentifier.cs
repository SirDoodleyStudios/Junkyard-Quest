using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeLinkIdentifier : MonoBehaviour
{
    public CircleGenerator circleGenerator;
    public bool isToBeDestroyed { get; set; }

    public List<GameObject> linkedOuterNodes = new List<GameObject>();
    public List<GameObject> linkedInnerNodes = new List<GameObject>();
    //contains the pari of nodes and link objects
    //key is the node linked to, value is the link object itself
    public Dictionary<GameObject, GameObject> pairOuterNodeLink = new Dictionary<GameObject, GameObject>();
    public Dictionary<GameObject, GameObject> pairInnerNodeLink = new Dictionary<GameObject, GameObject>();

    private void Awake()
    {
        circleGenerator = gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<CircleGenerator>();
        circleGenerator.d_DestroyOverworldObjects += DestroyNode;
        isToBeDestroyed = false;
    }

    public void DestroyNode()
    {
        if (isToBeDestroyed == true)
        {
            Destroy(gameObject);
        }
    }


}
