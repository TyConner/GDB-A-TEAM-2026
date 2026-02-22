using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KillFeedManager : MonoBehaviour
{
    [Header("UI")] 
    [SerializeField] public KillFeedItem killfeedItemPrefab;
    [SerializeField] public Transform killfeedItemParent;
    [SerializeField] public WeaponIconLibrary iconLibrary;
    [SerializeField] private int maxRows;
    [SerializeField] private float rowLifetime;

    public static KillFeedManager instance;

    private void Awake()
    {
        instance = this;    
    }

    public void HandleKill(string killerName, string victimName, string weaponName) {
        KillFeedItem row = Instantiate(killfeedItemPrefab, killfeedItemParent);
        row.transform.SetAsFirstSibling();

        Sprite icon = iconLibrary.GetSprite(weaponName);
        row.InsertKillData(killerName, victimName, icon);

        while (killfeedItemParent.childCount > maxRows) {
            Destroy(killfeedItemParent.GetChild(killfeedItemParent.childCount - 1).gameObject);
        }

        Destroy(row.gameObject, rowLifetime);
    }

    
}
