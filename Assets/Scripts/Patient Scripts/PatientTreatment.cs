using UnityEngine;

public class PatientTreatment : MonoBehaviour
{
    [Header("References")]
    public PatientVitals vitals;

    [Header("Oxygen Treatment")]
    [Tooltip("How much oxygen treatment increases SpO2.")]
    public float oxygenIncrease = 5f;

    [Header("IV Fluids")]
    [Tooltip("How much IV fluids increase systolic BP.")]
    public float fluidsSystolicIncrease = 10f;

    [Tooltip("How much IV fluids increase diastolic BP.")]
    public float fluidsDiastolicIncrease = 5f;

    [Header("Vasopressor")]
    [Tooltip("How much vasopressor increases systolic BP.")]
    public float vasopressorSystolicIncrease = 20f;

    [Tooltip("How much vasopressor increases diastolic BP.")]
    public float vasopressorDiastolicIncrease = 10f;

    [Header("Blood Pressure Control")]
    [Tooltip("How much BP control reduces systolic BP.")]
    public float bpControlSystolicReduction = 15f;

    [Tooltip("How much BP control reduces diastolic BP.")]
    public float bpControlDiastolicReduction = 8f;

    [Header("Heart Rate Control")]
    [Tooltip("How much HR control reduces heart rate.")]
    public float heartRateReduction = 15f;

    [Header("Heart Rate Support")]
    [Tooltip("How much HR support increases heart rate.")]
    public float heartRateIncrease = 15f;


    private void Awake()
    {
        // Automatically find PatientVitals on this Patient
        if (vitals == null)
        {
            vitals = GetComponent<PatientVitals>();
        }

        if (vitals == null)
        {
            Debug.LogError(
                "PatientTreatment: No PatientVitals component found."
            );
        }
    }


  
    // OXYGEN
    public void GiveOxygen()
    {
        if (vitals == null)
            return;

        vitals.SetOxygenSaturation(
            vitals.OxygenSaturation + oxygenIncrease
        );

        Debug.Log("Treatment: Oxygen administered");
    }


  
    // IV FLUIDS
    public void GiveIVFluids()
    {
        if (vitals == null)
            return;

        vitals.SetBloodPressure(
            vitals.SystolicPressure + fluidsSystolicIncrease,
            vitals.DiastolicPressure + fluidsDiastolicIncrease
        );

        Debug.Log("Treatment: IV fluids administered");
    }


  
    // VASOPRESSOR
    public void GiveVasopressor()
    {
        if (vitals == null)
            return;

        vitals.SetBloodPressure(
            vitals.SystolicPressure + vasopressorSystolicIncrease,
            vitals.DiastolicPressure + vasopressorDiastolicIncrease
        );

        Debug.Log("Treatment: Vasopressor administered");
    }


  
    // ATIVAN
    public void GiveBloodPressureControl()
    {
        if (vitals == null)
            return;

        vitals.SetBloodPressure(
            vitals.SystolicPressure - bpControlSystolicReduction,
            vitals.DiastolicPressure - bpControlDiastolicReduction
        );

        Debug.Log("Treatment: Ativan administered");
    }


  
    // HEART RATE CONTROL

    public void GiveHeartRateControl()
    {
        if (vitals == null)
            return;

        vitals.SetHeartRate(
            vitals.HeartRate - heartRateReduction
        );

        Debug.Log("Treatment: Heart rate control administered");
    }


  
    // HEART RATE SUPPORT
    public void GiveHeartRateSupport()
    {
        if (vitals == null)
            return;

        vitals.SetHeartRate(
            vitals.HeartRate + heartRateIncrease
        );

        Debug.Log("Treatment: Heart rate support administered");
    }
}