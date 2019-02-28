using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class StartVRMode : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        

    }

    private void Awake()
    {
        StartCoroutine(ActivatorVR("Cardboard"));
    }


    public IEnumerator ActivatorVR(string str)
    {
        XRSettings.LoadDeviceByName(str);
        yield return null;
        XRSettings.enabled = true;

    }

}
