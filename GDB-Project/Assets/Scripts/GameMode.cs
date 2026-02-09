using NUnit.Framework;
using System.Collections.Generic;
using Unity.Physics.Authoring;
using UnityEngine;
using static MyScore;
using static GameMode_Config;

public class GameMode : MonoBehaviour
{
    public GameMode_Config config;
    public static GameMode instance;
    [SerializeField] GameObject PlayerStatePrefab;
    public List<GameObject> OurPlayersStates;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        InitMatch();
    }
    void InitMatch()
    {
        int botCount = config.bots;
        for (int i = 0; i < botCount; i++)
        {
            GameObject bot = Instantiate(PlayerStatePrefab);
            PlayerState bot_PS = bot.GetComponent<PlayerState>();
            if(bot_PS != null)
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
                        }
                        else
                        {
                            bot_PS.PS_Score.Assigned_Team = Team.B;
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
        GameObject player = Instantiate(PlayerStatePrefab);
        player.name = "Player_PlayerState_Prefab";
        PlayerState player_PS = player.GetComponent<PlayerState>();
        if(player_PS != null)
        {
            player_PS.PS_Score.PlayerName = "Player";
            player_PS.PS_Type = PlayerState.PlayerType.player;
            OurPlayersStates.Add(player);
        }
        int playercount = botCount + 1;

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
