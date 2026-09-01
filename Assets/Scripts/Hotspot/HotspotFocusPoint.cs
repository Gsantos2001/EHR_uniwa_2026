using UnityEngine;

public class HotspotFocusPoint : MonoBehaviour
{
    [Tooltip("Target position and rotation for the camera when focused.")]
    public Transform cameraFocusTarget; 

    [Header("Custom Focus UI Position")]
    [Tooltip("Check this if this specific hotspot needs the text placed somewhere unique on screen.")]
    public bool useCustomTextPosition = false;

    [Tooltip("UI Anchored Position (X, Y) relative to Canvas center (e.g., X: 0, Y: -350 for bottom, X: 0, Y: 350 for top banner).")]
    public Vector2 customTextPosition = new Vector2(0f, -350f);
}