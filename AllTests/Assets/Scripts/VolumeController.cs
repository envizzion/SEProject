using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VolumeController : MonoBehaviour {
  public AudioSource audio;
    public Slider slider;
    private void Start()
    {
        audio.volume = 0.5f;
        slider.value = 0.5f;
    }


    public void setVolume() {

        audio.volume = slider.value;




    }

}
