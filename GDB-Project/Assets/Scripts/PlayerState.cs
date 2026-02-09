using UnityEngine;
using static MyScore;
public class PlayerState : MonoBehaviour
{
    public enum PlayerType { player, bot };
    public PlayerType PS_Type;
    public MyScore PS_Score;

    [SerializeField] GameObject PlayerPrefab;
    [SerializeField] GameObject BotPrefab;

    public EnemyStats botStats;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PS_Type == PlayerType.bot && GameMode.instance)
        {
            botStats = GameMode.instance.config.Difficulty;

        }
    }

    public void Respawn()
    {
        //respawn player/bot;
        // if its a bot change its difficulty to botstats
        //talk to gameModeInstance for a random spawnpos from a placable spawnpoint prefab
    }

    public void updateScore(Category cat, int amount)
    {
        PS_Score.ChangeScore(cat, amount);
        if(cat == Category.Kills)
        {
            if(GameMode.instance.bHasReachedGoal(this)){
                Debug.Log(PS_Score.PlayerName + " has won!");
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
