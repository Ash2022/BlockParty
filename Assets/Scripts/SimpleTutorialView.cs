using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TutorialView;
using UnityEngine.XR;
using System;

public class SimpleTutorialView : MonoBehaviour
{
    [SerializeField] RectTransform draggingHand;
    Sequence tutorialSequence;

    GameObject currentDragShape = null;

    public void ShowTutorial(GameObject currShape)
    {
        KillObjectAndStopAnimation();

        currentDragShape = currShape;

        currShape.transform.SetParent(draggingHand.transform);

        RectTransform shapeRect = currShape.GetComponent<RectTransform>();

        shapeRect.localPosition = new Vector2(-70,100);
        shapeRect.localScale = Vector3.one;

        gameObject.SetActive(true);

        tutorialSequence = DOTween.Sequence();

        draggingHand.transform.localPosition = new Vector3(82, -900);

        tutorialSequence.Append(draggingHand.DOLocalMove(new Vector3(80, -270), 2f).SetDelay(0.5f).SetEase(Ease.InOutSine).SetLoops(50, LoopType.Restart));

        tutorialSequence.Play();
    }


    public void CloseTutorial()
    {       
        gameObject.SetActive(false);
        KillObjectAndStopAnimation();
    }

    private void KillObjectAndStopAnimation()
    {
        if(tutorialSequence != null)
            tutorialSequence.Kill();

        if(currentDragShape != null)
            Destroy(currentDragShape);

    }

   

    //generates a mock suggestion for play
    internal void GenerateSuggestion(Vector2 center, ShapeView shapeView)
    {
        KillObjectAndStopAnimation();

        currentDragShape = Instantiate(shapeView.gameObject);

        currentDragShape.transform.SetParent(draggingHand.transform);

        currentDragShape.GetComponent<ShapeView>().TutorialMode(shapeView.ColorIndex);

        RectTransform shapeRect = currentDragShape.GetComponent<RectTransform>();

        shapeRect.localPosition = new Vector2(-70, 100);        
        shapeRect.localScale = Vector3.one;
        
        gameObject.SetActive(true);

        tutorialSequence = DOTween.Sequence();

        //draggingHand.transform.position = center;

        Vector2 extraOffset = new Vector2(0.07f, -0.1f) * 5;

        Vector2 center3 = center + extraOffset;

        draggingHand.transform.position = shapeView.ShapeRectTransform.position+ (Vector3)extraOffset;

        //draggingHand.transform.localPosition += new Vector3(70, -100, 0);
        

        tutorialSequence.Append(draggingHand.DOMove(center3, 2f).SetDelay(0.5f).SetEase(Ease.InOutSine).SetLoops(50, LoopType.Restart));

        tutorialSequence.Play();

    }
}
