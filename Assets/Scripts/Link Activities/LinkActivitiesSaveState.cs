using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LinkActivitiesSaveState
{
    //contains the current moving node position
    public int currentLinkNodeIndex;
    //the list of LinkActivities to be loaded
    List<LinkActivityEnum> linkActivityEnums = new List<LinkActivityEnum>();
}
