using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using static EnemyAI;
using static MyScore;
public class PlayerState : MonoBehaviour
{
    public enum PlayerType { player, bot , environment};
    public enum PlayerPhase { Alive, Dead };
    public PlayerPhase PS_Phase;

    public PlayerType PS_Type;
    public MyScore PS_Score;

    public GameObject PlayerPrefab;
    public GameObject BotPrefab;

    public EnemyStats botStats;

    public GameObject EntityRef;

    public float Item_Drop_Height = 1.0f;

    public GameObject Item_Drop;
    public struct DamagersInfo
    {
        public PlayerState Player;
        public float Damage;
        public float DamageTimer;

    }

    List<DamagersInfo> RecentDamagers = new List<DamagersInfo>();

    private ScoreboardEntryUI myscoreUI;
    private void Awake()
    {
        PS_Score = new MyScore();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PS_Type == PlayerType.bot && GameMode.instance)
        {
            botStats.SetStats(GameMode.instance.config.Difficulty);

        }
        //GameManager.instance.scoreboard.GetComponent<Scoreboard>().AddEntry(new ScoreboardEntryData()
        //{
        //    playerState = this,
        //    entryName = gameObject.name,
        //    entryScore = 0
        //});
        
    }

    private void AddToScoreBoard()
    {

        if(PS_Type != PlayerType.environment)
        {
            myscoreUI = GameManager.instance.scoreboard.GetComponent<Scoreboard>().AddEntry(PS_Score);
        }
        if(PS_Type == PlayerType.player && GameMode.instance.config.ThisMatch == GameMode_Config.MatchType.FFA)
        {
            myscoreUI.TeamColor = new Color(0f,255f,0f,40f);
            myscoreUI.Refresh(PS_Score);
        }

    }
    private void LogRecentDamagers(PlayerState Instigator, int Amount)
    {
        bool bContainsPlayer = false;
        for (int i = 0; i < RecentDamagers.Count; i++)
        {
            if (RecentDamagers[i].Player == Instigator)
            {
                RecentDamagers[i] = new DamagersInfo { Player = Instigator, Damage = RecentDamagers[i].Damage + Amount, DamageTimer = 0 };
                bContainsPlayer = true;
                return;
            }
            
         
        }
        if (!bContainsPlayer)
        {
            //if we dont have the player in the list we add them.
            RecentDamagers.Add(new DamagersInfo { Player = Instigator, Damage = Amount, DamageTimer = 0 });
        }
    }

    private void RecentDamagersTimer()
    {
        for (int i = 0; i < RecentDamagers.Count; i++)
        {
            RecentDamagers[i] = new DamagersInfo { Player = RecentDamagers[i].Player, Damage = RecentDamagers[i].Damage, DamageTimer = RecentDamagers[i].DamageTimer + Time.deltaTime };
            if (RecentDamagers[i].DamageTimer >= 90f)
            {
                RecentDamagers.RemoveAt(i);
            }
        }
    }

    private void ClearRecentDamagers()
    {
                RecentDamagers.Clear();
    }

    private void AwardAssists(PlayerState Killer)
    {
        for (int i = 0; i < RecentDamagers.Count; i++)
        {
            if (RecentDamagers[i].Player != Killer)
            {
                RecentDamagers[i].Player.updateScore(Category.Assists, 1);
            }
        }
    }

    public void AwardKill(PlayerState Instigator, bool headshot)
    {
        if (Instigator != null && this)
        {
            Instigator.updateScore(Category.Kills, 1);
            if(headshot)
                {
                    Instigator.updateScore(Category.Headshots, 1);
            }
        }

    }

    public void OnDamaged(PlayerState Instigator, int Amount)
    {
        LogRecentDamagers(Instigator, Amount);
    }
    public void OnDeath(PlayerState Killer, bool headshot)
    {
        
        updateScore(Category.Deaths, 1);
        AwardAssists(Killer);
        AwardKill(Killer, headshot);
        ClearRecentDamagers();
        //Debug.Log(PS_Type.ToString());
        if (Item_Drop != null)
        {
            Vector3 pos = transform.position + new Vector3(0, Item_Drop_Height, 0);
            GameObject dropped_Item = Instantiate(Item_Drop, pos, Quaternion.identity);

        }
        switch (PS_Type)
        {
            case PlayerType.player:
                Destroy(EntityRef);
                GameMode.instance.ToggleSpectatorCamera(true);
                break;

            case PlayerType.bot:
                EntityRef.GetComponent<EnemyAI>().enabled = false;
                //EntityRef.GetComponent<Animator>().enabled = false;
                GameObject Body = EntityRef;
                //Debug.Log(Body);
                StartCoroutine(BodyCleanUp(Body));
                break;
        }
        PS_Phase = PlayerPhase.Dead;
        RequestRespawn();
    }
    IEnumerator BodyCleanUp(GameObject Body)
    {
        //Debug.Log("Yield Destroy Body Time: " +GameMode.instance.config.BodyCleanUpTime );
        yield return new WaitForSeconds(GameMode.instance.config.BodyCleanUpTime);
        Destroy(Body);
    }
    public void RequestRespawn()
    {
        GameMode.instance.TryRespawn(this);
    }
    public void MatchStart()
    {

        EntityRef.SetActive(true);
        AddToScoreBoard();

    }

    public void MatchOver()
    {
        Destroy(EntityRef);
    }

    public void OnRespawn()
    {
        PS_Phase = PlayerPhase.Alive;
    }
    public void DropGun()
    {
        switch (PS_Type)
        {
            case PlayerType.player:
                PlayerController pc = EntityRef.GetComponent<PlayerController>();
                if (pc)
                {
                                       pc.DropGun();
                }
                break;
            case PlayerType.bot:
                EnemyAI ai = EntityRef.GetComponent<EnemyAI>();
                if (ai)
                {
                    ai.DropGun();
                }
                break;
        }
    }
    public void updateScore(Category cat, int amount)
    {
        if(PS_Type != PlayerType.environment)
        {
            // if we are the envirnment we dont keep score
            // if match is over we escape
            if(GameMode.instance.Phase != GameMode.GamePhase.Playing)
            {
                return;
            }
            PS_Score.ChangeScore(cat, amount);
            
            if (cat == Category.Kills)
            {
                GameManager.instance.scoreboard.GetComponent<Scoreboard>().UpdateUI();
                
                if (GameMode.instance.Phase == GameMode.GamePhase.Playing)
                {
                    switch(GameMode.instance.config.ThisMatch)
                    {
                        case GameMode_Config.MatchType.TDM:
                            GameMode.instance.TeamScoreUpdate(PS_Score.Assigned_Team, cat, amount);
                            break;

                        case GameMode_Config.MatchType.FFA:
                                if (GameMode.instance.bHasReachedGoal(this))
                                {
                                    //Debug.Log(PS_Score.PlayerName + " has won!");
                                    GameMode.instance.OnMatchOver(this);
                            }
                            break;

                    }
                    
                    if (PS_Type == PlayerType.player && GameMode.instance.config.ThisMatch == GameMode_Config.MatchType.FFA)
                    {
                        //update our players UI goal tracker if we are in FFA and we got a kill
                        GameManager.instance.updateGameGoal(PS_Score.GetScore(Category.Kills));
                    }
                    
                }

            }
            myscoreUI.Refresh(PS_Score);
        }
       
    }
   

    

   
    // Update is called once per frame
    void Update()
    {
        RecentDamagersTimer();
    }
}
