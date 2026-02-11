using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, iDamage, iOwner
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

    [SerializeField] GameObject DebugGunPref;
    Gun Gun;
    [SerializeField] Transform WeaponHoldPos;

    int jumpCount = 0;
    int startingHP;
    float startingMovespeed;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVelocity;

    public PlayerState MyPlayerState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingHP = HP;
        startingMovespeed = moveSpeed;
        GameManager.instance.player = this.gameObject;
        UpdateUI();

        //GameManager.instance.updateGunUI(fields);
    }

    // Update is called once per frame
    void Update()
    {
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

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        moveDir = Vector3.ClampMagnitude(moveDir, 1f); //We must clamp or we go faster when pressing two directions at once
        characterController.Move(moveDir * moveSpeed * Time.deltaTime);

        if (Input.GetButton("Jump"))
        {
            Jump();
        }

        characterController.Move(playerVelocity * Time.deltaTime);

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

    public void takeDamage(int amount, PlayerState Instagator)
    {
        HP -= Mathf.Clamp(amount, 0, startingHP);
        UpdateUI();
        if (HP <= 0)
        {
            Instagator.updateScore(MyScore.Category.Kills, 1);
            Die();
            Debug.Log("Killed by: " + Instagator.PS_Score.PlayerName);
        }
    }

    void Die()
    {

        MyPlayerState.OnDeath();
        //you died ui screen to be called in playerstate
        print("You died");
        
        
  
    }

    void Shoot()//Gun script will handle shoot implementation
    {
        if(Gun == null) { return; }
        
        Gun.Shoot(MyPlayerState);
    }

    void DebugGiveGun()
    {
        Gun newGun = Instantiate(DebugGunPref, WeaponHoldPos).GetComponent<Gun>();
        EquipGun(newGun);
    }

    void EquipGun(Gun newGun)
    {
        if(Gun != null)
        {
            Gun.OnUnequip();
        }

        Gun = newGun;
        Gun.OnEquip();
        Gun.transform.SetParent(WeaponHoldPos);
        Gun.transform.localPosition = Vector3.zero;
        Gun.transform.localRotation = Quaternion.identity;
        Gun.transform.localScale = Vector3.one;


    }

    void DropGun()
    {
        if(Gun == null) return;

        Gun.OnUnequip();
        Destroy(Gun.gameObject);
        Gun = null;
    }

    void UpdateUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / 100;

    }

    public PlayerState OwningPlayer()
    {
        return MyPlayerState;
    }



    public void takeDamage(int amount, PlayerState Instigator, bool Headshot)
    {
        //
    }
}
