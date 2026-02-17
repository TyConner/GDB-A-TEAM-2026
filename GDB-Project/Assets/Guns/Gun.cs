using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] public Sprite GunIcon;
    [SerializeField] public Sprite CrosshairIcon;
    [SerializeField] public string GunName;
    public int AmmoCur;
    [SerializeField] public int AmmoMax;
    [SerializeField] public float ReloadTime = 1f;
    [SerializeField] public float FireRate = 1f;
    [SerializeField] bool bRaycastBullet; // if false, will spawn a projectile, if true will raycast and apply hitscan damage
    [SerializeField] public GameObject Bullet;
    [SerializeField] Transform BulletOrigin;
    [SerializeField] LayerMask ignorelayer;
    bool bInReload;
    bool bFireCooldown;
    // float FireRateTimer;

    PlayerState OwningPlayer;
    public void OnEquip()
    {
        GameManager.instance.updateGunUI(GunIcon, CrosshairIcon, AmmoMax, AmmoCur, GunName);
    }

    public void OnUnequip()
    {
        // TODO: prob input update gun ui for no gun?
        GameManager.instance.ClearGunUI();
    }

    public void Reload()
    {
        if (bInReload) return;

        StartCoroutine(DoReload());
    }
    IEnumerator DoReload()
    {
        bInReload = true;
        yield return new WaitForSeconds(ReloadTime);
        bInReload = false;
        AmmoCur = AmmoMax;
        GameManager.instance.updateAmmoUI(AmmoMax, AmmoCur);
    }

    IEnumerator FireCooldown()
    {
        bFireCooldown = true;
        yield return new WaitForSeconds(FireRate);
        bFireCooldown = false;
    }


    public virtual void SecondaryFire()
    {
    }

    public virtual void Shoot(PlayerState Instagator)
    {
        if (!GetCanFire()) return;

        StartCoroutine(FireCooldown());
        AmmoCur--;
        //print("Pew");
        GameManager.instance.updateAmmoUI(AmmoMax, AmmoCur);
        iOwner owner = Instagator.EntityRef.GetComponent<iOwner>();

        Vector3 spawnPosition = transform.position;
        Quaternion spawnRotation = transform.rotation;

        if (owner != null)
        {
            Transform cam = owner.GetCameraTransform();

            spawnPosition = cam.position + cam.forward * 0.7f;
            spawnRotation = cam.rotation;
            if (!bRaycastBullet)
            {
                GameObject abullet = Instantiate(Bullet, spawnPosition, spawnRotation);
                abullet.GetComponent<Projectile>().MyOwner = Instagator;
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(cam.position, cam.forward, out hit, ~ignorelayer))
                {
                    Debug.Log("Hit: " + hit.collider.name);
                    iDamage dmg = hit.collider.GetComponent<iDamage>();


                    if (dmg != null)
                    {
                        PlayerState otherplayer = null;
                        iOwner otherplayerowner = null;
                        otherplayerowner = hit.transform.root.GetComponent<iOwner>();
                        Projectile mybullet = Bullet.GetComponent<Projectile>();

                        if (mybullet != null)
                        {
                            int DamageAmount = mybullet.DamageAmount;
                            if (otherplayer != null)
                            {
                                print("Raycast Hit: " + hit.collider.name + "\n  Shot by " + OwningPlayer.PS_Score.PlayerName + " at " + otherplayer.PS_Score.PlayerName + " Damage: " + DamageAmount);
                            }
                            else
                            {
                                print("RayCast Hit: " + hit.collider.name + "\n  Shot by " + OwningPlayer.PS_Score.PlayerName + " Damage: " + DamageAmount + " no player state found attached on other object");
                            }

                        }

                    }


                }
            }
        }
        if (AmmoCur == 0)
        {
            Reload();
        }
    }




    

    bool GetCanFire()
    {
        return AmmoCur > 0 && !bInReload && !bFireCooldown;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        AmmoCur = AmmoMax;
        //print(AmmoCur);
        OwningPlayer = transform.root.GetComponent<iOwner>().OwningPlayer();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Debug.DrawRay(BulletOrigin.position, transform.forward, Color.red, Time.deltaTime);
    }
}
