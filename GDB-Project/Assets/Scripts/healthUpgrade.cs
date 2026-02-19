using UnityEngine;
using System.Collections;

public class healthUpgrade : MonoBehaviour
{
    [SerializeField] int healthIncrease = 20;
    public AudioClip healthupgradeSound;
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            AudioSource.PlayClipAtPoint(healthupgradeSound, transform.position);
            player.addHealth(healthIncrease);
            Destroy(gameObject); 
        }
    }
}