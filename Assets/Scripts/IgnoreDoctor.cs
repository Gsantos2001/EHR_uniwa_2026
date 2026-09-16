using UnityEngine;

public class IgnoreDoctor : MonoBehaviour
{
    [Header("Τα αντικείμενα που δεν θα συγκρούονται")]
    public Collider doctorCollider;
    public Collider blockerCollider;

    private void Start()
    {
        if (doctorCollider != null && blockerCollider != null)
        {
            Physics.IgnoreCollision(doctorCollider, blockerCollider, true);
        }
        else
        {
            Debug.LogWarning("Λείπουν τα Colliders στο script IgnoreDoctor!");
        }
    }
}