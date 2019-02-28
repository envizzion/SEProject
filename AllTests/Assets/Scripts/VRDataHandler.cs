using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class VRDataHandler : MonoBehaviour {

     //List<float> speeds;
     float currSpeed=5;
     float calories;
     uint  seconds;
     float distanceKM;
     float weight=45; 
    // Use this for initialization
    public Button startButton;
    public Button exitButton;
    public Text speedTxt;
    public Text caloryTxt;
    public Text distanceTxt;
    public Text timeTxt;

    protected FireBaseController fire;
    protected pluginWrapper wrapper;
    // Use this for initialization
    void Start()
    {
        startButton.onClick.AddListener(startSession);
        exitButton.onClick.AddListener(finishSession);

    }
    public void startSession() {

        wrapper = GameObject.FindGameObjectWithTag("myCanvas").GetComponent<pluginWrapper>();


        StartCoroutine(dataGrabRoutine());
        startButton.enabled = false;
    }


    public void finishSession()
    {
        StopCoroutine(dataGrabRoutine());

        if (Math.Round(calories, 2) < 1) {
            SSTools.ShowMessage("Invalid Session : Calories burned < 1 ", SSTools.Position.bottom, SSTools.Time.twoSecond);

            StartCoroutine(waitForExitWithoutSaving());
            return;
        }


        SSTools.ShowMessage("Saving Session", SSTools.Position.bottom, SSTools.Time.twoSecond);

        fire = GameObject.FindGameObjectWithTag("FireBaseObject").GetComponent<FireBaseController>();

        float timeMins = seconds/60;
        
        Task tsk = fire.SaveSessionAsync((float)Math.Round(distanceKM,2) , (float)Math.Round(timeMins,2) , (float)Math.Round(calories,2));

        StartCoroutine(waitForSave(tsk));
    }






    IEnumerator waitForSave(Task tsk)
    {

        fire.logText = "";
        while (!tsk.IsCompleted) yield return null;

        string tx = fire.logText;
        SSTools.ShowMessage(tx, SSTools.Position.bottom, SSTools.Time.twoSecond);

        yield return new WaitForSeconds(3);
        if (tx == ("Save Session Completed"))
        {
          new changeScenes().goToMenu();
        }

    }

    IEnumerator waitForExitWithoutSaving() {

        yield return new WaitForSeconds(3);
        new changeScenes().goToMenu();


    }


    IEnumerator dataGrabRoutine() {
        float counter = 0;
        float cal;
        while(true) {
            counter++;
            
                string[]  str=wrapper.sendData();
                currSpeed = int.Parse(str[1]);

        //set distance >> currspeed in ms-1
        distanceKM += currSpeed/1000;


            //set calories variable
             cal= ((float)(    (54.35026948 * (currSpeed * 2.237) + 4.949954089 * (weight * 2.20462) - 875)   ) / 3600 );

            if (cal > 0) calories += cal;

            caloryTxt.text = Math.Round(calories,2).ToString();

            //set time variable
            seconds++;
            timeTxt.text = seconds.ToString();
    
           yield return new  WaitForSeconds(1);

        }

      }


   
	
	// Update is called once per frame
	void Update () {
        speedTxt.text = currSpeed.ToString();
        distanceTxt.text = distanceKM.ToString();
        
        
        		
	}
}
