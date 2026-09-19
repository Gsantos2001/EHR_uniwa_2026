using System.Collections;
using UnityEngine;

public class AutomaticDoor : MonoBehaviour
{
    [Header("Ρυθμίσεις Πόρτας")]
    public float openAngle = 90f; // Mοίρες 
    public float openSpeed = 3f;  // Πόσο γρήγορα θα ανοίγει

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine doorCoroutine;

    private void Start()
    {
        closedRotation = transform.rotation;

        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Doctor"))
        {
            if (doorCoroutine != null) StopCoroutine(doorCoroutine);
            doorCoroutine = StartCoroutine(RotateDoor(openRotation));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Doctor"))
        {
            if (doorCoroutine != null) StopCoroutine(doorCoroutine);
            doorCoroutine = StartCoroutine(RotateDoor(closedRotation));
        }
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.rotation = targetRotation;
    }
}