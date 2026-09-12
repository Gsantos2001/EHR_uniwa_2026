using System;

[Serializable]
public class VitalsHistoryEntry
{
    public string dateTime;
    public float heartRate;
    public float oxygenSaturation;
    public float systolicPressure;
    public float diastolicPressure;
    public float respiratoryRate;
    public float temperature;


    public VitalsHistoryEntry(
        string dateTime,
        float heartRate,
        float oxygenSaturation,
        float systolicPressure,
        float diastolicPressure,
        float respiratoryRate,
        float temperature)
    {
        this.dateTime = dateTime;

        this.heartRate = heartRate;
        this.oxygenSaturation = oxygenSaturation;

        this.systolicPressure = systolicPressure;
        this.diastolicPressure = diastolicPressure;

        this.respiratoryRate = respiratoryRate;
        this.temperature = temperature;
    }
}