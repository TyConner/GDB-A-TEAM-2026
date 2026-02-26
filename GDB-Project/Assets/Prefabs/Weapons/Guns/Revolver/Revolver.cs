using UnityEngine;
using UnityEngine.UI;

public class Revolver : Gun
{
    public new void OnEquip(PlayerState owner)
    {
        base.OnEquip(owner);
        
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
        //print("Revolver start");
    }
}
