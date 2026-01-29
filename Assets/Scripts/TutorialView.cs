using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class TutorialView : MonoBehaviour
{
    [SerializeField] Button mainButton;
    [SerializeField] RectTransform draggingHand;
    [SerializeField]CanvasGroup canvasGroup;
    [SerializeField] Material _mat;
    [SerializeField] TMP_Text text;
    [SerializeField] RectTransform textRect;
    [SerializeField] GameObject hand;
    [SerializeField] GameObject clickToContinueText;

    float ScaleFactor = 1.0f;

    public enum TutorialPart
    {
        None,
        Part0,
        Part1,
        Part2,
        Part3,
        Part4,
        Part5,
        Part6,
        Part7,
        Part8
    }

    TutorialPart currTutorialPart = TutorialPart.None;

    Sequence tutorialSequence;

    public TutorialPart CurrTutorialPart { get => currTutorialPart; set => currTutorialPart = value; }

    public void ShowTutorialPart0()
    {
        canvasGroup.blocksRaycasts = false;
        mainButton.enabled = false;

        draggingHand.gameObject.SetActive(false);
        currTutorialPart = TutorialPart.Part0;

        text.text = "YOU NEED TO EMPTY\nTHE QUEUE";
        textRect.localPosition = new Vector3(0, 150f / ScaleFactor);
        canvasGroup.alpha = 0;

        SetHoleParameters(new Vector2(0.5f, 0.73f), 0.1f, 0.01f, new Color(0, 0, 0, 0.85f));

        gameObject.SetActive(true);

        canvasGroup.DOFade(1, 0.35f).OnComplete(()=>
        {
            clickToContinueText.SetActive(true);
            canvasGroup.blocksRaycasts = true;
            mainButton.enabled = true;
        });

    }

    public void ShowTutorialPart1()
    {
        clickToContinueText.SetActive(false);
        canvasGroup.blocksRaycasts = false;
        mainButton.enabled = false;

        currTutorialPart = TutorialPart.Part1;

        text.text = "DRAG A SHAPE\nTO THE BOARD";
        textRect.localPosition = new Vector3(0, 225f / ScaleFactor);
        canvasGroup.alpha = 0;

        SetHoleParameters(new Vector2(0.22f, 0.08f), 0.1f, 0.01f, new Color(0, 0, 0, 0.85f));

        gameObject.SetActive(true);

        canvasGroup.DOFade(1, 0.35f);

        tutorialSequence = DOTween.Sequence();

        draggingHand.transform.localPosition = new Vector3(-220, -835);

        tutorialSequence.Append(draggingHand.DOLocalMove(new Vector3(45, -180) , 2f).SetDelay(0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Restart));

        tutorialSequence.Play();

    }

    public void ShowTutorialPart2()
    {
        currTutorialPart = TutorialPart.Part2;

        mainButton.enabled = false;
        textRect.localPosition = new Vector3(0, -550f/ScaleFactor);
        text.text = "IF COLORS MATCH\nQUEUE COMPLETES THE LINE";
        canvasGroup.alpha = 0;

        SetHoleParameters(new Vector2(0.5f, 0.5f), 0.22f, 0.01f, new Color(0, 0, 0, 0.85f));

        gameObject.SetActive(true);

        canvasGroup.DOFade(1, 0.35f).OnComplete(()=>
        {
            clickToContinueText.SetActive(true);
            canvasGroup.blocksRaycasts = true;
            mainButton.enabled = true;
        });

    }

    internal async void ShowTutorialPart3()
    {
        clickToContinueText.SetActive(false);
        mainButton.enabled = false;

        currTutorialPart = TutorialPart.Part3;

        text.text = "";

        await Task.Delay(1750);

        text.text = "DRAG ANOTHER SHAPE\nTO EMPTY THE QUEUE";
        textRect.localPosition = new Vector3(0, -350f / ScaleFactor);

        SetHoleParameters(new Vector2(0.64f, 0.06f), 0.15f, 0.01f, new Color(0, 0, 0, 0.85f));
        canvasGroup.blocksRaycasts = false;
    }

    internal void ShowTutorialPart4()
    {
        currTutorialPart = TutorialPart.Part4;

        mainButton.enabled = false;
        textRect.localPosition = new Vector3(0, -550f / ScaleFactor);
        text.text = "QUEUE COMPLETES\nLONGEST POSSILBE LINE";
        canvasGroup.alpha = 0;

        SetHoleParameters(new Vector2(0.5f, 0.52f), 0.24f, 0.01f, new Color(0, 0, 0, 0.85f));

        gameObject.SetActive(true);

        canvasGroup.DOFade(1, 0.35f).OnComplete(() =>
        {
            clickToContinueText.SetActive(true);
            canvasGroup.blocksRaycasts = true;
            mainButton.enabled = true;
        });
    }

    internal async void ShowTutorialPart5()
    {
        clickToContinueText.SetActive(false);

        currTutorialPart = TutorialPart.Part5;

        text.text = "";

        await Task.Delay(1750);

        text.text = "WELL DONE\nYOU EMPTIED THE QUEUE";
        
        textRect.localPosition = new Vector3(0, -500f/ ScaleFactor);

        SetHoleParameters(new Vector2(0.5f, 0.72f), 0.15f, 0.01f, new Color(0, 0, 0, 0.85f));

        clickToContinueText.SetActive(true);

        canvasGroup.blocksRaycasts = true;
    }

    

    internal void ShowTutorialPart6()
    {
        gameObject.SetActive(false);
        currTutorialPart = TutorialPart.Part6;

        text.text = "";
        
    }

    internal void ShowTutorialPart7()
    {
        canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        draggingHand.gameObject.SetActive(false );

        canvasGroup.blocksRaycasts = false;
        mainButton.enabled = false;

        currTutorialPart = TutorialPart.Part7;

        text.text = "IF THE QUEUE CAN FILL A LINE\nIT COMPLETES AUTOMATICALLY";
        textRect.localPosition = new Vector3(0, -500f / ScaleFactor);

        SetHoleParameters(new Vector2(0.5f, 0.46f), 0.15f, 0.01f, new Color(0, 0, 0, 0.85f));

        canvasGroup.DOFade(1, 0.35f).OnComplete(() =>
        {
            clickToContinueText.SetActive(true);
            canvasGroup.blocksRaycasts = true;
            mainButton.enabled = true;
        });

    }

    internal async void ShowTutorialPart8()
    {
        clickToContinueText.SetActive(false);
        canvasGroup.blocksRaycasts = false;
        mainButton.enabled = false;

        currTutorialPart = TutorialPart.Part8;

        text.text = "";

        await Task.Delay(2500);

        text.text = "WELL DONE\nYOU EMPTIED THE QUEUE";
        clickToContinueText.SetActive(true);
        textRect.localPosition = new Vector3(0, -500f / ScaleFactor);

        SetHoleParameters(new Vector2(0.5f, 0.46f), 0.15f, 0.01f, new Color(0, 0, 0, 0.85f));
        mainButton.enabled = true;
    }

    public void TutorialBGClicked()
    {
        if(currTutorialPart == TutorialPart.Part0)
        {
            ShowTutorialPart1();
        }
        if(currTutorialPart == TutorialPart.Part2)
        {
            GameManager.Instance.ResumePlacingShape();
            ShowTutorialPart3();
            //CloseTutorial();
        }
        else if(currTutorialPart == TutorialPart.Part4)
        {
            GameManager.Instance.ResumePlacingShape();
            ShowTutorialPart5();
        }
        else if(currTutorialPart == TutorialPart.Part5)
        {
            //GameManager.Instance.ResumePlacingShape();
            //ShowTutorialPart6();
            currTutorialPart = TutorialPart.None;
            CloseTutorial();
            GameManager.Instance.TutorialComplete();
        }
        else if(currTutorialPart == TutorialPart.Part6)
        {
            currTutorialPart = TutorialPart.None;
            CloseTutorial();
            GameManager.Instance.TutorialComplete();
        }
        else if (currTutorialPart == TutorialPart.Part7)
        {
            GameManager.Instance.ResumePlacingShape();
            ShowTutorialPart5();
        }
        else if(currTutorialPart == TutorialPart.Part8)
        {
            currTutorialPart = TutorialPart.None;
            CloseTutorial();
            GameManager.Instance.TutorialComplete();
        }

    }

    public void CloseTutorial()
    {
        if (currTutorialPart == TutorialPart.Part1)
            currTutorialPart = TutorialPart.Part2;

        if (currTutorialPart == TutorialPart.Part3)
            currTutorialPart = TutorialPart.Part4;

        tutorialSequence.Kill();
        canvasGroup.DOFade(0, 0.15f).OnComplete(()=>
        {
            hand.SetActive(false);
            gameObject.SetActive(false);
        });
    }


    /// <summary>
    /// Sets the parameters on the material for the SmoothHole shader.
    /// </summary>
    /// <param name="center">Hole center in UV coordinates (0..1). E.g. (0.5, 0.5) for center.</param>
    /// <param name="radius">Radius of the hole in UV space.</param>
    /// <param name="smooth">Width of the smooth transition around the hole edge in UV space.</param>
    /// <param name="color">Overlay color (e.g. black with alpha=1).</param>
    public void SetHoleParameters(Vector2 center, float radius, float smooth, Color color)
    {
        float deltaY = center.y - 0.5f;

        // Scale that offset from center
        float newY = 0.5f + (deltaY * ScaleFactor);



        if (_mat == null) return;

        // Hole center in a Vector4, ignoring z/w
        _mat.SetVector("_HoleCenter", new Vector4(center.x, newY, 0, 0));
        _mat.SetFloat("_HoleRadius", radius);
        _mat.SetFloat("_Smooth", smooth);
        _mat.SetColor("_MainColor", color);
    }

    /// <summary>
    /// If using the NoDistortion version of the shader, 
    /// call this to set the aspect ratio (width/height) 
    /// so the circle doesn't become an ellipse.
    /// </summary>
    /// <param name="aspectRatio">Typically rect width / rect height.</param>
    public void SetAspectRatio(float aspectRatio)
    {
        if (_mat == null) return;
        _mat.SetFloat("_AspectRatio", aspectRatio);

        ScaleFactor = aspectRatio/ 0.5625f;
    }

    
}
