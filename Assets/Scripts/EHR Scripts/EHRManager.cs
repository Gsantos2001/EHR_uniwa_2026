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

    [Header("Logging")]
    public ScenarioLogger scenarioLogger;
    public ScenarioEngine scenarioEngine;
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

        if (vitalsHistory.Count > 0)
        {
            VitalsHistoryEntry lastEntry =
                vitalsHistory[vitalsHistory.Count - 1];

            bool sameVitals =
                Mathf.Approximately(lastEntry.heartRate, currentPatient.HeartRate) &&
                Mathf.Approximately(lastEntry.oxygenSaturation, currentPatient.OxygenSaturation) &&
                Mathf.Approximately(lastEntry.systolicPressure, currentPatient.SystolicPressure) &&
                Mathf.Approximately(lastEntry.diastolicPressure, currentPatient.DiastolicPressure) &&
                Mathf.Approximately(lastEntry.respiratoryRate, currentPatient.RespiratoryRate) &&
                Mathf.Approximately(lastEntry.temperature, currentPatient.Temperature);

            if (sameVitals)
            {
                return;
            }
        }

        VitalsHistoryEntry entry =
            new VitalsHistoryEntry(
                System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                currentPatient.HeartRate,
                currentPatient.OxygenSaturation,
                currentPatient.SystolicPressure,
                currentPatient.DiastolicPressure,
                currentPatient.RespiratoryRate,
                currentPatient.Temperature
            );

        vitalsHistory.Add(entry);
        if (scenarioLogger != null && scenarioEngine != null && !string.IsNullOrEmpty(scenarioEngine.CurrentNodeId))
        {
            string details =
                "BP=" +
                entry.systolicPressure.ToString("0") +
                "/" +
                entry.diastolicPressure.ToString("0") +
                ", SpO2=" +
                entry.oxygenSaturation.ToString("0") +
                "%, HR=" +
                entry.heartRate.ToString("0") +
                ", RR=" +
                entry.respiratoryRate.ToString("0") +

                ", Temp=" +
                entry.temperature.ToString("0.0");

            scenarioLogger.LogEvent(
                "VITALS_CHANGE",
                scenarioEngine.CurrentNodeId,
                "patient_vitals",
                details,
                scenarioEngine.CurrentScore
            );
        }

        Debug.Log(
            "EHR VITALS: " +
            entry.dateTime +
            " | BP " +
            entry.systolicPressure + "/" +
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