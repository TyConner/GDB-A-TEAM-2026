using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Physics.Authoring;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
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
    MyScore RedTeam = new();
    [SerializeField] public Material RedMat;
    MyScore BlueTeam = new();
    [SerializeField] public Material BlueMat;

    int PlayerCount;
    public GameObject Player;
    public PlayerState player_PS;
    public List<Spawner> SpawnLocs;
    GameObject Spectator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {

        FindSpawners();
        Spectator = GameObject.FindGameObjectWithTag("SpectatorCam");
        InitMatch();
       
    }

    private void FindSpawners()
    {
        GameObject[] SpawnerObjs = GameObject.FindGameObjectsWithTag("SpawnPos");
        foreach (GameObject obj in SpawnerObjs)
        {
            if (obj != null)
            {
                Spawner tmp = obj.GetComponent<Spawner>();
                if (tmp != null)
                {
                    SpawnLocs.Add(tmp);
                }
            }
        }
    }
    public Material GetTeamMat(Team team)
    {
        switch (team)
        {
            case Team.A:
                return RedMat;
            case Team.B:
                return BlueMat;
            case Team.FFA:
                return RedMat;
            default:
                return null;
        }
    }
    private Spawner GetValidSpawner(MyScore.Team playersTeam)
    {
        int count = SpawnLocs.Count;
        List<Spawner> validspawns = new List<Spawner>();
        for (int i = 0; i< count; i++)
        {
            if(!SpawnLocs[i].IsEnemyInRange() && SpawnLocs[i].QueryRespawn() && SpawnLocs[i].team == playersTeam)
            {
                validspawns.Add(SpawnLocs[i]);
            }
        }
        //Debug.Log("valid spawners: " + validspawns.Count());

        if (validspawns.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, validspawns.Count);
            return validspawns[index];
        }
       

        return null;
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
                bot.name = bot_PS.PS_Score.PlayerName + "_PlayerState";
                OurPlayersStates.Add(bot_PS);
            }
        }
        PlayerCount = botCount;
    }

    void InitPlayer()
    {
        if (!config.BotsOnly) {

            GameObject player = Instantiate(PlayerStatePrefab);
            player_PS = player.GetComponent<PlayerState>();
            player_PS.PS_Score.PlayerName = "Player";
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
                player.name = player_PS.PS_Score.PlayerName + "_PlayerState";
                OurPlayersStates.Add(player_PS);
            }
            PlayerCount++;
        }

       

    }

    void Respawn(PlayerState player)
    {
        // ask game made to RespawnMe
        Spawner SpawnPoint = GetValidSpawner(player.PS_Score.Assigned_Team);
        Vector3 pos;
        if (SpawnPoint != null)
        {
            
            pos = SpawnPoint.transform.position;
            SpawnPoint.TimeOutSpawner();

        }
        else
        {
            Debug.Log("No Spawner Found");
            TryRespawn(player);
           return;
        }
            switch (player.PS_Type)
            {
                case PlayerState.PlayerType.player:
                    player.EntityRef = Instantiate(player.PlayerPrefab, pos, Quaternion.identity, null);
                    player.EntityRef.GetComponent<iOwner>().SetOwningPlayer(player);
                    Spectator.SetActive(false);

                    break;
                case PlayerState.PlayerType.bot:
                    player.EntityRef = Instantiate(player.BotPrefab, pos, Quaternion.identity);
                    player.EntityRef.GetComponent<EnemyAI>().SetAIConfig(player.botStats);
                    player.EntityRef.GetComponent<EnemyAI>().GetComponent<iOwner>().SetOwningPlayer(player);
                    break;
            }
        player.EntityRef.name = player.PS_Score.PlayerName;
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

    public void OnMatchOver(PlayerState player)
    {
        Phase = GamePhase.PostMatchScreen;
        //display scoreboard and otherend 
        if (player == null)
        {
            bool PlayerWin = DidPlayerWin();
            if (PlayerWin)
            {
                GameManager.instance.youWin();
            }
            else
            {
                GameManager.instance.youLose();
            }
        }
        else if (player.PS_Type == PlayerState.PlayerType.player)
        {
            GameManager.instance.youWin();
        }
        else
        {
            GameManager.instance.youLose();
        }


        DeactivateBots();
    }

    public bool TDM_HasReachedGoal()
    {
        int A_Team_Score = RedTeam.GetScore(Category.Kills);
        int B_Team_Score = BlueTeam.GetScore(Category.Kills);
        
        if (A_Team_Score >= config.GameGoal || B_Team_Score >= config.GameGoal)
        {
            return true;
        }
        else { return false; }
    }

    public bool FFA_HasReachedGoal(PlayerState player)
    {
        if (player.PS_Score.GetScore(Category.Kills) >= config.GameGoal)
        {
            return true;
        }
        else { return false; }
    }
    public bool bHasReachedGoal(PlayerState player)
    {
        switch(config.ThisMatch)
        {
            case MatchType.FFA:
                return FFA_HasReachedGoal(player);
            case MatchType.TDM:
                return TDM_HasReachedGoal();
            default:
                return false;
        }
  

    }

    public void TeamScoreUpdate(MyScore.Team team, MyScore.Category category, int amount)
    {
        if (config.ThisMatch == MatchType.TDM)
        {
            switch (team)
            {
                case Team.A:
                    RedTeam.ChangeScore(category, amount);
                    break;
                case Team.B:
                    BlueTeam.ChangeScore(category, amount);
                    break;
            }
            if(category == Category.Kills)
            {
                if(bHasReachedGoal(null))
                {
                    OnMatchOver(null);
                }
            }
        }
        else
        {
            return;
        }

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
