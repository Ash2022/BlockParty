using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeatureUnlockView : MonoBehaviour
{
    [SerializeField] CanvasGroup _unlocksBlackCover;
    [SerializeField] RectTransform _mainImageRect;
    [SerializeField] TMP_Text colorNameText;
    [SerializeField] Image unlockImage;

    public void ShowColorUnlocked(int colorIndex)
    {
        colorNameText.text = ModelManager.Instance.GetColorNameByIndex(colorIndex);
        colorNameText.color = ModelManager.Instance.GetColorByColorIndex(colorIndex);
        unlockImage.color = ModelManager.Instance.GetColorByColorIndex(colorIndex);
        _mainImageRect.localScale = Vector3.zero;
        _unlocksBlackCover.alpha = 0;
        _unlocksBlackCover.DOFade(1, 0.75f);

        SoundsController.Instance.PlayLobbyUnlockFeature();

        Vector3 scaleTo = Vector3.one;

        _mainImageRect.DOScale(scaleTo, 0.35f).SetEase(Ease.OutBounce).SetDelay(0.35f);
        gameObject.SetActive(true);
    }

    public void CloseFeatureUnlocked()
    {
        _unlocksBlackCover.DOFade(0, 0.35f).OnComplete(() =>
        {
            GameManager.Instance.FeatureUnlockClosed();
            gameObject.SetActive(false);
        });
    }
}
