using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using static MyScore;

public class EnemyAI : MonoBehaviour, iFootStep, iDamage, iOwner
{

    [Space(2)]
    [Header("---------Serialized Fields---------")]
    [Space(5)]

    [Header("---------Debug---------")]
    [Space(5)]
    [SerializeField] bool bDebug;

    [Space(2)]
    [Header("---------Dependancies---------")]
    [Space(5)]

    [SerializeField] GameObject[] Characters;
    int CharacterIndex;

    [Space(2)]
    [SerializeField] Transform headPos;

    [Space(2)]
    [SerializeField] NavMeshAgent Agent;

    [Space(2)]
    [SerializeField] Animation_State_Controller controller;

    [Space(2)]
    [SerializeField] RigBuilder Ik_Rig;

    [Space(2)]
    [SerializeField] GameObject Gun;

    [Space(2)]
    [SerializeField] GameObject MuzzleFlash;
    
    [Space(2)]
    [Header("---------Audio Settings---------")]
    [Space(5)]

    [SerializeField] EnemyAudioConfig AudioConfig;

    [Space(2)]
    [Header("---------Enemy Stats--------")]
    [Space(5)]

    [Range(1, 100)][SerializeField] int HP = 100;

    [Space(2)]
    [Range(1, 10)][SerializeField] int AnimationTransSpeed = 8;

    [Space(2)]
    [Header("---------Public Fields---------")]
    [Space(5)]

    [Space(2)]
    [Header("---------AI Difficulty stats---------")]
    [Space(5)]

    EnemyStats Config;
    
    [Space(2)]
    [Header("---------Score and Team---------")]
    PlayerState MyPlayerState;
    List<GameObject> NearbyEnemyPlayers;


    //private vars
    bool isFlashingRed;

    //testing
    float shootTimer;
    float shootRate = 1.1f;

    Color orig;

    enum Behaviors { Fight, Flee, Search, Assist, Roam };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RandomCharacter();
        NearbyEnemyPlayers = new List<GameObject>();
        orig = Characters[CharacterIndex].GetComponent<SkinnedMeshRenderer>().material.color;
    }

 
    // Update is called once per frame
    void Update()
    {
        bool bCanPlay = Config != null && Agent.enabled && GameMode.instance != null && GameMode.instance.Phase == GameMode.GamePhase.Playing;
        
        if (bCanPlay)
        {
            AiLogic();
        }
    }

    void RandomCharacter()
    {
        foreach (var character in Characters)
        {
            character.SetActive(false);

        }
        CharacterIndex = UnityEngine.Random.Range(0, Characters.Length);
        Characters[CharacterIndex].SetActive(true);
    }
    void LocoAnim()
    {
        controller.SetSpeed(Agent.velocity.normalized.magnitude, AnimationTransSpeed);
    }
   void AiLogic()
    {
        shootTimer += Time.deltaTime;
        LocoAnim();
        //Shoot();
        
        
    }

    void faceTarget(GameObject target)
    {
        Vector3 TargetPos = target.transform.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(TargetPos.x, transform.position.y, TargetPos.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * Config.get_faceTargetSpeed());
    }
    void Shoot()
    {
        if (shootTimer > shootRate && Gun != null)
        {
            shootTimer = 0;

            Transform pos = Gun.transform.Find("ProjectileOrigin");
            RaycastHit hit;
            if (Physics.Raycast(pos.position, Gun.transform.forward, out hit, 75))
            {
                //Debug.Log(hit.collider.name);

                iDamage dmg = hit.collider.GetComponent<iDamage>();
                if (dmg != null)
                {
                    dmg.takeDamage(30, MyPlayerState );
                }

            }
            controller.OnShoot();
        
        }
       
    }
    public void createBullet()
    {
        //called from animation event in clip

        Transform pos = Gun.transform.Find("ProjectileOrigin");
        GameObject flash = Instantiate(MuzzleFlash, pos);
        AudioSource.PlayClipAtPoint(AudioConfig.gunshot[0], pos.position, AudioConfig.gunshot_Vol);
        Destroy(flash, .05f);

    }

    void OnDeath()
    {
        Agent.enabled = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        Ik_Rig.Clear();
        AudioSource.PlayClipAtPoint(AudioConfig.dying[UnityEngine.Random.Range(0, AudioConfig.dying.Length)], transform.position, AudioConfig.dying_Vol);
        //MyPlayerState.updateScore(Category.Deaths, 1);
        controller.OnDeath();
        MyPlayerState.OnDeath();
   

    }
 

    IEnumerator flashred()
    {
        if (!isFlashingRed)
        {
            isFlashingRed = true;
            Characters[CharacterIndex].GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
            yield return new WaitForSeconds(.1f);
            Characters[CharacterIndex].GetComponent<SkinnedMeshRenderer>().material.color = orig;
            isFlashingRed = false;
        }
        
    }
   
    bool CanSeeTarget()
    {
        return false;
        //    playerdir = GameManager.instance.player.transform.position - headPos.position;
        //    angleToPlayer = Vector3.Angle(playerdir, transform.forward);

        //if (bDebug)
        //{
        //    Debug.DrawRay(headPos.position, playerdir);
        //}
        //    RaycastHit hit;

        //    if (Physics.Raycast(headPos.position, playerdir, out hit, float.MaxValue))
        //    {
        //        if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
        //        {
        //            agent.SetDestination(GameManager.instance.player.transform.position);
        //            if (!Alerted)
        //            {
        //                EnemySpotted();
        //            }
        //            else
        //            {
        //                LastKnownLoc = GameManager.instance.player.transform.position;
        //            }



        //            if (agent.remainingDistance <= agent.stoppingDistance)
        //            {
        //                faceTarget();

        //                aim();
        //            }

        //            if (shootTimer >= shootRate && agent.remainingDistance <= agent.stoppingDistance)
        //            {
        //                shoot();
        //            }

        //            agent.stoppingDistance = stoppingDistOrig;
        //            return true;
        //        }

        //    }

        //    agent.stoppingDistance = 0;
        //    return false;
    }

    /// ****** Collider Logic ******
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.transform.root.CompareTag("Bot") || other.transform.root.CompareTag("Player") && other.transform.root.gameObject == other.gameObject)
        {
            if (!NearbyEnemyPlayers.Contains(other.transform.root.gameObject) && GameMode.instance.Phase == GameMode.GamePhase.Playing)
            {
                PlayerState otherPlayer = other.transform.root.gameObject.GetComponent<iOwner>().OwningPlayer();
                if (otherPlayer != null && otherPlayer.PS_Score.Assigned_Team == Team.FFA || otherPlayer.PS_Score.Assigned_Team != MyPlayerState.PS_Score.Assigned_Team)
                {
                    //Debug.Log(other.transform.root.gameObject.name + " Added to list of enemies");
                    NearbyEnemyPlayers.Add(other.transform.root.gameObject);
                }


            }



        }

        //By prototype two scan for pickups here and log it

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (NearbyEnemyPlayers.Contains(other.gameObject.transform.root.gameObject) && GameMode.instance != null)
        {
            NearbyEnemyPlayers.Remove(other.transform.root.gameObject);
        }
    }


    /// ****** IMPLEMENTED INTERFACE LOGIC *******
    public void SetOwningPlayer(PlayerState p)
    {
        MyPlayerState = p;
    }
    public PlayerState OwningPlayer()
    {
        return MyPlayerState;
    }
    public void SetAIConfig(EnemyStats stats)
    {
        Config = stats;
    }

    public void takeDamage(int amount, PlayerState Instagator)
    {
        HP -= amount;

        if (bDebug)
        {
            Debug.Log(MyPlayerState.PS_Score.PlayerName + " was shot by " + Instagator.PS_Score.PlayerName + "for " + amount + " damage.");
        }
        controller.OnHit();
        StartCoroutine(flashred());
        if (HP <= 0)
        {
            if (Instagator != null)
            {
                Instagator.updateScore(Category.Kills, 1);
                if (bDebug)
                {
                    Debug.Log(MyPlayerState.PS_Score.PlayerName + " was killed by " + Instagator.PS_Score.PlayerName);
                }

            }

            OnDeath();


        }
    }

    public void takeDamage(int amount, PlayerState Instigator, bool Headshot)
    {
        if (HP - amount < 0)
        {
            Instigator.updateScore(Category.Headshots, 1);
            takeDamage(amount, Instigator);
        }
        else
        {
            takeDamage(amount, Instigator);
        }
    }

    public void onStepDetected(Vector3 Pos)
    {
        //Debug.Log("FootStepEvent Detected");
        AudioSource.PlayClipAtPoint(AudioConfig.footsteps[UnityEngine.Random.Range(0, AudioConfig.footsteps.Length)], Pos, AudioConfig.footsteps_Vol);
    }

}