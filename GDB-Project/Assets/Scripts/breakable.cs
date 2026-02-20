using UnityEngine;

public class breakable : MonoBehaviour, iDamage
{
    [Range(1,300)][SerializeField] int health = 25;
    [SerializeField] AudioClip hitSound;
    [SerializeField] float hitVolume = .5f;
    [SerializeField] GameObject breakEffect;
    [SerializeField] AudioClip breakSound;
    [SerializeField] float soundVolume = .5f;
    [SerializeField] bool destroyOnBreak = true;
    [SerializeField] bool RandomModel = false;
    [SerializeField] GameObject[] models;

    void Start()
    {
        if(RandomModel && models.Length > 0)
        {
            int index = Random.Range(0, models.Length);
            for(int i = 0; i < models.Length; i++)
            {
                if(i == index)
                {
                    models[i].SetActive(true);
                }
                else
                {
                    models[i].SetActive(false);
                }
            }
        }
    }

    public void takeDamage(int amount, PlayerState Instigator)
    {
        if(health <= 0)
        {
            return;
        }
        health -= amount;
        if(health > 0 && hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position, hitVolume);
        }
        if (health <= 0)
        {
            if(breakEffect != null)
            {
                Instantiate(breakEffect, transform.position, Quaternion.identity);
            }
            if(breakSound != null)
            {
                AudioSource.PlayClipAtPoint(breakSound, transform.position, soundVolume);
            }
            if(destroyOnBreak)
            {
                Destroy(gameObject);
            }
        }
    }


}
