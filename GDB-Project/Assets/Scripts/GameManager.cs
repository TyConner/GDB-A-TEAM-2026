using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("       Initialization        ")]
    [SerializeField] String PlayerTag;
    [SerializeField] String PlayerSpawnTag;
    [Header("       Match Initialization        ")]
    [Tooltip("Time in seconds the match lasts")]
    [Range(0, 300)][SerializeField] int MatchTime = 120;
    [Tooltip("Goal to reach so the match ends")]
    [Range(1,10)][SerializeField] int WinGoal = 5;
    [Tooltip("Match Begin Timer")]
    [Range(1, 10)][SerializeField] int TimeUntilMatchStarts = 5;


    [Header("       Menus        ")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    


    [Header("       Game State      ")]
    public bool isPaused;
    public TMP_Text gameGoalCountText;
    public TMP_Text RemainingMatchTime;
    public TMP_Text countdownText;
    public int BodyCleanUpTime = 3;

    [Header("       Player Variable      ")]
    public GameObject player;
    public PlayerController playerScript;
   

    [Header("       Player UI      ")]
    public GameObject DamageScreen;
    public GameObject playerCompass;
    public Image playerCompassNeedle;
    public Image playerCrossHair;
    [SerializeField] Sprite DefaultCrossHairSprite;
    [SerializeField] Sprite DefaultWeaponSprite;


    [Header("       Player Weapon UI Elements      ")]
    public Image playerHPBar;
    public Image playerGun;
    public TMP_Text playerGunName;
    public TMP_Text playerAmmoCur;
    public TMP_Text playerAmmoReserve;



    int matchstarttimer;
    float timeScaleOrig;
    float currenttime;
    int gameGoalCount;

    public static GameManager instance;
    string GetTime() {
        //remaining minutes
        int min = (int)currenttime / 60;
        int sec = (int)currenttime % 60;
        return min.ToString("D2") + ":" + sec.ToString("D2");
    }
    void toggleCrosshair(bool val)
    {
        playerCrossHair.enabled = val;
    }
    
   void updateScore()
    {
        gameGoalCountText.text = "Score: " + gameGoalCount;
    }
    void ismatchOver()
    {
        if(currenttime <= 0)
        {
            statePause();
            if(gameGoalCount >= WinGoal)
            {
                youWin();
            }
            else
            {
                youLose();
            }

        }
    }
    public void updateGunUI(Sprite gunsprite, Sprite crosshairsprite, int ammo_reserve, int ammo_cur, string GunName)
    {

        // change crosshair
        playerGun.sprite = gunsprite;
        // change gun sprite
        playerCrossHair.sprite = crosshairsprite;
        // change ammo reserve count
        updateAmmoUI(ammo_reserve, ammo_cur);
        // change gun name
        playerGunName.text = GunName;

    }
    
    public void ClearGunUI()
    {
      
        // change crosshair
        playerGun.sprite = DefaultWeaponSprite;
        // change gun sprite
        playerCrossHair.sprite = DefaultCrossHairSprite;
        // change ammo reserve count
        updateAmmoUI(0, 0);
        // change gun name
        playerGunName.text = "";

    }
    public void updateAmmoUI(int ammo_reserve, int ammo_cur)
    {   // change ammo reserve count
        playerAmmoReserve.text = ammo_reserve.ToString();
        // change ammo current
        playerAmmoCur.text = ammo_cur.ToString();
    }
    
    IEnumerator countdown()
    {
        if(matchstarttimer >= 0)
        {
            countdownText.text = matchstarttimer.ToString();
            yield return new WaitForSecondsRealtime(1);
            matchstarttimer--;
            countdownText.text = matchstarttimer.ToString();
            StartCoroutine(countdown());
        }
        else
        {
            Time.timeScale = timeScaleOrig;
            currenttime = MatchTime;
            countdownText.enabled = false;
        }
    }
    void initalizeMatch()
    {
        timeScaleOrig = 1;
        Time.timeScale = 0;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        currenttime = MatchTime;
        matchstarttimer = TimeUntilMatchStarts;
        RemainingMatchTime.text = GetTime();
        countdownText.enabled = true;
        StartCoroutine(countdown());
        updateScore();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        initalizeMatch();
    }
    void timer()
    {
        if(Time.timeScale == timeScaleOrig)
        {
            currenttime = currenttime - Time.deltaTime;
            RemainingMatchTime.text = GetTime();
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        timer();
        ismatchOver();
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
        toggleCrosshair(false);
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void stateUnPause()
    {
        toggleCrosshair(true);
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

    public void youWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);

    }
    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        updateScore();
        if (gameGoalCount <= 0)
        {
            //you win
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);

        }
    }
}
public static class RunState
{
    public static bool WasRestarted = false;
}