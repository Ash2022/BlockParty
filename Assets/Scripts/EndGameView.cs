using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class EndGameView : MonoBehaviour
{
    [SerializeField] Button endScreenButton;
    [SerializeField] Image bottomButtonImage;
    [SerializeField] Sprite continueSprite;
    [SerializeField] Sprite tryAgainSprite;
    [SerializeField] Sprite reviveSprite;
    [SerializeField] CanvasGroup mainCanvasGroup;
    [SerializeField] CanvasGroup winCanvasGroup;
    [SerializeField] CanvasGroup loseCanvasGroup;
    [SerializeField] GameObject winGroup;
    [SerializeField] GameObject loseGroup;
    [SerializeField] GameObject particles;
    [SerializeField] TMP_Text loseText;
    [SerializeField] TMP_Text winText;
    [SerializeField] TMP_Text scoreText;

    [SerializeField] GameObject reviveImage;

    [SerializeField] EndScreenUnlockView endScreenUnlockView;

    Coroutine endGameRoutine;
    private Action gameOverComplete;
    bool wonInLevel;
    bool noMoreUnlocks = false;
    int currLevelIndex;
    
    public void InitEndScreen(bool win,int levelIndex,int score, Action _gameOverComplete)
    {
        winText.text = "LEVEL " + (levelIndex + 1) + " COMPLETE";
        loseText.text = "LEVEL " + (levelIndex + 1) + "\nFAILED\n\nNO MORE MOVES";

        reviveImage.SetActive(false);

        
        currLevelIndex = levelIndex;
        gameOverComplete = _gameOverComplete;
        wonInLevel = win;
        mainCanvasGroup.alpha = 0;

        if (wonInLevel)
        {
            bottomButtonImage.sprite = continueSprite;
            bottomButtonImage.GetComponent<RectTransform>().localPosition = new Vector3(0, -800, 0);
        }
        else
        {
            //if(everLostInLevel == false)
            //{
                bottomButtonImage.sprite = reviveSprite;

                loseText.text = "ONE MORE CHANCE";

                reviveImage.SetActive(true);
            //}
            //else
            //{
            //    bottomButtonImage.sprite = tryAgainSprite;
            //}
                bottomButtonImage.GetComponent<RectTransform>().localPosition = new Vector3(0, -600, 0);
        }

        endScreenUnlockView.BackParticles.SetActive(false);
        endScreenUnlockView.CanvasGroup.alpha = 0;

        bottomButtonImage.gameObject.SetActive(false);
        endScreenButton.interactable = false;

        winGroup.SetActive(win);
        loseGroup.SetActive(!win);

        particles.SetActive(false);
        gameObject.SetActive(true);

        endGameRoutine = StartCoroutine(EndGameFlowAnimation(win, score));

    }

    IEnumerator EndGameFlowAnimation(bool win, int score)
    {
        mainCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.InOutSine).SetDelay(0.5f);
        yield return new WaitForSeconds(0.85f);

        if (win)
        {
            SoundsController.Instance.PlayLevelCompelte(true);

            winCanvasGroup.DOFade(1, 0.25f).SetDelay(0.25f);

            DOVirtual.Int(0, score, 1, (newValue) =>
            {
                scoreText.text = newValue.ToString("N0");
            });

            yield return new WaitForSeconds(0.5f);

            SoundsController.Instance.PlayConfettiPop();
            particles.SetActive(true);

            yield return new WaitForSeconds(1f);

            InitEndScreenUnlockView();

            yield return new WaitForSeconds(.5f);

            if (!noMoreUnlocks)
            {
                ProgressUnlockView();
                yield return new WaitForSeconds(.5f);
            }

            bottomButtonImage.sprite = continueSprite;

            bottomButtonImage.gameObject.SetActive(true);
            endScreenButton.interactable = true;

        }
        else
        {
            SoundsController.Instance.PlayLevelCompelte(false);
            loseCanvasGroup.DOFade(1, 0.25f).SetDelay(0.15f);
            yield return new WaitForSeconds(0.15f);

           // bottomButtonImage.sprite = tryAgainSprite;
            bottomButtonImage.gameObject.SetActive(true);
            endScreenButton.interactable = true;
        }


    }

    public void GameOverClicked()
    {
        bottomButtonImage.gameObject.SetActive(false);
        endScreenButton.interactable = false;


        //close game over
        //make it in active
        HideGameOver();

    }

    public void HideGameOver()
    {
        //in win - i want to fade the end screen to black - when its black show the lobby and fade out

        mainCanvasGroup.DOFade(0, 0.15f).SetDelay(0.25f).OnComplete(() =>
        {
            gameOverComplete?.Invoke();
            gameObject.SetActive(false);

        });

        endScreenUnlockView.HideEndScreenUnlocks();
    }


    private void ProgressUnlockView()
    {
        //need to see if need to update the model for powerUps 
        SoundsController.Instance.PlayProgressBar();
        endScreenUnlockView.UpdateProgress();
    }

    private void InitEndScreenUnlockView()
    {
        //init to show the current state

        //if win - need to increase the progress bar - need to update the text value

        //need to check the level i am on - then see all the unlocks - see where i am - set the total and current steps

        List<int> unlocksIndexList = ModelManager.Instance.UnlocksIndexList;

        //now i need to see where i am in this list

        int startIndex = 0;
        int endIndex = 0;
        int presentIndex = 0;

        for (int i = 0; i < unlocksIndexList.Count; i++)
        {
            if (currLevelIndex < unlocksIndexList[i] && endIndex == 0)
            {
                endIndex = unlocksIndexList[i];
                presentIndex = i;

                if (i > 0)
                    startIndex = unlocksIndexList[i - 1];

            }

        }

        //if end index == 0 -- no more unlocks to show

        if (endIndex == 0 || ModelManager.Instance.AllLevelsComplete())
        {
            noMoreUnlocks = true;
            endScreenUnlockView.HideEndScreenUnlocks();
        }
        else
        {

            int total = endIndex - startIndex;
            int curr = currLevelIndex - startIndex;

            Debug.Log("presentIndex " + presentIndex);

            endScreenUnlockView.InitDisplay(null, ModelManager.Instance.GetImageByColorIndex(presentIndex+3), total, curr);
        }


    }

}
