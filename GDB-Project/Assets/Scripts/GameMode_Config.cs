using UnityEngine;


[CreateAssetMenu]
public class GameMode_Config : ScriptableObject
{
    public void CopyThis(GameMode_Config config)
    {
       ThisMatch = config.ThisMatch;
        bots = config.bots;
        MatchLength = config.MatchLength;
        BotsOnly = config.BotsOnly;
        Difficulty = config.Difficulty;
        GameGoal = config.GameGoal;
        Respawn_Timer = config.Respawn_Timer;
        BodyCleanUpTime = config.BodyCleanUpTime;
    }
    public enum MatchType { TDM, FFA };
    [Header("Match Configuration")]
    [Range(0, 12)]public  int bots = 5;
    [Range(1, 300)]public int MatchLength = 120;
    public bool BotsOnly = false;
    public EnemyStats Difficulty;
    [Range(1,50)]public int GameGoal = 10;
    public MatchType ThisMatch = MatchType.TDM;
    public int Respawn_Timer = 3;
    public int BodyCleanUpTime = 5;

    [Range(0,100)]public int Assist_Coin_Reward = 20;
    [Range(0,1000)]public int Kill_Coin_Reward = 100;
    [Range(0,200)]public int Headshot_Coin_Reward = 50;


}
