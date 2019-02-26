using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SignOutFromMainMenu : MonoBehaviour {


    public Text userEmail;
    public Button signOut;

    FireBaseController fire;

	// Use this for initialization
	void Start () {
        signOut.onClick.AddListener(signOutListner);
        fire = GameObject.FindGameObjectWithTag("FireBaseObject").GetComponent<FireBaseController>();

        Debug.Log("Logged in as: " + fire.getCurrUserEmail());
        userEmail.text ="Logged in as: "+ fire.getCurrUserEmail();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void signOutListner() {
        fire.logText = "";
        fire.SignOut();
        SSTools.ShowMessage(fire.logText, SSTools.Position.bottom, SSTools.Time.twoSecond);
        
    }
}
