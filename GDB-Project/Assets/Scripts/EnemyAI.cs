using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.Transforms;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using static MyScore;

public class EnemyAI : MonoBehaviour, iFootStep, iDamage, iOwner, iUseWeapons
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
    [SerializeField] GameObject Weapon;
    [Space(2)]
    [SerializeField] Gun GunScript;
    [Space(2)]
    [SerializeField] Transform WeaponHoldPos;

    [Space(2)]
    [SerializeField] GameObject PlayerTag;

    [Space(2)]
    [SerializeField] TMP_Text PlayerNameField;

    [Space(2)]
    [SerializeField] Image PlayerColorIndicator;

    [Space(2)]
    public Material TeamMaterial;


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
    List<GameObject> NearbyEnemyPlayers = new();
    List<GameObject> NearbyAllyPlayers = new();

    //private vars
    bool isFlashingRed;

    Color orig;

    public enum Behaviors {Fight, Flee, Search, Assist, Roam, Dead, Idle};
    public Behaviors CurrentState;

    Vector3 SpawningLocation;
    struct TargetInfo{
        public GameObject Obj;
        public Vector3 TargetLastKnownLoc;
        public Vector3 TargetDir;
        public float TargetAngleToMe;
        public float TargetTimer;
       
        public bool CanSee;

        public void Clear()
        {
            Obj = null;
            TargetLastKnownLoc = Vector3.zero;
            TargetDir = Vector3.zero;
            TargetAngleToMe = 0;
            TargetTimer = 0;
            CanSee = false; 
        }
    }

    int HPOrig;

    TargetInfo MyTarget;

    //-----Timers-----

   float SearchTimer;
   float SearchPauseTimer;
   float RoamTimer;
   float FleeTimer;
    // -----------------------
   public bool bFleeing = false;
   bool bStandGround = false;
   int fleeCount = 0;
   float StoppingDistOrig;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RandomCharacter();
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
        PlayerNameField.text = MyPlayerState.PS_Score.PlayerName;
        PlayerColorIndicator.material = GameMode.instance.GetTeamMat(MyPlayerState.PS_Score.Assigned_Team);
        PlayerNameField.color = PlayerColorIndicator.material.color;
        DisableName();
        //Equip our default gun
        EquipDefaultWeapon();
        //TurnOffCollision();
    }

 
    // Update is called once per frame
    void Update()
    {
        bool bCanPlay = Config != null && Agent.enabled && GameMode.instance != null && GameMode.instance.Phase == GameMode.GamePhase.Playing;
        
        if (bCanPlay)
        {
            AiLogic();
            if(CurrentState != Behaviors.Dead)
            {
                UpdateTagRotation();
            }
            else if(CurrentState == Behaviors.Dead)
             {
                DisableTag();
            }
        }
        else
        {
            controller.SetSpeed(0, AnimationTransSpeed);
        }
       
    }

    void UpdateTagRotation()
    {
        if (PlayerTag)
        {
            if (Camera.main)
            {
                PlayerTag.transform.rotation = Quaternion.LookRotation(PlayerTag.transform.position - Camera.main.transform.position);
            }
            
        }
    }

    void DisableTag()
    {
        if (PlayerTag)
        {
            PlayerTag.SetActive(false);
        }
    }

    void EnableTag() {         
        if (PlayerTag)
        {
            PlayerTag.SetActive(true);
        }
    }
    void EnableName()
    {
        if (PlayerTag)
        {
            PlayerNameField.enabled = true;
        }
    }

    void DisableName()
    {
        if (PlayerTag)
        {
            PlayerNameField.enabled = false;
        }
    }

    void EnableIndicator()
    {
        if (PlayerTag)
        {
            PlayerColorIndicator.enabled = true;
        }
    }
    void DisableIndicator()
    {
        if (PlayerTag)
        {
            PlayerColorIndicator.enabled = false;
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
        //shootTimer += Time.deltaTime;
        LocoAnim();
        AssessBehavior();
        BehaviorTree(CurrentState);
         
    }

    void AssessBehavior()
    {
        if(CurrentState != Behaviors.Dead || CurrentState != Behaviors.Idle)
        {
            RoamTimer += Time.deltaTime;
            if (HP <= Config.get_LowHPThreshhold() && fleeCount < Config.get_MaxFleeCount() && !bStandGround && !bFleeing)
            {
                int roll = UnityEngine.Random.Range(0, 100);
                if (roll <= Config.get_FleeChance())
                {
                    CurrentState = Behaviors.Flee;
                    bStandGround = false;
                    // decided to runaway stop assessment and panic.
                    return;
                }
            }
            if (fleeCount >= Config.get_MaxFleeCount())
            {
                bStandGround = true;
            } 
            if (!bStandGround && bFleeing)
            {
                return;
                
            }
            GameObject Threat = ReturnClosestThreat();
            if (Threat != null)
            {
                MyTarget = CanSeeTarget(Threat);
                if (MyTarget.CanSee)
                {
                    CurrentState = Behaviors.Fight;
                }
                else
                {
                    CurrentState = Behaviors.Search;
                }
            }
            else
            {
                    CurrentState = Behaviors.Roam;
            }
           
        }
       
    }
    void BehaviorTree(Behaviors state)
    {
        switch (state)
        {
            case Behaviors.Roam:
                {
                    Agent.stoppingDistance = 0;
                    if (Agent.remainingDistance < 0.01f && RoamTimer >= Config.get_RoamPauseTime())
                    {
                        RoamTimer = 0;
                        NavMeshHit hit = UnitSphere_Rand(SpawningLocation, Config.get_RoamDist());
                        if (hit.hit){ GoTo(hit.position);}
                    }
                    
                    break;
                }
            case Behaviors.Search:
                {
                    Agent.stoppingDistance = 0;
                    SearchTimer += Time.deltaTime;
                    SearchPauseTimer += Time.deltaTime;
                    if(Agent.remainingDistance < 0.01f && SearchTimer >= Config.get_AgentSearchPauseTime())
                    {
                        SearchPauseTimer = 0;
                        NavMeshHit hit = UnitSphere_Rand(MyTarget.TargetLastKnownLoc, Config.get_AgentSearchDistance());
                        if (hit.hit){ GoTo(hit.position);}
                        
                    }
                    if(SearchTimer >= Config.get_AgentSearchTime())
                    {
                        SearchTimer = 0;
                        MyTarget.Clear();
                    }
                    break;
                }
            case Behaviors.Flee:
                {
                    
                    Agent.stoppingDistance = 0;
                    if(FleeTimer <= Config.get_FleeTime() && bFleeing == true)
                    {
                        FleeTimer += Time.deltaTime;
                    }
                    else
                    {
                        bFleeing = false;
                        fleeCount++;
                    }
                    if (!bFleeing && fleeCount < Config.get_MaxFleeCount())
                    {
                        FleeTimer = 0;
                        Vector3 DangerZone = new Vector3(0, 0, 0);
                        Vector3 SafeDir = new Vector3();
                        if (NearbyEnemyPlayers.Count > 0)
                        {
                            foreach (GameObject enemy in NearbyEnemyPlayers)
                            {
                                DangerZone += enemy.transform.position;
                            }
                        }
                        else
                        {
                            DangerZone = transform.position;
                            NavMeshHit aroundme = UnitSphere_Rand(DangerZone, Config.get_SafeZoneRadius());
                            if (aroundme.hit)
                            {
                                DangerZone = aroundme.position;
                            }
                        }
                        SafeDir = (DangerZone - transform.position).normalized;
                        SafeDir = SafeDir * -1;
                        Vector3 SafeZone = new Vector3();
                        SafeZone = (SafeDir * UnityEngine.Random.Range(Config.get_FleeDist() / 2, Config.get_FleeDist())) + transform.position;
                        NavMeshHit hit = UnitSphere_Rand(SafeZone, Config.get_SafeZoneRadius());
                        if (hit.hit) { GoTo(hit.position); }
                        bFleeing = true;
                    }
                    
                        break;
                }
            case Behaviors.Assist:
                {
                    break;
                }
            case Behaviors.Fight:
                {
                    Agent.stoppingDistance = StoppingDistOrig;
                    if (MyTarget.Obj)
                    {
                        GoTo(MyTarget.TargetLastKnownLoc);
                        Agent.stoppingDistance = StoppingDistOrig;
                        faceTarget(MyTarget.Obj);
                        Shoot();
                    }
                    
                    break;
                }
            case Behaviors.Dead:
                {
                    break;
                }
            case Behaviors.Idle:
                {
                    Agent.stoppingDistance = StoppingDistOrig;
                    Agent.SetDestination(transform.position);
                    controller.Idle();
                    break;
                }
        }
    }

    NavMeshHit UnitSphere_Rand(Vector3 pos, int radius)
    {
        Vector3 ranPos = UnityEngine.Random.insideUnitSphere * radius;
        ranPos += pos;
        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, radius, 1);
        return hit;

    }
    void faceTarget(GameObject target)
    {
        
        Vector3 TargetPos = target.transform.position - headPos.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(TargetPos.x, transform.position.y, TargetPos.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * Config.get_faceTargetSpeed());
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

    void cleanList()
    {
        NearbyAllyPlayers.RemoveAll(Player => Player == null);
        NearbyEnemyPlayers.RemoveAll(Player => Player == null);
    }
    GameObject ReturnClosestThreat()
    {
        cleanList();
        GameObject target = null;
        if (NearbyEnemyPlayers.Count > 0)
        {
            foreach (GameObject enemy in NearbyEnemyPlayers)
            {
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
        if (Agent && CurrentState != Behaviors.Dead)
        {

            Agent.SetDestination(Location);

        }
    }
    TargetInfo CanSeeTarget(GameObject target)
    {
        if (target)
        {
            TargetInfo info = new();
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
                    if(hit.collider.gameObject == info.Obj || hit.collider.transform.IsChildOf(info.Obj.transform))
                    {
                        info.CanSee = true;
                    }
                    else
                    {
                        info.CanSee = false;
                    }
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

        iOwner HasOwner = other.transform.root.gameObject.GetComponent<iOwner>();
        if (HasOwner != null)
        {
            PlayerState otherPlayer = HasOwner.OwningPlayer();
            if(otherPlayer == GameMode.instance.player_PS)
            {
                EnableName();
            }
            if (otherPlayer != null && otherPlayer != MyPlayerState)
            {
                cleanList();
                switch (IsEnemy(otherPlayer))
            {
                case true:
                    if(NearbyEnemyPlayers.Count > 0)
                    {
                        if (!NearbyEnemyPlayers.Contains(otherPlayer.EntityRef))
                        {
                            NearbyEnemyPlayers.Add(otherPlayer.EntityRef);
                        }
                    }
                    else
                    {
                        NearbyEnemyPlayers.Add(otherPlayer.EntityRef);
                    }
                        break;
                case false:
                    if (NearbyAllyPlayers.Count > 0)
                    {
                        if (!NearbyAllyPlayers.Contains(otherPlayer.EntityRef))
                        {
                            NearbyAllyPlayers.Add(otherPlayer.EntityRef);
                        }
                    }
                    else
                    {
                        NearbyAllyPlayers.Add(otherPlayer.EntityRef);
                    }
                    break;
                    
                }
            }
        }
       



    }
    private bool IsEnemy(PlayerState player)
    {
        bool val = false;
        if (GameMode.instance.config.ThisMatch == GameMode_Config.MatchType.FFA)
        {
            val = true;
            return val;
        }
        return (player.PS_Score.Assigned_Team != MyPlayerState.PS_Score.Assigned_Team);
    
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        iOwner HasOwner = other.transform.root.gameObject.GetComponent<iOwner>();
        if (HasOwner != null)
        {
            PlayerState otherPlayer = HasOwner.OwningPlayer();
            if (otherPlayer == GameMode.instance.player_PS)
            {
                DisableName();
            }
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
        if (CurrentState == Behaviors.Dead)
        {

            return;

        }
        if(MyPlayerState.PS_Score.Assigned_Team == Team.FFA || MyPlayerState.PS_Score.Assigned_Team != Instagator.PS_Score.Assigned_Team) {
            HP -= amount;
            if (Instagator == GameMode.instance.player_PS)
            {
                GameManager.instance.playHitmarker();
            }

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

            MyTarget = CanSeeTarget(Instagator.EntityRef);
            GoTo(MyTarget.TargetLastKnownLoc);

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
        return GunScript.transform.Find("ProjectileOrigin");
    }

    public void Shoot()
    {
        if (GunScript == null) { return; }

        GunScript.Shoot(MyPlayerState);
    }

    public void EquipDefaultWeapon()
    {
      
        Gun newGun = Instantiate(Weapon, WeaponHoldPos).GetComponent<Gun>();
        EquipGun(newGun);

    }
    public void EquipGun(Gun newGun)
    {
        if(GunScript != null)
        {
            GunScript.OnUnequip();
        }

        GunScript = newGun;
        GunScript.OnEquip(MyPlayerState);
        GunScript.transform.SetParent(WeaponHoldPos);
        GunScript.transform.localPosition = Vector3.zero;
        GunScript.transform.localRotation = Quaternion.identity;
        GunScript.transform.localScale = Vector3.one;
    }

    public void DropGun()
    {
        return;
    }

    void fire()
    {
        //if (shootTimer > shootRate && MyGun != null)
        //{
        //    shootTimer = 0;

        //    controller.OnShoot();

        //}

    }
    public void createBullet()
    {
        ////called from animation event in clip

        //Transform pos = MyGun.transform.Find("ProjectileOrigin");
        //GameObject bullet = Instantiate(Projectile, pos.position, transform.rotation);
        //GameObject flash = Instantiate(MuzzleFlash, pos);
        //bullet.GetComponent<Projectile>().MyOwner = MyPlayerState;
        //AudioSource.PlayClipAtPoint(AudioConfig.gunshot[0], pos.position, AudioConfig.gunshot_Vol);
        //Destroy(flash, .05f);

    }
}