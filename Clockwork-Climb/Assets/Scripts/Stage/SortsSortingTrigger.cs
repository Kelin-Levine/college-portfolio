using System.Collections;
using UnityEngine;

public class SortsSortingTrigger : MonoBehaviour
{
    // Configuration
    [Tooltip("Sorts ID of cuckoos to accept for this trigger.")]
    public int sortID;
    [Tooltip("Particle effects for when a cuckoo is incorrectly sorted.")]
    public ParticleSystem[] failSortParticles;
    [Tooltip("Bonus level manager of this scene.")]
    public BonusLevelManager bonusLevelManager;


    // Instance Functions
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cuckoo"))
        {
            Cuckoo cuckoo = other.GetComponent<Cuckoo>();
            cuckoo.IsFacingRight = true;
            if (cuckoo.sortsID == sortID)
            {
                // Correctly sorted! Hooray!
                bonusLevelManager.SortedCuckoo();
            }
            else
            {
                // Incorrectly sorted. Boo.
                BonusLevelManager.CuckooMissedSortEvent.Invoke(other.gameObject);
                StartCoroutine(PlayFailParticles());
            }
        }
    }

    private IEnumerator PlayFailParticles()
    {
        yield return null;
        foreach (ParticleSystem particle in failSortParticles) particle.Play();
    }
}
