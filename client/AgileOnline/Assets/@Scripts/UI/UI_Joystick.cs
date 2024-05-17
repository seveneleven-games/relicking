using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_Joystick : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]
    Image _background;

    [SerializeField]
    Image _handler;

    float _joystickRadius;
    Vector2 _touchPosition;
    Vector2 _moveDir;

    void Start()
    {
        _joystickRadius = _background.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2;
        _background.gameObject.SetActive(false);
        _handler.gameObject.SetActive(false);
    }

    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        _background.gameObject.SetActive(true);
        _handler.gameObject.SetActive(true);
        
        _background.transform.position = eventData.position;
        _handler.transform.position = eventData.position;
        _touchPosition = eventData.position;
        
        Managers.Game.JoystickState = EJoystickState.PointerDown;
    }

    public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        _background.gameObject.SetActive(false);
        _handler.gameObject.SetActive(false);
        
        _moveDir = Vector2.zero;
        Managers.Game.MoveDir = _moveDir;
        Managers.Game.JoystickState = EJoystickState.PointerUp;
    }

    public void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        Vector2 touchDir = (eventData.position - _touchPosition);

        float moveDist = Mathf.Min(touchDir.magnitude, _joystickRadius);
        _moveDir = touchDir.normalized;
        Vector2 newPosition = _touchPosition + _moveDir * moveDist;
        _handler.transform.position = newPosition;

        Managers.Game.MoveDir = _moveDir;
        Managers.Game.JoystickState = EJoystickState.Drag;
    }
    
    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData) {}
}