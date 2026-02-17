using System.Collections;
using System.Threading;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    int EnemiesinRange;
    bool bCanRespawn = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        
        if(other.transform.root.CompareTag("Bot") || other.transform.root.CompareTag("Player"))
        {
            EnemiesinRange++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.transform.root.CompareTag("Bot") || other.transform.root.CompareTag("Player"))
        {
            EnemiesinRange--;
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
