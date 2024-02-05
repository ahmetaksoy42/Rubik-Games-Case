using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tray : MonoBehaviour, IDraggable
{
    [SerializeField] private UIManager uiManager;
    private Vector3 offset;
    public int cakeAmount;
    private float minX = -6f;
    private float maxX = 12f;
    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }
    public void OnMouseDown()
    {
        offset = transform.position - MouseWorldPos();

        transform.GetComponent<Collider>().enabled = false;
    }
    public void OnMouseDrag()
    {
        var yPos = transform.position.y;
        var zPos = transform.position.z;
        transform.position = new Vector3(Mathf.Clamp((MouseWorldPos().x + offset.x), minX, maxX),yPos,zPos);
    }
    public void OnMouseUp()
    {
        transform.GetComponent<Collider>().enabled = true;
        PlaceObject();
    }

    public Vector3 MouseWorldPos()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }
    private Transform FindClosestPoint()
    {
        float minDistance = Mathf.Infinity;
        Transform nearestPoint = null;

        foreach (var point in GameManager.Instance.placementPoints)
        {
            if (!point.activeSelf)
                continue;
            float distance = Vector3.Distance(transform.position, point.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPoint = point.transform;
            }
        }
        return nearestPoint;
    }
    private void PlaceObject()
    {
        float duration = 1f;
        var nearestPoint = FindClosestPoint();
        if (nearestPoint != null)
        {
            Transform currentParent = transform.parent;
            if (nearestPoint.transform.childCount > 0) //Yer deðiþtirme
            {
                Transform otherTepsi = nearestPoint.GetChild(0).transform;
                transform.SetParent(nearestPoint);
                transform.DOMove(nearestPoint.position, duration);
                if (currentParent == null)
                {
                    float newMinDistance = Mathf.Infinity;
                    Transform newNearestPoint = null;
                    foreach (var point in GameManager.Instance.placementPoints)
                    {
                        if (point.transform.childCount > 0 && point.transform.GetChild(0) != this.transform) // Dolu ise atla
                        {
                            continue;
                        }
                        float distance = Vector3.Distance(otherTepsi.position, point.transform.position);

                        if (distance < newMinDistance)
                        {
                            newMinDistance = distance;
                            newNearestPoint = point.transform;
                        }
                    }
                    otherTepsi.SetParent(newNearestPoint);
                    otherTepsi.DOMove(newNearestPoint.position, duration);
                }
                else
                {
                    otherTepsi.DOMove(currentParent.position, duration);
                    otherTepsi.SetParent(currentParent);
                }
            }
            else
            {
                transform.parent = nearestPoint;
                transform.DOMove(nearestPoint.position, duration);
            }
        }
        bool canClick = true;
        foreach (var point in GameManager.Instance.placementPoints)
        {
            if (point.activeSelf && point.transform.childCount == 0)
            {
                canClick = false;
            }
        }
        uiManager.ChangeButtonState(canClick);
    }
}

