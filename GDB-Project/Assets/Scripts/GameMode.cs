using NUnit.Framework;
using System.Collections.Generic;
using Unity.Physics.Authoring;
using UnityEngine;
using static MyScore;
using static GameMode_Config;

public class GameMode : MonoBehaviour
{
    public GameMode_Config config;
    public enum GamePhase { Initialization, PendingStart, Playing, PostMatchScreen, End }
    public GamePhase Phase;
    public static GameMode instance;
    [SerializeField] GameObject PlayerStatePrefab;
    public List<GameObject> OurPlayersStates;
    int Team_A;
    int Team_B;
    int PlayerCount;
    public PlayerController Player;
    public PlayerState Player_PS;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        InitMatch();
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
                OurPlayersStates.Add(bot);
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

        }
        PlayerCount++;

    }
    void InitMatch()
    {
        Phase = GamePhase.Initialization;
        InitBots();
        InitPlayer();
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
