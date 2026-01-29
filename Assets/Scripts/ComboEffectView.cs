using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboEffectView : MonoBehaviour
{
    public enum ComboTypes
    {
        ClearGroup,
        ComboIncreased,
        ClearBoard
    }

    [SerializeField] TMP_Text comboText;
    [SerializeField]CanvasGroup canvasGroup;
    [SerializeField] RectTransform rect;

    [SerializeField] GameObject clearBlocksGroup;
    [SerializeField] TMP_Text clearGroupComboText;
    [SerializeField] RectTransform clearGroupTextRect;
    [SerializeField]Image clearGroupComboImage;
    [SerializeField] RectTransform clearGroupComboImageRect;

    [SerializeField] GameObject comboIncreaseGroup;
    [SerializeField] TMP_Text comboIncreaseText;
    [SerializeField] RectTransform comboIncreaseRect;
    [SerializeField] TMP_Text comboIncreaseXText;

    [SerializeField] GameObject clearBoardGroup;
    [SerializeField] RectTransform comboClearBoardImageRect;


    public void InitComboEffect(ComboTypes comboType, int amountOfCleared,int currentComboMulti, Vector3 position, Color color)
    {
        rect.position = position;
        canvasGroup.alpha = 0;
        comboText.fontSize = 50 + (currentComboMulti * 5);

        comboText.color = color;

        if (comboType == ComboTypes.ClearGroup)
        {
            Sprite comboImage = ModelManager.Instance.GetComboImageByIndex(GetWordIndex(amountOfCleared));

            clearGroupComboImage.sprite = comboImage;
            clearGroupComboImage.SetNativeSize();

            comboIncreaseGroup.SetActive(false);
            clearBoardGroup.SetActive(false);
            clearBlocksGroup.SetActive(true);

            //int fontSize = 55 + amountCombo;

            //int numberFontSize = Convert.ToInt32(fontSize * 1.35f);

            //comboText.fontSize = 55 + amountCombo;
            clearGroupComboText.color = new Color(color.r,color.g,color.b,0);
            clearGroupComboText.text = "x"+amountOfCleared.ToString();

            if (rect == null)
            {
                Destroy(gameObject);
                return;
            }

            clearGroupComboImageRect.localScale = Vector3.one * 0.4f;
            clearGroupTextRect.localScale = Vector3.one * 0.5f;
            rect.localScale = Vector3.one * .75f;
            rect.DOLocalMoveY(rect.localPosition.y + 80, 1.15f).SetEase(Ease.OutSine);
            //rect.DOScale(Vector3.one * .75f, 1.15f).SetEase(Ease.InOutSine);

            clearGroupComboImageRect.DOScale(Vector3.one * 1.1f, 1.15f);

            clearGroupComboText.DOFade(1, 0.25f).SetDelay(0.15f);

            float scaleFactor = 1.25f + (0.04f * amountOfCleared);

            clearGroupTextRect.DOScale(Vector3.one * scaleFactor, 1.15f).SetEase(Ease.InOutSine);
            //clearGroupTextRect.DOLocalMoveY(clearGroupTextRect.localPosition.y + 100, 1.15f).SetEase(Ease.InOutSine);


            canvasGroup.DOFade(1, 0.25f);
            canvasGroup.DOFade(0, .25f).SetDelay(.95f).OnComplete(() =>
            {

                Destroy(gameObject);
            });
        }
        else if (comboType == ComboTypes.ComboIncreased)
        {
            rect.position = new Vector3(0, 1f, 0);

            comboIncreaseGroup.SetActive(true);
            clearBoardGroup.SetActive(false);
            clearBlocksGroup.SetActive(false);

            comboIncreaseText.color = color;
            comboIncreaseXText.color = color;

            comboIncreaseText.text = currentComboMulti.ToString();

            if (rect == null)
            {
                Destroy(gameObject);
                return;
            }

            rect.DOMoveY(1.5f, 1.95f).SetEase(Ease.OutSine);
            rect.DOScale(Vector3.one * 1.15f, 0.5f).SetEase(Ease.InOutSine);

            comboIncreaseRect.DOScale(Vector3.one * 1.35f, 0.5f).SetEase(Ease.InOutSine);

            canvasGroup.DOFade(1, 0.25f);
            canvasGroup.DOFade(0, 0.5f).SetDelay(1.5f).OnComplete(() =>
            {

                Destroy(gameObject);
            });
        }
        else if (comboType == ComboTypes.ClearBoard)
        {
            rect.position = new Vector3(0,1.25f, 0);

            comboIncreaseGroup.SetActive(false);
            clearBoardGroup.SetActive(true);
            clearBlocksGroup.SetActive(false);

            rect.DOMoveY(2.25f, 1.95f).SetEase(Ease.OutSine);
            rect.DOScale(Vector3.one * 1.25f, 0.5f).SetEase(Ease.InOutSine);

            comboClearBoardImageRect.DOScale(Vector3.one * 1.25f, 0.5f).SetEase(Ease.InOutSine);

            canvasGroup.DOFade(1, 0.25f);
            canvasGroup.DOFade(0, 0.5f).SetDelay(1.5f).OnComplete(() =>
            {

                Destroy(gameObject);
            });
        }

    }

    public int GetWordIndex(int currentCombo)
    {
        if (currentCombo < 9)
        {
            return 0; // Below 3, return -1
        }

        if (currentCombo > 41)
        {
            return 12; // Cosmic index
        }

        // Define thresholds and return index based on the range
        if (currentCombo > 39) return 11; // Universal
        if (currentCombo > 37) return 10; // Infinite
        if (currentCombo > 35) return 9;  // Godlike
        if (currentCombo > 33) return 8;  // Legendary
        if (currentCombo > 30) return 7;  // Epic
        if (currentCombo > 27) return 6;  // Enormous
        if (currentCombo > 24) return 5;  // Colossal
        if (currentCombo > 21) return 4;  // Gigantic
        if (currentCombo > 18) return 3;  // Massive
        if (currentCombo > 15) return 2;   // Huge
        if (currentCombo > 12) return 1;   // Large
        if (currentCombo >= 9) return 0;   // Big

        return 0; // Default case, should never be reached
    }

}
