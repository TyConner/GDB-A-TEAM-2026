using UnityEngine;

public interface iUseWeaponsAndItems
{
    public void EquipGun(Gun newGun);

    public void DropGun();
    public void addHealth(int amount);
    public void AddTNT(int amount);

}
