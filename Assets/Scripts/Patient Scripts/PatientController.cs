using System.Collections;
using UnityEngine;

public class PatientController : MonoBehaviour
{
    private Animator animator;

    [Header("Ρυθμίσεις Βήχα")]
    public float minCoughTime = 10f; // Ελάχιστος χρόνος μεταξύ βήχα (σε δευτερόλεπτα)
    public float maxCoughTime = 25f; // Μέγιστος χρόνος μεταξύ βήχα

    private void Start()
    {
        animator = GetComponent<Animator>();
        // Ξεκινάει η λούπα του βήχα
        StartCoroutine(CoughRoutine());
    }

    private IEnumerator CoughRoutine()
    {
        while (true) // Ατέρμονη λούπα
        {
            // Περιμένει έναν τυχαίο χρόνο π.χ. μεταξύ 10 και 25 δευτερολέπτων
            float waitTime = Random.Range(minCoughTime, maxCoughTime);
            yield return new WaitForSeconds(waitTime);

            // Ελέγχει αν ΔΕΝ έχει σπασμούς αυτή τη στιγμή. 
            // Αν είναι καλά, τότε ρίχνει έναν βήχα.
            if (!animator.GetBool("isSeizing"))
            {
                animator.SetTrigger("cough");
                Debug.Log("Ο ασθενής βήχει.");
            }
        }
    }


    // Κάλεσε αυτή τη μέθοδο όταν οι ενδείξεις στο EHR γίνουν κακές
    public void StartSeizure()
    {
        animator.SetBool("isSeizing", true);
        Debug.Log("ΚΡΙΣΙΜΗ ΚΑΤΑΣΤΑΣΗ: Ο ασθενής έχει σπασμούς!");
    }

    // Κάλεσε αυτή τη μέθοδο όταν ο παίκτης/γιατρός φτιάξει τις ενδείξεις
    public void StopSeizure()
    {
        animator.SetBool("isSeizing", false);
        Debug.Log("ΣΤΑΘΕΡΟΠΟΙΗΣΗ: Οι σπασμοί σταμάτησαν.");
    }
}