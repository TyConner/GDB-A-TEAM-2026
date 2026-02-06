using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
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

    [SerializeField] int shootDamage = 20;
    [SerializeField] int shootDistance = 500;
    [SerializeField] float shootRate = 0.25f;

    [SerializeField] int ammo_reserve = 6;
    [SerializeField] int ammo_cur = 0;

    [SerializeField] Sprite gunsprite;
    [SerializeField] Sprite crosshairsprite;

    string GunName = "default";

    int jumpCount = 0;
    int startingHP;
    float startingMovespeed;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingHP = HP;
        startingMovespeed = moveSpeed;
        UpdateUI();
        updateGunUI();
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
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);
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

    public void TakeDamage(int amount)
    {
        HP -= Mathf.Clamp(amount, 0, startingHP);
        UpdateUI();
        if (HP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        print("You died");
    }

    void Shoot()//Gun script will handle shoot implementation
    {
        //check ammo cur see if we have enough bullets to shoot

        if (shootTimer >= shootRate) //Gun will handle fire rate
        {
            shootTimer = 0;

            //decrement ammo cur


            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreLayer))
            {
                Debug.Log(hit.collider.name);
                //TODO: Implement shoot mechanic (this likely will be done in a gun script, not here)
                // gun.shoot();
            }
        }
    }
    void updateGunUI()
    {
        
        // change crosshair
        GameManager.instance.playerGun.sprite = gunsprite;
        // change gun sprite
        GameManager.instance.playerCrossHair.sprite = crosshairsprite;
        // change ammo reserve count
        GameManager.instance.playerAmmoReserve.text = ammo_reserve.ToString();
        // change ammo current
        GameManager.instance.playerAmmoCur.text = ammo_cur.ToString();
        // change gun name
        GameManager.instance.playerGunName.text = GunName;

    }
    void UpdateUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / 100;

    }
}
