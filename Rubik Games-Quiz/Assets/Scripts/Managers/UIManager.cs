using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Button finishButton;
    public void FinishButton()
    {
        GameManager.Instance.CheckFinish();
    }
    public void ChangeButtonState(bool interact)
    {
        finishButton.interactable = interact;
    }
}
