using UnityEngine;

public class PatientMonitorDisplay : MonoBehaviour
{
    [Header("Bed")]
    public BedPatientManager bedPatientManager;

    [Header("Monitor Screen")]
    public Renderer screenRenderer;

    [Tooltip("Usually _BaseMap for URP or _MainTex for Standard materials.")]
    public string textureProperty = "_BaseMap";

    [Header("Monitor Textures")]
    public Texture noPatientTexture;
    public Texture normalTexture;
    public Texture warningTexture;
    public Texture criticalTexture;
    public Texture expiredTexture;

    private PatientStatus currentPatientStatus;

    private void Start()
    {
        if (bedPatientManager == null)
        {
            Debug.LogError(
                "PatientMonitorDisplay: BedPatientManager not assigned."
            );

            return;
        }

        if (screenRenderer == null)
        {
            Debug.LogError(
                "PatientMonitorDisplay: Screen Renderer not assigned."
            );

            return;
        }

        bedPatientManager.OnPatientChanged += ChangePatient;

        ChangePatient(bedPatientManager.CurrentPatient);
    }

    private void OnDestroy()
    {
        if (bedPatientManager != null)
        {
            bedPatientManager.OnPatientChanged -= ChangePatient;
        }

        UnsubscribeFromPatient();
    }

    private void ChangePatient(PatientVitals patient)
    {
        UnsubscribeFromPatient();

        if (patient == null)
        {
            SetMonitorTexture(noPatientTexture);
            return;
        }

        currentPatientStatus = patient.GetComponent<PatientStatus>();

        if (currentPatientStatus == null)
        {
            Debug.LogWarning(
                "PatientMonitorDisplay: Patient has no PatientStatus."
            );

            SetMonitorTexture(noPatientTexture);
            return;
        }

        currentPatientStatus.OnConditionChanged += UpdateMonitor;

        UpdateMonitor(currentPatientStatus.CurrentCondition);
    }

    private void UnsubscribeFromPatient()
    {
        if (currentPatientStatus != null)
        {
            currentPatientStatus.OnConditionChanged -= UpdateMonitor;
        }

        currentPatientStatus = null;
    }

    private void UpdateMonitor(PatientCondition condition)
    {
        switch (condition)
        {
            case PatientCondition.NoPatient:
                SetMonitorTexture(noPatientTexture);
                break;

            case PatientCondition.Normal:
                SetMonitorTexture(normalTexture);
                break;

            case PatientCondition.Warning:
                SetMonitorTexture(warningTexture);
                break;

            case PatientCondition.Critical:
                SetMonitorTexture(criticalTexture);
                break;

            case PatientCondition.Expired:
                SetMonitorTexture(expiredTexture);
                break;
        }
    }

    private void SetMonitorTexture(Texture newTexture)
    {
        if (screenRenderer == null || newTexture == null)
            return;

        screenRenderer.material.SetTexture(
            textureProperty,
            newTexture
        );
    }
}