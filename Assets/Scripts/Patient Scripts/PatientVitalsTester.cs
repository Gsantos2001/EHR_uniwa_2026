using UnityEngine;
using UnityEngine.InputSystem;

public class PatientVitalsTester : MonoBehaviour
{
    public PatientVitals patient;
    public BedPatientManager bedPatientManager;
    public PatientTreatment treatment;
    private void Update()
    {
        if (Keyboard.current == null)
            return;

        bool shiftHeld =
            Keyboard.current.leftShiftKey.isPressed ||
            Keyboard.current.rightShiftKey.isPressed;

        if (!shiftHeld)
            return;

        // Shift + Numpad 0 = Put patient on bed
        if (Keyboard.current.numpad0Key.wasPressedThisFrame)
        {
            if (!bedPatientManager.HasPatient())
            {
                bedPatientManager.AssignPatient(patient);
                Debug.Log("TEST: Patient placed on bed");
            }

            return;
        }

        // Shift + Numpad . = Remove patient from bed
        if (Keyboard.current.numpadPeriodKey.wasPressedThisFrame)
        {
            if (bedPatientManager.HasPatient())
            {
                bedPatientManager.RemovePatient();
                Debug.Log("TEST: Patient removed from bed");
            }

            return;
        }

        // Do not change vitals if the bed is empty
        if (!bedPatientManager.HasPatient())
        {
            Debug.Log("TEST: Cannot change vitals - no patient on bed");
            return;
        }

        // Shift + Numpad 1 = Normal
        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            patient.SetVitals(75f, 98f, 120f, 80f);
            Debug.Log("Patient vitals set to NORMAL");
        }

        // Shift + Numpad 2 = High Heart Rate
        if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            patient.SetVitals(135f, 97f, 125f, 82f);
            Debug.Log("Patient vitals set to HIGH HEART RATE");
        }

        // Shift + Numpad 3 = Low Oxygen
        if (Keyboard.current.numpad3Key.wasPressedThisFrame)
        {
            patient.SetVitals(95f, 84f, 115f, 75f);
            Debug.Log("Patient vitals set to LOW OXYGEN");
        }

        // Shift + Numpad 4 = Critical
        if (Keyboard.current.numpad4Key.wasPressedThisFrame)
        {
            patient.SetVitals(155f, 78f, 75f, 40f);
            Debug.Log("Patient vitals set to CRITICAL");
        }

        // Shift + Numpad 5 = Expired
        if (Keyboard.current.numpad5Key.wasPressedThisFrame)
        {
            patient.SetVitals(0f, 0f, 0f, 0f);
            Debug.Log("Patient vitals set to EXPIRED");
        }

        // Shift + Numpad 6 = Absurd BP and O2 levels
        if (Keyboard.current.numpad6Key.wasPressedThisFrame)
        {
            patient.SetVitals(9000f, 105f, 120f, 95f);
            Debug.Log("Patient vitals set to 9000 HEART RATE and 105 OXYGEN");
        }
        // Shift + Numpad 7 = Absurd blood pressure
        if (Keyboard.current.numpad7Key.wasPressedThisFrame)
        {
            patient.SetVitals(95, 97, 250f, 150f);
            Debug.Log("Patient vitals set to 250 SYSTOLIC and 150 DIASTOLIC");
        }
        // Shift + Numpad 8 = Absurd all vital signs
        if (Keyboard.current.numpad8Key.wasPressedThisFrame)
        {
            patient.SetVitals(9000f, 105f, 250f, 150f);
            Debug.Log("Patient vitals set to 9000 HEART RATE, 105 OXYGEN, 250 SYSTOLIC and 150 DIASTOLIC");
        }
        // Shift + F1 = Oxygen
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            treatment.GiveOxygen();
        }

        // Shift + F2 = IV Fluids
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            treatment.GiveIVFluids();
        }

        // Shift + F3 = Vasopressor
        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            treatment.GiveVasopressor();
        }

        // Shift + F4 = Ativan
        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            treatment.GiveBloodPressureControl();
        }

        // Shift + F5 = Heart Rate Control
        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            treatment.GiveHeartRateControl();
        }

        // Shift + F6 = Heart Rate Support
        if (Keyboard.current.f6Key.wasPressedThisFrame)
        {
            treatment.GiveHeartRateSupport();
        }
    }
}