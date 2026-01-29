using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GridSquareView : MonoBehaviour
{
    const float COLOR_FADE_TIME = 0.15f;

    public int row;
    public int col;
    [SerializeField]Image image;
    [SerializeField] Sprite emptySprite;
    [SerializeField] Sprite fullSprite;

    [SerializeField] Image indicationImage;
    [SerializeField]RectTransform rectTransform;
    [SerializeField] GameObject particleSystem;

    [SerializeField] SimulationIndiactionView simulateIndication;


    bool isFull;
    Color startColor;
    bool validAttempt = false;
    Coroutine explodeRoutine;
    int currColorIndex = 0;

    Color currentHighLightColor = Color.white;
    Color transWhite = Color.white;

    Sequence imageSequence;
    Sequence indicationImageSequence;

    public bool ValidAttempt { get => validAttempt; set => validAttempt = value; }
    public RectTransform RectTransform { get => rectTransform; set => rectTransform = value; }
    public SimulationIndiactionView SimulateIndication { get => simulateIndication; set => simulateIndication = value; }
    public bool IsFull { get => isFull; set => isFull = value; }

    internal void Init(int _row,int _col, Vector3 _squarePosition, float _squareScale, Color color, int colorIndex)
    {
        transWhite.a = 0;
        row = _row;
        col = _col;
        rectTransform.localPosition = _squarePosition;
        Vector3 gridSquareScale = new Vector3(_squareScale, _squareScale, _squareScale);

        transform.localScale = gridSquareScale;
        startColor = color;
        image.color = startColor;
        image.sprite = emptySprite;

        particleSystem.GetComponent<RectTransform>().localScale = gridSquareScale;
    }

    public void SquareStateChanged()
    {
        isFull = GameManager.Instance.GameBoard[row, col]!=-1;
    }

    public void SetIndicationToFull(int colorIndex)
    {
        isFull = true;
    }

    public void SetColorToFull(int colorIndex)
    {
        isFull = true;
        currColorIndex = colorIndex;
        image.sprite = ModelManager.Instance.GetImageByColorIndex(colorIndex);
        image.color = Color.white;
      

        // Debug.Log(color);
        if (imageSequence != null)// && DGColorSequence.IsPlaying())
            imageSequence.Kill();

        //image.color = ModelManager.Instance.GetColorByColorIndex(colorIndex);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        indicationImage.enabled = true;

        ShapeSquareView shapeSquareView = collision.gameObject.GetComponent<ShapeSquareView>();

        if (isFull)
        {

            //Debug.Log("Full");

            if(indicationImageSequence != null)// && DGColorSequence.IsPlaying())
                indicationImageSequence.Kill();

            indicationImageSequence = DOTween.Sequence();

            indicationImageSequence.Append(indicationImage.DOBlendableColor(ModelManager.Instance.fullColor, COLOR_FADE_TIME));

            indicationImageSequence.Play();

            validAttempt = false;
        }
        else
        {
            Color highlightColor = ModelManager.Instance.aviableColor;

            if(shapeSquareView != null)
            {
                highlightColor = ModelManager.Instance.GetColorByColorIndex(shapeSquareView.ColorIndex);

                Color.RGBToHSV(highlightColor, out float h, out float s, out float v);

                // Make the color lighter by increasing the value (v)
                v = Mathf.Clamp01(v + 0.5f); // Increase brightness by 20% (clamp to max 1)
                s = Mathf.Clamp01(s - 0.3f); // Increase brightness by 20% (clamp to max 1)

                // Convert back to RGB
                highlightColor = Color.HSVToRGB(h, s, v);

                highlightColor.a = 0.35f;

                currentHighLightColor= highlightColor;
            }

            if (indicationImageSequence != null)// && DGColorSequence.IsPlaying())
                indicationImageSequence.Kill();

            indicationImageSequence = DOTween.Sequence();

            indicationImageSequence.Append(indicationImage.DOBlendableColor(highlightColor,COLOR_FADE_TIME));

            indicationImageSequence.Play();

            validAttempt = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(indicationImage.enabled == false)
            indicationImage.enabled = true;

        if (isFull)
        {
            validAttempt = false;
        }
        else
        {
            validAttempt = true;
        }

        if(validAttempt)
            indicationImage.color = currentHighLightColor;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (indicationImageSequence != null)// && DGColorSequence.IsPlaying())
            indicationImageSequence.Kill();

        indicationImageSequence = DOTween.Sequence();
        indicationImageSequence.Append(indicationImage.DOBlendableColor(transWhite, COLOR_FADE_TIME));

        indicationImageSequence.Play();


        validAttempt = false;
    }

    internal void ClearSquere()
    {
        //image.sprite = emptySprite;

        if (imageSequence != null)// && DGColorSequence.IsPlaying())
            imageSequence.Kill();

        imageSequence = DOTween.Sequence();
        
        imageSequence.Join(image.DOFade(0, 0.25f)).OnComplete(() =>
        {
            image.sprite = emptySprite;
            image.color = startColor;
        });
        /*
        imageSequence.Join(image.DOBlendableColor(startColor, 0.25f)).OnComplete(()=>
        {
            image.sprite = emptySprite;
        });*/
        imageSequence.Play();
        
        //indicationImage.enabled = false;
        //image.DOColor(startColor,0.25f);



        SquareStateChanged();
    }

    //gets called on completeing a row or column
    public void ExplodeSquere(float delay)
    {
        if(explodeRoutine != null)
            StopCoroutine(explodeRoutine);

        explodeRoutine = StartCoroutine(ExplodeSquareSequence(delay));


    }

    IEnumerator ExplodeSquareSequence(float startDelay)
    {
        if (imageSequence != null)
            imageSequence.Kill();

        yield return new WaitForSeconds(startDelay);

        Taptic.Medium();

        SoundsController.Instance.PlayGridSquarePop();

        ParticleSystem.MainModule main = particleSystem.GetComponent<ParticleSystem>().main;

        Color baseColor = ModelManager.Instance.GetColorByColorIndex(currColorIndex);
        Color color2 = ModelManager.Instance.GetSecondaryParticleColor(currColorIndex);

        main.startColor = new ParticleSystem.MinMaxGradient(baseColor, color2); // random between 2 colors;
        particleSystem.SetActive(false);
        particleSystem.SetActive(true);

        if (imageSequence != null)
            imageSequence.Kill();

        imageSequence = DOTween.Sequence();

        imageSequence.Append(image.DOFade(0.25f, 0.2f));
        imageSequence.Play();

        //image.DOFade(0.5f, 0.5f);

        yield return new WaitForSeconds(0.2f);

        ClearSquere();

        yield return new WaitForSeconds(0.5f);

        particleSystem.SetActive(false);

    }

    private void OnDestroy()
    {
        if (imageSequence != null)
            imageSequence.Kill();
    }

}
