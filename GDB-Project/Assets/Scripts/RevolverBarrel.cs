using UnityEngine;

public class RevolverBarrel : MonoBehaviour
{
    [SerializeField] GameObject RevolverPrefab;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.DropGun();

        Gun newGun = Instantiate(RevolverPrefab).GetComponent<Gun>();
        player.EquipGun(newGun);

        Destroy(gameObject);
    }
}
