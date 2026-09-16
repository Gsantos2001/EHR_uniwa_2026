using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnlineHelpUI : MonoBehaviour
{
    [Header("General Online Help")]
    public GameObject generalHelpPanel;
    public TextMeshProUGUI generalHelpText;
    public ScrollRect generalScrollRect;

    [Header("Hotspot Online Help")]
    public GameObject hotspotHelpPanel;
    public TextMeshProUGUI hotspotHelpText;
    public ScrollRect hotspotScrollRect;

    [Header("Online Help Icon")]
    public GameObject helpIcon;

    [Header("General Help Message")]
    [TextArea(3, 10)]
    public string generalHelpMessage =
        "Γενικές οδηγίες παιχνιδιού...\n\n" +
        "[E] - Interact με αντικείμενα\n" +
        "[Q] - On-line Help";

[Header("Camera")]
public PlayerCam playerCam;

    public bool IsHelpVisible { get; private set; }

    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisible;
    private bool cursorStateSaved;
    private bool previousPlayerCamEnabled;
private bool cameraStateSaved;
    private void Start()
    {
        if (playerCam == null)
    playerCam = FindObjectOfType<PlayerCam>();

        if (generalHelpPanel != null)
            generalHelpPanel.SetActive(false);

        if (hotspotHelpPanel != null)
            hotspotHelpPanel.SetActive(false);

        if (helpIcon != null)
            helpIcon.SetActive(true);

        IsHelpVisible = false;
        cursorStateSaved = false;
    }

    public void ShowGeneralHelp()
    {
        SaveCursorState();

        if (generalHelpText != null)
            generalHelpText.text = generalHelpMessage;

        if (generalHelpPanel != null)
            generalHelpPanel.SetActive(true);

        if (hotspotHelpPanel != null)
            hotspotHelpPanel.SetActive(false);

        if (helpIcon != null)
            helpIcon.SetActive(false);

        IsHelpVisible = true;

        // Το Online Help χρειάζεται ελεύθερο mouse.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

       if (playerCam != null)
    playerCam.enabled = false;

        ResetScroll(generalScrollRect);
    }

    public void ShowHotspotHelp(string message)
    {
        SaveCursorState();

        if (hotspotHelpText != null)
            hotspotHelpText.text = message;

        if (hotspotHelpPanel != null)
            hotspotHelpPanel.SetActive(true);

        if (generalHelpPanel != null)
            generalHelpPanel.SetActive(false);

        if (helpIcon != null)
            helpIcon.SetActive(false);

        IsHelpVisible = true;

        // Το Online Help χρειάζεται ελεύθερο mouse.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ResetScroll(hotspotScrollRect);
    }

    public void HideHelp()
    {
        if (generalHelpPanel != null)
            generalHelpPanel.SetActive(false);

        if (hotspotHelpPanel != null)
            hotspotHelpPanel.SetActive(false);

        if (helpIcon != null)
            helpIcon.SetActive(true);

        IsHelpVisible = false;

        // Επαναφέρουμε την κατάσταση του mouse
        // που υπήρχε ΠΡΙΝ ανοίξει το Online Help.
        RestoreCursorState();
    }

    public void ToggleGeneralHelp()
    {
        if (IsHelpVisible)
            HideHelp();
        else
            ShowGeneralHelp();
    }

    public void ToggleHotspotHelp(string message)
    {
        if (IsHelpVisible)
            HideHelp();
        else
            ShowHotspotHelp(message);
    }

    private void SaveCursorState()
    {
        if (cursorStateSaved)
            return;

        previousCursorLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;


        if (playerCam != null)
    {
        previousPlayerCamEnabled = playerCam.enabled;
        cameraStateSaved = true;
    }

        cursorStateSaved = true;
    }

    private void RestoreCursorState()
    {
        if (!cursorStateSaved)
            return;

        Cursor.lockState = previousCursorLockState;
        Cursor.visible = previousCursorVisible;

if (playerCam != null && cameraStateSaved)
        playerCam.enabled = previousPlayerCamEnabled;


        cursorStateSaved = false;
        cameraStateSaved = false;
    }

    private void ResetScroll(ScrollRect scrollRect)
    {
        if (scrollRect == null)
            return;

        Canvas.ForceUpdateCanvases();

        scrollRect.verticalNormalizedPosition = 1f;
        scrollRect.horizontalNormalizedPosition = 0f;
    }
}