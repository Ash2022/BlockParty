using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShapeHolderView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    Action<bool> _mouseUpDown;
    [SerializeField]RectTransform _rectTransform;

    public RectTransform RectTransform { get => _rectTransform; set => _rectTransform = value; }

    public void InitShapeHolder(Action<bool> mouseUpDown)
    {
        _mouseUpDown = mouseUpDown;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _mouseUpDown?.Invoke(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _mouseUpDown?.Invoke(false);
    }

    private void OnMouseDown()
    {
        
    }

    private void OnMouseUp()
    {
        
    }


}
