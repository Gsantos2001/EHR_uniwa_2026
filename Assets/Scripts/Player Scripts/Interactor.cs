using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Interactor : MonoBehaviour
{
    public float rayDistance = 100f;
    private Camera mainCamera;
    private int hotspotLayerIndex;

    [Header("UI Settings")]
    public TextMeshProUGUI promptText; 
    public string defaultPromptMessage = "[E] - Interact";

    [Header("Scenario Connection")]
    [Tooltip("Σύνδεσε εδώ το GameObject που έχει το ScenarioEngine")]
    public ScenarioEngine scenarioEngine;

    private GameObject currentHoveredHotspot;

    private void Start()
    {
        mainCamera = Camera.main;
        hotspotLayerIndex = LayerMask.NameToLayer("Hotspot");

        if (hotspotLayerIndex == -1)
            Debug.LogWarning("Hotspot layer not initialized");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        HidePrompt();
    }

    private void Update()
    {
        HandleHover();

        if (Keyboard.current.eKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            Interact();
        }
    }

    private void HandleHover()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.gameObject.layer == hotspotLayerIndex)
            {
                HotspotObject hotspotInfo = hit.collider.GetComponent<HotspotObject>();
                if (hotspotInfo == null)
                    hotspotInfo = hit.collider.GetComponentInParent<HotspotObject>();

                if (hotspotInfo != null && !string.IsNullOrEmpty(hotspotInfo.hotspotId))
                {
                    currentHoveredHotspot = hit.collider.gameObject;

                    // Debug.Log($"[Hovering Hotspot] Target: {hit.collider.gameObject.name} | ID: {hotspotInfo.hotspotId}");

                    string rawName = hotspotInfo.hotspotId.Replace("hs_", "").Replace("_", " ").Replace("-", " ");
                    
                    TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                    string formattedName = textInfo.ToTitleCase(rawName.ToLower());

                    if (promptText != null)
                    {
                        promptText.text = $"[E] - {formattedName}";
                        promptText.gameObject.SetActive(true);
                    }
                    return;
                }
            }
        }

        ClearHover();
    }

    private void Interact()
    {
        if (currentHoveredHotspot == null) return;

        Debug.Log("Hotspot interacted: " + currentHoveredHotspot.name);

        HidePrompt();

        CallButton button = currentHoveredHotspot.GetComponentInParent<CallButton>();
        if (button != null)
        {
            button.PressButton();
        }

        HotspotObject hotspotInfo = currentHoveredHotspot.GetComponentInParent<HotspotObject>();
        if (hotspotInfo != null && scenarioEngine != null)
        {
            scenarioEngine.TrySelectOptionByHotspot(hotspotInfo.hotspotId);
        }

        currentHoveredHotspot = null;
    }

    private void ClearHover()
    {
        currentHoveredHotspot = null;
        HidePrompt();
    }

    private void HidePrompt()
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }
}