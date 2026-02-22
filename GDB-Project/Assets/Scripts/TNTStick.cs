using UnityEngine;

public class TNTStick : MonoBehaviour
{
    public float delay = 3f;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    float countdown;
    bool hasExploded = false;
    public GameObject explosion;
    public int contactDamage = 25;
    public PlayerState MyOwner;

    void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        countdown -= Time.deltaTime;
        if(countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        Instantiate(explosion, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            iDamage damageable = nearbyObject.transform.root.GetComponent<iDamage>();
            if (damageable != null)
            {
                damageable.takeDamage(50, MyOwner); 
            }
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;

        iDamage damageable = collision.transform.root.GetComponent<iDamage>();

        if (damageable != null)
        {
            damageable.takeDamage(contactDamage, MyOwner);
        }
    }
}
