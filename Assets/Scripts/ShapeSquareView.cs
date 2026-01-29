using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquareView : MonoBehaviour
{
    [SerializeField]BoxCollider2D boxCollider;
    [SerializeField] RectTransform rectTransform;
    [SerializeField]Image image;
    Vector2Int relativeIndexPosition;
    int colorIndex;
    /*
    Action _down;
    Action _up;
    Action _drag;
    */
    public Vector2Int BoardIndexPosition { get => relativeIndexPosition; set => relativeIndexPosition = value; }
    public int ColorIndex { get => colorIndex; set => colorIndex = value; }

    public void InitShapeSquare(Vector3 Position,Vector2Int _boardIndexPosition, int _colorIndex, float scale)//,Action upAction,Action downAction,Action dragAction)
    {
        colorIndex = _colorIndex;
        rectTransform.localPosition = Position;
        relativeIndexPosition = _boardIndexPosition;

        //image.color = ModelManager.Instance.GetColorByColorIndex(colorIndex);
       
        image.sprite = ModelManager.Instance.GetImageByColorIndex(colorIndex);

        rectTransform.sizeDelta = new Vector2(scale, scale);
        boxCollider.size = new Vector2((scale / 100) * 40f, (scale / 100) * 40f);

    }

    public void SetColliderSize(Vector2 newSize)
    {
        
    }

    internal void TutorialMode(int _colorIndex)
    {
        boxCollider.enabled = false;

        //Color modelColor = ModelManager.Instance.GetColorByColorIndex(_colorIndex);

        image.sprite = ModelManager.Instance.GetImageByColorIndex(_colorIndex);

        image.color = new Color(1, 1, 1, 0.5f);
    }
   
}
