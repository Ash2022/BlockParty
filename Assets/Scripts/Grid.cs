using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    const float IN_TIME = 0.15f;
    const float OUT_TIME = 0.5f;


    [SerializeField] CanvasGroup gridCanvas;
    public GameObject gridSquarePrefab;
    public Vector3 startPosition = Vector2.zero;
    public float squareScale = 100f;
    public float squareGap = 3.5f;

    private Vector2 _offset = Vector2.zero;

    private GridSquareView[,] _gridSquareViews;
    [SerializeField] Color gridDarkColor;
    [SerializeField] Color gridLightColor;
    [SerializeField]RectTransform _rectTransform;
    [SerializeField] RectTransform gridBG;
    [SerializeField] RectTransform gridBGFrame;

    [SerializeField] RectTransform gridBGFrameGlow;
    [SerializeField] CanvasGroup glowCanvasGroup;

    [SerializeField] Image glowImage;
    [SerializeField] Image BGImage;
    [SerializeField] Image BoardBGImage;
    [SerializeField] Image FrameImage;

    [SerializeField] Color bgNotLitColor;
    [SerializeField] Color boardNotBGLitColor;

    [SerializeField] Color bgLitColor;
    [SerializeField] Color boardBGLitColor;

    //[SerializeField]

    Sequence comboSequence;
    public GridSquareView[,] GridSquareViews { get => _gridSquareViews; set => _gridSquareViews = value; }
    public CanvasGroup GridCanvas { get => gridCanvas; set => gridCanvas = value; }


    public float GetSquareScale()
    {
        return squareScale;
    }

    public float GetSquareGap()
    {
        return squareGap;
    }

    public void ClearGrid()
    {
        foreach (GridSquareView gridSquareView in _gridSquareViews)
        {
            if(gridSquareView.IsFull)
                gridSquareView.ClearSquere();
        }
    }

  

    /// <summary>
    /// Creates a grid that fits within 1000×1000. 
    /// squareScale is computed so that neither width nor height exceeds 1000.
    /// gap = squareScale/30. 
    /// Returns half of the final board's total height.
    /// </summary>
    public float CreateGridFixed1000(int rows, int cols, List<List<int>> boardStartData)
    {
        GridCanvas.alpha = 0;
        transform.position = Vector3.zero;

        // Destroy previous squares
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // 1) Compute scale for rows => ensures the height <= 1000
        float scaleForRows = ComputeScaleForDimension(rows,925);
        // 2) Compute scale for cols => ensures the width <= 1000
        float scaleForCols = ComputeScaleForDimension(cols,925);
        // We take the smaller to ensure BOTH dimension <= 1000
        float finalScale = Mathf.Min(scaleForRows, scaleForCols);

        squareScale = finalScale;
        
        squareScale = Mathf.Min(140,squareScale);
        squareGap = squareScale / 30f;

        squareGap = 0f;

        // 3) Spawn squares
        _gridSquareViews = new GridSquareView[rows, cols];
        SpawnGridSquares(rows, cols, boardStartData);

        // 4) Compute the total board size 
        //    totalWidth  = cols*s + (cols-1)*(s/30)
        //    totalHeight = rows*s + (rows-1)*(s/30)
        float totalWidth = (cols * squareScale) + (cols - 1) * squareGap;
        float totalHeight = (rows * squareScale) + (rows - 1) * squareGap;

        // 5) Position the board so it's centered
        //_rectTransform.localPosition = new Vector2(-totalWidth / 2f, totalHeight / 2f);

        //_rectTransform.localPosition = new Vector2(-400, 250);

        float extra = 0;

        if (rows < cols)
            extra = squareScale / 4f;

        //_rectTransform.localPosition = new Vector2(-totalWidth / 2f + (squareScale/2f), totalHeight / 2f - (squareScale/2f)-140f - extra);
                
        float boardY = 0;

        if (rows == 3)
            boardY = -10;

        if (rows == 4)
            boardY = 30;

        if (rows == 5)
            boardY = 110;

        if (rows == 6)
            boardY = 175f;

        if (rows == 7)
            boardY = 208f;

        if(rows == 8)
            boardY = 220f;

        if (rows == 9)
            boardY = 235f;


        _rectTransform.localPosition = new Vector2(-totalWidth / 2f + (squareScale / 2f), boardY);

        gridBG.sizeDelta = new Vector2(totalWidth+20, totalHeight+20);
        gridBG.localPosition = new Vector2(0 , boardY - (rows/2)*squareScale);

        gridBGFrame.sizeDelta = new Vector2(totalWidth + 40, totalHeight + 40);
        gridBGFrame.localPosition = new Vector2(0, boardY - (rows / 2) * squareScale);

        gridBGFrameGlow.sizeDelta = new Vector2(totalWidth + 40, totalHeight + 40);
        gridBGFrameGlow.localPosition = new Vector2(0, boardY - (rows / 2) * squareScale);

        // Return half the total height
        return totalHeight / 2f;
        //325 --------   -700
    }

    //called from GameManger when there is a combo effect
    public void DoComboEffect(int colorIndex)
    {
        glowCanvasGroup.alpha = 0;
        //glowImage.color = ModelManager.Instance.GetColorByColorIndex(colorIndex);

        if (comboSequence != null)// && DGColorSequence.IsPlaying())
            comboSequence.Kill();

        comboSequence = DOTween.Sequence();
        //comboSequence.Append(Camera.main.gameObject.transform.DOShakePosition(105f, IN_TIME));
        comboSequence.Append(glowCanvasGroup.DOFade(1, IN_TIME));
        comboSequence.Join(BGImage.DOColor(bgLitColor, IN_TIME));
        comboSequence.Join(BoardBGImage.DOColor(boardBGLitColor, IN_TIME));
        comboSequence.Append(BoardBGImage.DOColor(boardNotBGLitColor, OUT_TIME));
        comboSequence.Join(BGImage.DOColor(bgNotLitColor, OUT_TIME));
        comboSequence.Join(glowCanvasGroup.DOFade(0, OUT_TIME));
        comboSequence.Play();

    }

    /// <summary>
    /// Spawns the squares using the computed squareScale and squareGap.
    /// Checkerboard color if desired.
    /// </summary>
    private void SpawnGridSquares(int rows, int cols, List<List<int>> boardStartData)
    {
        bool alternateColor = true;

        for (int row = 0; row < rows; row++)
        {
            // Flip row color if you want consistent checker pattern
            if (cols % 2 == 0)
                alternateColor = !alternateColor;

            for (int col = 0; col < cols; col++)
            {
                GameObject gridSquare = Instantiate(gridSquarePrefab, transform);
                GridSquareView view = gridSquare.GetComponent<GridSquareView>();
                _gridSquareViews[row, col] = view;

                // The position formula: 
                //  x = col * s + col * gap
                //  y = -row * s - row * gap
                Vector3 squarePos = startPosition + new Vector3(
                    col * squareScale + col * squareGap,
                    -(row * squareScale + row * squareGap),
                    0f
                );

                alternateColor = !alternateColor;
                Color color = alternateColor ? gridLightColor : gridDarkColor;

                int startValue = boardStartData[row][col];
                
                float localScale = squareScale / 100f;
                view.Init(row, col, squarePos, localScale, color, startValue);


                if(startValue != -1)
                    view.SetColorToFull(startValue);

            }
        }
    }

    /// <summary>
    /// Computes the scale needed so that 'count' squares (and count-1 gaps)
    /// do not exceed 1000 in that dimension:
    /// totalDim = count*s + (count-1)*(s/30) <= 1000
    /// Solve for s => s*(count + (count-1)/30) <= 1000
    /// => s <= 1000 / (count + ((count-1)/30))
    /// We'll pick s = that exact value so it "just fits."
    /// </summary>
    private float ComputeScaleForDimension(int count, float dim)
    {
        // totalDim = count*s + (count-1)*(s/30)
        //          = s*count + s*(count-1)/30
        //          = s*( count + (count-1)/30 )
        // we want that <= 1000 => s <= 1000 / [count + (count-1)/30].
        if (count < 1) count = 1; // safety
        float denominator = count + (count - 1) / 30f;
        return dim / denominator;
    }
}
