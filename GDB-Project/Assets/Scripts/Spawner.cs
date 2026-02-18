using System.Collections;
using System.Threading;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    int EnemiesinRange;
    bool bCanRespawn = true;
    public MyScore.Team team = MyScore.Team.FFA;
    [SerializeField] Material[] TeamMats = new Material[3];

    private void OnValidate()
    {
        switch (team)
        {
            case MyScore.Team.A:
                GetComponent<MeshRenderer>().material = TeamMats[0];
                break;
            case MyScore.Team.B:
                GetComponent<MeshRenderer>().material = TeamMats[1];
                break;
            case MyScore.Team.FFA:
                GetComponent<MeshRenderer>().material = TeamMats[2];
                break;
        }
    }

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if(GameMode.instance.config.ThisMatch == GameMode_Config.MatchType.FFA)
        {
            if (other.transform.root.CompareTag("Bot") || other.transform.root.CompareTag("Player"))
            {
                EnemiesinRange++;
            }
        }
        else
        {
            if (other.transform.root.CompareTag("Bot") || other.transform.root.CompareTag("Player"))
            {
                MyScore score = other.transform.root.GetComponent<MyScore>();
                if(score != null && score.Assigned_Team != team)
                {
                    EnemiesinRange++;
                }
            }
        }
      
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (GameMode.instance.config.ThisMatch == GameMode_Config.MatchType.FFA)
        {
            if (other.transform.root.CompareTag("Bot") || other.transform.root.CompareTag("Player"))
            {
                EnemiesinRange--;
            }
        }
        else
        {
            if (other.transform.root.CompareTag("Bot") || other.transform.root.CompareTag("Player"))
            {
                MyScore score = other.transform.root.GetComponent<MyScore>();
                if (score != null && score.Assigned_Team != team)
                {
                    EnemiesinRange--;
                }
            }
        }

    }

    IEnumerator SpawnerTimeOut()
    {
        bCanRespawn = false;
        yield return new WaitForSeconds(5);
        bCanRespawn = true;
    }

    public void TimeOutSpawner()
    {
        if (bCanRespawn)
        {
            StartCoroutine(SpawnerTimeOut());
        }
    }

    public bool IsEnemyInRange()
    {
        return (EnemiesinRange> 0);
    }

    public bool QueryRespawn()
    {
        return bCanRespawn;
    }
}
