using UnityEngine;

public class TNTThrower : MonoBehaviour
{
    public float throwForce = 20f;
    public GameObject tntPref;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            ThrowExplosive();
        }
    }

    void ThrowExplosive()
    {
        GameObject tnt = Instantiate(tntPref, transform.position, transform.rotation);
        Rigidbody rb = tnt.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
    }
}
