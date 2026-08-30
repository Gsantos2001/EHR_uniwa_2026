using System.Collections;
using UnityEngine;
using TMPro;

public class ScenarioUIManager : MonoBehaviour
{
    [Header("Engine Reference")]
    [Tooltip("Σύνδεσε το GameObject που έχει το ScenarioEngine")]
    public ScenarioEngine scenarioEngine;

    [Header("Score UI")]
    public TMP_Text scoreText;

    [Header("Toast UI")]
    public TMP_Text toastText;
    public float toastDuration = 2f;

    private Coroutine currentToastCoroutine;

    private void Start()
    {
        if (scenarioEngine != null)
        {
            scenarioEngine.OnScoreChanged += UpdateScoreText;
            scenarioEngine.OnToastRequested += ShowToast;
        }
        else
        {
            Debug.LogError("Δεν έχει συνδεθεί το ScenarioEngine στο ScenarioUIManager!");
        }

        HideToast();
    }

    private void OnDestroy()
    {
        if (scenarioEngine != null)
        {
            scenarioEngine.OnScoreChanged -= UpdateScoreText;
            scenarioEngine.OnToastRequested -= ShowToast;
        }
    }

    private void UpdateScoreText(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + newScore;
        }
    }

    private void ShowToast(string message, string style)
    {
        if (currentToastCoroutine != null)
        {
            StopCoroutine(currentToastCoroutine);
        }

        currentToastCoroutine = StartCoroutine(ToastRoutine(message, style));
    }

    private IEnumerator ToastRoutine(string message, string style)
    {
        if (toastText != null)
        {
            toastText.text = message;

            if (style == "danger") 
                toastText.color = Color.red;
            else 
                toastText.color = Color.white;

            toastText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(toastDuration);

        HideToast();
    }

    private void HideToast()
    {
        if (toastText != null) toastText.gameObject.SetActive(false);
    }
}