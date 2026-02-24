using UnityEngine;

public class TNTPickup : MonoBehaviour
{
    public int tntAmount = 3;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerTNT = other.GetComponent<PlayerController>();

            if (playerTNT != null)
            {
                playerTNT.AddTNT(tntAmount);
            }

            Destroy(gameObject); 
        }
    }
}
