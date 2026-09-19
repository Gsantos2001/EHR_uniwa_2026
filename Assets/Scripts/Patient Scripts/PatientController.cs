using System.Collections;
using UnityEngine;

public class PatientController : MonoBehaviour
{
    private Animator animator;

    [Header("Ρυθμίσεις Βήχα")]
    public float minCoughTime = 10f;
    public float maxCoughTime = 25f;
    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(CoughRoutine());
    }

    private IEnumerator CoughRoutine()
    {
        while (true) 
        {
            float waitTime = Random.Range(minCoughTime, maxCoughTime);
            yield return new WaitForSeconds(waitTime);

            if (!animator.GetBool("isSeizing"))
            {
                animator.SetTrigger("cough");
                Debug.Log("Ο ασθενής βήχει.");
            }
        }
    }

    public void StartSeizure()
    {
        animator.SetBool("isSeizing", true);
        Debug.Log("ΚΡΙΣΙΜΗ ΚΑΤΑΣΤΑΣΗ: Ο ασθενής έχει σπασμούς!");
    }

    public void StopSeizure()
    {
        animator.SetBool("isSeizing", false);
        Debug.Log("ΣΤΑΘΕΡΟΠΟΙΗΣΗ: Οι σπασμοί σταμάτησαν.");
    }
}