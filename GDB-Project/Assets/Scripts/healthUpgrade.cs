using UnityEngine;

public class healthUpgrade : MonoBehaviour
{
    [SerializeField] int healthIncrease = 20;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.addHealth(healthIncrease);
            Destroy(gameObject); 
        }
    }
}