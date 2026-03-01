using System.Collections;
using UnityEngine;

public class TNTStick : MonoBehaviour
{
    public float delay = 3f;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    float countdown;
    //bool hasExploded = false;
    public GameObject explosion;
    public int contactDamage = 50;
    public PlayerState MyOwner;
    public AudioClip explosionSound;

    //void Start()
    //{
    //    //countdown = delay;

    //}
    void Start()
    {
        StartCoroutine(Fuse());
    }

    IEnumerator Fuse()
    {
        yield return new WaitForSeconds(delay);
        Explode();
    }
    //private void Update()
    //{
    //    countdown -= Time.deltaTime;
    //    if(countdown <= 0f && !hasExploded)
    //    {
    //        Explode();
    //        hasExploded = true;
    //    }
    //}

    void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);

        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, explosionRadius);

        AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        foreach (Collider nearbyObject in collidersToDestroy)
        {
            iDamage damageable = nearbyObject.transform.root.GetComponent<iDamage>();
            //correct
            if (damageable != null)
            {
                if (MyOwner == null)
                {
                    Debug.LogWarning("TNTStick: MyOwner is null, assigning to self.");
                    MyOwner = GetComponentInParent<PlayerState>(); 
                }
                damageable.takeDamage(50, MyOwner);
            }
        }

        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider nearbyObject in collidersToMove)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
        Destroy(gameObject);
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (hasExploded) return;

    //    iDamage damageable = collision.transform.root.GetComponent<iDamage>();

    //    if (damageable != null)
    //    {
    //        damageable.takeDamage(contactDamage, MyOwner);
    //    }
    //}
}
