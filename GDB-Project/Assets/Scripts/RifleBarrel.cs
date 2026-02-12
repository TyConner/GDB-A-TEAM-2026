using UnityEngine;

public class RifleBarrel : MonoBehaviour
{
    [SerializeField] GameObject RiflePrefab;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.DropGun();

        Gun newGun = Instantiate(RiflePrefab).GetComponent<Gun>();
        player.EquipGun(newGun);

        Destroy(gameObject);
    }

}
