using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [SerializeField] int DamageAmount;
    [SerializeField] int Speed = 1;
    [SerializeField] float Lifetime = 0f;
    [SerializeField] ParticleSystem HitEffect;

    public PlayerState MyOwner;

    private void Awake()
    {
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
    void Start()
    {

        rb.linearVelocity = transform.forward * Speed;
        if (Lifetime != 0f)
        {
            Destroy(gameObject, Lifetime);
        }

    }

    private void Update()
    {
        RaycastHit hit;
        if (rb.SweepTest(transform.forward, out hit, Speed * Time.deltaTime))
        {
            HandleHit(hit.collider);
        }

    }

    private void HandleHit(Collider other)
    {
        print("Projectile Hit: "+ other.name);
        if (other.isTrigger || other.GetComponent<PlayerState>() == MyOwner)
        {
            
            return;
        }

        iDamage dmg = other.GetComponent<iDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(DamageAmount, MyOwner);
        }

        if (HitEffect != null)
        {
            Instantiate(HitEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
