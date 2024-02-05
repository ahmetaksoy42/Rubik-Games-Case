using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private RadialController radialController;
    private GameManager gameManager;
    private UIManager uiManager;
    public int Level { get; private set; } = 1;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        gameManager = GameManager.Instance;
        uiManager = FindObjectOfType<UIManager>();
        gameManager.placementPoints = GameObject.FindGameObjectsWithTag(Consts.Tags.DROP_POINT_TAG);
        foreach (var p in gameManager.placementPoints)
        {
            p.SetActive(false);
        }
    }
    public void RestartGame()
    {
        Level = 1;
        uiManager.CloseFinishPanel();
        foreach (var p in gameManager.placementPoints)
        {
            p.SetActive(false);
        }
        StartCoroutine(radialController.FillTheTrays(2));
    }
    public IEnumerator FinishLevel()
    {
        var trays = gameManager.trays;
        var numbers = gameManager.numbers;

        foreach (GameObject numberObj in numbers)
        {
            var number = numberObj.GetComponent<Numbers>();
            numberObj.transform.DOMove(number.startPos, 1);
            numberObj.transform.SetParent(number.defParent);
        }
        foreach (GameObject t in trays)
        {
            Destroy(t);
        }

        Array.Clear(trays, 0, trays.Length);
        uiManager.ChangeButtonState(false);
        yield return new WaitForSeconds(3);
        StartNextLevel();
    }
    private void StartNextLevel()
    {
        uiManager.image.SetActive(false);
        int trayCount;
        Level++;
        switch (Level)
        {
            case 1:
                trayCount = 2;
                StartCoroutine(radialController.FillTheTrays(trayCount));
               
                break;
            case 2:
                trayCount = 3;
                StartCoroutine(radialController.FillTheTrays(trayCount));
                
                break;
            case 3:
                trayCount = 4;
                StartCoroutine(radialController.FillTheTrays(trayCount));
                
                break;
            case 4:
                trayCount = 4;
                StartCoroutine(radialController.FillTheTrays(trayCount));

                break;
            case 5:
                trayCount = 4;
                StartCoroutine(radialController.FillTheTrays(trayCount));

                break;
            case 6:
                uiManager.OpenFinishPanel();
                break;
        }
    }
}
