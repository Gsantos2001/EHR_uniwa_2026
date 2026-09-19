using UnityEngine;

public class PatientTreatment : MonoBehaviour
{
    [Header("References")]
    public PatientVitals vitals;

    [Header("Oxygen Treatment")]
    public float oxygenIncrease = 5f;

    [Header("IV Fluids")]
    public float fluidsSystolicIncrease = 10f;

    public float fluidsDiastolicIncrease = 5f;

    [Header("Vasopressor")]
    public float vasopressorSystolicIncrease = 20f;

    public float vasopressorDiastolicIncrease = 10f;

    [Header("Blood Pressure Control")]
    public float bpControlSystolicReduction = 15f;

    public float bpControlDiastolicReduction = 8f;

    [Header("Heart Rate Control")]
    public float heartRateReduction = 15f;

    [Header("Heart Rate Support")]
    public float heartRateIncrease = 15f;


    private void Awake()
    {
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

    public void GiveOxygen()
    {
        if (vitals == null)
            return;

        vitals.SetOxygenSaturation(
            vitals.OxygenSaturation + oxygenIncrease
        );

        Debug.Log("Treatment: Oxygen administered");
    }

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

    public void GiveHeartRateControl()
    {
        if (vitals == null)
            return;

        vitals.SetHeartRate(
            vitals.HeartRate - heartRateReduction
        );

        Debug.Log("Treatment: Heart rate control administered");
    }

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