using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FillImageAnim : MonoBehaviour
{
    [SerializeField]
     Image loadingFill;
    [SerializeField]
    [Range(0f, 0.9f)]
    public float defaultFill;



    private void OnEnable()
    {
        if (loadingFill == null)
            loadingFill = transform.GetChild(0).GetComponent<Image>();

        defaultFill = loadingFill.fillAmount;

        loadingFill.fillAmount = 0f;
        Invoke("Delay", 0.2f);

    }
    void Delay()
    {
        StartCoroutine(loading());
    }
    IEnumerator loading()
    {
        while (loadingFill.fillAmount < defaultFill)
        {
            loadingFill.fillAmount += (0.02f);
            yield return null;
        }

       
        yield return 0;
    }

    
}
