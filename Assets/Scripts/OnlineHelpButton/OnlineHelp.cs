using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class OnlineHelp : MonoBehaviour
{
    [Header("General UI")]
    [SerializeField] private GameObject onlineHelpPanel;
    [SerializeField] private GameObject onlineHelpIcon;

    [Header("General Scroll View")]
    [SerializeField] private ScrollRect scrollRect;

    [Header("General Help Text")]
    [SerializeField] private TMP_Text onlineHelpText;

    [Header("Hotspot Help UI")]
    [SerializeField] private GameObject hotspotHelpPanel;

    [Header("Hotspot Help Text")]
    [SerializeField] private TMP_Text hotspotHelpText;

    [Header("Player")]
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private ScrollRect hotspotHelpScrollRect;

    private bool helpIsOpen = false;

    // Το hotspot που χρησιμοποιεί αυτή τη στιγμή ο Player
    private HotspotOnlineHelp currentHotspotHelp;


    private void Start()
    {
        // Γενικό Help κλειστό
        if (onlineHelpPanel != null)
            onlineHelpPanel.SetActive(false);

        // Hotspot Help κλειστό
        if (hotspotHelpPanel != null)
            hotspotHelpPanel.SetActive(false);

        // Icon εμφανές
        if (onlineHelpIcon != null)
            onlineHelpIcon.SetActive(true);

        helpIsOpen = false;
    }


    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.qKey.wasPressedThisFrame)
        {
            ToggleHelp();
        }

       // =========================================================
// SCROLL HELP
// =========================================================

if (helpIsOpen && Mouse.current != null)
{
    float scroll = Mouse.current.scroll.ReadValue().y;

    if (Mathf.Abs(scroll) > 0.01f)
    {
        // HOTSPOT HELP
        if (currentHotspotHelp != null &&
            hotspotHelpScrollRect != null)
        {
            hotspotHelpScrollRect.verticalNormalizedPosition +=
                scroll * 0.010f;

            hotspotHelpScrollRect.verticalNormalizedPosition =
                Mathf.Clamp01(
                    hotspotHelpScrollRect.verticalNormalizedPosition
                );
        }

        // GENERAL HELP
        else if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition +=
                scroll * 0.010f;

            scrollRect.verticalNormalizedPosition =
                Mathf.Clamp01(
                    scrollRect.verticalNormalizedPosition
                );
        }
    }
}
    }


    // =========================================================
    // SET CURRENT HOTSPOT
    // =========================================================

    public void SetCurrentHotspot(HotspotOnlineHelp hotspot)
    {
        currentHotspotHelp = hotspot;

        Debug.Log(
            "OnlineHelp: Current hotspot help set to " +
            hotspot.gameObject.name
        );
    }


    public void ClearCurrentHotspot()
    {
        currentHotspotHelp = null;

        Debug.Log("OnlineHelp: Current hotspot help cleared");
    }


    // =========================================================
    // Q
    // =========================================================

    private void ToggleHelp()
    {
        helpIsOpen = !helpIsOpen;

        if (helpIsOpen)
        {
            OpenHelp();
        }
        else
        {
            CloseHelp();
        }
    }


    // =========================================================
    // OPEN
    // =========================================================

    private void OpenHelp()
    {
        // =====================================================
        // HOTSPOT-SPECIFIC HELP
        // =====================================================

        if (currentHotspotHelp != null)
        {
            if (hotspotHelpText != null)
            {
                hotspotHelpText.text =
                    currentHotspotHelp.helpMessage;
            }
            if (hotspotHelpScrollRect != null)
{
    hotspotHelpScrollRect.verticalNormalizedPosition = 1f;
}

            if (hotspotHelpPanel != null)
                hotspotHelpPanel.SetActive(true);

            if (onlineHelpPanel != null)
                onlineHelpPanel.SetActive(false);

            if (onlineHelpIcon != null)
                onlineHelpIcon.SetActive(false);

            if (playerController != null)
                playerController.enabled = false;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            return;
        }


        // =====================================================
        // GENERAL HELP
        // =====================================================

        if (onlineHelpPanel != null)
            onlineHelpPanel.SetActive(true);

        if (hotspotHelpPanel != null)
            hotspotHelpPanel.SetActive(false);

        if (onlineHelpIcon != null)
            onlineHelpIcon.SetActive(false);

        if (playerController != null)
            playerController.enabled = false;

        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    // =========================================================
    // CLOSE
    // =========================================================

    private void CloseHelp()
    {
        if (onlineHelpPanel != null)
            onlineHelpPanel.SetActive(false);

        if (hotspotHelpPanel != null)
            hotspotHelpPanel.SetActive(false);

        if (onlineHelpIcon != null)
            onlineHelpIcon.SetActive(true);

        if (playerController != null)
            playerController.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


public void CloseHotspotHelp()
{
    helpIsOpen = false;

    if (hotspotHelpPanel != null)
        hotspotHelpPanel.SetActive(false);

    if (onlineHelpIcon != null)
        onlineHelpIcon.SetActive(true);

    if (playerController != null)
        playerController.enabled = true;
}

    public bool IsHelpOpen()
{
    return helpIsOpen;
}



}