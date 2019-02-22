using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Net;
using System;
using SimpleFileBrowser;
public class DownloadFile : MonoBehaviour
{
    string path="null";
    public Text txt;
    string error = "null";
    





    void DownloadFilee(string filepath,string type)
    {
       filepath= filepath.Replace ('\\','/');
        string[] parts = filepath.Split('/');
        string fileName = parts[parts.Length - 1];

        string savePath = Application.persistentDataPath + "/Video/" + fileName;

        if(type.Equals("Audio")) savePath = Application.persistentDataPath + "/Audio/" + fileName;



        Debug.Log("filePath:" + filepath + "  FileName:" + fileName);
        Debug.Log("savePath :" + savePath);
        FileInfo dir = new FileInfo(savePath);
        if (dir.Exists)
        {
            SSTools.ShowMessage("FileName Already exists", SSTools.Position.bottom, SSTools.Time.threeSecond);
            txt.text = savePath;
            return;
        }
        
        
        Debug.Log(fileName);
        txt.text = savePath;
      
        WebClient client = new WebClient();
        client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(DownloadFileCompleted);
        client.DownloadFileAsync(new Uri(filepath),savePath);
        
    }

    void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        if (e.Error == null)
        {
            // AllDone();
            //    SSTools.ShowMessage("write Success", SSTools.Position.bottom, SSTools.Time.threeSecond);
            error = "success";
        }
        else {
          error=e.Error.ToString();
            Debug.Log(e.Error);
        }
    }


    void deleteFilee(String filepath,string type) {
        filepath = filepath.Replace('\\', '/');
        string[] parts = filepath.Split('/');
        string fileName = parts[parts.Length - 1];

        string savePath = Application.persistentDataPath + "/Video/" + fileName;

        if (type.Equals("Audio")) savePath = Application.persistentDataPath + "/Audio/" + fileName;

        FileInfo dir = new FileInfo(savePath);

        if (!dir.Exists)
        {
            SSTools.ShowMessage("FileName Doesn't exists in :"+type, SSTools.Position.bottom, SSTools.Time.threeSecond);
            txt.text = savePath;
            return;
        }


        Debug.Log(fileName);
        txt.text = savePath;

        
        try
        {
            File.Delete(savePath);
            SSTools.ShowMessage(type +" "+fileName+" deleted" , SSTools.Position.bottom, SSTools.Time.threeSecond);

        }
        catch (System.Exception e)
        {
            txt.text = e.Message;
        }


    }




    public void getLocalVideo()
    {

        StartCoroutine(ShowLoadDialogCoroutine("Video","Add"));
    }

    public void getLocalAudio()
    {

        StartCoroutine(ShowLoadDialogCoroutine("Audio","Add"));
    }


    public void deleteMyVideo()
    {

        StartCoroutine(ShowLoadDialogCoroutine("Video", "Delete"));
    }

    public void deleteMyAudio()
    {

        StartCoroutine(ShowLoadDialogCoroutine("Audio", "Delete"));
    }





    IEnumerator ShowLoadDialogCoroutine(string type,string addOrDel)
    {



      //  FileBrowser.SetFilters(false, new FileBrowser.Filter("Video", ".mp4"), new FileBrowser.Filter("Audio", ".mp3"));
        string delPath = null;

        if (type.Equals("Video")) {

            FileBrowser.SetFilters(false, new FileBrowser.Filter("Video", ".mp4"));

           FileBrowser.SetDefaultFilter(".mp4");
            if (addOrDel == "Delete") delPath=Application.persistentDataPath + "/Video/";
        }
        else if (type.Equals("Audio")) {
            FileBrowser.SetFilters(false, new FileBrowser.Filter("Audio", ".ogg",".wav"));

            FileBrowser.SetDefaultFilter(".ogg");
            if (addOrDel == "Delete") delPath = Application.persistentDataPath + "/Audio/";
        }

       // FileBrowser.ShowLoadDialog(null, null, false, delPath, "Load File", "Load");



        yield return FileBrowser.WaitForLoadDialog(false,delPath ,"Load File", "Load");

        path = FileBrowser.Result;


        if (addOrDel.Equals("Delete"))
        {
            deleteFilee(path, type);
        }
        else
        {
            DownloadFilee(path, type);
        }
    }

    void Update()
    {
        if (!error.Equals("null")) {
            SSTools.ShowMessage("write Success", SSTools.Position.bottom, SSTools.Time.threeSecond);
            error = "null";

        }
        
    }

}
