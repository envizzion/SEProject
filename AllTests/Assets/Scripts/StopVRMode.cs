using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class StopVRMode : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        StartCoroutine(ActivatorVR("None"));

    }


    public IEnumerator ActivatorVR(string str)
    {
        XRSettings.LoadDeviceByName(str);
        yield return null;
        XRSettings.enabled = false;

    }

}
