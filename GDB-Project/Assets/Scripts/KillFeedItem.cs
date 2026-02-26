using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedItem : MonoBehaviour
{
    [SerializeField] TMP_Text killerText;
    [SerializeField] TMP_Text victimText;
    [SerializeField] Image weaponSprite;


    public void InsertKillData(string killer, string victim, Sprite weapon) {
        killerText.text = killer;
        victimText.text = victim;

        if (weapon != null)
        {
            weaponSprite.enabled = true;
            weaponSprite.sprite = weapon;
        }
        else
        {
            weaponSprite.enabled = false;    
        }
    }
}
