using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediaNavigator : MonoBehaviour {

    // Use this for initialization
    VideoController Vplayer;
    AudioController Aplayer;
    VRDataHandler dataHandler;
	void Start () {

        try
        {
            
            
            Vplayer = GameObject.FindGameObjectWithTag("VidPlayer").GetComponent<VideoController>();
            Aplayer = GameObject.FindGameObjectWithTag("AudPlayer").GetComponent<AudioController>();
            dataHandler = GameObject.FindGameObjectWithTag("myCanvas").GetComponent<VRDataHandler>();

            
            
            
        }
        catch (System.Exception e) {
            Debug.Log(e.Message);
        }
        }

    public void playNextVideo() {
        Vplayer.playNext();

    }
    public void playPrevVideo()
    {
        Vplayer.playPrev();

    }

    public void playNextAudio()
    {
        Aplayer.playNext();

    }
    public void playPrevAudio()
    {
        Aplayer.playPrev();

    }

    public void startAlll()
    {
        
        //Vplayer.startVideo();
        Vplayer.startVideo();
        Aplayer.playNext();
        dataHandler.startSession();
    }


}
