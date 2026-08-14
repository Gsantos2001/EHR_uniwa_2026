using System;
using UnityEngine;

public class PatientVitals : MonoBehaviour
{
    [Header("Current Vital Signs")]
    [SerializeField] private float heartRate = 75f;
    [SerializeField] private float oxygenSaturation = 98f;
    [SerializeField] private float systolicPressure = 120f;
    [SerializeField] private float diastolicPressure = 80f;

    public float HeartRate => heartRate;
    public float OxygenSaturation => oxygenSaturation;
    public float SystolicPressure => systolicPressure;
    public float DiastolicPressure => diastolicPressure;

    public event Action OnVitalsChanged;

    public void SetHeartRate(float value)
    {
        heartRate = Mathf.Clamp(value, 0f, 250f);
        VitalsChanged();
    }

    public void SetOxygenSaturation(float value)
    {
        oxygenSaturation = Mathf.Clamp(value, 0f, 100f);
        VitalsChanged();
    }

    public void SetBloodPressure(float systolic, float diastolic)
    {
        systolicPressure = Mathf.Clamp(systolic, 0f, 250f);
        diastolicPressure = Mathf.Clamp(diastolic, 0f, 150f);

        VitalsChanged();
    }

    public void SetVitals(
        float newHeartRate,
        float newOxygen,
        float newSystolic,
        float newDiastolic)
    {
        heartRate = Mathf.Clamp(newHeartRate, 0f, 250f);
        oxygenSaturation = Mathf.Clamp(newOxygen, 0f, 100f);
        systolicPressure = Mathf.Clamp(newSystolic, 0f, 250f);
        diastolicPressure = Mathf.Clamp(newDiastolic, 0f, 150f);

        VitalsChanged();
    }

    private void VitalsChanged()
    {
        OnVitalsChanged?.Invoke();
    }
}