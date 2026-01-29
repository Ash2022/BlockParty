using System;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquere : MonoBehaviour
{
    public Image OccupiedImage;
    public Image normalImage;

    private void Start()
    {
        OccupiedImage.gameObject.SetActive(false);    
    }

    public void ActivateShape()
    {
        gameObject.GetComponent<Collider2D>().enabled = true;
        gameObject.SetActive(true);
    }

    public void DeActivateShape()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        gameObject.SetActive(false);
    }

    public void SetOccupied()
    {
        OccupiedImage.gameObject.gameObject.SetActive(true);
    }

    public void UnSetOccupied()
    {
        OccupiedImage.gameObject.gameObject.SetActive(false);
    }

    internal void SetColorIndex(int shapeColorIndex)
    {
        //normalImage.color = Grid.GetColorByColorIndex(shapeColorIndex);
    }
}
