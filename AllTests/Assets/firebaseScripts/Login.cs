using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Threading.Tasks;
public class Login : MonoBehaviour {

    public Text txt;

    public InputField email;
    public InputField password;
    public InputField confirmPassword;
    
    public Button register;
    public Button signin;
    public Button exit;
    protected FireBaseController fire;

	// Use this for initialization
	void Start () {
              
        register.onClick.AddListener(registerListner);
        signin.onClick.AddListener(signinListner);
        exit.onClick.AddListener(exitListner);
        if (SaveLoadManager.checkFile()) {
            string[] arr = SaveLoadManager.LoadData();

            Debug.Log(arr[0]+" "+arr[1]);
            email.text = arr[0];
            password.text = arr[1];


          
        }

        
        
               
    }

   
 

    void registerListner() {


        if (!password.text.Equals(confirmPassword.text)) {
            SSTools.ShowMessage("Passwords Doesn't Match", SSTools.Position.bottom, SSTools.Time.twoSecond);
        }

        fire = GameObject.FindGameObjectWithTag("FireBaseObject").GetComponent<FireBaseController>();
        Task tsk= fire.CreateUserWithEmailAsync(email.text, password.text); ;

        StartCoroutine(waitForRegist(tsk));

    }

    void signinListner() {
        fire = GameObject.FindGameObjectWithTag("FireBaseObject").GetComponent<FireBaseController>();
        Task tsk = fire.SigninWithEmailCredentialAsync(email.text, password.text); ;
        
        StartCoroutine(waitForSignin(tsk));
    }


    void exitListner()
    {

        Application.Quit();
    }


    IEnumerator waitForRegist(Task tsk)
    {

        fire.logText = "";
        while (!tsk.IsCompleted) yield return null;



        string tx = txt.text = fire.logText;
        SSTools.ShowMessage(tx, SSTools.Position.bottom, SSTools.Time.twoSecond);

        if (tx == ("User Creation Completed"))
        {

            SaveLoadManager.saveData(email.text, password.text);
            new changeScenes().goToMenu();
        }
    }


        IEnumerator waitForSignin(Task tsk) {

            fire.logText = "";
            while (!tsk.IsCompleted) yield return null;



            string tx = txt.text = fire.logText;
            SSTools.ShowMessage(tx, SSTools.Position.bottom, SSTools.Time.twoSecond);

            if (tx == ("Sign in Completed"))
            {

                SaveLoadManager.saveData(email.text, password.text);
                new changeScenes().goToMenu();
            }

        }



}
