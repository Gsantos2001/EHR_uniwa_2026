using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EHRUI : MonoBehaviour
{
    [Header("Scenario")]
    public ScenarioEngine scenarioEngine;

    [Header("Tabs")]
    public GameObject assessmentPanel;
    public GameObject interventionPanel;
    public GameObject communicationPanel;

    [Header("Form Scroll Views")]
    public ScrollRect assessmentScrollRect;
    public ScrollRect interventionScrollRect;
    public ScrollRect communicationScrollRect;

    [Header("Player")]
    public PlayerMovement playerMovement;
    public PlayerCam playerCam;

    [Header("References")]
    public EHRManager ehrManager;
    public GameObject ehrPanel;

    [Header("Patient Information")]
    public TMP_Text nameText;
    public TMP_Text ageText;
    public TMP_Text locationText;
    public TMP_Text diagnosisText;

    [Header("Vitals")]
    public TMP_Text vitalsHistoryText;

    [Header("Assessment")]
    public TMP_InputField observationInput;
    public TMP_InputField skinColorInput;
    public TMP_InputField consciousnessInput;

    [Header("Intervention")]
    public TMP_InputField deviceInput;
    public TMP_InputField fiO2Input;
    public TMP_InputField flowRateInput;

    [Header("Communication")]
    public TMP_InputField recipientInput;
    public TMP_InputField reasonInput;
    public TMP_InputField outcomeInput;

    [Header("Feedback")]
    public TMP_Text feedbackText;

    [Header("Logging")]
    public ScenarioLogger scenarioLogger;


    private void Start()
    {
        CloseEHR();
    }


    // UI REFRESH

    public void RefreshUI()
    {
        if (ehrManager == null)
            return;

        if (!ehrManager.HasPatient())
        {
            if (nameText != null)
                nameText.text = "Patient: --";

            if (ageText != null)
                ageText.text = "Age: --";

            if (locationText != null)
                locationText.text = "Location: --";

            if (diagnosisText != null)
                diagnosisText.text = "Admission Diagnosis: --";

            if (vitalsHistoryText != null)
                vitalsHistoryText.text = "NO PATIENT ON BED";

            return;
        }

        if (nameText != null)
        {
            nameText.text =
                "Patient: " + ehrManager.patientInfo.fullName;
        }

        if (ageText != null)
        {
            ageText.text =
                "Age: " + ehrManager.patientInfo.age;
        }

        if (locationText != null)
        {
            locationText.text =
                "Location: " + ehrManager.patientInfo.location;
        }

        if (diagnosisText != null)
        {
            diagnosisText.text =
                "Admission Diagnosis: " +
                ehrManager.patientInfo.admissionDiagnosis;
        }

        RefreshVitalsHistory();
    }


    public void RefreshVitalsHistory()
    {
        if (ehrManager == null || vitalsHistoryText == null)
            return;

        StringBuilder builder = new StringBuilder();

        foreach (VitalsHistoryEntry entry in ehrManager.vitalsHistory)
        {
            builder.AppendLine(
                entry.dateTime.PadRight(21) +
                (
                    entry.systolicPressure.ToString("0") +
                    "/" +
                    entry.diastolicPressure.ToString("0")
                ).PadRight(10) +
                (
                    entry.oxygenSaturation.ToString("0") +
                    "%"
                ).PadRight(9) +
                entry.heartRate.ToString("0")
            );
        }

        vitalsHistoryText.text = builder.ToString();
    }


    // TABS

    public void ShowAssessmentTab()
    {
        assessmentPanel.SetActive(true);
        interventionPanel.SetActive(false);
        communicationPanel.SetActive(false);
    }


    public void ShowInterventionTab()
    {
        assessmentPanel.SetActive(false);
        interventionPanel.SetActive(true);
        communicationPanel.SetActive(false);
    }


    public void ShowCommunicationTab()
    {
        assessmentPanel.SetActive(false);
        interventionPanel.SetActive(false);
        communicationPanel.SetActive(true);
    }


    // OPEN / CLOSE EHR

    public void OpenEHR()
    {
        ehrPanel.SetActive(true);

        if (playerCam != null)
            playerCam.inputEnabled = false;

        if (playerMovement != null)
            playerMovement.inputEnabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (feedbackText != null)
            feedbackText.text = "";

        ShowAssessmentTab();
        RefreshUI();
    }


    public void CloseEHR()
    {
        ehrPanel.SetActive(false);

        if (playerCam != null)
            playerCam.inputEnabled = true;

        if (playerMovement != null)
            playerMovement.inputEnabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    // SAVE DOCUMENTATION

    public void SaveDocumentation()
    {
        if (ehrManager == null)
        {
            Debug.LogError(
                "EHRUI: EHRManager is not assigned."
            );

            SetFeedback(
                "EHR system is not available."
            );

            return;
        }

        if (!ehrManager.HasPatient())
        {
            SetFeedback(
                "No patient is currently assigned to the bed."
            );

            return;
        }


        // SAVE CURRENT FORM VALUES

        ehrManager.assessment.observation =
            observationInput != null
            ? observationInput.text.Trim()
            : "";

        ehrManager.assessment.skinColor =
            skinColorInput != null
            ? skinColorInput.text.Trim()
            : "";

        ehrManager.assessment.consciousness =
            consciousnessInput != null
            ? consciousnessInput.text.Trim()
            : "";


        ehrManager.intervention.device =
            deviceInput != null
            ? deviceInput.text.Trim()
            : "";

        ehrManager.intervention.fiO2Setting =
            fiO2Input != null
            ? fiO2Input.text.Trim()
            : "";

        ehrManager.intervention.flowRate =
            flowRateInput != null
            ? flowRateInput.text.Trim()
            : "";


        ehrManager.communication.recipient =
            recipientInput != null
            ? recipientInput.text.Trim()
            : "";

        ehrManager.communication.reason =
            reasonInput != null
            ? reasonInput.text.Trim()
            : "";

        ehrManager.communication.outcome =
            outcomeInput != null
            ? outcomeInput.text.Trim()
            : "";


        Debug.Log("EHR documentation saved.");
        if (scenarioLogger != null)
        {
            string details =
                "Observation=" +
                ehrManager.assessment.observation +
                " | FiO2=" +
                ehrManager.intervention.fiO2Setting +
                " | Recipient=" +
                ehrManager.communication.recipient +
                " | Outcome=" +
                ehrManager.communication.outcome;

            scenarioLogger.LogEvent(
                "EHR_SUBMIT",
                scenarioEngine != null
                    ? scenarioEngine.CurrentNodeId
                    : "",
                "hs_ehr",
                details,
                scenarioEngine != null
                    ? scenarioEngine.CurrentScore
                    : 0
            );
        }


        // NO SCENARIO CONNECTION

        if (scenarioEngine == null)
        {
            SetFeedback(
                "Documentation saved."
            );

            return;
        }


        // GATE 1 VALIDATION
        // Observation + FiO2 Setting

        if (scenarioEngine.CurrentNodeId ==
            "n4_gate_documentation_1")
        {
            // Observation missing
            if (string.IsNullOrWhiteSpace(
                ehrManager.assessment.observation))
            {
                SetFeedback(
                    "Missing required field: Observation\n" +
                    "Tab: Assessment"
                );

                ShowAssessmentTab();

                if (observationInput != null)
                {
                    StartCoroutine(
                        ScrollToField(
                            assessmentScrollRect,
                            observationInput.GetComponent<RectTransform>()
                        )
                    );

                    observationInput.Select();
                    observationInput.ActivateInputField();
                }

                return;
            }


            // FiO2 missing
            if (string.IsNullOrWhiteSpace(
                ehrManager.intervention.fiO2Setting))
            {
                SetFeedback(
                    "Missing required field: FiO2 Setting\n" +
                    "Tab: Intervention"
                );

                ShowInterventionTab();

                if (fiO2Input != null)
                {
                    StartCoroutine(
                        ScrollToField(
                            interventionScrollRect,
                            fiO2Input.GetComponent<RectTransform>()
                        )
                    );

                    fiO2Input.Select();
                    fiO2Input.ActivateInputField();
                }

                return;
            }


            // Both fields exist -> try to pass Gate 1
            TryCompleteScenarioGate();

            return;
        }


        // GATE 2 VALIDATION
        // Recipient + Outcome

        if (scenarioEngine.CurrentNodeId ==
            "n7_gate_documentation_2")
        {
            // Recipient missing
            if (string.IsNullOrWhiteSpace(
                ehrManager.communication.recipient))
            {
                SetFeedback(
                    "Missing required field: Recipient\n" +
                    "Tab: Communication"
                );

                ShowCommunicationTab();

                if (recipientInput != null)
                {
                    StartCoroutine(
                        ScrollToField(
                            communicationScrollRect,
                            recipientInput.GetComponent<RectTransform>()
                        )
                    );

                    recipientInput.Select();
                    recipientInput.ActivateInputField();
                }

                return;
            }


            // Outcome missing
            if (string.IsNullOrWhiteSpace(
                ehrManager.communication.outcome))
            {
                SetFeedback(
                    "Missing required field: Outcome\n" +
                    "Tab: Communication"
                );

                ShowCommunicationTab();

                if (outcomeInput != null)
                {
                    StartCoroutine(
                        ScrollToField(
                            communicationScrollRect,
                            outcomeInput.GetComponent<RectTransform>()
                        )
                    );

                    outcomeInput.Select();
                    outcomeInput.ActivateInputField();
                }

                return;
            }


            // Both fields exist -> try to pass Gate 2
            TryCompleteScenarioGate();

            return;
        }


        // NOT CURRENTLY AT A DOCUMENTATION GATE

        SetFeedback(
            "Documentation saved.\n" +
            "No documentation gate is currently active."
        );
    }


    // GATE HELPER

    private void TryCompleteScenarioGate()
    {
        bool gatePassed =
            scenarioEngine.TryCompleteCurrentEHRGate(
                ehrManager,
                out string gateFeedback
            );

        SetFeedback(gateFeedback);

        Debug.Log(
            "EHR Gate result: " +
            gatePassed
        );
    }

    private IEnumerator ScrollToField(
    ScrollRect scrollRect,
    RectTransform targetField)
    {
        if (scrollRect == null || targetField == null)
            yield break;

       
        yield return null;

        Canvas.ForceUpdateCanvases();

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        if (content == null || viewport == null)
            yield break;

       
        Vector3 targetWorldPosition = targetField.TransformPoint(
            targetField.rect.center
        );

        Vector3 targetLocalPosition =
            content.InverseTransformPoint(targetWorldPosition);

        
        Vector3 viewportWorldPosition = viewport.TransformPoint(
            viewport.rect.center
        );

        Vector3 viewportLocalPosition =
            content.InverseTransformPoint(viewportWorldPosition);

        float difference =
            viewportLocalPosition.y -
            targetLocalPosition.y;

        Vector2 contentPosition =
            content.anchoredPosition;

        contentPosition.y += difference;

        content.anchoredPosition =
            contentPosition;

        Canvas.ForceUpdateCanvases();
    }
    // FEEDBACK HELPER

    private void SetFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
    }
}