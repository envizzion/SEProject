using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AudioController : MonoBehaviour {
    public AudioSource audio;
    List<string> audioPaths;
    int CurrtrackNo = 0;
    string audDir;
    bool start=false;
    bool beingHandled = false;
	// Use this for initialization
	void Start () {

        audDir = Application.persistentDataPath + "/Audio/";
        audDir = audDir.Replace('\\', '/');
        audioPaths = new List<string>();

        DirectoryInfo dataDir = new DirectoryInfo(audDir);
        try
        {
            FileInfo[] fileinfo = dataDir.GetFiles();

            for (int i = 0; i < fileinfo.Length; i++)
            {
                string name = fileinfo[i].Name;
                if (name.EndsWith(".ogg") || name.EndsWith(".wav"))
                {
                    Debug.Log("name  " + audDir + name);
                    audioPaths.Add(audDir + name);
                }
            }

        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }

        //  playAudio(audioPaths[0]);
       // StartCoroutine(LoadAudioAtStart(audioPaths[0]));

    }

    private void Update()
    {
        if (!audio.isPlaying && start && !beingHandled)
        {

            this.playNext();
            StartCoroutine(HandleIt(2));

        }
    }


    public void startAudio(){
        CurrtrackNo = 0;
        //playAudio(audioPaths[0]);
        playNext();
        start = true;
    }



    public void playAudio(string clipDir)
    {

        StartCoroutine(LoadAudio(clipDir));

    }

    public void playNext()
    {
        CurrtrackNo++;
        if (CurrtrackNo == audioPaths.Count) CurrtrackNo = 0;
        playAudio(audioPaths[CurrtrackNo]);

    }

    public void playPrev()
    {
        CurrtrackNo--;
        if (CurrtrackNo == -1) CurrtrackNo = audioPaths.Count - 1;
        playAudio(audioPaths[CurrtrackNo]);
    }



    IEnumerator LoadAudio(string FullPath)
    {
        

        print(FullPath); // result: D:\files\audio files\ZOOM0001.WAV

        //FullPath = FullPath.Replace('\\', '/');
        FullPath = "file:///" + FullPath;

        print(FullPath); // result: file:///D:/files/audio files/ZOOM0001.WAV

        WWW URL = new WWW(FullPath);
        yield return URL;

        audio.clip = URL.GetAudioClip();
        audio.Play();
    }

    IEnumerator LoadAudioAtStart(string FullPath)
    {


        print(FullPath); // result: D:\files\audio files\ZOOM0001.WAV

        //FullPath = FullPath.Replace('\\', '/');
        FullPath = "file:///" + FullPath;

        print(FullPath); // result: file:///D:/files/audio files/ZOOM0001.WAV

        WWW URL = new WWW(FullPath);
        yield return URL;
        audio.Pause();
        audio.clip = URL.GetAudioClip();
        audio.Pause();

    }

    private IEnumerator HandleIt(long time)
    {
        beingHandled = true;
        // process pre-yield
        yield return new WaitForSeconds(time);
        // process post-yield
        beingHandled = false;
    }

}
