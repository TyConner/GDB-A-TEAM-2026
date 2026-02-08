using System.Collections;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [Header("Type")][Space(5)][SerializeField] DamageType Type;

    [Header("Stats")]
    [Space(5)]
    [SerializeField] int Dmg;
    [Space(2)]
    [SerializeField] int DmgRate;
    [Space(2)]
    [SerializeField] int Spd;
    [Space(2)]
    [SerializeField] int SelfDestructTime;
    [Space(2)]
    [Header("Components")]
    [Space(5)]
    [SerializeField] Rigidbody rb;
    [SerializeField] ParticleSystem hitEff;



    enum DamageType { DOT, Static, Bullet };

    bool isDamaging;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Type == DamageType.Bullet)
        {
            rb.linearVelocity = transform.forward * Spd;
            Destroy(gameObject, SelfDestructTime);

        }

    }
    IEnumerator DamageOverTime(int dmg, iDamage target)
    {

        yield return new WaitForSeconds(DmgRate);
        target.takeDamage(dmg);

    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.isTrigger)
        {
            return;
        }

        iDamage dmg = other.GetComponent<iDamage>();

        if (dmg != null && Type != DamageType.DOT)
        {
            dmg.takeDamage(Dmg);
        }
        if (Type == DamageType.Bullet)
        {
            if (hitEff != null)
            {
                Instantiate(hitEff, transform.position, Quaternion.identity);

            }

            Destroy(gameObject);
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        iDamage dmg = other.GetComponent<iDamage>();

        if (dmg != null && Type == DamageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }
    }
    IEnumerator damageOther(iDamage d)
    {
        isDamaging = true;
        d.takeDamage(Dmg);
        yield return new WaitForSeconds(DmgRate);
        isDamaging = false;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
