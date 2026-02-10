using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, iDamage
{
    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] CharacterController characterController;
    [SerializeField] CameraController cameraController;
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float sprintModifier = 1.7f;
    [SerializeField] int maxHP = 100;
    [SerializeField] int startingHP = 100;
    [SerializeField] int currentHP;
    [SerializeField] int jumpVelocity = 15;
    [SerializeField] int jumpMax = 1;
    [SerializeField] float gravity = 32f;

    [SerializeField] GameObject DebugGunPref;
    Gun Gun;
    [SerializeField] Transform WeaponHoldPos;

    int jumpCount = 0;
    float startingMovespeed;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (RunState.WasRestarted)
        {
            currentHP = maxHP;          
            RunState.WasRestarted = false;
        }
        else
        {
            currentHP = Mathf.Clamp(startingHP, 0, maxHP); 
        }
        startingMovespeed = moveSpeed;
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

    public void takeDamage(int amount, GameObject Instagator, GameObject Victim)
    {
        currentHP -= amount;
        currentHP -= Mathf.Clamp(currentHP, 0, maxHP);
        UpdateUI();
        if (currentHP <= 0)
        {
            Die();
            Debug.Log("Killed by: " + Instagator.name);
        }
    }

    public void addHealth(int amount)
    {
            currentHP += amount;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            UpdateUI();
    }

    void Die()
    {
        print("You died");
    }

    void Shoot()//Gun script will handle shoot implementation
    {
        if(Gun == null) { return; }
        
        Gun.Shoot();
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
        if (GameManager.instance == null) return;
        if (GameManager.instance.playerHPBar == null) return;
        GameManager.instance.playerHPBar.fillAmount = (float)currentHP / maxHP;
    }

}
