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
    public void ChangeButtonState(bool isInteractable)
    {
        finishButton.interactable = isInteractable;
    }
    public void OpenFinishPanel()
    {
        finishPanel.SetActive(true);
    }
    private IEnumerator ButtonCloseAndOpen()
    {
        finishButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        finishButton.gameObject.SetActive(true);

    }
    public void ButtonControl()
    {
        StartCoroutine(ButtonCloseAndOpen());
    }
    public void CloseFinishPanel()
    {
        finishPanel.SetActive(false);
    }
}
