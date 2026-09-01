using UnityEngine;
using TMPro;

public class BedsideMonitorDisplay : MonoBehaviour
{
    [Header("Patient Source")]
    public BedPatientManager bedPatientManager;

    [Header("5 Scenario Vitals UI Elements")]
    public TMP_Text heartRateText;
    public TMP_Text spo2Text;
    public TMP_Text bloodPressureText;
    public TMP_Text respRateText;
    public TMP_Text tempText;

    [Header("Waveform UI Elements")]
    public WaveformLineRenderer ecgWaveform;
    public WaveformLineRenderer spo2Waveform;
    public WaveformLineRenderer respWaveform;

    [Header("Vital Threshold Colors")]
    public Color normalColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color criticalColor = Color.red;

    private PatientVitals currentPatient;

    private void Start()
    {
        if (bedPatientManager != null)
        {
            bedPatientManager.OnPatientChanged += OnPatientChanged;
            OnPatientChanged(bedPatientManager.CurrentPatient);
        }
    }

    private void Update()
    {
        if (currentPatient == null && bedPatientManager != null && bedPatientManager.CurrentPatient != null)
        {
            OnPatientChanged(bedPatientManager.CurrentPatient);
        }
    }

    private void OnDestroy()
    {
        if (bedPatientManager != null)
        {
            bedPatientManager.OnPatientChanged -= OnPatientChanged;
        }

        Unsubscribe();
    }

    private void OnPatientChanged(PatientVitals newPatient)
    {
        Unsubscribe();

        currentPatient = newPatient;

        if (currentPatient != null)
        {
            currentPatient.OnVitalsChanged += RefreshDisplay;
            RefreshDisplay();
        }
        else
        {
            ShowFlatline();
        }
    }

    private void Unsubscribe()
    {
        if (currentPatient != null)
        {
            currentPatient.OnVitalsChanged -= RefreshDisplay;
        }
    }

    public void RefreshDisplay()
    {
        if (currentPatient == null) return;

        // 1. Heart Rate
        float hr = currentPatient.HeartRate;
        Color hrColor = (hr >= 60f && hr <= 100f) ? normalColor : criticalColor;
        if (heartRateText != null)
        {
            heartRateText.text = $"{hr:0} BPM";
            heartRateText.color = hrColor;
        }
        if (ecgWaveform != null)
        {
            ecgWaveform.frequency = hr / 60f; 
            ecgWaveform.color = hrColor;
            ecgWaveform.waveType = hr > 0 ? WaveformLineRenderer.WaveType.ECG : WaveformLineRenderer.WaveType.Flatline;
        }

        // 2. SpO2
        float spo2 = currentPatient.OxygenSaturation;
        Color spo2Color = normalColor;
        if (spo2 >= 95f) spo2Color = normalColor;
        else if (spo2 >= 90f) spo2Color = warningColor;
        else spo2Color = criticalColor;

        if (spo2Text != null)
        {
            spo2Text.text = $"{spo2:0}%";
            spo2Text.color = spo2Color;
        }
        if (spo2Waveform != null)
        {
            spo2Waveform.frequency = hr / 60f; 
            spo2Waveform.color = spo2Color;
            spo2Waveform.waveType = spo2 > 0 ? WaveformLineRenderer.WaveType.SpO2 : WaveformLineRenderer.WaveType.Flatline;
        }

        // 3. Blood Pressure
        if (bloodPressureText != null)
        {
            string bp = string.IsNullOrEmpty(currentPatient.BloodPressure) ? "--/--" : currentPatient.BloodPressure;
            bloodPressureText.text = $"{bp}\nmmHg";
            
            float sys = currentPatient.SystolicPressure;
            bloodPressureText.color = (sys >= 90f && sys <= 120f) ? normalColor : warningColor;
        }

        // 4. Respiration Rate
        float rr = currentPatient.RespiratoryRate;
        Color rrColor = (rr >= 12f && rr <= 20f) ? normalColor : warningColor;
        if (respRateText != null)
        {
            respRateText.text = $"{rr:0} / min";
            respRateText.color = rrColor;
        }
        if (respWaveform != null)
        {
            respWaveform.frequency = rr / 60f; 
            respWaveform.color = rrColor;
            respWaveform.waveType = rr > 0 ? WaveformLineRenderer.WaveType.Respiration : WaveformLineRenderer.WaveType.Flatline;
        }

        // 5. Temperature
        float temp = currentPatient.Temperature;
        if (tempText != null)
        {
            tempText.text = $"{temp:0.0} °C";
            tempText.color = (temp >= 36.5f && temp <= 37.5f) ? normalColor : warningColor;
        }
    }

    private void ShowFlatline()
    {
        if (heartRateText != null) { heartRateText.text = "--- BPM"; heartRateText.color = criticalColor; }
        if (spo2Text != null) { spo2Text.text = "--- %"; spo2Text.color = criticalColor; }
        if (bloodPressureText != null) { bloodPressureText.text = "--/--\nmmHg"; bloodPressureText.color = criticalColor; }
        if (respRateText != null) { respRateText.text = "--- / min"; respRateText.color = criticalColor; }
        if (tempText != null) { tempText.text = "--.- °C"; tempText.color = criticalColor; }

        if (ecgWaveform != null) { ecgWaveform.waveType = WaveformLineRenderer.WaveType.Flatline; ecgWaveform.color = criticalColor; }
        if (spo2Waveform != null) { spo2Waveform.waveType = WaveformLineRenderer.WaveType.Flatline; spo2Waveform.color = criticalColor; }
        if (respWaveform != null) { respWaveform.waveType = WaveformLineRenderer.WaveType.Flatline; respWaveform.color = criticalColor; }
    }
}