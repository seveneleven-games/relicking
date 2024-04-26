using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class MySounder : MonoBehaviour
{

    public static MySounder instance = null;
    [Space]
    [Header("Sounds set 1")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    
    [Space]
    [Header("Sounds set 2")]
    public AudioClip hoverSound2;
    public AudioClip clickSound2;
    [Space]
    public AudioClip onEnableSound;
   
 
    public AudioSource audi;
   
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.transform);
        audi = GetComponent<AudioSource>();
    }

   

    public void Hover(bool set1sounds,bool set2sounds)
    {
        if (set1sounds == false && set2sounds == true)
        {
            playSet2Sound_hover();
        }
        else if (set1sounds == true && set2sounds == false)
        {
            playSet1Sound_hover();
        }
        else
        {
            playSet1Sound_hover();
        }
    }
    public void Click(bool set1sounds, bool set2sounds)
    {

        if (set1sounds == false && set2sounds == true)
        {
            playSet2Sound_click();
        }
        else if (set1sounds == true && set2sounds == false)
        {
            playSet1Sound_click();
        }
        else {
            playSet1Sound_click();
        }



    }
    
  

    void playSet1Sound_click()
    {
        audi.clip = clickSound;
        audi.Play();
    }

    void playSet2Sound_click()
    {
        audi.clip = clickSound2;
        audi.Play();
    }

    void playSet1Sound_hover()
    {
        audi.clip = hoverSound;
        audi.Play();
    }

    void playSet2Sound_hover()
    {
        audi.clip = hoverSound2;
        audi.Play();
    }

}
