using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class makeFolder : MonoBehaviour {

    void Awake() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("DontDestroy");
        if (objs.Length > 1) { Destroy(this.gameObject); }
        else { DontDestroyOnLoad(this.gameObject); }
    }

     void Start()
    {
        makeDir();
        
    }


    public void makeDir()
    {
        string dirV = Application.persistentDataPath + "/Video";
        string dirA = Application.persistentDataPath + "/Audio";
        DirectoryInfo posePathV = new DirectoryInfo(dirV);
        DirectoryInfo posePathA = new DirectoryInfo(dirA);
        if (!posePathV.Exists)
        {
            posePathV.Create();
           // Debug.Log("Directory created!");
          
            SSTools.ShowMessage("Created Video Directory", SSTools.Position.bottom, SSTools.Time.threeSecond);

        }
        else if (!posePathA.Exists)
        {
            posePathA.Create();
            //Debug.Log("Directory created!");

            SSTools.ShowMessage("Created Audio Directory", SSTools.Position.bottom, SSTools.Time.threeSecond);

        }
        else
        {
           // Debug.Log("Directory already exists:" + dir);
           // txt.text = "Directory already exists:" + dir;
            SSTools.ShowMessage("All Directories exist",SSTools.Position.bottom,SSTools.Time.threeSecond);
        }
    }

   
}

