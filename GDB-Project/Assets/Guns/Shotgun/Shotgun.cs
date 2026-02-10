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

    public new void Shoot(PlayerState Instagator)
    {
        base.Shoot(Instagator);
    }

    protected override void Start()
    {
        base.Start();
    }
}
