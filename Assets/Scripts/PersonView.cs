using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PersonView : MonoBehaviour
{
    const int FRAMES_CHANGE_IMAGE = 6;

    public Image personImage;
    int personColorIndex = 0;
    [SerializeField]RectTransform personRectTransform;
    [SerializeField] CanvasGroup personCanvasGroup;
    [SerializeField] Image highLightImage;
    Sequence personSequence;

    bool moving = false;
    Vector3 oldPosition;
    Vector3 headingDirection = new Vector3();

    int animationCounter = 0;
    int currAnimationIndex = 0;
    List<Sprite> animationSprites;

    
    public RectTransform PersonRectTransform { get => personRectTransform; set => personRectTransform = value; }
    public CanvasGroup PersonCanvasGroup { get => personCanvasGroup; set => personCanvasGroup = value; }
    public int PersonColorIndex { get => personColorIndex; set => personColorIndex = value; }

    internal void InitPerson(int personColorInd, Vector2 position, float scale)
    {
        personCanvasGroup.alpha = 0;

        personColorIndex = personColorInd;
        personRectTransform.anchoredPosition = position;
        PersonRectTransform.localScale = new Vector3(scale / 100f, scale / 100f, scale / 100f);
        //personImage.color = ModelManager.Instance.GetColorByColorIndex(personColorInd);

        animationSprites = ModelManager.Instance.GetPersonImagesByColorIndex(personColorIndex);
        personImage.sprite = animationSprites[1];
        currAnimationIndex = 0;
        oldPosition = position;
    }

    public void ShowPerson()
    {
        personCanvasGroup.DOFade(1, 0.25f);

    }

    private void OnDestroy()
    {
        if (personSequence != null)
            personSequence.Kill();
    }

    private void Update()
    {
        if (moving)
        {
            // << then you change the cube parent's position here
            
            headingDirection = (transform.position - oldPosition).normalized;
            oldPosition = transform.position;

            personRectTransform.localRotation = Quaternion.FromToRotation(Vector3.down, headingDirection);

            animationCounter++;

            if(animationCounter == FRAMES_CHANGE_IMAGE)
            {
                animationCounter = 0;
                currAnimationIndex++;

                if (currAnimationIndex >= animationSprites.Count)
                    currAnimationIndex = 0;

                personImage.sprite = animationSprites[currAnimationIndex];
            }
        }
    }

    public void PersonMove(Vector3 targetPosition,Vector3 scale, float duration, Action done)
    {
        //Debug.Log("Move");

        if(personSequence != null)
            personSequence.Kill();


        personSequence = DOTween.Sequence();

        //personSequence.Append(personRectTransform.DOScale(scale, duration).SetEase(Ease.OutCirc));

        personSequence.Append(personRectTransform.DOMove(targetPosition,duration).SetEase(Ease.Linear).OnComplete(()=>
        {
            moving = false;
            personImage.sprite = animationSprites[1];
            done?.Invoke();
        }));

        moving = true;
        personSequence.Play();
    }

    public void PersonMoveAnchored(Vector3 targetPosition, float duration, Action done)
    {
        //Debug.Log("MoveAnchor");

        if (personSequence != null)
            personSequence.Kill();


        personSequence = DOTween.Sequence();

        personSequence.Append(personRectTransform.DOAnchorPos(targetPosition, duration).OnComplete(() =>
        {
            moving = false;
            personImage.sprite = animationSprites[1];
            done?.Invoke();
        }));
        moving = true;
        personSequence.Play();
    }

    public void PersonMoveAnchoredY(float targetY, float duration, Action done)
    {
        //Debug.Log("MoveAnchorY");

        if (personSequence != null)
            personSequence.Kill();


        personSequence = DOTween.Sequence();

        personSequence.Append(personRectTransform.DOAnchorPosY(targetY, duration).SetEase(Ease.OutSine).OnComplete(() =>
        {
            //moving = false;
            personImage.sprite = animationSprites[1];
            done?.Invoke();
        }));
        //moving = true;
        personSequence.Play();
    }

    internal void ShowHidePersonHighLight(bool show,int colorIndex)
    {
        if(show)
            highLightImage.color = ModelManager.Instance.GetColorByColorIndex(colorIndex);

        highLightImage.enabled= show;
    }
}
