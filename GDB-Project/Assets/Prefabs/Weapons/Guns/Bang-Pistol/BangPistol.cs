using System.Collections;
using UnityEngine;

public class BangPistol : Gun   
{
    [Range(0f, 100f)][SerializeField] float Chance_To_Shoot = 10f;
    [SerializeField] int ShootDistance = 100;
    [SerializeField] float ExplosionRadius = 5f;
    [SerializeField] int ExplosionDamage = 100;
    [SerializeField] float ExplosionForce = 500f;

    [SerializeField] GameObject BangEffect;
    [SerializeField] GameObject MuzzleFlash;
    [SerializeField] GameObject MuzzleFX;


    [SerializeField] AudioClip BangSound;
    [Range(0f, 1f)][SerializeField] float BangVolume = 1f;
    [SerializeField] AudioClip ShootSound;
    [Range(0f, 1f)][SerializeField] float ShootVol = 1f;
    [SerializeField] GameObject Explosion;
    [SerializeField] AudioClip ExplosionSound;
    [Range(0f, 1f)][SerializeField] float ExplosionVolume = 1f;
    [SerializeField] AudioClip EmptySound;
    [Range(0f, 1f)][SerializeField] float EmptyVolume = 1f;
   
    


    public new void OnEquip(PlayerState owner)
    {
        base.OnEquip(owner);

    }

    public new void OnUnequip()
    {
        base.OnUnequip();
    }

    public override void Reload()
    {
        // no reload for this gun
    }

    public new void SecondaryFire()
    {
        // no secondary fire for this gun
    }
    public override void Shoot(PlayerState Instagator)
    {
        if (GetCanFire()) {
            //ammo to fire
            //and can fire, so we attempt to shoot
            StartCoroutine(FireCooldown());
            AmmoCur--;
            if (Chance_To_Shoot >= UnityEngine.Random.Range(0f, 100f))
            {
                RaycastHit hit;
                Transform cam = Instagator.EntityRef.GetComponent<iOwner>().GetCameraTransform();
                GameObject Flash = Instantiate(MuzzleFlash, BulletOrigin.transform.position, Quaternion.identity, transform);
                Destroy(Flash, 0.05f);
                AudioSource.PlayClipAtPoint(ShootSound, BulletOrigin.transform.position, ShootVol);

                if (Physics.Raycast(cam.position, cam.forward, out hit, 100f))
                {
                    AudioSource.PlayClipAtPoint(ExplosionSound, hit.point, ExplosionVolume);
                    GameObject exp = Instantiate(Explosion, hit.point, Quaternion.identity);
                    Destroy(exp, 2f);
                    Collider[] colliders = Physics.OverlapSphere(hit.point, ExplosionRadius);
                    foreach (Collider nearby in colliders)
                    {
                        Rigidbody rb = nearby.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.AddExplosionForce(ExplosionForce, hit.point, ExplosionRadius);
                        }
                        iDamage damageable = nearby.GetComponent<iDamage>();
                        if (damageable != null)
                        {
                            damageable.takeDamage(ExplosionDamage, OwningPlayer);
                        }

                    }
                }
            }
            else
            {
                // didnt shoot, make bang sound and effect
                StartCoroutine(BangEffectRoutine());
                GameObject FX = Instantiate(MuzzleFX, BulletOrigin.transform.position, Quaternion.identity, transform);
                Destroy(FX, 0.05f);
                AudioSource.PlayClipAtPoint(BangSound, BulletOrigin.transform.position, BangVolume);


            }
            GameManager.instance.updateAmmoUI(AmmoMax, AmmoCur);
        }
        else if(AmmoCur == 0 && !bFireCooldown)
        {
            // play empty sound
            AudioSource.PlayClipAtPoint(EmptySound, BulletOrigin.transform.position, EmptyVolume);
            StartCoroutine(FireCooldown());
        }



            


    }

    IEnumerator BangEffectRoutine()
    {
        BangEffect.SetActive(true);
        yield return new WaitForSeconds(FireRate * .6f);
        BangEffect.SetActive(false);
    }
    protected override void Start()
    {
        base.Start();
        
    }
}
