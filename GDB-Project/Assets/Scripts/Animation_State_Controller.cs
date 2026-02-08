using UnityEngine;

public class Animation_State_Controller : MonoBehaviour
{
    Animator animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnShoot()
    {
        animator.SetTrigger("Shoot");
    }
    public void OnHit()
    {
        animator.SetTrigger("Hit");
    }

    public void OnDeath()
    {
        animator.SetTrigger("Death");
        animator.SetInteger("RandomDeath", Random.Range(0, 3));
    }
    public void SetSpeed(float currentSpeed, float TransSpeed)
    {
        float agentSpeedAnim = animator.GetFloat("Speed");

        animator.SetFloat("Speed", Mathf.MoveTowards(agentSpeedAnim, currentSpeed, Time.deltaTime * TransSpeed));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
