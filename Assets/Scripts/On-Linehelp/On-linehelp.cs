using System.Collections;
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
        {
            playerCam = FindObjectOfType<PlayerCam>();
        }

        if (generalHelpPanel != null)
        {
            generalHelpPanel.SetActive(false);
        }

        if (hotspotHelpPanel != null)
        {
            hotspotHelpPanel.SetActive(false);
        }

        if (helpIcon != null)
        {
            helpIcon.SetActive(true);
        }

        IsHelpVisible = false;
        cursorStateSaved = false;
    }
    
    public void ShowGeneralHelp()
    {
        SaveCursorState();

        if (generalHelpPanel != null)
        {
            generalHelpPanel.SetActive(true);
        }

        if (hotspotHelpPanel != null)
        {
            hotspotHelpPanel.SetActive(false);
        }

        if (generalHelpText != null)
        {
            generalHelpText.text = generalHelpMessage;
        }

        if (helpIcon != null)
        {
            helpIcon.SetActive(false);
        }

        IsHelpVisible = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerCam != null)
        {
            playerCam.enabled = false;
        }
		
        StartCoroutine(
            RefreshHelpLayout(
                generalScrollRect,
                generalHelpText
            )
        );
    }

    public void ShowHotspotHelp(string message)
    {
        SaveCursorState();

        if (hotspotHelpPanel != null)
        {
            hotspotHelpPanel.SetActive(true);
        }

        if (generalHelpPanel != null)
        {
            generalHelpPanel.SetActive(false);
        }

        if (hotspotHelpText != null)
        {
            hotspotHelpText.text = message;
        }

        if (helpIcon != null)
        {
            helpIcon.SetActive(false);
        }

        IsHelpVisible = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerCam != null)
        {
            playerCam.enabled = false;
        }

        StartCoroutine(
            RefreshHelpLayout(
                hotspotScrollRect,
                hotspotHelpText
            )
        );
    }

    public void HideHelp()
    {
        if (generalHelpPanel != null)
        {
            generalHelpPanel.SetActive(false);
        }

        if (hotspotHelpPanel != null)
        {
            hotspotHelpPanel.SetActive(false);
        }

        if (helpIcon != null)
        {
            helpIcon.SetActive(true);
        }

        IsHelpVisible = false;

        RestoreCursorState();
    }

    public void ToggleGeneralHelp()
    {
        if (IsHelpVisible)
        {
            HideHelp();
        }
        else
        {
            ShowGeneralHelp();
        }
    }


    public void ToggleHotspotHelp(string message)
    {
        if (IsHelpVisible)
        {
            HideHelp();
        }
        else
        {
            ShowHotspotHelp(message);
        }
    }

    private void SaveCursorState()
    {
        if (cursorStateSaved)
        {
            return;
        }

        previousCursorLockState =
            Cursor.lockState;

        previousCursorVisible =
            Cursor.visible;

        if (playerCam != null)
        {
            previousPlayerCamEnabled =
                playerCam.enabled;

            cameraStateSaved = true;
        }

        cursorStateSaved = true;
    }

    private void RestoreCursorState()
    {
        if (!cursorStateSaved)
        {
            return;
        }

        Cursor.lockState =
            previousCursorLockState;

        Cursor.visible =
            previousCursorVisible;

        if (playerCam != null &&
            cameraStateSaved)
        {
            playerCam.enabled =
                previousPlayerCamEnabled;
        }

        cursorStateSaved = false;
        cameraStateSaved = false;
    }


    private IEnumerator RefreshHelpLayout(
        ScrollRect scrollRect,
        TextMeshProUGUI helpText)
    {
        if (scrollRect == null ||
            helpText == null)
        {
            yield break;
        }

        yield return null;

        helpText.ForceMeshUpdate();

        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            helpText.rectTransform
        );

        if (scrollRect.content != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                scrollRect.content
            );
        }

        Canvas.ForceUpdateCanvases();

        scrollRect.StopMovement();

        scrollRect.verticalNormalizedPosition = 1f;

        scrollRect.horizontalNormalizedPosition = 0f;
    }
}