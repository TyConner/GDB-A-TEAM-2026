using System.Collections;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UI;
using static MyScore;

public class PlayerController : MonoBehaviour, iDamage, iOwner, iUseWeaponsAndItems, IPush
{
    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] CharacterController characterController;
    [SerializeField] CameraController cameraController;
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float sprintModifier = 1.7f;
    [SerializeField] int HP = 100;
    [SerializeField] int jumpVelocity = 15;
    [SerializeField] int jumpMax = 1;
    [SerializeField] float gravity = 32f;
    [SerializeField] GameObject tntPref;
    [SerializeField] Transform throwPoint;
    [SerializeField] float throwForce = 20f;
    [SerializeField] int pushVelTime;

    [SerializeField] GameObject DebugGunPref;
    Gun Gun;
    [SerializeField] Transform WeaponHoldPos;

    int jumpCount = 0;
    int startingHP;
    float startingMovespeed;
    public int currentTNT = 0;
    public int maxTNT = 3;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVelocity;
    Vector3 pushVel;

    public PlayerState MyPlayerState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingHP = HP;
        startingMovespeed = moveSpeed;
        GameManager.instance.player = this.gameObject;
        GameManager.instance.UpdateTNTUI(currentTNT);
        EquipDefaultWeapon();
        UpdateUI();
        
        //GameManager.instance.updateGunUI(fields);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameMode.instance.Phase != GameMode.GamePhase.Playing)
        {
            return;
        }
        Movement();
        Sprint();

        shootTimer += Time.deltaTime;

        if (Input.GetButton("Fire1"))
        {
            Shoot();
        }

        if(Input.GetButtonDown("DebugGun"))
        {
            if(Gun == null)
            {
                DebugGiveGun();
            }
            else
            {
                DropGun();
            }
            
        }
        if (Input.GetButtonDown("Reload"))
        {
            if (Gun != null)
            {
                Gun.Reload();
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            ThrowTNT();
        }
    }

    public void SetOwningPlayer(PlayerState Player)
    {

        MyPlayerState = Player;

    }
    void Movement()
    {
        if (characterController.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero; //zero out gravity
        }

        pushVel = Vector3.Lerp(pushVel, Vector3.zero, pushVelTime * Time.deltaTime);

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        moveDir = Vector3.ClampMagnitude(moveDir, 1f); //We must clamp or we go faster when pressing two directions at once
        characterController.Move(moveDir * moveSpeed * Time.deltaTime);

        if (Input.GetButton("Jump"))
        {
            Jump();
        }

        characterController.Move((playerVelocity + pushVel) * Time.deltaTime);

        playerVelocity.y -= gravity * Time.deltaTime;//gravity


    }

    void Jump()
    {
        if (jumpCount < jumpMax)
        {
            playerVelocity.y = jumpVelocity;
            jumpCount++;
        }
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            moveSpeed *= sprintModifier;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            moveSpeed = startingMovespeed;
        }
    }

    public void takeDamage(int amount, PlayerState Instagator, bool Headshot = false)
    {
        if (MyPlayerState == null || MyPlayerState.PS_Score == null)
        {
            Debug.LogWarning("EnemyAI: MyPlayerState or PS_Score is null!");
            return;
        }

        if (Instagator == null || Instagator.PS_Score == null)
        {
            Debug.LogWarning("EnemyAI: Instagator or PS_Score is null!");
            return; 
        }
        
        if (MyPlayerState.PS_Score.Assigned_Team == Team.FFA ||
           MyPlayerState.PS_Score.Assigned_Team != Instagator.PS_Score.Assigned_Team)
        {
            HP -= Mathf.Clamp(amount, 0, startingHP);
            MyPlayerState.OnDamaged(Instagator, amount);
            UpdateUI();
            StartCoroutine(flashScreen());

            if (HP <= 0)
            {
                //change when the player can die of a headshot.
                MyPlayerState.OnDeath(Instagator, Headshot);
                Die(Instagator);
                Debug.Log("Killed by: " + Instagator.PS_Score.PlayerName);
            }
        }
    }

    void Die(PlayerState Instagator)
    {
        Gun Killers_Gun = null;
        if (Instagator.EntityRef != null)
        {
             Killers_Gun = Instagator.EntityRef.GetComponentInChildren<Gun>();
        }
        
        if(Killers_Gun != null)
        {
            KillFeedManager.instance.HandleKill(Instagator.PS_Score.PlayerName, MyPlayerState.PS_Score.PlayerName, Instagator.EntityRef.GetComponentInChildren<Gun>().GunName);
        }
        GameManager.instance.DamageScreen.SetActive(false);
        //MyPlayerState.OnDeath();
        //you died ui screen to be called in playerstate
        print("You died");
        
        
  
    }

    void Shoot()//Gun script will handle shoot implementation
    {
        if (Gun == null) { return; }

        Gun.Shoot(MyPlayerState);
    }

    void DebugGiveGun()
    {
        Gun newGun = Instantiate(DebugGunPref, WeaponHoldPos).GetComponent<Gun>();
        EquipGun(newGun);
    }

    public void EquipGun(Gun newGun)
    {
        if(Gun != null)
        {
            Gun.OnUnequip();
        }

        Gun = newGun;
        Gun.OnEquip(MyPlayerState);
        Gun.transform.SetParent(WeaponHoldPos);
        Gun.transform.localPosition = Vector3.zero;
        Gun.transform.localRotation = Quaternion.identity;
        Gun.transform.localScale = Vector3.one;


    }

    public void addHealth(int amount)
    {
        startingHP += amount;
        startingHP = Mathf.Clamp(startingHP, 0, HP);
        UpdateUI();
        StartCoroutine(flashHealScreen());
    }

    public void DropGun()
    {
        if(Gun == null) return;

        Gun.OnUnequip();
        Destroy(Gun.gameObject);
        Gun = null;
    }

    void UpdateUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP/ startingHP;

    }

    public PlayerState OwningPlayer()
    {
        return MyPlayerState;
    }

    IEnumerator flashScreen()
    {
        GameManager.instance.DamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.DamageScreen.SetActive(false);
    }

    IEnumerator flashHealScreen()
    {
        GameManager.instance.HealScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.HealScreen.SetActive(false);
    }

    public RaycastHit GetRaycastHit()
    {
        RaycastHit hit;
        Physics.Raycast(cameraController.transform.position, cameraController.transform.forward, out hit);
        print(GameManager.instance.RemainingMatchTime.text + " Hit: "+hit.collider.name);
        return hit;
    }

    public Transform GetCameraTransform()
    {
        return cameraController.transform;
    }

    public void AddTNT(int amount)
    {
        currentTNT += amount;
        currentTNT = Mathf.Clamp(currentTNT, 0, maxTNT);
        GameManager.instance.UpdateTNTUI(currentTNT);
    }

    public bool UseTNT()
    {
        if (currentTNT <= 0)
            return false;

        currentTNT--;
        GameManager.instance.UpdateTNTUI(currentTNT);
        return true;
    }

    public void EquipDefaultWeapon()
    {
        Gun newGun = Instantiate(DebugGunPref, WeaponHoldPos).GetComponent<Gun>();
        EquipGun(newGun);
    }

    public void ThrowTNT()
    {
        if (!UseTNT()) return;

        GameObject tnt = Instantiate(tntPref, throwPoint.position, throwPoint.rotation);

        TNTStick stick = tnt.GetComponent<TNTStick>();
        stick.MyOwner = MyPlayerState; 

        Rigidbody rb = tnt.GetComponent<Rigidbody>();
        rb.AddForce(throwPoint.forward * throwForce, ForceMode.VelocityChange);
    }

    public void getPushVel(Vector3 pushAmount)
    {
        pushVel += pushAmount;
    }
}
