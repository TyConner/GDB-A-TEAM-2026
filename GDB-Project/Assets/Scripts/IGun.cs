using UnityEngine;
using UnityEngine.UI;


public interface IGun
{
    void OnEquip();
    void OnUnequip();
    void Shoot();
    void SecondaryFire();
    void Reload();
}
