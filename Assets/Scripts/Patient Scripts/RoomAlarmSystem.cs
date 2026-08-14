using UnityEngine;

public class RoomAlarmSystem : MonoBehaviour
{
    [Header("Bed")]
    public BedPatientManager bedPatientManager;

    [Header("Emergency Lights")]
    public Light[] emergencyLights;

    [Header("Colors")]
    public Color warningColor = new Color(1f, 0.5f, 0f);
    public Color criticalColor = Color.red;

    [Header("Light Settings")]
    public float warningIntensity = 2f;
    public float criticalIntensity = 4f;
    public float flashSpeed = 4f;

    [Header("Audio")]
    public AudioSource alarmAudioSource;
    public AudioClip warningAlarm;
    public AudioClip criticalAlarm;
    public AudioClip expiredAlarm;

    private PatientStatus currentStatus;

    private bool flashing;
    private Color currentLightColor;
    private float currentLightIntensity;

    private void Start()
    {
        if (bedPatientManager == null)
        {
            Debug.LogError("RoomAlarmSystem: BedPatientManager not assigned.");
            return;
        }

        bedPatientManager.OnPatientChanged += ChangePatient;

        ChangePatient(bedPatientManager.CurrentPatient);
    }

    private void Update()
    {
        if (!flashing)
            return;

        float flashValue = Mathf.PingPong(Time.time * flashSpeed, 1f);

        foreach (Light lightSource in emergencyLights)
        {
            if (lightSource == null)
                continue;

            lightSource.color = currentLightColor;
            lightSource.intensity = currentLightIntensity * flashValue;
        }
    }

    private void OnDestroy()
    {
        if (bedPatientManager != null)
            bedPatientManager.OnPatientChanged -= ChangePatient;

        UnsubscribeFromPatient();
    }

    private void ChangePatient(PatientVitals patient)
    {
        UnsubscribeFromPatient();

        if (patient == null)
        {
            StopAlarm();
            return;
        }

        currentStatus = patient.GetComponent<PatientStatus>();

        if (currentStatus == null)
        {
            StopAlarm();
            return;
        }

        currentStatus.OnConditionChanged += HandleConditionChanged;

        HandleConditionChanged(currentStatus.CurrentCondition);
    }

    private void UnsubscribeFromPatient()
    {
        if (currentStatus != null)
            currentStatus.OnConditionChanged -= HandleConditionChanged;

        currentStatus = null;
    }

    private void HandleConditionChanged(PatientCondition condition)
    {
        switch (condition)
        {
            case PatientCondition.NoPatient:
            case PatientCondition.Normal:
                StopAlarm();
                break;

            case PatientCondition.Warning:
                StartAlarm(
                    warningColor,
                    warningIntensity,
                    warningAlarm
                );
                break;

            case PatientCondition.Critical:
                StartAlarm(
                    criticalColor,
                    criticalIntensity,
                    criticalAlarm
                );
                break;

            case PatientCondition.Expired:
                StartAlarm(
                    criticalColor,
                    criticalIntensity,
                    expiredAlarm
                );
                break;
        }
    }

    private void StartAlarm(Color color, float intensity, AudioClip clip)
    {
        flashing = true;

        currentLightColor = color;
        currentLightIntensity = intensity;

        if (alarmAudioSource != null && clip != null)
        {
            if (alarmAudioSource.clip != clip || !alarmAudioSource.isPlaying)
            {
                alarmAudioSource.Stop();
                alarmAudioSource.clip = clip;
                alarmAudioSource.loop = true;
                alarmAudioSource.Play();
            }
        }
    }

    private void StopAlarm()
    {
        flashing = false;

        foreach (Light lightSource in emergencyLights)
        {
            if (lightSource == null)
                continue;

            lightSource.intensity = 0f;
        }

        if (alarmAudioSource != null)
        {
            alarmAudioSource.Stop();
            alarmAudioSource.clip = null;
        }
    }
}