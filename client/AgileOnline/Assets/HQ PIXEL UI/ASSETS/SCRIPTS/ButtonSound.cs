
using UnityEngine;
using UnityEngine.EventSystems;
public class ButtonSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public bool soundOnClick = true;
    [Space]
    public bool soundOnHover = true;
    public bool soundOnUnHover = true;
    [Space]
    [Header("Select sound sets [if both false or true] set1 will be used")]
    [Space]
    public bool useSet1Sounds = false;
    public bool useSet2Sounds = false;

    public void OnPointerClick(PointerEventData eventData)
    {

        if (soundOnClick)
        {
           
            MySounder.instance.Click(useSet1Sounds, useSet2Sounds);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (soundOnHover)
            MySounder.instance.Hover(useSet1Sounds, useSet2Sounds);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(soundOnUnHover)
        MySounder.instance.Hover(useSet1Sounds, useSet2Sounds);
    }
}
