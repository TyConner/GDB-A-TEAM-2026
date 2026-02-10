using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [SerializeField] int DamageAmount;
    [SerializeField] int Speed = 1;
    [SerializeField] float Lifetime = 0f;
    [SerializeField] ParticleSystem HitEffect;

    public PlayerState MyOwner;
    void Start()
    {

        rb.linearVelocity = transform.forward * Speed;
        if (Lifetime != 0f)
        {
            Destroy(gameObject, Lifetime);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
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
