using UnityEngine;

public class WeaponBarrel : MonoBehaviour
{
    [SerializeField] private GameObject gunPref;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.DropGun();

        Gun newGun = Instantiate(gunPref).GetComponent<Gun>();
        player.EquipGun(newGun);

        Destroy(gameObject);
    }
}
