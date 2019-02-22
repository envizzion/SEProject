using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class changeScenes : MonoBehaviour {

	public void goToVR() {
        SceneManager.LoadScene("VR");

    }

    public void goToMenu()
    {
        SceneManager.LoadScene("SampleScene");

    }
}
