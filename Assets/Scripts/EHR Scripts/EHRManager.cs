using System;
using System.Collections.Generic;
using UnityEngine;

public class EHRManager : MonoBehaviour
{
    [Header("Current Patient")]
    public BedPatientManager bedPatientManager;

    [Header("Patient Information")]
    public PatientBasicInfo patientInfo = new PatientBasicInfo();

    [Header("Assessment Form")]
    public EHRAssessmentData assessment = new EHRAssessmentData();

    [Header("Intervention Form")]
    public EHRInterventionData intervention = new EHRInterventionData();

    [Header("Communication Log")]
    public EHRCommunicationData communication = new EHRCommunicationData();

    [Header("Vitals History")]
    public List<VitalsHistoryEntry> vitalsHistory =
        new List<VitalsHistoryEntry>();

    private PatientVitals currentPatient;

    private void Start()
    {
        if (bedPatientManager == null)
        {
            Debug.LogError("EHRManager: BedPatientManager not assigned.");
            return;
        }

        bedPatientManager.OnPatientChanged += ChangePatient;

        ChangePatient(bedPatientManager.CurrentPatient);
    }

    private void OnDestroy()
    {
        if (bedPatientManager != null)
            bedPatientManager.OnPatientChanged -= ChangePatient;

        UnsubscribeFromPatient();
    }

    private void ChangePatient(PatientVitals newPatient)
    {
        UnsubscribeFromPatient();

        currentPatient = newPatient;

        ClearPatientEHR();

        if (currentPatient == null)
            return;

        currentPatient.OnVitalsChanged += RecordCurrentVitals;

        RecordCurrentVitals();
    }

    private void UnsubscribeFromPatient()
    {
        if (currentPatient != null)
            currentPatient.OnVitalsChanged -= RecordCurrentVitals;

        currentPatient = null;
    }

    public bool HasPatient()
    {
        return currentPatient != null;
    }

    public PatientVitals GetCurrentPatient()
    {
        return currentPatient;
    }

    public void RecordCurrentVitals()
    {
        if (currentPatient == null)
            return;

        VitalsHistoryEntry entry =
            new VitalsHistoryEntry(
                DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                currentPatient.HeartRate,
                currentPatient.OxygenSaturation,
                currentPatient.SystolicPressure,
                currentPatient.DiastolicPressure
            );

        vitalsHistory.Add(entry);

        Debug.Log(
            "EHR VITALS: " +
            entry.dateTime +
            " | BP " +
            entry.systolicPressure +
            "/" +
            entry.diastolicPressure +
            " | SpO2 " +
            entry.oxygenSaturation +
            " | HR " +
            entry.heartRate
        );
    }

    public bool DocumentationGate1Complete()
    {
        return
            !string.IsNullOrWhiteSpace(assessment.observation) &&
            !string.IsNullOrWhiteSpace(intervention.fiO2Setting);
    }

    public bool DocumentationGate2Complete()
    {
        return
            !string.IsNullOrWhiteSpace(communication.recipient) &&
            !string.IsNullOrWhiteSpace(communication.outcome);
    }

    public void ClearPatientEHR()
    {
        assessment = new EHRAssessmentData();
        intervention = new EHRInterventionData();
        communication = new EHRCommunicationData();

        vitalsHistory.Clear();
    }
}