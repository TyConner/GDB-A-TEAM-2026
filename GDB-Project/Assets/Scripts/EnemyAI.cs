using System;
using System.Collections;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using static UnityEditor.PlayerSettings;

public class EnemyAI : MonoBehaviour, iFootStep, iDamage
{
    [Header("------Dependancies--------")]

    [Space(5)][SerializeField] GameObject[] Characters;

    [Space(2)][SerializeField] NavMeshAgent Agent;

    [Space(2)][SerializeField] Animation_State_Controller controller;
    [SerializeField] RigBuilder Ik_Rig;

    [Space(2)][SerializeField] Collider Head;

    [Space(2)][SerializeField] GameObject Gun;

    [Space(2)]
    [SerializeField] GameObject MuzzleFlash;

    [Header("---------Audio Settings--------")]
    [Space(5)]
    [Space(2)][SerializeField] EnemyAudioConfig AudioConfig;
    [Header("---------Enemy Stats--------")]
    [Space(5)]
    [Space(2)][Range(1, 100)][SerializeField] int HP;
    [Space(2)][SerializeField] float Item_Drop_Height = 1.0f;
    [Range(1, 10)]
    [SerializeField] int AnimationTransSpeed = 8;
    [Header("AI Difficulty stats")]
    [Space(2)][SerializeField] EnemyStats Config;

    [Header("------Score and Team-------")]
    [Space(2)]
    [SerializeField] MyScore ScoreData;


    //testing
    float shootTimer;
    float shootRate = 1.1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Config != null)
        {
            AiLogic();
        }
    }

    void LocoAnim()
    {
        controller.SetSpeed(Agent.velocity.normalized.magnitude, AnimationTransSpeed);
    }
   void AiLogic()
    {
        shootTimer += Time.deltaTime;
        Shoot();
        LocoAnim();
        Agent.SetDestination(GameManager.instance.player.transform.position);
        
    }

    void Shoot()
    {
        if (shootTimer > shootRate && Gun != null)
        {
            shootTimer = 0;
            Shoot();
            Transform pos = Gun.transform.Find("ProjectileOrigin");
            
            RaycastHit hit;
            if (Physics.Raycast(pos.position, Gun.transform.forward, out hit, 50))
            {
                Debug.Log(hit.collider.name);

                iDamage dmg = hit.collider.GetComponent<iDamage>();
                if (dmg != null)
                {
                    dmg.takeDamage(5);
                }

            }
            controller.OnShoot();
            GameObject flash = Instantiate(MuzzleFlash, pos);
            Destroy(flash, .05f);
        }
       
    }
    IEnumerator BodyCleanUp()
    {
        yield return new WaitForSeconds(GameManager.instance.BodyCleanUpTime);
        Destroy(gameObject);
    }
    void OnDeath()
    {
        Agent.enabled = false;
        controller.OnDeath();
        StartCoroutine(BodyCleanUp());

    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        controller.OnHit();
        if (HP < 0) HP = 0;
        {

            OnDeath();

        }
    }

    public void onStepDetected(Vector3 Pos)
    {
        AudioSource.PlayClipAtPoint(AudioConfig.footsteps[UnityEngine.Random.Range(0,AudioConfig.footsteps.Length)], Pos, AudioConfig.footsteps_Vol);
    }

}