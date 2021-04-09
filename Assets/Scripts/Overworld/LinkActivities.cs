using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkActivities : MonoBehaviour
{
    public List<LinkActivityEnum> linkActivities = new List<LinkActivityEnum>();

    void Awake()
    {
        for (int i = 0; i <= 2; i++)
        {
            //creates a random list of activities, to be updated later to have a logic in identifying probabilities
            linkActivities.Add((LinkActivityEnum)Random.Range(0, 3));
        }
    }

}
