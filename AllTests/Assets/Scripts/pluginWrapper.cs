using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class pluginWrapper : MonoBehaviour
{
    public Text speedTxt;
    public Text MsgText;
    //public Text DataText;
    AndroidJavaClass bluetoothComunication;
    AndroidJavaObject bluetoothCom;
    // public GameObject cub;
    private string sol="";
    string data = "", msg = "";
    string d;
    void Start()
    {
        try
        {
            //Calling the Java Bluetooth class and initalising the connection
            bluetoothComunication = new AndroidJavaClass("library.Serialbluetooth");
            bluetoothCom = bluetoothComunication.CallStatic<AndroidJavaObject>("getInstance");
            //   sol = bluetoothCom.Call<string>("Loooper_Connect");                                    //The looper_Connect method's output will be saved in 'sol'
            //  myText.text=sol;          //assigning the connection status to a text object on the gui

            bluetoothCom.Call("startBThread");
        }
        catch (AndroidJavaException ex)
        {
            sol = ex.ToString();
        }
    }

   
    /* void Update()                  //Methods to be run continuosly for receiving the bluetooth data
     {

        // myText.text = "unity text";
         try
         {
             data = bluetoothCom.Call<string>("ReadData");               //Calling the ReadData  to receive data
             if (data.Length > 1)                                        //the output will be printed on Logcat in android studio 

             {
                 // cub.SetActive(true);

             }
             else
             {
                // myText.text = "no data";
             }
         }
         catch (AndroidJavaException ex)
         {
             data = ex.ToString();
         }
     }
     */


    void Update()
    {
      
        try
        {
            msg = bluetoothCom.Call<string>("getMessage");               //Calling the ReadData  to receive data
            data = bluetoothCom.Call<string>("getData");
            MsgText.text = msg;
            speedTxt.text=data;

    //  DataText.text = data;

}
        catch (AndroidJavaException ex)
        {
            data = ex.ToString();
        }
    }

    public string[] sendData() {
        string[] arr = {msg ,data};
        Debug.Log(data);
        return arr;

    }

}
