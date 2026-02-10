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

    public GameObject EntityRef;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PS_Type == PlayerType.bot && GameMode.instance)
        {
            botStats = GameMode.instance.config.Difficulty;

        }
    }

    public void MatchStart()
    {
        EntityRef.SetActive(true);

    }

    public void MatchOver()
    {
        Destroy(EntityRef);
    }
    public void Respawn()
    {
        GameMode.instance.Spectator.SetActive(false); 
        Vector3 pos = GameMode.instance.GetSpawnLoc();
        
        switch (PS_Type)
        {
            case PlayerType.player:
                //get pos from GameMode
                EntityRef = Instantiate(PlayerPrefab, pos, Quaternion.identity, null);
                EntityRef.GetComponent<PlayerController>().MyPlayerState = this;


                break;
            case PlayerType.bot:
                EntityRef = Instantiate(BotPrefab, pos, Quaternion.identity, null);
                EntityRef.GetComponent<EnemyAI>().Config = botStats;
                EntityRef.GetComponent<EnemyAI>().MyPlayerState = this;
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
            if (PS_Type == PlayerType.player)
            {
                GameManager.instance.updateGameGoal(PS_Score.GetScore(Category.Kills));
            }
            if (GameMode.instance.bHasReachedGoal(this)){
                Debug.Log(PS_Score.PlayerName + " has won!");
            }
        }
    }
   
<<<<<<< Updated upstream
=======
    public void Respawn_by_Timer()
    {
        GameMode.instance.Spectator.SetActive(true);
        StartCoroutine(SpawnTimer());
    }
>>>>>>> Stashed changes
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
            EntityRef.SetActive(false);

        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
