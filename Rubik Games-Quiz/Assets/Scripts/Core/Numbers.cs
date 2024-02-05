using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

public class Numbers : MonoBehaviour, IDraggable
{
    public int number;
    public Transform defParent;
    public Vector3 startPos;
    private Vector3 nextPoint;
    private Vector3 offset;
    private Vector3 inTrayPos;
    private Transform nextTray;
    private float minY = 5f;
    private float maxY = 10f;
    private float minZ = 7.5f;
    private float maxZ = 10f;
    private void Start()
    {
        startPos = transform.position;
        defParent = transform.parent;
    }
    public void OnMouseDown()
    {
        offset = transform.position - MouseWorldPos();
    }
    public void OnMouseDrag()
    {
        transform.position = new Vector3(MouseWorldPos().x + offset.x,Mathf.Clamp((MouseWorldPos().y + offset.y),minY,maxY), Mathf.Clamp((MouseWorldPos().z + offset.z), minZ, maxZ)); // y eksenini sýnýrla
    }
    public void OnMouseUp()
    {
        StartCoroutine(GoToTray());
    }
    public Vector3 MouseWorldPos()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }
    private void FindClosestPoint()
    {
        float minDistance = Mathf.Infinity;
        float trayOffset = 1.2f;
        Transform nearestPoint = null;

        foreach (var point in GameManager.Instance.trays)
        {
            float distance = Vector3.Distance(transform.position, point.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPoint = point.transform;
            }
        }

        nextPoint = new Vector3(nearestPoint.position.x, nearestPoint.position.y, nearestPoint.position.z - trayOffset);
        nextTray = nearestPoint;
    }
    public IEnumerator GoToTray()
    {
        float duration = 1f;
        FindClosestPoint();
        float distanceToStart = Vector3.Distance(transform.position, startPos);
        float distanceToTray = Vector3.Distance(transform.position, nextPoint);
        if (distanceToStart <= distanceToTray)
        {
            transform.DOMove(startPos, duration);
            yield return new WaitForSeconds(duration);

            transform.SetParent(defParent);
        }
        else
        {
            Transform currentTray = transform.parent;

            if (nextTray.childCount > 1) // Kekler olduðu için
            {
                var otherNum = nextTray.GetComponentInChildren<Numbers>().transform;
                if (currentTray.CompareTag(Consts.Tags.TRAY_TAG)) // eðer baþka bir tepsideyse yer deðiþtirsinler
                {
                    otherNum.DOMove(inTrayPos, duration);
                    transform.DOMove(nextPoint, duration);
                    yield return new WaitForSeconds(duration);
                    otherNum.SetParent(currentTray);
                    transform.SetParent(nextTray);
                }
                else // deðilse baþlangýç noktasýna dönsün
                {
                    otherNum.DOMove(otherNum.gameObject.GetComponent<Numbers>().startPos, duration);
                    transform.DOMove(nextPoint, duration);
                    yield return new WaitForSeconds(duration);
                    otherNum.SetParent(defParent);
                    transform.SetParent(nextTray);
                }
            }
            else
            {
                transform.DOMove(nextPoint, duration);
                yield return new WaitForSeconds(duration);
                transform.SetParent(nextTray);
            }
        }
        transform.GetComponent<Collider>().enabled = true;
        inTrayPos = transform.position; // bir tray e girdiyse pozisyonu switch için kaydet
    }
}
