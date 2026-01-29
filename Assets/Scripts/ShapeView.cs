using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ShapesDB;
using static TutorialView;


public class ShapeView : MonoBehaviour
{
    const float STORAGE_SIZE = 0.5f;
    [SerializeField] GameObject shapeSingleSquare;
    [SerializeField]RectTransform shapeRectTransform;
    List<ShapeSquareView> shapeSquares = new List<ShapeSquareView>();
    int colorIndex;
    Vector2 startPosition;
    bool dragging = false;
    Vector2 startMousePosition= Vector2.zero;
    Vector3 dragOffset = Vector3.zero;

    float storageScale;

    public RectTransform ShapeRectTransform { get => shapeRectTransform; set => shapeRectTransform = value; }
    public int ColorIndex { get => colorIndex; set => colorIndex = value; }

    public void BuildShapeFromDefinition(ShapeDefinition shapeDefinition, int _colorIndex, float _storageScale)
    {
        storageScale = _storageScale;
        colorIndex = _colorIndex;
        gameObject.name = shapeDefinition.Name;

        int minRow = int.MaxValue;
        int maxRow = int.MinValue;
        int minCol = int.MaxValue;
        int maxCol = int.MinValue;

        float squareScale = GameManager.Instance.GameGrid.GetSquareScale();
        float squareGap = GameManager.Instance.GameGrid.GetSquareGap();

        // Calculate bounds (min and max rows/columns)
        foreach (var cell in shapeDefinition.Cells)
        {
            minRow = Mathf.Min(minRow, cell.x);
            maxRow = Mathf.Max(maxRow, cell.x);
            minCol = Mathf.Min(minCol, cell.y);
            maxCol = Mathf.Max(maxCol, cell.y);
        }

        // Calculate the center offset of the shape
        float centerX = (maxCol - minCol + 1) * (squareScale + squareGap) / 2f - (squareScale + squareGap) / 2f;
        float centerY = (maxRow - minRow + 1) * (squareScale + squareGap) / 2f - (squareScale + squareGap) / 2f;

        // Instantiate squares based on offsets
        foreach (var cell in shapeDefinition.Cells)
        {
            int adjustedRow = cell.x - minRow;
            int adjustedCol = cell.y - minCol;

            GameObject shapeSquare = Instantiate(shapeSingleSquare, transform);
            ShapeSquareView shapeSquareView = shapeSquare.GetComponent<ShapeSquareView>();
            shapeSquares.Add(shapeSquareView);

            Vector3 pos = new Vector3(
                adjustedCol * (squareScale + squareGap) - centerX,
                -adjustedRow * (squareScale + squareGap) + centerY,
                0
            );

            Vector2Int relativePosition = new Vector2Int(adjustedRow, adjustedCol);
            shapeSquareView.InitShapeSquare(pos, relativePosition, colorIndex, squareScale);
        }

        // Adjust RectTransform for proper scaling and alignment
        shapeRectTransform.localScale = new Vector3(storageScale, storageScale, storageScale);
        shapeRectTransform.sizeDelta = new Vector2(squareScale, squareScale);
        shapeRectTransform.localPosition = Vector2.zero;

        startPosition = shapeRectTransform.localPosition;
    }


    public void MouseDownUp(bool down)
    {
        if (GameManager.Instance.lockDrag)
            return;

        //if(GameManager.Instance.GameState == GameManager.GameStates.ShowingTutorial)
            GameManager.Instance.HideTutorialOrSuggestions();


        if (down)
        {
            if (shapeRectTransform == null)
                return;

            dragOffset = Vector3.zero;
            shapeRectTransform.localPosition = startPosition;
            dragOffset = Camera.main.WorldToScreenPoint(dragOffset)- Input.mousePosition;
            shapeRectTransform.DOScale(1f,0.15f).SetEase(Ease.OutSine);

        }
        else
        {
            //check if placed on the board or not

            //check if shape can be positioned
            if (GameManager.Instance.GetNumValidAttemptsToPosition() == shapeSquares.Count)
                GameManager.Instance.PositionCurrentShape(this, colorIndex);
            else
            {
                shapeRectTransform.DOScale(storageScale, 0.1f).SetEase(Ease.InSine);
                shapeRectTransform.DOLocalMove(startPosition,0.1f).SetEase(Ease.InSine);

                GameManager.Instance.StartSuggestedMoveTimer();
            }
        }
        dragging = down;
    }

    

    private void Update()
    {
        if(dragging)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.Instance.CanvasRect, Input.mousePosition+ dragOffset, Camera.main, out pos);

            shapeRectTransform.localPosition = new Vector2(pos.x, pos.y + 250);

            if (GameManager.Instance.GetNumValidAttemptsToPosition() == shapeSquares.Count)
            {
                GameManager.Instance.SimulatePlacingShape(GameManager.Instance.GetValidAttemptedPositions(), colorIndex);

            }
            else
                GameManager.Instance.ClearSimulationIndications();

        }

    }


    internal List<Vector2Int> GetShapeRelativeGridPositions()
    {
        List<Vector2Int> relativePosition = new List<Vector2Int>();

        foreach (ShapeSquareView shapeSquareView in shapeSquares)
            relativePosition.Add(shapeSquareView.BoardIndexPosition);
        
        return relativePosition;
    }

    internal void TutorialMode(int colorIndex)
    {
        foreach (Transform shapeSquareTransform in transform)
        {
            ShapeSquareView shapeSquareView = shapeSquareTransform.gameObject.GetComponent<ShapeSquareView>();

            shapeSquareView.TutorialMode(colorIndex);
        }
    }
}
