using System.Collections;
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

    GameObject EntityRef;

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
        Vector3 pos = Vector3.zero; // = return func from game mode that gives a spawn location
        
        switch (PS_Type)
        {
            case PlayerType.player:
                //get pos from GameMode
                EntityRef = Instantiate(PlayerPrefab, pos, Quaternion.identity, null);
                PlayerController controller = EntityRef.GetComponent<PlayerController>();

                break;
            case PlayerType.bot:
                EntityRef = Instantiate(BotPrefab, pos, Quaternion.identity, null);
                EntityRef.GetComponent<EnemyStats>().SetStats(botStats);
                break;
        }
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
   
    IEnumerator SpawnTimer()
    {
        if (GameMode.instance.Phase == GameMode.GamePhase.Playing)
        {
            yield return new WaitForSeconds(GameMode.instance.config.Respawn_Timer);
            Respawn();
        }
        else if(GameMode.instance.Phase == GameMode.GamePhase.PendingStart)
        {

            Respawn();
            switch (PS_Type)
            {
                case PlayerType.bot:
                    break;
                case PlayerType.player:
                    break;
            }

        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
