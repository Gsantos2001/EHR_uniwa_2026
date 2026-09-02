using System;

[Serializable]
public class PatientBasicInfo
{
    public string fullName;
    public int age;
    public string location;
    public string admissionDiagnosis;
}

[Serializable]
public class EHRAssessmentData
{
    public string observation;
    public string skinColor;
    public string consciousness;
}

[Serializable]
public class EHRInterventionData
{
    public string device;
    public string fiO2Setting;
    public string flowRate;
}

[Serializable]
public class EHRCommunicationData
{
    public string recipient;
    public string reason;
    public string outcome;
}