using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
   
    float angle;
    Vector3 playerdir;
    [Header("     Needle Sprite      ")]
    [SerializeField] Image needle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance != null && GameManager.instance.player != null && needle != null)
        {
            playerdir = GameManager.instance.player.transform.forward;
            playerdir.y = 0;
            angle = Mathf.Atan2(playerdir.x,playerdir.z) * Mathf.Rad2Deg;

            needle.rectTransform.localEulerAngles = new Vector3(0, 0, -angle);
        }
        
    }
}
