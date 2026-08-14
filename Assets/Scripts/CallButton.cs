using UnityEngine;

public class CallButton : MonoBehaviour
{
    public DoctorController doctor; // Σύνδεση με γιατρό

    public void PressButton()
    {
        Debug.Log("Button pressed");
        if (doctor != null)
        {
            doctor.GoTreatPatient();
        }
        else
        {
            Debug.LogWarning("Something went wrong");
        }
    }
}