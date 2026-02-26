using System;
using UnityEngine;

public class WeaponIconLibrary : MonoBehaviour
{
    [Serializable]
    public class WeaponIcon {
        public string weaponName;
        public Sprite sprite;
    }

    [SerializeField] private WeaponIcon[] weaponIcons;

    public Sprite GetSprite(string weaponName) {
        if (string.IsNullOrEmpty(weaponName)) {
            return null;
        }


        for (int i = 0; i < weaponIcons.Length; i++) {
            if (weaponIcons[i] != null && weaponIcons[i].weaponName == weaponName) {
                return weaponIcons[i].sprite;
            }
        }

        return null;
    }
}
