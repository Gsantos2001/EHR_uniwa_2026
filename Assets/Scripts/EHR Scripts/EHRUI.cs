using System.Text;
using TMPro;
using UnityEngine;

public class EHRUI : MonoBehaviour
{
    [Header("Tabs")]
    public GameObject assessmentPanel;
    public GameObject interventionPanel;
    public GameObject communicationPanel;
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
    public PlayerCam playerCam;
    private void Start()
    {
        CloseEHR();
    }


    public void RefreshUI()
    {
        if (ehrManager == null)
            return;

        if (!ehrManager.HasPatient())
        {
            nameText.text = "Patient: --";
            ageText.text = "Age: --";
            locationText.text = "Location: --";
            diagnosisText.text = "Admission Diagnosis: --";

            vitalsHistoryText.text = "NO PATIENT ON BED";

            return;
        }

        nameText.text =
            "Patient: " + ehrManager.patientInfo.fullName;

        ageText.text =
            "Age: " + ehrManager.patientInfo.age;

        locationText.text =
            "Location: " + ehrManager.patientInfo.location;

        diagnosisText.text =
            "Admission Diagnosis: " +
            ehrManager.patientInfo.admissionDiagnosis;

        RefreshVitalsHistory();
    }

    public void RefreshVitalsHistory()
    {
        StringBuilder builder = new StringBuilder();

        builder.AppendLine(
            "DATE / TIME              BP          SpO2       HR"
        );

        foreach (VitalsHistoryEntry entry in ehrManager.vitalsHistory)
        {
            builder.AppendLine(
                entry.dateTime +
                "     " +
                entry.systolicPressure.ToString("0") +
                "/" +
                entry.diastolicPressure.ToString("0") +
                "        " +
                entry.oxygenSaturation.ToString("0") +
                "%        " +
                entry.heartRate.ToString("0")
            );
        }

        vitalsHistoryText.text = builder.ToString();
    }

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

    public void OpenEHR()
    {
        ehrPanel.SetActive(true);

        if (playerCam != null)
            playerCam.inputEnabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowAssessmentTab();
        RefreshUI();
    }

    public void CloseEHR()
    {
        ehrPanel.SetActive(false);

        if (playerCam != null)
            playerCam.inputEnabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SaveDocumentation()
    {
        if (!ehrManager.HasPatient())
        {
            feedbackText.text = "Δεν υπάρχει ασθενής στην κλίνη.";
            return;
        }

        ehrManager.assessment.observation =
            observationInput.text;

        ehrManager.assessment.skinColor =
            skinColorInput.text;

        ehrManager.assessment.consciousness =
            consciousnessInput.text;

        ehrManager.intervention.device =
            deviceInput.text;

        ehrManager.intervention.fiO2Setting =
            fiO2Input.text;

        ehrManager.intervention.flowRate =
            flowRateInput.text;

        ehrManager.communication.recipient =
            recipientInput.text;

        ehrManager.communication.reason =
            reasonInput.text;

        ehrManager.communication.outcome =
            outcomeInput.text;

        feedbackText.text = "Η τεκμηρίωση αποθηκεύτηκε.";

        Debug.Log("EHR documentation saved.");
    }
}