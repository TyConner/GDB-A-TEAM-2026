using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Physics.Authoring;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static GameMode_Config;
using static MyScore;

public class GameMode : MonoBehaviour
{
    public GameMode_Config config;
    public enum GamePhase { Initialization, PendingStart, Playing, PostMatchScreen, End }
    public GamePhase Phase;
    public static GameMode instance;
    [SerializeField] GameObject PlayerStatePrefab;
    public List<PlayerState> OurPlayersStates;
    int Team_A;
    int Team_B;
    int PlayerCount;
    public GameObject Player;
    public PlayerState player_PS;
    public GameObject[] SpawnLocs;
    GameObject Spectator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
        SpawnLocs = GameObject.FindGameObjectsWithTag("SpawnPos");
        Spectator = GameObject.FindGameObjectWithTag("SpectatorCam");
        InitMatch();
       
    }

    public Vector3 GetSpawnLoc()
    {

        return SpawnLocs[UnityEngine.Random.Range(0, SpawnLocs.Length)].transform.position;
    }
    public void SetPhase(GamePhase _phase)
    {
        Phase = _phase;
    }

    public void ToggleSpectatorCamera(bool val)
    {
        Spectator.SetActive(val);
    }
    void InitBots()
    {
        int botCount = config.bots;
        for (int i = 0; i < botCount; i++)
        {
            GameObject bot = Instantiate(PlayerStatePrefab);

            PlayerState bot_PS = bot.GetComponent<PlayerState>();
            if (bot_PS != null)
            {
                
                bot_PS.botStats = config.Difficulty;
                bot_PS.PS_Type = PlayerState.PlayerType.bot;
                //Debug.Log(bot_PS.PS_Score.GetScore(Category.Kills));
                bot_PS.PS_Score.PlayerName = "Bot " + i;
                switch (config.ThisMatch)
                {
                    case GameMode_Config.MatchType.TDM:
                        if (i % 2 == 0)
                        {
                            bot_PS.PS_Score.Assigned_Team = Team.A;
                            Team_A++;
                        }
                        else
                        {
                            bot_PS.PS_Score.Assigned_Team = Team.B;
                            Team_B++;
                        }
                        break;

                    case GameMode_Config.MatchType.FFA:
                        bot_PS.PS_Score.Assigned_Team = Team.FFA;
                        break;
                }
                bot.name = bot_PS.name + "_PlayerState_Prefab";
                OurPlayersStates.Add(bot_PS);
            }
        }
        PlayerCount = botCount;
    }

    void InitPlayer()
    {
        GameObject player = Instantiate(PlayerStatePrefab);
        player.name = "Player";
        player_PS = player.GetComponent<PlayerState>();
        if (player_PS != null)
        {

            switch (config.ThisMatch)
            {
                case GameMode_Config.MatchType.TDM:
                    if (Team_A > Team_B)
                    {
                        player_PS.PS_Score.Assigned_Team = Team.B;
                    }
                    else
                    {
                        player_PS.PS_Score.Assigned_Team = Team.A;
                    }
                    break;
                case GameMode_Config.MatchType.FFA:
                    {
                        player_PS.PS_Score.Assigned_Team = Team.FFA;
                        break;
                    }
            }
            OurPlayersStates.Add(player_PS);
        }
        PlayerCount++;

    }

    void Respawn(PlayerState player)
    {
        // ask game made to RespawnMe
        Vector3 pos = GetSpawnLoc();

        switch (player.PS_Type)
        {
            case PlayerState.PlayerType.player:
                player.EntityRef = Instantiate(player.PlayerPrefab, pos, Quaternion.identity, null);
                player.EntityRef.GetComponent<PlayerController>().MyPlayerState = player;
                Spectator.SetActive(false);

                break;
            case PlayerState.PlayerType.bot:
                player.EntityRef = Instantiate(player.BotPrefab, pos, Quaternion.identity, null);
                player.EntityRef.GetComponent<EnemyAI>().Config = player.botStats;
                player.EntityRef.GetComponent<EnemyAI>().MyPlayerState = player;
                break;
        }
    }

    public void TryRespawn(PlayerState player)
    {
        if(Phase == GamePhase.Playing)
        {
            StartCoroutine(RespawnByTimer(player));
        }
    }


    IEnumerator RespawnByTimer(PlayerState player)
    {
        if (GameMode.instance.Phase == GameMode.GamePhase.Playing)
        {
            yield return new WaitForSeconds(GameMode.instance.config.Respawn_Timer);
            Respawn(player);
        }
    }
    void InitialSpawn()
    {
        foreach (PlayerState var in OurPlayersStates)
        {
            Respawn(var);
        }
    }

    void PlayersActivate()
    {
        foreach (PlayerState var in OurPlayersStates)
        {
            var.MatchStart();
        }
    }

    void DeactivateBots()
    {
        foreach (PlayerState var in OurPlayersStates)
        {
            if(var.PS_Type != PlayerState.PlayerType.player)
            {
                var.MatchOver();
            }
            
        }
    }

    private PlayerState FFA_ChooseWinner()
    {
        PlayerState Winner = null;
        foreach (PlayerState var in OurPlayersStates)
        {
            if (Winner == null)
            {
                Winner = var;
            }
            else
            {
                if (Winner.PS_Score.GetScore(Category.Kills) < var.PS_Score.GetScore(Category.Kills))
                {
                    Winner = var;
                }
            }
        }
        return Winner;
    }
    private Team TDM_ChooseWinner()
    {
        Team WinningTeam;
        int A_Team_Score = 0;
        int B_Team_Score = 0;
        foreach (PlayerState var in OurPlayersStates)
        {
            switch (var.PS_Score.Assigned_Team)
            {
                case Team.A:
                    {
                        A_Team_Score += var.PS_Score.GetScore(Category.Kills);
                        break;
                    }
                case Team.B:
                    {
                        B_Team_Score += var.PS_Score.GetScore(Category.Kills);
                        break;
                    }
            }
        }
        if (A_Team_Score > B_Team_Score)
        {
            WinningTeam = Team.A;
        }
        else { WinningTeam = Team.B; }
        return WinningTeam;
    }
    bool DidPlayerWin()
    {
        bool val = false;
        switch (config.ThisMatch)
        {
            case MatchType.FFA:
                {
                    if(player_PS == FFA_ChooseWinner())
                    {
                        val = true;
                        break;
                        
                    }
                    else
                    {
                        val = false;
                        break;
                    }
                        
                }

            case MatchType.TDM:
                {
                    if(player_PS.PS_Score.Assigned_Team == TDM_ChooseWinner())
                    {
                        val = true;
                        break;
                    }
                    else
                    {
                        val = false;
                        break;
                    }
                    
                }
        }
        return val;
    }
    void InitMatch()
    {
        Phase = GamePhase.Initialization;
        InitBots();
        InitPlayer();
        InitialSpawn();
        Phase = GamePhase.PendingStart;
        GameManager.instance.initalizeMatch(config.MatchLength);
    }

    public void OnPlay()
    {
        Phase = GamePhase.Playing;
        PlayersActivate();

    }

    public void OnMatchOver()
    {
        Phase = GamePhase.PostMatchScreen;
        DeactivateBots();
        //GameManager.instance.ScoreBoard;
        if (DidPlayerWin())
        {
            GameManager.instance.youWin();
        }
        else
        {
            GameManager.instance.youLose();
        }
    }


    public bool bHasReachedGoal(PlayerState player)
    {
        if(config.GameGoal <= player.PS_Score.GetScore(MyScore.Category.Kills))
        {
            if (player.PS_Type == PlayerState.PlayerType.player)
            {
                //player won
            }
            else
            {
               //someone won
            }
            //someone Won, end match
            OnMatchOver();
            return true;
            
            
        }
        return false;

    }

    void EndMatch()
    {
        //cleanup and further logic here


    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
