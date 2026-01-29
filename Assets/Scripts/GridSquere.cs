using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquere : MonoBehaviour
{

    public Image activeImage;
    public Image hoverImage;
    public Image normalImage;
    public List<Sprite> normalImages;
    public RectTransform squereRect;

    public bool Selected {  get; set; }
    public int SquereIndex { get; set; }
    public int SquereOccupied { get; set; }//changed from bool to index to add color



    private void Start()
    {
        Selected = false;
        SquereOccupied = -1;
    }

    public void PlaceShapeOnBoard(int shapeColorIndex)
    {
        ActivateSquere(shapeColorIndex);
    }

    public bool CanWeUseInThisSquere()
    {
        return hoverImage.gameObject.activeSelf;
    }

    public void ActivateSquere(int shapeColorIndex)
    {
        hoverImage.gameObject.SetActive(false);
        
        //set active image based on this color

        //activeImage.color = Grid.GetColorByColorIndex(shapeColorIndex);

        activeImage.gameObject.SetActive(true);
        Selected = true;
        SquereOccupied= shapeColorIndex;
    }

    public void DeActivateSquere()
    {
        activeImage.gameObject.SetActive(false);
    }

    public void ClearOccupied()
    {
        Selected= false;
        SquereOccupied = -1;
    }

    public void SetImage(bool setFirstImage)
    {
        normalImage.sprite = setFirstImage ? normalImages[1] : normalImages[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(SquereOccupied == -1)
        {
            hoverImage.gameObject.SetActive(true);
            Selected = true;
        }
        else if(collision.gameObject.GetComponent<ShapeSquere>() !=null)
        {
            collision.gameObject.GetComponent<ShapeSquere>().SetOccupied();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Selected = true;
        
        if (SquereOccupied == -1)
        {
            hoverImage.gameObject.SetActive(true);
        }
        else if (collision.gameObject.GetComponent<ShapeSquere>() != null)
        {
            collision.gameObject.GetComponent<ShapeSquere>().SetOccupied();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (SquereOccupied == -1)
        {
            hoverImage.gameObject.SetActive(false);
            Selected = false;
        }
        else if (collision.gameObject.GetComponent<ShapeSquere>() != null)
        {
            collision.gameObject.GetComponent<ShapeSquere>().UnSetOccupied();
        }
    }

 
}
