using UnityEngine;

public class EHRHotspot : MonoBehaviour
{
    [Header("UI Reference")]
    public EHRUI ehrUI;

    [Header("Camera Focus References")]
    public HotspotCameraController cameraController;
    public HotspotFocusPoint focusPoint;

    private void Start()
    {
        if (cameraController == null)
            cameraController = FindObjectOfType<HotspotCameraController>();

        if (focusPoint == null)
            focusPoint = GetComponent<HotspotFocusPoint>();
    }

    public void OpenEHR()
    {
        // 1. Zoom/Focus camera on the hotspot screen
        if (cameraController != null && focusPoint != null && focusPoint.cameraFocusTarget != null)
        {
            cameraController.FocusOnTarget(focusPoint.cameraFocusTarget);
        }

        // 2. Open UI
        if (ehrUI != null)
        {
            ehrUI.OpenEHR();
        }
        else
        {
            Debug.LogError("EHRHotspot: EHRUI reference is missing in Inspector!");
        }
    }

   /* private void OnMouseDown()
    {
        OpenEHR();
    }*/
}