using UnityEngine;

public class HotspotFocusPoint : MonoBehaviour
{
    public Transform cameraFocusTarget; 

    [Header("Custom Focus UI Position")]
    public bool useCustomTextPosition = false;

    public Vector2 customTextPosition = new Vector2(0f, -350f);
}