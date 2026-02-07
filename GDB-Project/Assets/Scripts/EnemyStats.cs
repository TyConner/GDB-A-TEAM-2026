using UnityEngine;

[CreateAssetMenu]

public class EnemyStats : ScriptableObject
{
    [Space(2)][Range(1, 90)][SerializeField] int FOV;
    [Range(20, 100)][SerializeField] int faceTargetSpeed = 50;
    [Range(1.1f, 3.0f)][SerializeField] float AgentSprintMod = 2f;
    [Range(10, 50)][SerializeField] int AgentAlertedSearchDistance = 10;
    [Range(5, 120)][SerializeField] int AgentAlertTime = 30;
    [Range(1, 10)][SerializeField] int AgentAlertPauseTime = 2;
    [Range(1,150)][SerializeField] int RoamDist;
    [Range(1,10)][SerializeField] int RoamPauseTime;
}
