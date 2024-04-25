using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnEnable : MonoBehaviour {

    AudioSource onEnableSound;
    
    private void OnEnable()
    {

        if (onEnableSound == null)
            onEnableSound  = GameObject.Find("OnEnableSound").GetComponent<AudioSource>();

        onEnableSound.Play();
    }
   
}
