using UnityEngine;
using UnityEngine.UI;

public class Shotgun : Gun
{
    public new void OnEquip()
    {
        base.OnEquip();

    }

    public new void OnUnequip()
    {
        base.OnUnequip();
    }

    public new void Reload()
    {
        base.Reload();
    }

    public new void SecondaryFire()
    {
        base.SecondaryFire();
    }

    public new void Shoot()
    {
        base.Shoot();
    }

    protected override void Start()
    {
        base.Start();
        print("Obtained Shotgun");
    }
}
