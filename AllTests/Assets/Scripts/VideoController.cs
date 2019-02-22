
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using System.IO;

public class VideoController : MonoBehaviour
{
 
    public VideoPlayer video;
   // public Slider slider;
    string vidDir;
    int CurrtrackNo=0; //track index
    bool start = false;

   List<string> videoPaths;

    //Properties of the video player
    bool isDone = false;

    public bool isPlaying
    {
        get { return video.isPlaying; }

    }
    public bool isLooping
    {
        get { return video.isLooping; }
    }
    public bool isPrepared
    {
        get { return video.isPrepared; }
    }
    public bool isDonee
    {
        get { return isDone; }
        
    }
    public double time
    {
        get { return video.time; }
    }
    public ulong duration
    {
        get { return (ulong)(video.frameCount / video.frameRate); }
    }
    public double nTime
    {
        get { return time / duration; }
    }

    void OnEnable()
    {
        video.errorReceived += errorReceived;
        video.frameReady += frameReady;
        video.loopPointReached += loopPointReached;
        video.prepareCompleted += prepareCompleted;
        video.seekCompleted += seekCompleted;
        video.started += started;
    }

    void OnDisable()
    {
        video.errorReceived -= errorReceived;
        video.frameReady -= frameReady;
        video.loopPointReached -= loopPointReached;
        video.prepareCompleted -= prepareCompleted;
        video.seekCompleted -= seekCompleted;
        video.started -= started;
    }

    void errorReceived(VideoPlayer v, string msg)
    {
        Debug.Log("Video Player Error" + msg);
    }
    void frameReady(VideoPlayer v, long frame)
    {
        Debug.Log("Heavy cpu use");

    }
    void loopPointReached(VideoPlayer v)
    {
        Debug.Log("Video Player loop point reached");
        isDone = true;
        this.playNext();

    }
    void prepareCompleted(VideoPlayer v)
    {
        Debug.Log("Video Player finished preparing");
        if (start == true) video.Play();
        isDone = false;

    }
    void seekCompleted(VideoPlayer v)
    {
        Debug.Log("Video Player finished seeking");
        isDone = false;
    }
    void started(VideoPlayer v)
    {
        Debug.Log("Video Player started");

    }


     

    public void LoadVideo(string name)
    {

        //string temp = Application.dataPath + "/videos/" + name;//name = video name with the format
       // if (video.url == temp) return;
        video.url = name;
        video.Prepare();
        Debug.Log("can set playback speed: " + video.canSetPlaybackSpeed);
        if(video.isPrepared)Debug.Log("video is prepared");
        else Debug.Log("video is not prepared");
    }
    public void playVideo()
    {
        if (!isPrepared) return;
        video.Play();
    }
    public void pauseVideo()
    {
        if (!isPrepared) return;
        video.Pause();
    }
    public void restartVideo()
    {
        if (!isPrepared) return;
        pauseVideo();
        seek(0);
    }
    public void loopVideo(bool Toggle)
    {
        if (!isPrepared) return;
        video.isLooping = Toggle;
    }
    public void seek(float nTime)
    {
        if (!isPrepared) return;
        nTime = Mathf.Clamp(nTime, 0, 1);
        video.time = nTime * duration;
    }
    public void incrementPlaybackSpeed()
    {
        if (!video.canSetPlaybackSpeed) return;
        video.playbackSpeed += 1;
        video.playbackSpeed = Mathf.Clamp(video.playbackSpeed, 0, 10);
    }
    public void dicrementPlaybackSpeed()
    {
        video.playbackSpeed -= 0.05f;
        video.playbackSpeed = Mathf.Clamp(video.playbackSpeed, 0, 10);

    }

    public void playNext() {
        CurrtrackNo++;
        if (CurrtrackNo == videoPaths.Count) CurrtrackNo = 0;
        this.LoadVideo(videoPaths[CurrtrackNo]);
        this.playVideo();
        
    }

    public void playPrev()
    {
        CurrtrackNo--;
        if (CurrtrackNo == -1) CurrtrackNo = videoPaths.Count-1;
        this.LoadVideo(videoPaths[CurrtrackNo]);
        this.playVideo();
       
    }


    public void startVideo()
    {
        CurrtrackNo = 0;
        video.playOnAwake = true;
        video.Play();
        start = true;
        
    }

    void Start()
    {
        vidDir=Application.persistentDataPath + "/Video/";
        vidDir = vidDir.Replace('\\','/');
        videoPaths = new List<string>();

        DirectoryInfo dataDir = new DirectoryInfo(vidDir);
        try
        {
            FileInfo[] fileinfo = dataDir.GetFiles();

            for (int i = 0; i < fileinfo.Length; i++)
            {
                string name = fileinfo[i].Name;
                if (name.EndsWith(".mp4"))
                {
                    Debug.Log("name  " + vidDir  + name);
                    videoPaths.Add(vidDir + name);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }

        //video.Pause();
        //video.playOnAwake = false;
        video.Pause();
        this.LoadVideo(videoPaths[CurrtrackNo]);
        
        
        //this.playVideo();
    }







void Update()
    {
        //if (!isPrepared) return;

        //slider.value = (float)nTime;
        
    }
}

