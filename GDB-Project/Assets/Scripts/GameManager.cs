using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor.UIElements;
using System;

public class GameManager : MonoBehaviour
{
    [Header("       Initialization        ")]
    [SerializeField] String PlayerTag;
    [SerializeField] String PlayerSpawnTag;
   

    [Header("       Menus        ")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    


    [Header("       Game State      ")]
    public bool isPaused;
    public TMP_Text gameGoalCountText;
    public TMP_Text RemainingMatchTime;


    [Header("       Player Variable      ")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject playerSpawnLoc;

    [Header("       Player UI      ")]
    public GameObject DamageScreen;
    
    public GameObject playerCompass;
    public Image playerCompassNeedle;


    [Header("       Player Weapon UI Elements      ")]
    public Image playerHPBar;
    public Image playerGun;

    public TMP_Text playerAmmoCur;
    public TMP_Text playerAmmoReserve;

   


    float timeScaleOrig;

    int gameGoalCount;

    public static GameManager instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag(PlayerTag.ToString());
        playerScript = player.GetComponent<PlayerController>();
        playerSpawnLoc = GameObject.FindWithTag(PlayerSpawnTag.ToString());

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);

            }
            else if (menuActive == menuPause)
            {
                stateUnPause();
            }
        }
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void stateUnPause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);

    }
    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
        if (gameGoalCount <= 0)
        {
            //you win
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);

        }
    }
}

