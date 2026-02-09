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

    [SerializeField] public GameObject Bullet;
    [SerializeField] Transform BulletOrigin;

    bool bInReload;
    bool bFireCooldown;
    float FireRateTimer;
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

    public virtual void Shoot()
    {
        if (!GetCanFire()) return;

        StartCoroutine(FireCooldown());
        AmmoCur--;
        print("Pew");
        GameManager.instance.updateAmmoUI(AmmoMax, AmmoCur);
        Instantiate(Bullet, BulletOrigin);
        if(AmmoCur == 0)
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
        print(AmmoCur);
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }
}
