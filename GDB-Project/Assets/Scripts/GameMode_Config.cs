using UnityEngine;


[CreateAssetMenu]
public class GameMode_Config : ScriptableObject
{
    enum MatchType { TDM, FFA };
    [Header("Match Configuration")]
    [Range(1,10)][SerializeField] int bots;
    [Range(1, 300)][SerializeField] int MatchLength;
    [SerializeField] EnemyStats Difficulty;
    [Range(1,50)][SerializeField] int GameGoal;
    [SerializeField] MatchType ThisMatch;

}
