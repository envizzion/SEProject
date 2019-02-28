using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitScript : MonoBehaviour {

    VRDataHandler dataHandler;

    public void callDataHandler()
    {
        dataHandler = GameObject.FindGameObjectWithTag("myCanvas").GetComponent<VRDataHandler>();
        dataHandler.finishSession();

    }

}
