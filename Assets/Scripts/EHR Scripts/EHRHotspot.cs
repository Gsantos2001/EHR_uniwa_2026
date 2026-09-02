using UnityEngine;

public class EHRHotspot : MonoBehaviour
{
    public EHRUI ehrUI;

    public void OpenEHR()
    {
        if (ehrUI != null)
        {
            ehrUI.OpenEHR();
        }
        else
        {
            Debug.LogError("EHRHotspot: EHRUI not assigned.");
        }
    }
}