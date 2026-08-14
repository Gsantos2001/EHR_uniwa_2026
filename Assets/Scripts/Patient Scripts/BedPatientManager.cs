using System;
using UnityEngine;

public class BedPatientManager : MonoBehaviour
{
    [Header("Current Patient")]
    [SerializeField] private PatientVitals currentPatient;

    public PatientVitals CurrentPatient => currentPatient;

    public event Action<PatientVitals> OnPatientChanged;

    public void AssignPatient(PatientVitals newPatient)
    {
        currentPatient = newPatient;

        OnPatientChanged?.Invoke(currentPatient);

        Debug.Log("Patient assigned to bed");
    }

    public void RemovePatient()
    {
        currentPatient = null;

        OnPatientChanged?.Invoke(null);

        Debug.Log("Patient removed from bed");
    }

    public bool HasPatient()
    {
        return currentPatient != null;
    }
}