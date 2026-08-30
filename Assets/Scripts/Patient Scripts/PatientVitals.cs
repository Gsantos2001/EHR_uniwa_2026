using System;
using UnityEngine;

public class PatientVitals : MonoBehaviour
{
    [Header("Current Vital Signs (Loaded from JSON)")]
    [SerializeField] private float heartRate;
    [SerializeField] private float oxygenSaturation;
    [SerializeField] private string bloodPressure; 
    [SerializeField] private float respiratoryRate; 
    [SerializeField] private float temperature;

    public float HeartRate => heartRate;
    public float OxygenSaturation => oxygenSaturation;
    public string BloodPressure => bloodPressure;
    public float RespiratoryRate => respiratoryRate;
    public float Temperature => temperature;

    // ΠΡΟΣΘΗΚΗ: Κρατάμε τις παλιές μεταβλητές για να μην βγάζουν error τα PatientStatus, PatientTreatment κλπ.
    public float SystolicPressure { get; private set; } = 120f;
    public float DiastolicPressure { get; private set; } = 80f;

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

    // Η ΝΕΑ μέθοδος για το JSON
    public void SetBloodPressure(string value)
    {
        bloodPressure = value;

        // Προσπαθούμε να ενημερώσουμε και τις παλιές μεταβλητές αν το κείμενο είναι στη μορφή "125/80"
        if (!string.IsNullOrEmpty(value) && value.Contains("/"))
        {
            string[] parts = value.Split('/');
            if (parts.Length == 2 && float.TryParse(parts[0], out float sys) && float.TryParse(parts[1], out float dia))
            {
                SystolicPressure = sys;
                DiastolicPressure = dia;
            }
        }
        
        VitalsChanged();
    }

    // Η ΠΑΛΙΑ μέθοδος για να μην χτυπάνε error τα παλιά σου scripts
    public void SetBloodPressure(float systolic, float diastolic)
    {
        SystolicPressure = Mathf.Clamp(systolic, 0f, 250f);
        DiastolicPressure = Mathf.Clamp(diastolic, 0f, 150f);
        bloodPressure = SystolicPressure.ToString("0") + "/" + DiastolicPressure.ToString("0");
        VitalsChanged();
    }

    public void SetRespiratoryRate(float value)
    {
        respiratoryRate = Mathf.Clamp(value, 0f, 60f);
        VitalsChanged();
    }

    public void SetTemperature(float value)
    {
        temperature = Mathf.Clamp(value, 30f, 45f);
        VitalsChanged();
    }

    public void SetVitals(
        float newHeartRate,
        float newOxygen,
        string newBloodPressure,
        float newRespiratoryRate,
        float newTemperature)
    {
        heartRate = Mathf.Clamp(newHeartRate, 0f, 250f);
        oxygenSaturation = Mathf.Clamp(newOxygen, 0f, 100f);
        SetBloodPressure(newBloodPressure); // Χρησιμοποιεί τη μέθοδο παραπάνω
        respiratoryRate = Mathf.Clamp(newRespiratoryRate, 0f, 60f);
        temperature = Mathf.Clamp(newTemperature, 30f, 45f);

        VitalsChanged();
    }

    private void VitalsChanged()
    {
        OnVitalsChanged?.Invoke();
    }
}