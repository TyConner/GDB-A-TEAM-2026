using UnityEngine;

public class ShotgunBarrel : MonoBehaviour
{
    [SerializeField] GameObject ShotgunPrefab;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.DropGun();

        Gun newGun = Instantiate(ShotgunPrefab).GetComponent<Gun>();
        player.EquipGun(newGun);

        Destroy(gameObject);
    }
}
