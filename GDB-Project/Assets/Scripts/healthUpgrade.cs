using UnityEngine;
using System.Collections;

public class healthUpgrade : MonoBehaviour
{
    [SerializeField] int healthIncrease = 20;
    public AudioClip healthupgradeSound;
    [SerializeField] MeshRenderer mesh;
    [SerializeField] int RespawnTimer = 3;
    enum state { Interactable, PhasedOut }
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
            iUseWeaponsAndItems player = other.GetComponent<iUseWeaponsAndItems>();

            if (player != null)
            {
                {
                    AudioSource.PlayClipAtPoint(healthupgradeSound, transform.position);
                    player.addHealth(healthIncrease);
                    StartCoroutine(TimeOut());
                }
            }
        }
    }
}