using UnityEngine;

public class BodyDamageSensor : MonoBehaviour, iDamage
{
    [SerializeField] float DamageMultiplier = 1.0f;
    [SerializeField] bool bDebug = false;


    public void takeDamage(int amount, PlayerState Instigator)
    {
        if(Instigator != null)
        {
            if(bDebug) Debug.Log("BodyDamageSensor: " + transform.name + " took damage from " + Instigator.name + " for " + amount);
            transform.root.GetComponent<iDamage>().takeDamage(amount, Instigator);
        }
       

    }


    public void takeDamage(int amount, PlayerState Instigator, bool Headshot)
    {
        //required and does nothing :D
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
