using UnityEngine;

public class TNTThrower : MonoBehaviour
{
    public float throwForce = 20f;
    public GameObject tntPref;
    public PlayerState MyPlayerState;
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("TNTThrower: No PlayerController found in parent hierarchy!");
        }

        if (tntPref == null)
        {
            Debug.LogError("TNTThrower: tntPref is not assigned!");
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            if (playerController != null && playerController.UseTNT())
            {
                ThrowExplosive();
                Debug.Log("TNT thrown, remaining: " + playerController.currentTNT);
            }
            else
            {
                Debug.Log("No TNT left or PlayerController missing!");
            }
        }
    }

    void ThrowExplosive()
    {
        GameObject tnt = Instantiate(tntPref, transform.position, transform.rotation);

        TNTStick stick = tnt.GetComponent<TNTStick>();
        if (stick != null)
        {
            stick.MyOwner = MyPlayerState; // <-- assign the player who threw it
        }

        Rigidbody rb = tnt.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
        }
    }
}