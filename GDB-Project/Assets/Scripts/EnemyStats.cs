using UnityEngine;

[CreateAssetMenu]

public class EnemyStats : ScriptableObject
{
    [Space(2)][Range(1, 90)][SerializeField] int FOV;
    [Range(20, 100)][SerializeField] int faceTargetSpeed = 50;
    [Range(10, 50)][SerializeField] int AgentAlertedSearchDistance = 10;
    [Range(5, 120)][SerializeField] int AgentAlertTime = 30;
    [Range(1, 10)][SerializeField] int AgentAlertPauseTime = 2;
    [Range(1,150)][SerializeField] int RoamDist;
    [Range(1,10)][SerializeField] int RoamPauseTime;

    public void SetStats(EnemyStats stats)
    {
        FOV = stats.FOV;
        faceTargetSpeed = stats.faceTargetSpeed;
        AgentAlertedSearchDistance = stats.AgentAlertedSearchDistance;
        AgentAlertTime = stats.AgentAlertTime;
        AgentAlertPauseTime = stats.AgentAlertPauseTime;
        RoamDist = stats.RoamDist;
        RoamPauseTime = stats.RoamPauseTime;

    }

    public int get_FOV() { return FOV; }
    public int get_faceTargetSpeed() {  return faceTargetSpeed; }
    public int get_AgentAlertedSearchDistance() {  return AgentAlertedSearchDistance; }
    public int get_AgentAlertTime() { return AgentAlertTime; }
    public int get_AgentAlertPauseTime() { return AgentAlertPauseTime; }  
    public int get_RoamDist() { return RoamDist; }
    public int get_RoamPauseTime() { return RoamPauseTime; }

}
