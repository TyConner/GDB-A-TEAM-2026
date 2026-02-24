using UnityEngine;

public class BodyDamageSensor : MonoBehaviour, iDamage
{
    [SerializeField] float DamageMultiplier = 1.0f;
    [SerializeField] bool isHead = false;
    [SerializeField] bool bDebug = false;

    public void takeDamage(int amount, PlayerState Instigator, bool Headshot = false)
    {
        if (Instigator != null)
        {
            if (bDebug) Debug.Log("BodyDamageSensor: " + transform.name + " took damage from " + Instigator.name + " for " + amount);
            if (!isHead)
            {
                transform.root.GetComponent<iDamage>().takeDamage((int)(amount * DamageMultiplier), Instigator);
            }
            else
            {
                transform.root.GetComponent<iDamage>().takeDamage((int)(amount * DamageMultiplier), Instigator, isHead);
            }
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
