using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationIndiactionView : MonoBehaviour
{
    const float END_SCALE = 7.5f;

    [SerializeField] Image top;
    [SerializeField] Image bottom;
    [SerializeField] Image left;
    [SerializeField] Image right;

    [SerializeField] Image topLeft;
    [SerializeField] Image bottomLeft;
    [SerializeField] Image topRight;
    [SerializeField] Image bottomRight;

    [SerializeField] RectTransform rectTransform;
    [SerializeField]CanvasGroup canvasGroup;

    [SerializeField] Material indicationMaterial;
    [SerializeField] Material explodeMaterial;

    public void ShowIndication(int row,int col,List<Vector2Int> otherBlocks,int colorIndex)
    {

        ChangeLineScale(1f);

        rectTransform.localScale = Vector3.one;

        canvasGroup.alpha = 1;

        //canvasGroup.DOFade(1, 0.15f);

        if (row - 1 < 0 || !otherBlocks.Contains(new Vector2Int(row - 1, col)))
            top.enabled = true;

        if (row + 1 >= GameManager.Instance.GetCurrNumRows() || !otherBlocks.Contains(new Vector2Int(row + 1, col)))
            bottom.enabled = true;

        if (col - 1 < 0 || !otherBlocks.Contains(new Vector2Int(row, col-1)))
            left.enabled = true;

        if (col + 1 >= GameManager.Instance.GetCurrNumCols() || !otherBlocks.Contains(new Vector2Int(row, col + 1)))
            right.enabled = true;

        if(top.enabled && left.enabled)
            topLeft.enabled = true;

        if(top.enabled && right.enabled)
            topRight.enabled = true;

        if (bottom.enabled && left.enabled)
            bottomLeft.enabled = true;

        if (bottom.enabled && right.enabled)
            bottomRight.enabled = true;


        Color currColor = ModelManager.Instance.GetColorByColorIndex(colorIndex);

        top.color = currColor;
        bottom.color = currColor;
        left.color = currColor;
        right.color = currColor;
        topLeft.color = currColor;
        bottomRight.color = currColor;
        bottomLeft.color = currColor;
        topRight.color = currColor;

    }

    public void HideAllIndications()
    {
        top.enabled = false;
        bottom.enabled = false;
        left.enabled = false;
        right.enabled = false;
        topLeft.enabled = false;
        bottomLeft.enabled = false;
        bottomRight.enabled = false;
        topRight.enabled = false;
    }

    private void SetMaterial(Material material)
    {
       
            top.material = material;
            bottom.material = material;
            left.material = material;
            right.material = material;
            topRight.material = material;
            bottomLeft.material = material;
            bottomRight.material = material;
            topLeft.material = material;
       
    }


    private void ChangeLineScale(float Factor)
    {
        top.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, Factor, 1);
        bottom.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, Factor, 1);
        left.gameObject.GetComponent<RectTransform>().localScale = new Vector3(Factor, 1, 1);
        right.gameObject.GetComponent<RectTransform>().localScale = new Vector3(Factor, 1, 1);
        topLeft.gameObject.GetComponent<RectTransform>().localScale = Vector3.one * Factor;
        bottomLeft.gameObject.GetComponent<RectTransform>().localScale = Vector3.one * Factor;
        bottomRight.gameObject.GetComponent<RectTransform>().localScale = Vector3.one * Factor;
        topRight.gameObject.GetComponent<RectTransform>().localScale = Vector3.one * Factor;
    }

    public void ShowComplete()
    {
        ChangeLineScale(END_SCALE);

        //rectTransform.DOScale(Vector3.one * 1.05f, 0.35f);
        canvasGroup.DOFade(0, 0.35f);
    }

}
