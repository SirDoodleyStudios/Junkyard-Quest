using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EventSaveState
{
    //enum key of current event
    public AllEvents eventEnum;
    //state of evenet, will vary per event
    public int eventSequence;

}
