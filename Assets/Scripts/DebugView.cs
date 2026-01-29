using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugView : MonoBehaviour
{

    [SerializeField] Toggle _removeOnPeopleAssistToggle;
    [SerializeField] Toggle _removeBonusFromQueueToggle;
    [SerializeField] Toggle _queueOptimizationToggle;


    public void InitDebug()//, bool RemoveLineBonus, bool queueOptimization)
    {
        gameObject.SetActive(true);
    }

    public void CloseClicked()
    {
        gameObject.SetActive(false);
    }

    public void LevelUpDownClicked(bool up)
    {
        GameManager.Instance.CheatLevels(up);        
    }

    public void RestartClicked()
    {
        GameManager.Instance.RestartLevel();
        gameObject.SetActive(false);
    }

    public void OnToggleRemoveOnAssistValueChanged(bool toggle)
    {
        //GameManager.Instance.RemoveTouchingOnPeopleAssist = toggle;
        //GameManager.Instance.SetRemoveTouchingPeopleAssist(toggle);
    }

    public void OnToggleRemoveLineBonusValueChanged(bool toggle)
    {
        //GameManager.Instance.RemoveFromQueueBonus = toggle;
        //GameManager.Instance.SetRemoveQueueBonus(toggle);
    }

    public void OnToggleQueueOptimizationChanged(bool toggle)
    {
        //GameManager.Instance.RemoveFromQueueBonus = toggle;
        //GameManager.Instance.SetQueueOptimization(toggle);
    }
}
