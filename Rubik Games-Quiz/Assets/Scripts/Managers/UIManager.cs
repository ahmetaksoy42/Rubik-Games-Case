using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button finishButton;
    [SerializeField] private GameObject finishPanel;
    public GameObject image;
    public void FinishButton()
    {
        GameManager.Instance.CheckFinish();
    }
    public void ChangeButtonState(bool interact)
    {
        finishButton.interactable = interact;
    }
    public void OpenFinishPanel()
    {
        finishPanel.SetActive(true);
    }
    public void CloseFinishPanel()
    {
        finishPanel.SetActive(false);
    }
}
