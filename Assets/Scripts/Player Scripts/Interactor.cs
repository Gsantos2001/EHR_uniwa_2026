using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public float rayDistance = 100f;
    private Camera mainCamera;
    private int hotspotLayerIndex;

    private void Start()
    {
        mainCamera = Camera.main;
        hotspotLayerIndex = LayerMask.NameToLayer("Hotspot");

        if (hotspotLayerIndex == -1)
            Debug.LogWarning("Hotspot layer not initialized");
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            shootRay();
    }

    private void shootRay()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.gameObject.layer == hotspotLayerIndex)
            {
                Debug.Log("Hotspot detected");
                CallButton button = hit.collider.GetComponent<CallButton>();
                if (button != null)
                {
                    button.PressButton();
                }
            }
        }
    }
}