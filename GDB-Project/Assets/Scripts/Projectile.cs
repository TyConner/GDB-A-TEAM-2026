using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    public int DamageAmount;
    [SerializeField] int Speed = 1;
    [SerializeField] float Lifetime = 0f;
    public ParticleSystem HitEffect;

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
        //RaycastHit hit;
        //if (rb.SweepTest(transform.forward, out hit, Speed * Time.deltaTime))
        //{
        //    HandleHit(hit.collider);
        //}

    }
    
    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other);
        
    }
    private void HandleHit(Collider other)
    {
        PlayerState otherplayer = null;
        iOwner otherplayerowner = null;
        otherplayer = other.transform.root.GetComponent<PlayerState>();
        otherplayerowner = other.transform.root.GetComponent<iOwner>();
        
        if (other != null  && otherplayerowner != null)
        {
            
        }

        if (otherplayer != null  && other.isTrigger || otherplayer == MyOwner)
        {
            
            return;

        }

        iDamage dmg = other.GetComponent<iDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(DamageAmount, MyOwner);
            
            if (otherplayer != null)
            {
                print("Projectile Hit: " + other.name + "\n Bullet Shot by " + MyOwner.PS_Score.PlayerName + " at " + otherplayer.PS_Score.PlayerName + " Damage: " + DamageAmount );
            }
            else
            {
                print("Projectile Hit: " + other.name + "\n Bullet Shot by " + MyOwner.PS_Score.PlayerName + " Damage: " + DamageAmount + " no player state found attached on other object");
            }

            }
        else
        {
            print("Projectile Hit: " + other.name + "\n Bullet Shot by " + MyOwner.PS_Score.PlayerName + " Damage: " + DamageAmount + " no player state found attached on other object");
        }
        if (HitEffect != null)
        {
            Instantiate(HitEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
