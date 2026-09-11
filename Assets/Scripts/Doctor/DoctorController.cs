using System.Collections;
using UnityEngine;

public class DoctorController : MonoBehaviour
{
    [Header("Ρυθμίσεις Κίνησης")]
    public Transform patientLocation; // Το σημείο που θα πάει 
    public float moveSpeed = 3f;
    public float treatmentTime = 2f;  // Πόσο χρόνο θα κάτσει στον ασθενή

    private Vector3 startPosition;
    private Quaternion startRotation; // Αποθήκευση αρχικής περιστροφής
    private bool isMoving = false;

    private Animator animator;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        
        animator = GetComponent<Animator>();
    }

    public void GoTreatPatient()
    {
        if (!isMoving) 
        {
            StartCoroutine(TreatmentRoutine());
        }
    }

    private IEnumerator TreatmentRoutine()
    {
        isMoving = true;

        animator.SetBool("IsWalking", true);
        yield return StartCoroutine(MoveToPosition(patientLocation.position));

        animator.SetBool("IsWalking", false);
        
        animator.SetTrigger("Treat");
        Debug.Log("Ο γιατρός εξετάζει τον ασθενή...");
        
        yield return new WaitForSeconds(treatmentTime);

        animator.SetBool("IsWalking", true);
        yield return StartCoroutine(MoveToPosition(startPosition));

        animator.SetBool("IsWalking", false);
        
        yield return StartCoroutine(RotateToQuaternion(startRotation));
        
        isMoving = false;
        Debug.Log("Ο γιατρός επέστρεψε στη θέση του.");
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8f);
            }
            
            yield return null;
        }
        
        transform.position = targetPosition; 
    }

    private IEnumerator RotateToQuaternion(Quaternion targetRotation)
    {
        float turnSpeed = 8f;
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
            yield return null;
        }
        
        transform.rotation = targetRotation; // Ensure exact final alignment
    }
}