using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public float rayDistance = 100f;
    private Camera mainCamera;
    private int hotspotLayerIndex;

    [Header("Scenario Connection")]
    [Tooltip("Σύνδεσε εδώ το GameObject που έχει το ScenarioEngine")]
    public ScenarioEngine scenarioEngine;

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
                Debug.Log("Hotspot detected: " + hit.collider.gameObject.name);
                
                // Παλιά λογική για το CallButton
                CallButton button = hit.collider.GetComponent<CallButton>();
                if (button != null)
                {
                    button.PressButton();
                }

                // Νέα λογική για το JSON Σενάριο
                HotspotObject hotspotInfo = hit.collider.GetComponent<HotspotObject>();
                if (hotspotInfo != null && scenarioEngine != null)
                {
                    // Στέλνουμε το ID (π.χ. "hs_patient") στο Engine
                    scenarioEngine.TrySelectOptionByHotspot(hotspotInfo.hotspotId);
                }
            }
        }
    }
}