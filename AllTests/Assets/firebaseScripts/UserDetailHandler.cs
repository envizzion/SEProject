using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
public class UserDetailHandler : MonoBehaviour {

    // Use this for initialization
    public Button saveButton;
    public Button exitButton;
    public InputField height;
    public InputField Weight;
    public InputField age;

    protected FireBaseController fire;

    void Start () {
        saveButton.onClick.AddListener(saveEvent);
        exitButton.onClick.AddListener(exitEvent);
        



    }


   

    private bool isValidated(string[] arr)
    {
        int num;
        decimal dnum;

        foreach (string s in arr) {

            if (!int.TryParse(s, out num) || !decimal.TryParse(s, out dnum)) {
                SSTools.ShowMessage("Invalid Value: "+s, SSTools.Position.bottom, SSTools.Time.twoSecond);
                StartCoroutine(waitAndExit(false));
                return false;
            }

        }
        return true;
       

    }

    void saveEvent() {
        string[] arr = { height.text,Weight.text,age.text};

        if (!isValidated(arr)) return;

        fire = GameObject.FindGameObjectWithTag("FireBaseObject").GetComponent<FireBaseController>();

        Task tsk = fire.SaveUserDetailsAsync(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));
        StartCoroutine(waitForDetailSaving(tsk));
    }

    void exitEvent() {
        new changeScenes().goToMenu();
    }


    void updateFields() {


    }


    IEnumerator waitAndExit(bool isOK) {
        yield return new WaitForSeconds(3);

        if (isOK) new changeScenes().goToMenu();


    }
    IEnumerator waitForDetailSaving(Task tsk)
    {



        fire.logText = "";
        while (!tsk.IsCompleted) yield return null;

        SSTools.ShowMessage("Saving Details", SSTools.Position.bottom, SSTools.Time.twoSecond);


        if ( fire.logText== ("Save Details Completed"))
        {

            yield return new WaitForSeconds(3);
            new changeScenes().goToMenu();
        }

    }

    // Update is called once per frame

}
