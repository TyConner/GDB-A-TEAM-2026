using UnityEngine;


[CreateAssetMenu]
public class GameMode_Config : ScriptableObject
{
    public enum MatchType { TDM, FFA };
    [Header("Match Configuration")]
    [Range(1,10)]public  int bots;
    [Range(1, 300)]public int MatchLength;
    public EnemyStats Difficulty;
    [Range(1,50)]public int GameGoal;
    public MatchType ThisMatch;
    public int Respawn_Timer;
    public int BodyCleanUpTime;

}
