using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] placementPoints;
    public GameObject[] trays;
    public GameObject[] numbers;
    private bool isCorrectSorting;
    private bool isCorrectNumber;
    private float duration = 1f;
    private float strength = 0.1f;
    private int mistakeSortCount = 0;
    private int mistakeNumberCount = 0;
    //public List<GameObject> placementTransformsList = new();

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
    }
    public void GameStart()
    {
        //placementPoints = GameObject.FindGameObjectsWithTag(Consts.Tags.DROP_POINT_TAG);
        trays = GameObject.FindGameObjectsWithTag(Consts.Tags.TRAY_TAG);
        numbers = GameObject.FindGameObjectsWithTag(Consts.Tags.NUMBER_TAG);
        SortPoints();
        SortTrays();
    }
    private void SortPoints()
    {
        Array.Sort(placementPoints, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
    }
    private void SortTrays()
    {
        Array.Sort(trays, (a, b) => a.GetComponent<Tray>().cakeAmount.CompareTo(b.GetComponent<Tray>().cakeAmount));
    }
    private void CheckSorting()
    {
        int maxMistakeCount = 4;
        List<GameObject> falseTrays = new List<GameObject>(); // ayný sayýda olanlarda bug olduðu için eklendi
        isCorrectSorting = true;
        for (int i = 0; i < trays.Length; i++)
        {
            if (trays[i].transform.parent != placementPoints[i].transform)
            {
                falseTrays.Add(trays[i]);
                //isCorrectSorting = false; // bir tanesi bile yanlýþ ise false olsun.
                //trays[i].transform.DOShakePosition(duration, strength);
            }
        }
        bool control = falseTrays.All(t => t.GetComponent<Tray>().cakeAmount == falseTrays[0].GetComponent<Tray>().cakeAmount);

        if (control) // hatalý olanlarýn kek sayýlarý eþit ise görmezden gelmesi için
        {
            isCorrectSorting = true;
        }
        else
        {
            for (int i = 0; i < trays.Length; i++)
            {
                if (trays[i].transform.parent != placementPoints[i].transform)
                {
                    trays[i].transform.DOShakePosition(duration, strength);
                }
            }
            isCorrectSorting = false;
        }
        if (!isCorrectSorting)
            mistakeSortCount++;
        if (mistakeSortCount == maxMistakeCount && !isCorrectSorting)
        {
            for (int i = 0; i < trays.Length; i++) // oyun sonu için
            {
                //trays[i].transform.position = placementPoints[i].transform.position;
                trays[i].transform.DOJump(placementPoints[i].transform.position, 2, 1, duration);
                //trays[i].transform.DOMove(placementPoints[i].transform.position,duration);
                StartCoroutine(SetParent(trays[i], placementPoints[i]));
            }
        }
    }
    private void CheckNumbers()
    {
        List<GameObject> emptyOrWrongTrays = new();
        int maxNumMistakeCount = 1;
        isCorrectNumber = true;
        foreach (var t in trays)
        {
            GameObject number = t.transform.GetComponentInChildren<Numbers>()?.gameObject;
            GameObject tray = t.gameObject;
            if (number != null)
            {
                if (number.GetComponent<Numbers>().number != tray.GetComponent<Tray>().cakeAmount)
                {
                    isCorrectNumber = false;
                    mistakeNumberCount++;
                    emptyOrWrongTrays.Add(t);
                }
                else
                    isCorrectNumber = true;
            }
            else
            {
                isCorrectNumber = false;
                emptyOrWrongTrays.Add(t);
            }
        }
        if (!isCorrectNumber)
        {
            mistakeNumberCount++;
            Debug.Log(emptyOrWrongTrays.Count);
        }
        if (mistakeNumberCount == maxNumMistakeCount)
        {
            float duration = 1f;
            float trayOffset = 1.2f;
            foreach (var t in emptyOrWrongTrays)
            {
                if (t.transform.childCount > 1) // sayý yanlýþ ise ilk önce onu yerine gönder
                {
                    var otherNum = t.GetComponentInChildren<Numbers>();

                    otherNum.transform.DOMove(otherNum.startPos, duration);
                    otherNum.transform.SetParent(otherNum.defParent);

                }
                GameObject numberObject = numbers.FirstOrDefault(n => n.GetComponent<Numbers>().number == t.GetComponent<Tray>().cakeAmount && n.transform.parent.tag != Consts.Tags.TRAY_TAG);
                Vector3 nextPoint = new Vector3(t.transform.position.x, t.transform.position.y, t.transform.position.z - trayOffset);
                // ayný kek sayýsýna sahip tepsi varsa oluþan bug
                numberObject.transform.DOMove(nextPoint, duration);
                numberObject.transform.parent = t.transform;
            }
        }
    }
    private IEnumerator SetParent(GameObject childObj, GameObject parentObj)
    {
        yield return new WaitForSeconds(duration);
        childObj.transform.SetParent(parentObj.transform);
    }
    public void CheckFinish() // numaralar 2.de geliyor, tepsiler 3de deðiþiyor. Eðer sýralama doðruysa sayýlar tekde geliyor
    {
        CheckSorting();
        CheckNumbers();
        if (isCorrectNumber && isCorrectSorting)
        {
            isCorrectNumber = true;
            isCorrectNumber = true;
            mistakeNumberCount = 0;
            mistakeSortCount = 0;
            StartCoroutine(LevelManager.Instance.FinishLevel());
        }
    }
}
