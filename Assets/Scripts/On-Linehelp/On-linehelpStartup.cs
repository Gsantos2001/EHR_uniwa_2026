using UnityEngine;
using TMPro;

public class OnlineHelpStartUpMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI helpText;

    [SerializeField] private string message = "Press 'Q' for online help";

    [SerializeField] private float displayDuration = 3.5f;

    private void Start()
    {
        if (helpText == null)
        {
            Debug.LogWarning("HelpText is not assigned!");
            return;
        }

        helpText.text = message;

        // Δεν κάνει αλλαγή γραμμής
        helpText.textWrappingMode = TextWrappingModes.NoWrap;
        helpText.overflowMode = TextOverflowModes.Overflow;

        helpText.gameObject.SetActive(true);

        Invoke(nameof(HideMessage), displayDuration);
    }

    private void HideMessage()
    {
        if (helpText != null)
        {
            helpText.gameObject.SetActive(false);
        }
    }
}