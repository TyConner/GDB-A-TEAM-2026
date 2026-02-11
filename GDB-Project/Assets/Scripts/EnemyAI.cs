using System;
using System.Collections;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using static MyScore;

public class EnemyAI : MonoBehaviour, iFootStep, iDamage, iOwner
{
    [Header("------Dependancies--------")]

    [Space(5)][SerializeField] GameObject[] Characters;
    int CharacterIndex;

    [Space(2)][SerializeField] NavMeshAgent Agent;

    [Space(2)][SerializeField] Animation_State_Controller controller;
    [SerializeField] RigBuilder Ik_Rig;

    [Space(2)][SerializeField] Collider[] CharacterColliders;

    [Space(2)][SerializeField] GameObject Gun;

    [Space(2)]
    [SerializeField] GameObject MuzzleFlash;

    [Header("---------Audio Settings--------")]
    [Space(5)]
    [Space(2)][SerializeField] EnemyAudioConfig AudioConfig;
    [Header("---------Enemy Stats--------")]
    [Space(5)]
    [Space(2)][Range(1, 100)][SerializeField] int HP = 100;
    [Space(2)][SerializeField] float Item_Drop_Height = 1.0f;
    [Range(1, 10)]
    [SerializeField] int AnimationTransSpeed = 8;
    [Header("AI Difficulty stats")]
    [Space(2)]public EnemyStats Config;

    [Header("------Score and Team-------")]
    public PlayerState MyPlayerState;

    bool isFlashingRed;


    //testing
    float shootTimer;
    float shootRate = 1.1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RandomCharacter();
    }

    public void SetPlayerStateRef(PlayerState state)
    {

        MyPlayerState = state;

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
    // Update is called once per frame
    void Update()
    {
        if(Config != null && Agent.enabled)
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
            if (GameManager.instance != null && GameManager.instance.player != null)
            {
                //Agent.SetDestination(GameManager.instance.player.transform.position);
            }
        
    }

    void Shoot()
    {
        if (shootTimer > shootRate && Gun != null)
        {
            shootTimer = 0;

            Transform pos = Gun.transform.Find("ProjectileOrigin");
            RaycastHit hit;
            if (Physics.Raycast(pos.position, Gun.transform.forward, out hit, 50))
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
    public void onStepDetected(Vector3 Pos)
    {
        //Debug.Log("FootStepEvent Detected");
        AudioSource.PlayClipAtPoint(AudioConfig.footsteps[UnityEngine.Random.Range(0,AudioConfig.footsteps.Length)], Pos, AudioConfig.footsteps_Vol);
    }

    IEnumerator flashred()
    {
        if (!isFlashingRed)
        {
            isFlashingRed = true;
            Color orig = Characters[CharacterIndex].GetComponent<SkinnedMeshRenderer>().material.color;
            Characters[CharacterIndex].GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
            yield return new WaitForSeconds(.1f);
            Characters[CharacterIndex].GetComponent<SkinnedMeshRenderer>().material.color = orig;
            isFlashingRed = false;
        }
        
    }
    public void takeDamage(int amount, PlayerState Instagator)
    {
        HP -= amount;
        
        //Debug.Log("DAMAGE IN AMOUNT: " + amount);
        controller.OnHit();
        StartCoroutine(flashred());
        if (HP <= 0)
        {
            if (Instagator != null)
            {
                Instagator.updateScore(Category.Kills, 1);
                Debug.Log(MyPlayerState.PS_Score.PlayerName + " was killed by " + Instagator.PS_Score.PlayerName);
            }
            
            OnDeath();
            

        }
    }

    public PlayerState OwningPlayer()
    {
        return MyPlayerState;
    }

    public bool OnHeadShotNotify()
    {
        throw new NotImplementedException();
    }

    public void takeDamage(int amount, PlayerState Instigator, bool Headshot)
    {
        if(HP - amount < 0)
        {
            Instigator.updateScore(Category.Headshots, 1);
            takeDamage(amount, Instigator);
        }
        else
        {
            takeDamage(amount, Instigator);
        }
    }
}