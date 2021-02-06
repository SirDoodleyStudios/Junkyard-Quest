using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLogics : MonoBehaviour
{
    BaseUnitFunctions baseUnitStats;
    // Start is called before the first frame update
    void Start()
    {
        baseUnitStats = gameObject.GetComponent<BaseUnitFunctions>();
        baseUnitStats.InitializeStats();
    }

    //just for testing
    public void Update()
    {

    }


}
