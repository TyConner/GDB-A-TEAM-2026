using UnityEngine;
using System.Collections;

public class healthUpgrade : MonoBehaviour
{
    [SerializeField] int healthIncrease = 20;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            player.addHealth(healthIncrease);
            Destroy(gameObject); 
        }
    }
}