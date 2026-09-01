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

    [Header("Default Text Positions")]
    public Vector2 defaultWalkingTextPosition = new Vector2(0f, -200f);
    public Vector2 defaultFocusTextPosition = new Vector2(0f, -350f);

    [Header("Scenario Connection")]
    public ScenarioEngine scenarioEngine;

    private GameObject currentHoveredHotspot;
    private HotspotFocusPoint activeFocusedPoint;
    private HotspotCameraController cameraController;
    private RectTransform promptTextRect;

    private void Start()
    {
        mainCamera = Camera.main;
        cameraController = mainCamera.GetComponent<HotspotCameraController>();
        hotspotLayerIndex = LayerMask.NameToLayer("Hotspot");

        if (promptText != null)
        {
            promptTextRect = promptText.GetComponent<RectTransform>();
        }

        if (hotspotLayerIndex == -1)
            Debug.LogWarning("Hotspot layer not initialized");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        HidePrompt();
    }

    private void Update()
    {
        bool isFocused = cameraController != null && cameraController.IsFocused;

        if (!isFocused && activeFocusedPoint != null)
        {
            activeFocusedPoint = null;
        }

        HandleHover();

        if (!isFocused)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                TryInteractOrFocus();
            }
        }
        else
        {
            // If we are currently holding/dragging a knob, don't re-trigger standard click actions
            if (DialKnobController.ActiveDial != null && DialKnobController.ActiveDial.IsDragging)
            {
                return;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                TriggerHotspotAction();
            }
        }
    }

    private void HandleHover()
    {
        // Don't modify prompts or raycast targets mid-drag
        if (DialKnobController.ActiveDial != null && DialKnobController.ActiveDial.IsDragging)
        {
            if (promptText != null)
            {
                promptText.text = "Drag Mouse to Adjust Oxygen Flow";
                promptText.gameObject.SetActive(true);
            }
            return;
        }

        Ray ray;
        bool isFocused = cameraController != null && cameraController.IsFocused;

        if (isFocused)
        {
            ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        }
        else
        {
            ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        }

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.gameObject.layer == hotspotLayerIndex)
            {
                if (isFocused && activeFocusedPoint != null)
                {
                    bool isTargetFocusedObject = hit.collider.transform.IsChildOf(activeFocusedPoint.transform) ||
                                                 hit.collider.gameObject == activeFocusedPoint.gameObject;

                    if (!isTargetFocusedObject)
                    {
                        ClearHover();
                        ShowExitPromptOnly();
                        return;
                    }
                }

                HotspotObject hotspotInfo = hit.collider.GetComponent<HotspotObject>();
                if (hotspotInfo == null)
                    hotspotInfo = hit.collider.GetComponentInParent<HotspotObject>();

                if (hotspotInfo != null && !string.IsNullOrEmpty(hotspotInfo.hotspotId))
                {
                    currentHoveredHotspot = hit.collider.gameObject;

                    UpdateTextPosition(currentHoveredHotspot, isFocused);

                    string rawName = hotspotInfo.hotspotId.Replace("hs_", "").Replace("_", " ").Replace("-", " ");
                    TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                    string formattedName = textInfo.ToTitleCase(rawName.ToLower());

                    if (promptText != null)
                    {
                        if (isFocused)
                        {
                            DialKnobController dial = currentHoveredHotspot.GetComponentInParent<DialKnobController>();
                            if (dial == null) dial = currentHoveredHotspot.GetComponent<DialKnobController>();

                            if (dial != null)
                            {
                                promptText.text = "[Click & Drag] to turn dial | [Right-Click] - Exit";
                            }
                            else
                            {
                                promptText.text = "[Click] | [Right-Click] - Exit";
                            }
                        }
                        else
                        {
                            promptText.text = $"[E] - {formattedName}";
                        }
                        promptText.gameObject.SetActive(true);
                    }
                    return;
                }
            }
        }

        ClearHover();

        if (isFocused)
        {
            ShowExitPromptOnly();
        }
    }

    private void ShowExitPromptOnly()
    {
        if (promptText != null)
        {
            UpdateTextPosition(activeFocusedPoint != null ? activeFocusedPoint.gameObject : null, true);
            promptText.text = "[Right-Click] - Exit";
            promptText.gameObject.SetActive(true);
        }
    }

    private void UpdateTextPosition(GameObject targetHotspot, bool isFocused)
    {
        if (promptTextRect == null) return;

        if (isFocused)
        {
            HotspotFocusPoint focusPoint = null;
            if (targetHotspot != null)
            {
                focusPoint = targetHotspot.GetComponentInParent<HotspotFocusPoint>();
                if (focusPoint == null) focusPoint = targetHotspot.GetComponent<HotspotFocusPoint>();
            }

            if (focusPoint == null) focusPoint = activeFocusedPoint;

            if (focusPoint != null && focusPoint.useCustomTextPosition)
            {
                promptTextRect.anchoredPosition = focusPoint.customTextPosition;
            }
            else
            {
                promptTextRect.anchoredPosition = defaultFocusTextPosition;
            }
        }
        else
        {
            promptTextRect.anchoredPosition = defaultWalkingTextPosition;
        }
    }

    private void TryInteractOrFocus()
    {
        if (currentHoveredHotspot == null) return;

        HotspotObject hotspotInfo = currentHoveredHotspot.GetComponentInParent<HotspotObject>();
        if (hotspotInfo == null) hotspotInfo = currentHoveredHotspot.GetComponent<HotspotObject>();

        bool isPatient = hotspotInfo != null && hotspotInfo.hotspotId.ToLower().Contains("patient");

        HotspotFocusPoint focusPoint = currentHoveredHotspot.GetComponentInParent<HotspotFocusPoint>();
        if (focusPoint == null) focusPoint = currentHoveredHotspot.GetComponent<HotspotFocusPoint>();

        if (!isPatient && focusPoint != null && focusPoint.cameraFocusTarget != null && cameraController != null)
        {
            activeFocusedPoint = focusPoint;
            cameraController.FocusOnTarget(focusPoint.cameraFocusTarget);
        }
        else
        {
            TriggerHotspotAction();
        }
    }

    private void TriggerHotspotAction()
    {
        if (currentHoveredHotspot == null) return;

        DialKnobController dial = currentHoveredHotspot.GetComponentInParent<DialKnobController>();
        if (dial == null) dial = currentHoveredHotspot.GetComponentInChildren<DialKnobController>();

        if (dial != null)
        {
            dial.StartDragging();
            return; 
        }

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

        if (cameraController != null && cameraController.IsFocused)
        {
            activeFocusedPoint = null;
            cameraController.ExitFocus();
        }
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