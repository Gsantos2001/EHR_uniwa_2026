using System;
using UnityEngine;

public enum PatientCondition
{
    NoPatient,
    Normal,
    Warning,
    Critical,
    Expired
}

public class PatientStatus : MonoBehaviour
{
    [Header("Patient")]
    public PatientVitals patient;

    [Header("Heart Rate - Warning")]
    public float warningHeartRateHigh = 120f;
    public float warningHeartRateLow = 50f;

    [Header("Oxygen - Warning")]
    public float warningOxygenLow = 92f;

    [Header("Blood Pressure - Warning Low")]
    public float warningSystolicLow = 90f;
    public float warningDiastolicLow = 55f;

    [Header("Blood Pressure - Warning High")]
    public float warningSystolicHigh = 150f;
    public float warningDiastolicHigh = 95f;

    [Header("Heart Rate - Critical")]
    public float criticalHeartRateHigh = 150f;
    public float criticalHeartRateLow = 35f;

    [Header("Oxygen - Critical")]
    public float criticalOxygenLow = 85f;

    [Header("Blood Pressure - Critical Low")]
    public float criticalSystolicLow = 80f;
    public float criticalDiastolicLow = 45f;

    [Header("Blood Pressure - Critical High")]
    public float criticalSystolicHigh = 180f;
    public float criticalDiastolicHigh = 120f;

    public PatientCondition CurrentCondition { get; private set; }

    public event Action<PatientCondition> OnConditionChanged;

    private void Start()
    {
        if (patient != null)
        {
            patient.OnVitalsChanged += EvaluateCondition;
        }

        EvaluateCondition();
    }

    private void OnDestroy()
    {
        if (patient != null)
        {
            patient.OnVitalsChanged -= EvaluateCondition;
        }
    }

    public void EvaluateCondition()
    {
        PatientCondition newCondition;

        if (patient == null)
        {
            newCondition = PatientCondition.NoPatient;
        }
        else if (IsExpired())
        {
            newCondition = PatientCondition.Expired;
        }
        else if (IsCritical())
        {
            newCondition = PatientCondition.Critical;
        }
        else if (IsWarning())
        {
            newCondition = PatientCondition.Warning;
        }
        else
        {
            newCondition = PatientCondition.Normal;
        }

        if (newCondition != CurrentCondition)
        {
            CurrentCondition = newCondition;

            OnConditionChanged?.Invoke(CurrentCondition);
        }
    }

    private bool IsExpired()
    {
        return patient.HeartRate <= 0f &&
               patient.OxygenSaturation <= 0f &&
               patient.SystolicPressure <= 0f &&
               patient.DiastolicPressure <= 0f;
    }

    private bool IsCritical()
    {
        return

            patient.HeartRate >= criticalHeartRateHigh ||
            patient.HeartRate <= criticalHeartRateLow ||

            patient.OxygenSaturation <= criticalOxygenLow ||

            patient.SystolicPressure <= criticalSystolicLow ||
            patient.DiastolicPressure <= criticalDiastolicLow ||

            patient.SystolicPressure >= criticalSystolicHigh ||
            patient.DiastolicPressure >= criticalDiastolicHigh;
    }

    private bool IsWarning()
    {
        return
            patient.HeartRate >= warningHeartRateHigh ||
            patient.HeartRate <= warningHeartRateLow ||

            patient.OxygenSaturation <= warningOxygenLow ||

            patient.SystolicPressure <= warningSystolicLow ||
            patient.DiastolicPressure <= warningDiastolicLow ||

            patient.SystolicPressure >= warningSystolicHigh ||
            patient.DiastolicPressure >= warningDiastolicHigh;
    }
}