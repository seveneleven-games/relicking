using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupAnimation : MonoBehaviour {
    [Header("Add delay in start of animation")]
    [Range(0f,1.5f)]
    public float startDelay = 0.1f;

    [Header("Speed of FadeIn Animation")]
    [Range(1,5)]
    public int animationSpeed = 1;

    [Header("Play Animation on OnEnable only")]
    public bool onEnable;

    [Header("Play Animation on OnEnable and Start")]
    public bool onEnableAndStart;

    CanvasGroup cg;
	void Awake ()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;   
    }

    private void OnEnable()
    {
        if (onEnable)
        {
            cg = GetComponent<CanvasGroup>();
            cg.alpha = 0;
            Invoke("Delay", startDelay);
        }

    }
    private void Start()
    {
        if(!onEnable)
        Invoke("Delay", startDelay);
        if(onEnableAndStart)
            Invoke("Delay", startDelay);
    }
    void Delay()
    {
        StartCoroutine(fadeIn());
    }

    IEnumerator fadeIn()
    {
        while (cg.alpha < 1)
        {
            cg.alpha = cg.alpha + Time.deltaTime* animationSpeed;
            yield return null;
        }
        yield return 0;
    }
}
