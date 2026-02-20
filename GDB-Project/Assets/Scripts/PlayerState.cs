using System.Collections;
using UnityEngine;
using static MyScore;
public class PlayerState : MonoBehaviour
{
    public enum PlayerType { player, bot , environment};
    public PlayerType PS_Type;
    public MyScore PS_Score;

    public GameObject PlayerPrefab;
    public GameObject BotPrefab;

    public EnemyStats botStats;

    public GameObject EntityRef;

    public float Item_Drop_Height = 1.0f;

    public GameObject Item_Drop;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PS_Type == PlayerType.bot && GameMode.instance)
        {
            botStats = GameMode.instance.config.Difficulty;

        }
        GameManager.instance.scoreboard.GetComponent<Scoreboard>().AddEntry(new ScoreboardEntryData()
        {
            playerState = this,
            entryName = gameObject.name,
            entryScore = 0
        });
    }

    public void OnDeath()
    {
        updateScore(Category.Deaths, 1);
        Debug.Log(PS_Type.ToString());
        if(Item_Drop != null)
        {
            Vector3 pos = transform.position + new Vector3(0, Item_Drop_Height, 0);
            GameObject dropped_Item = Instantiate(Item_Drop, pos, Quaternion.identity);

        }
        switch (PS_Type)
        {
            case PlayerType.player:
                Destroy(EntityRef);
                GameMode.instance.ToggleSpectatorCamera(true);
                break;

            case PlayerType.bot:
                EntityRef.GetComponent<EnemyAI>().enabled = false;
                //EntityRef.GetComponent<Animator>().enabled = false;
                GameObject Body = EntityRef;
                //Debug.Log(Body);
                StartCoroutine(BodyCleanUp(Body));
                break;
        }
        
        RequestRespawn();
    }
    IEnumerator BodyCleanUp(GameObject Body)
    {
        //Debug.Log("Yield Destroy Body Time: " +GameMode.instance.config.BodyCleanUpTime );
        yield return new WaitForSeconds(GameMode.instance.config.BodyCleanUpTime);
        Destroy(Body);
    }
    public void RequestRespawn()
    {
        GameMode.instance.TryRespawn(this);
    }
    public void MatchStart()
    {

        EntityRef.SetActive(true);

    }

    public void MatchOver()
    {
        Destroy(EntityRef);
    }


    public void updateScore(Category cat, int amount)
    {
        if(PS_Type != PlayerType.environment)
        {
            // if we are the envirnment we dont keep score
            PS_Score.ChangeScore(cat, amount);
            
            if (cat == Category.Kills)
            {
<<<<<<< scoreboard
                GameManager.instance.scoreboard.GetComponent<Scoreboard>().UpdateUI();
=======
                
>>>>>>> main
                if (PS_Type == PlayerType.player && GameMode.instance.Phase == GameMode.GamePhase.Playing)
                {
                    
                    GameMode.instance.TeamScoreUpdate(PS_Score.Assigned_Team, cat, amount);
                    if(GameMode.instance.config.ThisMatch == GameMode_Config.MatchType.FFA)
                    {
                        GameManager.instance.updateGameGoal(PS_Score.GetScore(Category.Kills));
                    }
                    if (GameMode.instance.bHasReachedGoal(this))
                    {
                        Debug.Log(PS_Score.PlayerName + " has won!");

                    }
                }

            }
        }
       
    }
   

    

   
    // Update is called once per frame
    void Update()
    {
        
    }
}
