using System.Collections;
using UnityEngine;

public class WeaponBarrel : MonoBehaviour
{
    [SerializeField] private GameObject gunPref;
    [SerializeField] MeshRenderer mesh;
    [SerializeField] int RespawnTimer = 5;
    enum state { Interactable , PhasedOut}
    state mystate = state.Interactable;
    IEnumerator TimeOut()
    {
        mystate = state.PhasedOut;
        mesh.enabled = false;
        yield return new WaitForSeconds(RespawnTimer);
        mystate = state.Interactable;
        mesh.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if (mystate == state.Interactable)
        {
            iUseWeaponsAndItems player = other.transform.root.GetComponent<iUseWeaponsAndItems>();

            if (player != null)
            {

                player.DropGun();

                Gun newGun = Instantiate(gunPref).GetComponent<Gun>();
                player.EquipGun(newGun);

                StartCoroutine(TimeOut());
            }
        }
    }
}
