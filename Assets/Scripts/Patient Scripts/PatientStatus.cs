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



    // WARNING THRESHOLDS
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



    // CRITICAL THRESHOLDS
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



    // CURRENT CONDITION
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



    // EVALUATE PATIENT
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



    // EXPIRED
    private bool IsExpired()
    {
        return patient.HeartRate <= 0f &&
               patient.OxygenSaturation <= 0f &&
               patient.SystolicPressure <= 0f &&
               patient.DiastolicPressure <= 0f;
    }



    // CRITICAL
    private bool IsCritical()
    {
        return

            // Heart Rate
            patient.HeartRate >= criticalHeartRateHigh ||
            patient.HeartRate <= criticalHeartRateLow ||

            // Oxygen
            patient.OxygenSaturation <= criticalOxygenLow ||

            // Blood Pressure Too Low
            patient.SystolicPressure <= criticalSystolicLow ||
            patient.DiastolicPressure <= criticalDiastolicLow ||

            // Blood Pressure Too High
            patient.SystolicPressure >= criticalSystolicHigh ||
            patient.DiastolicPressure >= criticalDiastolicHigh;
    }



    // WARNING
    private bool IsWarning()
    {
        return

            // Heart Rate
            patient.HeartRate >= warningHeartRateHigh ||
            patient.HeartRate <= warningHeartRateLow ||

            // Oxygen
            patient.OxygenSaturation <= warningOxygenLow ||

            // Blood Pressure Too Low
            patient.SystolicPressure <= warningSystolicLow ||
            patient.DiastolicPressure <= warningDiastolicLow ||

            // Blood Pressure Too High
            patient.SystolicPressure >= warningSystolicHigh ||
            patient.DiastolicPressure >= warningDiastolicHigh;
    }
}