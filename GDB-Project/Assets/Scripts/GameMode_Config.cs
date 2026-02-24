using UnityEngine;


[CreateAssetMenu]
public class GameMode_Config : ScriptableObject
{
    
    public enum MatchType { TDM, FFA };
    [Header("Match Configuration")]
    [Range(0, 12)]public  int bots;
    [Range(1, 300)]public int MatchLength;
    public bool BotsOnly = false;
    public EnemyStats Difficulty;
    [Range(1,50)]public int GameGoal;
    public MatchType ThisMatch;
    public int Respawn_Timer;
    public int BodyCleanUpTime;

    [Range(0,100)]public int Assist_Coin_Reward = 20;
    [Range(0,1000)]public int Kill_Coin_Reward = 100;
    [Range(0,200)]public int Headshot_Coin_Reward = 50;


}
