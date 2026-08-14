using System.Collections;
using UnityEngine;

public class DoctorController : MonoBehaviour
{
    [Header("Ρυθμίσεις Κίνησης")]
    public Transform patientLocation; // Το σημείο που θα πάει 
    public float moveSpeed = 3f;
    public float treatmentTime = 2f;  // Πόσο χρόνο θα κάτσει στον ασθενή

    private Vector3 startPosition;
    private bool isMoving = false;

    private Animator animator;

    private void Start()
    {
        // Αποθηκεύουμε την αρχική θέση του γιατρού για να ξέρει πού να γυρίσει
        startPosition = transform.position;
        animator = GetComponent<Animator>();
    }

    // Καλείται από το κουμπί
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

        // Ξεκινάει το περπάτημα προς τον ασθενή
        animator.SetBool("IsWalking", true);
        yield return StartCoroutine(MoveToPosition(patientLocation.position));

        // Έφτασε στον ασθενή: Σταματάει να περπατάει
        animator.SetBool("IsWalking", false);
        
        // Ξεκινάει το animation θεραπείας
        animator.SetTrigger("Treat");
        Debug.Log("Ο γιατρός εξετάζει τον ασθενή...");
        
        // Περιμένει όσο διαρκεί η εξέταση
        yield return new WaitForSeconds(treatmentTime);

        // Ξεκινάει η επιστροφή
        animator.SetBool("IsWalking", true);
        yield return StartCoroutine(MoveToPosition(startPosition));

        // Έφτασε στη βάση του, σταματάει
        animator.SetBool("IsWalking", false);
        
        // Τον βάζουμε να κοιτάξει στην αρχική του κατεύθυνση
        transform.rotation = Quaternion.identity; 
        
        isMoving = false;
        Debug.Log("Ο γιατρός επέστρεψε στη θέση του.");
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Κίνηση
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            // Ομαλή Περιστροφή προς την κατεύθυνση που πάει
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
}