using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using SimpleFileBrowser;
using System.IO;


public class getPath : MonoBehaviour {
    public Text txt;
    public AudioSource source;
    

    public void loadFileBrowser() {
        // Set filters (optional)
        // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
        // if all the dialogs will be using the same filters
          FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"), new FileBrowser.Filter("Text Files", ".txt", ".pdf"));

        // Set default filter that is selected when the dialog is shown (optional)
        // Returns true if the default filter is set successfully
        // In this case, set Images filter as the default filter
           FileBrowser.SetDefaultFilter(".jpg");

        // Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
        // Note that when you use this function, .lnk and .tmp extensions will no longer be
        // excluded unless you explicitly add them as parameters to the function
        //   FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        // Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
        // It is sufficient to add a quick link just once
        // Name: Users
        // Path: C:\Users
        // Icon: default (folder icon)
        //  FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        // Show a save file dialog 
        // onSuccess event: not registered (which means this dialog is pretty useless)
        // onCancel event: not registered
        // Save file/folder: file, Initial path: "C:\", Title: "Save As", submit button text: "Save"
        // FileBrowser.ShowLoadDialog( null, null, false, "C:\\", "Save As", "Save" );

        // Show a select folder dialog 
        // onSuccess event: print the selected folder's path
        // onCancel event: print "Canceled"
        //  Load file/folder: folder, Initial path: default (Documents), Title: "Select Folder", submit button text: "Select"

        // FileBrowser.ShowLoadDialog( (path) => { Debug.Log( "Selected: " + path ); }, 
        //                                () => { Debug.Log( "Canceled" ); }, 
        //                               true, null, "Select Folder", "Select" );
        //txt.text = FileBrowser.Result;
        // Coroutine example

       // AudioClip clip;

      //  string path = EditorUtility.OpenFilePanel("d", "", "ogg");
      //  WWW www = new WWW("file:///" + path);
      //  clip = www.GetAudioClip();

      //  source.clip = clip;
     //   source.Play();

      StartCoroutine(ShowLoadDialogCoroutine());

    }



    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(false, null, "Load File", "Load");

        // Dialog is closed
        // Print whether a file is chosen (FileBrowser.Success)
        // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
        //  Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);
        txt.text = FileBrowser.Result;
        System.IO.DirectoryInfo dataDir = new DirectoryInfo(FileBrowser.Result);
        List<string> songdirs = new List<string>();
        AudioClip clip;
        try
        {
            FileInfo[] fileinfo = dataDir.GetFiles();
            
            for (int i = 0; i < fileinfo.Length; i++)
            {
                string name = fileinfo[i].Name;
                if (name.EndsWith(".ogg"))
                {
                    Debug.Log("name  "+fileinfo[i].DirectoryName+"\\"+ name);
                    songdirs.Add(fileinfo[i].DirectoryName + "\\" + name);
                //    WWW www = new WWW(songdirs[0]);
                //    clip = www.GetAudioClip();
                 //   clip.name = name;
                 //   source.clip = clip;
                 //   source.Play();
                }
                }

            


        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }



        LoadSong(songdirs[0]);
    }



    public void loadAudio() {

        AudioClip[] clips;
        var textures = Resources.LoadAll("Music", typeof(AudioClip));
        foreach (var t in textures)
            Debug.Log("nme: "+t.name);

        ///var  clips  = Resources.LoadAll("Music", typeof(AudioClip));
        ///
        try
        {
            clips = Resources.LoadAll<AudioClip>("Music") ;
            source.clip = clips[0];
            source.volume = 1;
            source.Play();
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }


    public void LoadSong(string path)
    {
        StartCoroutine(LoadSongCoroutine(path));
    }

    IEnumerator LoadSongCoroutine(string path)
    {
        string url = string.Format("file://{0}", path);
        WWW www = new WWW(url);
        yield return www;

        source.clip = www.GetAudioClip(false, false);
        source.Play();
        //source = song.clip.name;
        //length = song.clip.length;
    }



}