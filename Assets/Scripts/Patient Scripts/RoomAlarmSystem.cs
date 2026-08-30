using UnityEngine;

public class RoomAlarmSystem : MonoBehaviour
{
    [Header("Engine Reference")]
    [Tooltip("Σύνδεσε εδώ το GameObject που έχει το ScenarioEngine")]
    public ScenarioEngine scenarioEngine;

    [Header("Emergency Lights")]
    public Light[] emergencyLights;

    [Header("Colors")]
    public Color criticalColor = Color.red;

    [Header("Light Settings")]
    public float criticalIntensity = 4f;
    public float flashSpeed = 4f;

    [Header("Audio")]
    public AudioSource alarmAudioSource;
    public AudioClip criticalAlarm; 

    private bool flashing;
    private Color currentLightColor;
    private float currentLightIntensity;

    private void Start()
    {
        if (scenarioEngine == null)
        {
            Debug.LogError("RoomAlarmSystem: ScenarioEngine δεν έχει συνδεθεί.");
            return;
        }

        // Ακούμε το δυναμικό event του JSON Engine
        scenarioEngine.OnAlarmStateChanged += HandleAlarmState;
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
        if (scenarioEngine != null)
            scenarioEngine.OnAlarmStateChanged -= HandleAlarmState;
    }

    private void HandleAlarmState(bool isActive)
    {
        // Αν το ScenarioEngine (βάσει JSON) πει ότι έχουμε συναγερμό
        if (isActive)
        {
            StartAlarm(criticalColor, criticalIntensity, criticalAlarm);
        }
        else
        {
            StopAlarm();
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