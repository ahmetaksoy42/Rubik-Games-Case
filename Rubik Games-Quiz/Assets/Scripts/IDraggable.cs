using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDraggable
{
    void OnMouseDown();
    void OnMouseDrag();
    void OnMouseUp();
    Vector3 MouseWorldPos();
}
