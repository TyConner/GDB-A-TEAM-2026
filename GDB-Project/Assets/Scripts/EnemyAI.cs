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
    [SerializeField] GameObject Projectile;

    [Space(2)]
    [SerializeField] GameObject MuzzleFlash;

    [Space(2)]
    [SerializeField]
    Collider[] Colliders;
    
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
    List<GameObject> NearbyAllyPlayers;

    //private vars
    bool isFlashingRed;

    //testing
    float shootTimer;
    float shootRate = 1.1f;

    Color orig;

    public enum Behaviors { Fight, Flee, Search, Assist, Roam, Dead};
    public Behaviors CurrentState;

    Vector3 SpawningLocation;
    struct TargetInfo{
        public GameObject Obj;
        public Vector3 TargetLastKnownLoc;
        public Vector3 TargetDir;
        public float TargetAngleToMe;
        public float TargetTimer;
        public float SearchTimer;
        public bool CanSee;

        public void Clear()
        {
            Obj = null;
            TargetLastKnownLoc = Vector3.zero;
            TargetDir = Vector3.zero;
            TargetAngleToMe = 0;
            TargetTimer = 0;
            SearchTimer = 0;
            CanSee = false; 
        }
    }

    int HPOrig;

    TargetInfo MyTarget;
    

    float RoamTimer;

    float StoppingDistOrig;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RandomCharacter();
        NearbyEnemyPlayers = new List<GameObject>();
        NearbyAllyPlayers = new List<GameObject>();
        if (Characters[CharacterIndex].GetComponent<SkinnedMeshRenderer>())
        {
            orig = Characters[CharacterIndex].GetComponent<SkinnedMeshRenderer>().material.color;
        }
        else if (Characters[CharacterIndex].GetComponent<MeshRenderer>())
        {
            orig = Characters[CharacterIndex].GetComponentInChildren<MeshRenderer>().material.color;
        }
        SpawningLocation = transform.position;
        if (Agent)
        {
            StoppingDistOrig = Agent.stoppingDistance;
        }
        
        if (Config)
        {
            RoamTimer = Config.get_RoamPauseTime();
        }
       
        HPOrig = HP;
        initColliders();
    }

 
    // Update is called once per frame
    void Update()
    {
        bool bCanPlay = Config != null && Agent.enabled && GameMode.instance != null && GameMode.instance.Phase == GameMode.GamePhase.Playing;
        
        if (bCanPlay)
        {
            //AiLogic();
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
        if (controller)
        {
            controller.SetSpeed(Agent.velocity.normalized.magnitude, AnimationTransSpeed);
        }
        
    }
   void AiLogic()
    {
        shootTimer += Time.deltaTime;
        LocoAnim();
        AssessBehavior();
        BehaviorTree(CurrentState);
         
    }

    void AssessBehavior()
    {
        if(CurrentState != Behaviors.Dead)
        {
            RoamTimer += Time.deltaTime;
            MyTarget.SearchTimer += Time.deltaTime;
            GameObject Threat = ReturnClosestThreat();
            if (Threat != null)
            {
                MyTarget = CanSeeTarget(Threat);
                if (MyTarget.CanSee)
                {
                    CurrentState = Behaviors.Fight;
                    Agent.stoppingDistance = StoppingDistOrig;
                }
                else
                {
                    CurrentState = Behaviors.Search;
                    Agent.stoppingDistance = 0;
                }
            }
            else
            {
                CurrentState = Behaviors.Roam;
                Agent.stoppingDistance = 0;
            }
        }
       
    }
    void BehaviorTree(Behaviors state)
    {
        switch (state)
        {
            case Behaviors.Roam:
                {
                    if (Agent.remainingDistance < 0.01f && RoamTimer >= Config.get_RoamPauseTime())
                    {
                        RoamTimer = 0;
                        Vector3 ranPos = UnityEngine.Random.insideUnitSphere * Config.get_RoamDist();
                        ranPos += SpawningLocation;
                        NavMeshHit hit;
                        NavMesh.SamplePosition(ranPos, out hit, Config.get_RoamDist(), 1);
                        Agent.SetDestination(hit.position);
                    }
                    
                    break;
                }
            case Behaviors.Search:
                {
                    if(Agent.remainingDistance < 0.01f && MyTarget.SearchTimer >= Config.get_AgentSearchTime())
                    {
                        MyTarget.SearchTimer = 0;
                        Vector3 ranpos = UnityEngine.Random.insideUnitSphere * Config.get_AgentAlertedSearchDistance();
                        ranpos += MyTarget.TargetLastKnownLoc;
                        NavMeshHit hit;
                        NavMesh.SamplePosition(ranpos, out hit, Config.get_AgentAlertedSearchDistance(), 1);
                        Agent.SetDestination(hit.position);
                    }
                    
                    break;
                }
            case Behaviors.Flee:
                {
                    break;
                }
            case Behaviors.Assist:
                {
                    break;
                }
            case Behaviors.Fight:
                {
                    if (MyTarget.Obj)
                    {
                        GoTo(MyTarget.TargetLastKnownLoc);
                        Agent.stoppingDistance = StoppingDistOrig;
                        faceTarget(MyTarget.Obj);
                        if (shootTimer >= shootRate)
                        {
                            Shoot();
                        }
                    }
                    
                    break;
                }
            case Behaviors.Dead:
                {
                    break;
                }
        }
    }
    void faceTarget(GameObject target)
    {
        
        Vector3 TargetPos = target.transform.position - headPos.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(TargetPos.x, transform.position.y, TargetPos.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * Config.get_faceTargetSpeed());
    }
    void Shoot()
    {
        if (shootTimer > shootRate && Gun != null)
        {
            shootTimer = 0;

            controller.OnShoot();
        
        }
       
    }
    public void createBullet()
    {
        //called from animation event in clip

        Transform pos = Gun.transform.Find("ProjectileOrigin");
        GameObject bullet = Instantiate(Projectile, pos.position,transform.rotation);
        GameObject flash = Instantiate(MuzzleFlash, pos);
        bullet.GetComponent<Projectile>().MyOwner = MyPlayerState; 
        AudioSource.PlayClipAtPoint(AudioConfig.gunshot[0], pos.position, AudioConfig.gunshot_Vol);
        Destroy(flash, .05f);

    }

    void initColliders()
    {
        if(Colliders.Length > 0)
        {
            foreach (Collider var in Colliders)
            {
                var.enabled = true;
                var.isTrigger = false;
                
            }
        }
            
    }
    void TurnOffCollision()
    {
        if(Colliders.Length > 0)
        {
            foreach (Collider var in Colliders)
            {
                var.enabled = false;
            }
        }
            
    }
    void OnDeath()
    {
        CurrentState = Behaviors.Dead;
        if (Agent && Ik_Rig && controller && MyPlayerState && AudioConfig)
        {
            Agent.enabled = false;
            Ik_Rig.Clear();
            TurnOffCollision();
            controller.OnDeath();
            MyPlayerState.OnDeath();
            AudioSource.PlayClipAtPoint(AudioConfig.dying[UnityEngine.Random.Range(0, AudioConfig.dying.Length)], transform.position, AudioConfig.dying_Vol);
        }

        else
        {
            
            if (bDebug)
            {
                Debug.LogWarning(
                    "Object Not Initialized Component Status's:" + '\n' +
                    "Agent status: " + (Agent !=  null).ToString() + '\n' +
                    "ik rig status: " + (Ik_Rig != null).ToString() + '\n' +
                    "Controller  status: " + (controller != null).ToString() + '\n' +
                    "PlayerState status: " + (MyPlayerState != null).ToString() + '\n' +
                    "AudioConfig status: " + (AudioConfig != null).ToString() + '\n' +
                    "Destroying Object."
                    );

            }
            Destroy(gameObject);
        }




        




    }
 

    IEnumerator flashred()
    {
        if (!isFlashingRed && Characters[CharacterIndex].GetComponent<SkinnedMeshRenderer>())
        {
            isFlashingRed = true;
            Characters[CharacterIndex].GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
            yield return new WaitForSeconds(.1f);
            Characters[CharacterIndex].GetComponent<SkinnedMeshRenderer>().material.color = orig;
            isFlashingRed = false;
        }
        else if (!isFlashingRed && Characters[CharacterIndex].GetComponent<MeshRenderer>())
        {
            isFlashingRed = true;
            Characters[CharacterIndex].GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            yield return new WaitForSeconds(.1f);
            Characters[CharacterIndex].GetComponentInChildren<MeshRenderer>().material.color = orig;
            isFlashingRed = false;
        }

    }
   
    GameObject ClosestObject(GameObject obj, GameObject obj2)
    {
        
        GameObject CloserObject = null;
        if(obj == null && obj2 != null)
        {
            return obj;
        }
        else if(obj2 == null && obj != null) 
        {
            return obj2;
        }

        if ((obj.transform.position - transform.position).magnitude > (obj2.transform.position - transform.position).magnitude)
        {
            CloserObject = obj2;
        }
        else
        {
            CloserObject = obj;
        }
        return CloserObject;
    }
    GameObject ReturnClosestThreat()
    {

        GameObject target = null;
        if (NearbyEnemyPlayers.Count > 0)
        {
            foreach (GameObject enemy in NearbyEnemyPlayers)
            {
                if(enemy == null)
                {
                    NearbyEnemyPlayers.Remove(enemy);
                }
                if (target == null)
                {
                    target = enemy;
                }
                else
                {
                    GameObject result = ClosestObject(enemy, target);
                    if (result != null)
                    {
                        target = result;
                    }
                }
            }
        }
        if (bDebug && target != null)
        {
            Debug.Log(target.GetComponent<iOwner>().OwningPlayer().PS_Score.PlayerName);
        }
        
        return target;

    }
    void GoTo(Vector3 Location)
    {
        if (Agent)
        {

            Agent.SetDestination(Location);

        }
    }
    TargetInfo CanSeeTarget(GameObject target)
    {
        if (target)
        {
            TargetInfo info = new TargetInfo();
            info.TargetDir = target.transform.position - headPos.position;
            info.TargetAngleToMe = Vector3.Angle(info.TargetDir, transform.forward);
            info.TargetLastKnownLoc = target.transform.position;
            info.Obj = target;

            if (bDebug)
            {
                Debug.DrawRay(headPos.position, info.TargetDir);

            }
            RaycastHit hit;
            if (info.TargetAngleToMe <= Config.get_FOV())
            {
                if (Physics.Raycast(headPos.position, info.TargetDir, out hit, float.MaxValue))
                {
               
                    info.CanSee = true;
                    return info;
                }
            }
            else
            {
                Agent.stoppingDistance = 0;
                info.CanSee = false;
            }
                
            return info;
        }
        else
        {
            TargetInfo nullresult = new TargetInfo();
            return nullresult;
        }
        
    }

    /// ****** Collider Logic ******
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if (other)
        {
            if ((other.transform.root.CompareTag("Bot") || other.transform.root.CompareTag("Player")))
            {
                if (!NearbyEnemyPlayers.Contains(other.transform.root.gameObject))
                {
                    iOwner HasOwner = other.transform.root.gameObject.GetComponent<iOwner>();
                    if (HasOwner != null)
                    {
                        PlayerState otherPlayer = HasOwner.OwningPlayer();
                        if (otherPlayer != null)
                        {
                            if (otherPlayer.PS_Score.Assigned_Team == Team.FFA || otherPlayer.PS_Score.Assigned_Team != MyPlayerState.PS_Score.Assigned_Team)
                            {
                                //Debug.Log(other.transform.root.gameObject.name + " Added to list of enemies");
                                if (other.transform.root.gameObject != transform.root.gameObject)
                                {
                                    NearbyEnemyPlayers.Add(other.transform.root.gameObject);
                                }

                            }
                            else if (otherPlayer != null && GameMode.instance.config.ThisMatch != GameMode_Config.MatchType.FFA && otherPlayer.PS_Score.Assigned_Team == MyPlayerState.PS_Score.Assigned_Team)
                            {
                                NearbyAllyPlayers.Add(other.transform.root.gameObject);
                            }
                        }
                    }

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

        if (NearbyEnemyPlayers.Contains(other.gameObject.transform.root.gameObject))
        {
            NearbyEnemyPlayers.Remove(other.transform.root.gameObject);
        }
        if (NearbyAllyPlayers.Contains(other.gameObject.transform.root.gameObject))
        {
            NearbyAllyPlayers.Remove(other.transform.root.gameObject);
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
        if (CurrentState != Behaviors.Dead) {
            HP -= amount;

            if (bDebug)
            {
                if(MyPlayerState != null && Instagator != null)
                {
                    Debug.Log(MyPlayerState.PS_Score.PlayerName + " was shot by " + Instagator.PS_Score.PlayerName + "for " + amount + " damage.");
                }
                    
            }
            if (controller)
            {
                controller.OnHit();
            }
            if (AudioConfig)
            {
                AudioSource.PlayClipAtPoint(AudioConfig.hurt[UnityEngine.Random.Range(0, AudioConfig.hurt.Length)], transform.position, AudioConfig.hurt_Vol);
            }
           
            StartCoroutine(flashred());
            if (HP <= 0)
            {
                if (Instagator != null && MyPlayerState)
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
    }

    public void takeDamage(int amount, PlayerState Instigator, bool Headshot)
    {
        if (HP - amount <= 0)
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
        AudioSource.PlayClipAtPoint(AudioConfig.footsteps[UnityEngine.Random.Range(0, AudioConfig.footsteps.Length)], Pos, AudioConfig.footsteps_Vol);
    }

    public RaycastHit GetRaycastHit()
    {
        return new RaycastHit();
    }

    public Transform GetCameraTransform()
    {
        return headPos;
    }
}