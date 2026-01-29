using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenUnlockView : MonoBehaviour
{
    [SerializeField]CanvasGroup canvasGroup;
    [SerializeField] Image fillBGImage;
    [SerializeField] Image fillImage;
    //[SerializeField] Image mainFeatureImage;
    [SerializeField] RectTransform mainImageRect;
    [SerializeField] GameObject backParticles;
    

    //[SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text bottomLevelsLeftText;

    int totalSteps = 0;
    int currentStep = 0;

    public CanvasGroup CanvasGroup { get => canvasGroup; set => canvasGroup = value; }
    public GameObject BackParticles { get => backParticles; set => backParticles = value; }

    public void InitDisplay(Sprite spriteBG,Sprite spriteFill, int totalSteps, int currentStep)
    {
        canvasGroup.DOFade(1, 0.35f).OnComplete(()=>
        {
            //backParticles.SetActive(true);
        });

        this.totalSteps = totalSteps;
        this.currentStep = currentStep;

        fillImage.sprite = spriteFill;

        fillImage.fillAmount = (float)currentStep /totalSteps;

        
        mainImageRect.localScale = Vector3.one;

        //titleText.text = title;
        SetProgessText();
    }

    private void SetProgessText()
    {
        int delta = totalSteps - currentStep;

        if (delta == 0)
            bottomLevelsLeftText.text = "UNLOCKS <color=#FEF123>NOW</color>";
        else if(delta == 1)
            bottomLevelsLeftText.text = "UNLOCKS IN <color=#FEF123>1</color> LEVEL";
        else
            bottomLevelsLeftText.text = "UNLOCKS IN <color=#FEF123>" + delta + "</color> LEVELS";
    }

    public void UpdateProgress()
    {
        float startValue = fillImage.fillAmount;

        currentStep++;

        float endValue = (float)currentStep / totalSteps;
        
        DOVirtual.Float(startValue, endValue, 1f, v => fillImage.fillAmount = v).OnComplete(()=>
        {
            if(currentStep == totalSteps)
                SoundsController.Instance.PlayLobbyUnlockFeature();

            SetProgessText();
        });

    }

    public void HideEndScreenUnlocks()
    {
        backParticles.SetActive(false);
        canvasGroup.DOFade(0, 0.35f);
    }




}
