using UnityEngine;
using TMPro;

public class PatientVitalsUI : MonoBehaviour
{
    [Header("Bed")]
    public BedPatientManager bedPatientManager;

    [Header("UI")]
    public TMP_Text heartRateText;
    public TMP_Text oxygenText;
    public TMP_Text bloodPressureText;
    public TMP_Text statusText;

    [Header("Colours")]
    public Color normalColor = Color.white;
    public Color warningColor = new Color(1f, 0.5f, 0f);
    public Color criticalColor = Color.red;
    public Color notAssignedColor = Color.gray;

    [Header("Blink")]
    public float blinkSpeed = 2f;

    private PatientVitals currentPatient;
    private PatientStatus currentPatientStatus;

    private bool shouldBlink;
    private Color currentStatusColor;

    private void Start()
    {
        if (bedPatientManager == null)
        {
            Debug.LogError("PatientVitalsUI: BedPatientManager not assigned.");
            ShowNoPatient();
            return;
        }

        bedPatientManager.OnPatientChanged += ChangePatient;

        ChangePatient(bedPatientManager.CurrentPatient);
    }

    private void Update()
    {
        if (!shouldBlink || statusText == null)
            return;

        float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);

        Color color = currentStatusColor;
        color.a = alpha;

        statusText.color = color;
    }

    private void OnDestroy()
    {
        if (bedPatientManager != null)
        {
            bedPatientManager.OnPatientChanged -= ChangePatient;
        }

        UnsubscribeFromPatient();
    }

    private void ChangePatient(PatientVitals newPatient)
    {
        UnsubscribeFromPatient();

        currentPatient = newPatient;

        if (currentPatient == null)
        {
            ShowNoPatient();
            return;
        }

        currentPatientStatus = currentPatient.GetComponent<PatientStatus>();

        currentPatient.OnVitalsChanged += UpdateVitals;

        if (currentPatientStatus != null)
        {
            currentPatientStatus.OnConditionChanged += UpdateCondition;
        }

        UpdateVitals();

        if (currentPatientStatus != null)
        {
            UpdateCondition(currentPatientStatus.CurrentCondition);
        }
    }

    private void UnsubscribeFromPatient()
    {
        if (currentPatient != null)
        {
            currentPatient.OnVitalsChanged -= UpdateVitals;
        }

        if (currentPatientStatus != null)
        {
            currentPatientStatus.OnConditionChanged -= UpdateCondition;
        }

        currentPatient = null;
        currentPatientStatus = null;
    }

    private void UpdateVitals()
    {
        if (currentPatient == null)
        {
            ShowNoPatient();
            return;
        }

        heartRateText.text =
            "Heart Rate: " +
            currentPatient.HeartRate.ToString("0") +
            " BPM";

        oxygenText.text =
            "SpO2: " +
            currentPatient.OxygenSaturation.ToString("0") +
            " %";

        bloodPressureText.text =
            "Blood Pressure: " +
            currentPatient.SystolicPressure.ToString("0") +
            "/" +
            currentPatient.DiastolicPressure.ToString("0") +
            " mmHg";
    }

    private void UpdateCondition(PatientCondition condition)
    {
        switch (condition)
        {
            case PatientCondition.NoPatient:
                ShowNoPatient();
                break;

            case PatientCondition.Normal:
                ShowNormal();
                break;

            case PatientCondition.Warning:
                ShowWarning();
                break;

            case PatientCondition.Critical:
                ShowCritical();
                break;

            case PatientCondition.Expired:
                ShowExpired();
                break;
        }
    }

    private void ShowNoPatient()
    {
        heartRateText.text = "Heart Rate: --";
        oxygenText.text = "SpO2: --";
        bloodPressureText.text = "Blood Pressure: --/--";

        heartRateText.color = notAssignedColor;
        oxygenText.color = notAssignedColor;
        bloodPressureText.color = notAssignedColor;

        statusText.text = "NO PATIENT ON BED";
        statusText.color = notAssignedColor;

        shouldBlink = false;
    }

    private void ShowNormal()
    {
        heartRateText.color = normalColor;
        oxygenText.color = normalColor;
        bloodPressureText.color = normalColor;

        statusText.text = "";
        shouldBlink = false;
    }

    private void ShowWarning()
    {
        heartRateText.color = warningColor;
        oxygenText.color = warningColor;
        bloodPressureText.color = warningColor;

        statusText.text = "WARNING";
        currentStatusColor = warningColor;
        shouldBlink = true;
    }

    private void ShowCritical()
    {
        heartRateText.color = criticalColor;
        oxygenText.color = criticalColor;
        bloodPressureText.color = criticalColor;

        statusText.text = "CRITICAL CONDITION";
        currentStatusColor = criticalColor;
        shouldBlink = true;
    }

    private void ShowExpired()
    {
        heartRateText.color = criticalColor;
        oxygenText.color = criticalColor;
        bloodPressureText.color = criticalColor;

        statusText.text = "PATIENT EXPIRED";
        currentStatusColor = criticalColor;
        shouldBlink = true;
    }
}