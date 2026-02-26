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
    GameMode MatchGameMode;

    [Tooltip("Match Begin Timer")]
    [Range(1, 10)][SerializeField] int TimeUntilMatchStarts = 10;


    [Header("       Menus        ")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;



    [Header("       Game State      ")]
    int MatchTime;
    public bool isPaused;
    public TMP_Text gameGoalCountText;
    public TMP_Text RemainingMatchTime;
    public TMP_Text countdownText;
    public int BodyCleanUpTime = 3;

    [Header("       Player Variable      ")]
    public GameObject player;
    public PlayerController playerScript;


    [Header("       Player UI      ")]
    [SerializeField] public GameObject FFASCORE;
    public GameObject DamageScreen;
    public GameObject HealScreen;
    public GameObject playerCompass;
    public Image playerCompassNeedle;
    public Image playerCrossHair;
    [SerializeField] Sprite DefaultCrossHairSprite;
    [SerializeField] Sprite DefaultWeaponSprite;
    [SerializeField] public GameObject scoreboard;
    [SerializeField] public GameObject scoreboardBackground;

    public Image playerHitmarker;

    [Header("SFX")]
    [SerializeField] AudioClip hitmarkerSound2D;
    [SerializeField] float hitmarkerSoundVolume = 1f;

    [Header("       Player Weapon UI Elements      ")]
    public Image playerHPBar;
    public Image playerGun;
    public TMP_Text playerGunName;
    public TMP_Text playerAmmoCur;
    public TMP_Text playerAmmoReserve;
    public TMP_Text playerTNTText;



    int matchstarttimer;
    float timeScaleOrig;
    float currenttime;
    int gameGoalCount;
    AudioSource audioSource2D;

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
    
    public void playHitmarker()
    {
        StartCoroutine(HitmarkerRoutine());
        //Debug.Log("HitMarker");
    }

    IEnumerator HitmarkerRoutine()
    {
        audioSource2D.PlayOneShot(hitmarkerSound2D, hitmarkerSoundVolume);
        playerHitmarker.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        playerHitmarker.gameObject.SetActive(false);
    }
    void updateScore()
    {
        gameGoalCountText.text = "Score: " + gameGoalCount;
    }

    public void updateGunUI(Sprite gunsprite, Sprite crosshairsprite, int ammo_reserve, int ammo_cur, string GunName)
    {

        // change crosshair
        playerGun.sprite = gunsprite;
        // change gun sprite
        playerCrossHair.sprite = crosshairsprite;
        playerCrossHair.rectTransform.sizeDelta = new Vector2(crosshairsprite.rect.width, crosshairsprite.rect.height);
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
    public void UpdateTNTUI(int amount)
    {
        if (playerTNTText != null)
            playerTNTText.text = amount.ToString();
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
            GameMode.instance.OnPlay();
        }
    }
    public void initalizeMatch(int MatchLength)
    {
        timeScaleOrig = 1;
        Time.timeScale = timeScaleOrig;
        currenttime = MatchTime = MatchLength;
        //player = GameObject.FindWithTag("Player");
        //playerScript = player.GetComponent<PlayerController>();
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
        MatchGameMode = GameMode.instance;
        //initalizeMatch();
    }
    void timer()
    {
        if (GameMode.instance)
        {
            if (GameMode.instance.Phase.Equals(GameMode.GamePhase.Playing))
            {
                currenttime = currenttime - Time.deltaTime;
                RemainingMatchTime.text = GetTime();
            }
        }
        
        
    }
    // Update is called once per frame
    void Update()
    {

        timer();

        if(currenttime <= 0)
        {
            GameMode.instance.OnMatchOver(null);
        }
      
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

        if (Input.GetButtonDown("Tab"))
        {
            scoreboardBackground.SetActive(true);
        }else if (Input.GetButtonUp("Tab"))
        {
            scoreboardBackground.SetActive(false);
        }
    }

    private void Start()
    {
        audioSource2D = GetComponent<AudioSource>();

        if (audioSource2D == null)
        {
            audioSource2D = gameObject.AddComponent<AudioSource>();
        }

        audioSource2D.playOnAwake = false;
        audioSource2D.spatialBlend = 0f; // 0 = 2D, 1 = 3D
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
        gameGoalCount = amount;
        updateScore();
       
    }
}

