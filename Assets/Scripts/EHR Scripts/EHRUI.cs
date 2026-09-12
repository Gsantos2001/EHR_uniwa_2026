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
    [Header("Camera Focus")]
    public HotspotCameraController cameraController;
    
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

        // LOAD SAVED DATA INTO INPUT FIELDS
        if (observationInput != null) observationInput.text = ehrManager.assessment.observation ?? "";
        if (skinColorInput != null) skinColorInput.text = ehrManager.assessment.skinColor ?? "";
        if (consciousnessInput != null) consciousnessInput.text = ehrManager.assessment.consciousness ?? "";

        if (deviceInput != null) deviceInput.text = ehrManager.intervention.device ?? "";
        if (fiO2Input != null) fiO2Input.text = ehrManager.intervention.fiO2Setting ?? "";
        if (flowRateInput != null) flowRateInput.text = ehrManager.intervention.flowRate ?? "";

        if (recipientInput != null) recipientInput.text = ehrManager.communication.recipient ?? "";
        if (reasonInput != null) reasonInput.text = ehrManager.communication.reason ?? "";
        if (outcomeInput != null) outcomeInput.text = ehrManager.communication.outcome ?? "";

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


    // SYNC CURRENT INPUT VALUES TO EHRMANAGER
    public void SaveCurrentInputData()
    {
        if (ehrManager == null) return;

        if (observationInput != null) ehrManager.assessment.observation = observationInput.text.Trim();
        if (skinColorInput != null) ehrManager.assessment.skinColor = skinColorInput.text.Trim();
        if (consciousnessInput != null) ehrManager.assessment.consciousness = consciousnessInput.text.Trim();

        if (deviceInput != null) ehrManager.intervention.device = deviceInput.text.Trim();
        if (fiO2Input != null) ehrManager.intervention.fiO2Setting = fiO2Input.text.Trim();
        if (flowRateInput != null) ehrManager.intervention.flowRate = flowRateInput.text.Trim();

        if (recipientInput != null) ehrManager.communication.recipient = recipientInput.text.Trim();
        if (reasonInput != null) ehrManager.communication.reason = reasonInput.text.Trim();
        if (outcomeInput != null) ehrManager.communication.outcome = outcomeInput.text.Trim();
    }


    // TABS

    public void ShowAssessmentTab()
    {
        SaveCurrentInputData();
        assessmentPanel.SetActive(true);
        interventionPanel.SetActive(false);
        communicationPanel.SetActive(false);
    }


    public void ShowInterventionTab()
    {
        SaveCurrentInputData();
        assessmentPanel.SetActive(false);
        interventionPanel.SetActive(true);
        communicationPanel.SetActive(false);
    }


    public void ShowCommunicationTab()
    {
        SaveCurrentInputData();
        assessmentPanel.SetActive(false);
        interventionPanel.SetActive(false);
        communicationPanel.SetActive(true);
    }


    // OPEN / CLOSE EHR

    public void OpenEHR()
    {
        // Activate canvas GameObject and EHRPanel UI
        gameObject.SetActive(true);

        if (ehrPanel != null)
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





    // SAVE DOCUMENTATION

    public void SaveDocumentation()
    {
        if (ehrManager == null)
        {
            Debug.LogError("EHRUI: EHRManager is not assigned.");
            SetFeedback("EHR system is not available.");
            return;
        }

        if (!ehrManager.HasPatient())
        {
            SetFeedback("No patient is currently assigned to the bed.");
            return;
        }

        // SAVE CURRENT FORM VALUES
        SaveCurrentInputData();

        Debug.Log("EHR documentation saved.");
        if (scenarioLogger != null)
        {
            string details =
                "Observation=" + ehrManager.assessment.observation +
                " | FiO2=" + ehrManager.intervention.fiO2Setting +
                " | Recipient=" + ehrManager.communication.recipient +
                " | Outcome=" + ehrManager.communication.outcome;

            scenarioLogger.LogEvent(
                "EHR_SUBMIT",
                scenarioEngine != null ? scenarioEngine.CurrentNodeId : "",
                "hs_ehr",
                details,
                scenarioEngine != null ? scenarioEngine.CurrentScore : 0
            );
        }


        // NO SCENARIO CONNECTION

        if (scenarioEngine == null)
        {
            SetFeedback("Documentation saved.");
            return;
        }


        // GATE 1 VALIDATION
        // Observation + FiO2 Setting

        if (scenarioEngine.CurrentNodeId == "n4_gate_documentation_1")
        {
            // Observation missing
            if (string.IsNullOrWhiteSpace(ehrManager.assessment.observation))
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
            if (string.IsNullOrWhiteSpace(ehrManager.intervention.fiO2Setting))
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

        if (scenarioEngine.CurrentNodeId == "n7_gate_documentation_2")
        {
            // Recipient missing
            if (string.IsNullOrWhiteSpace(ehrManager.communication.recipient))
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
            if (string.IsNullOrWhiteSpace(ehrManager.communication.outcome))
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

        Debug.Log("EHR Gate result: " + gatePassed);
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


    public void CloseEHR()
    {
        ehrPanel.SetActive(false);

        if (cameraController != null &&
            cameraController.IsFocused)
        {
            cameraController.ExitFocus();
        }

        if (playerCam != null)
            playerCam.inputEnabled = true;

        if (playerMovement != null)
            playerMovement.inputEnabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}