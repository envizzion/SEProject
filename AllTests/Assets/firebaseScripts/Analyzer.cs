using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
public class Analyzer : MonoBehaviour {

    public Button btn;

    protected FireBaseController fire;
    
	// Use this for initialization
	void Start () {
      
        	
	}




    

    IEnumerator waitForSave(Task tsk)
    {

        fire.logText = "";
        while (!tsk.IsCompleted) yield return null;



        string tx=fire.logText;
        SSTools.ShowMessage(tx, SSTools.Position.bottom, SSTools.Time.twoSecond);

        if (tx == ("Sign in Completed"))
        {

           
            new changeScenes().goToMenu();
        }

    }
}


