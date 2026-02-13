using UnityEngine;

public class BodyDamageSensor : MonoBehaviour, iDamage
{
    [SerializeField] bool HeadHitBox = false;
    [SerializeField] float DamageMultiplier = 1.0f;



    public void takeDamage(int amount, PlayerState Instigator)
    {
        if(Instigator != null)
        {
            if (HeadHitBox)
            {
                transform.root.GetComponent<iDamage>().takeDamage((int)(amount * DamageMultiplier), Instigator, HeadHitBox);
            }
            else
            {
                transform.root.GetComponent<iDamage>().takeDamage(amount, Instigator);
            }
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
