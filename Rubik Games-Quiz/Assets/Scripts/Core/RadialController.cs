using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialController : MonoBehaviour
{
    [SerializeField] private List<Transform> trayPoints = new();
    [SerializeField] private List<GameObject> trays = new();
    [SerializeField] private List<GameObject> emptyTrays = new();
    private GameManager gameManager;
    private Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
        gameManager = GameManager.Instance;
        StartCoroutine(FillTheTrays(2));
    }
    public IEnumerator FillTheTrays(int trayCount)
    {
        float duration = 1f;
        float stepDuration = 2f;
        float yDistanceToBench = 2f;
        for (int i = 0; i < trayCount; i++)
        {
            emptyTrays[i].SetActive(true);
        }
        for (int i = 0; i < trayCount; i++)
        {
            int stepCount = 1; // LoopType kullanabilmek için
            transform.DOMoveX(trayPoints[i].position.x, duration);
            yield return new WaitForSeconds(duration);
            transform.DOMoveY(transform.position.y - yDistanceToBench, duration).SetLoops(2, LoopType.Yoyo).OnStepComplete(() =>
            {
                if (stepCount == 1)
                {
                    emptyTrays[i].SetActive(false);
                    GameObject tray = Instantiate(trays[Random.Range(0, trays.Count)], trayPoints[i]);
                    tray.transform.position = trayPoints[i].position;
                    stepCount++;
                }
            });
            yield return new WaitForSeconds(stepDuration);
        }
        transform.DOMove(startPos, stepDuration);
        GameManager.Instance.GameStart();
        for (int i = 0; i < trayCount; i++)
        {
            gameManager.placementPoints[i].SetActive(true);
        }
    }
}
