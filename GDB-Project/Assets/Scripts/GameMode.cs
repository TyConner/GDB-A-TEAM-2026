using NUnit.Framework;
using System.Collections.Generic;
using Unity.Physics.Authoring;
using UnityEngine;
using static MyScore;
using static GameMode_Config;
using System;

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
    public PlayerController Player;
    public PlayerState Player_PS;
    public GameObject[] SpawnLocs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        InitMatch();
        SpawnLocs =  GameObject.FindGameObjectsWithTag("SpawnPos");
    }

    public Vector3 GetSpawnLoc()
    {

        return SpawnLocs[UnityEngine.Random.Range(0, SpawnLocs.Length)].transform.position;
    }
    public void SetPhase(GamePhase _phase)
    {
        Phase = _phase;
    }
    void InitBots()
    {
        int botCount = config.bots;
        for (int i = 0; i < botCount; i++)
        {
            GameObject bot = Instantiate(PlayerStatePrefab);
            PlayerState bot_PS = bot.GetComponent<iOwner>().OwningPlayer();
            if (bot_PS != null)
            {
                bot_PS.botStats = config.Difficulty;
                bot_PS.PS_Type = PlayerState.PlayerType.bot;
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
        PlayerState player_PS = player.GetComponent<iOwner>().OwningPlayer();
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

    void SpawnPlayers()
    {
        foreach (PlayerState var in OurPlayersStates)
        {
            var.Respawn();
        }
    }

    void PlayersActivate()
    {
        foreach (PlayerState var in OurPlayersStates)
        {
            var.MatchStart();
        }
    }

    void PlayersDecativate()
    {
        foreach (PlayerState var in OurPlayersStates)
        {
            var.MatchOver();
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
                    if(Player_PS == FFA_ChooseWinner())
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
                    if(Player_PS.PS_Score.Assigned_Team == TDM_ChooseWinner())
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
        SpawnPlayers();
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
        PlayersDecativate();
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
                GameManager.instance.youWin();
            }
            else
            {
                GameManager.instance.youLose();
            }
            //someone Won, end match
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
