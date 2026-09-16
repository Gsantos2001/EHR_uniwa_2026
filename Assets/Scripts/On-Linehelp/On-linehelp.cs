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


    
    // START
    

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


    
    // GENERAL HELP
    

    public void ShowGeneralHelp()
    {
        SaveCursorState();

        // Πρώτα ανοίγουμε το panel ώστε το Unity
        // να μπορεί να υπολογίσει το layout.
        if (generalHelpPanel != null)
        {
            generalHelpPanel.SetActive(true);
        }

        if (hotspotHelpPanel != null)
        {
            hotspotHelpPanel.SetActive(false);
        }

        // Μετά βάζουμε το κείμενο.
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

        // Περιμένουμε το layout να δημιουργηθεί σωστά.
        StartCoroutine(
            RefreshHelpLayout(
                generalScrollRect,
                generalHelpText
            )
        );
    }


    
    // HOTSPOT HELP
    

    public void ShowHotspotHelp(string message)
    {
        SaveCursorState();

        // Πρώτα ανοίγουμε το σωστό panel.
        if (hotspotHelpPanel != null)
        {
            hotspotHelpPanel.SetActive(true);
        }

        if (generalHelpPanel != null)
        {
            generalHelpPanel.SetActive(false);
        }

        // Μετά βάζουμε το κείμενο.
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


    
    // HIDE HELP
    

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


    
    // TOGGLE
    

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


    
    // CURSOR / CAMERA STATE
    

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


    
    // REFRESH SCROLL / LAYOUT
    

    private IEnumerator RefreshHelpLayout(
        ScrollRect scrollRect,
        TextMeshProUGUI helpText)
    {
        if (scrollRect == null ||
            helpText == null)
        {
            yield break;
        }

        // Περιμένουμε ένα frame ώστε το panel
        // να έχει ενεργοποιηθεί πλήρως.
        yield return null;

        // Αναγκάζουμε το TMP να υπολογίσει
        // το πραγματικό preferred height.
        helpText.ForceMeshUpdate();

        Canvas.ForceUpdateCanvases();

        // Rebuild του ίδιου του Text.
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            helpText.rectTransform
        );

        // Rebuild του Content του Scroll View.
        if (scrollRect.content != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                scrollRect.content
            );
        }

        Canvas.ForceUpdateCanvases();

        // Το help ξεκινά πάντα από την κορυφή.
        scrollRect.StopMovement();

        scrollRect.verticalNormalizedPosition = 1f;

        scrollRect.horizontalNormalizedPosition = 0f;
    }
}