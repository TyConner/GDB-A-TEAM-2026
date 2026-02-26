using UnityEngine;

[CreateAssetMenu]

public class EnemyStats : ScriptableObject
{
    [Space(2)][Range(1, 90)][SerializeField] int FOV;
    [Range(20, 100)][SerializeField] int faceTargetSpeed = 50;
    [Range(10, 50)][SerializeField] int AgentSearchDistance = 10;
    [Range(5, 120)][SerializeField] int AgentSearchTime = 30;
    [Range(1, 10)][SerializeField] int AgentSearchPauseTime = 2;
    [Range(1,150)][SerializeField] int RoamDist;
    [Range(1,10)][SerializeField] int RoamPauseTime;
    [Range(1, 100)][SerializeField] int LowHPThreshhold = 50;
    [Range(1, 150)][SerializeField] int FleeDist = 30;
    [Range(1, 150)][SerializeField] int FleeTime = 10;
    [Range(1, 5)][SerializeField] int MaxFleeCount = 3;
    [Range(1, 100)][SerializeField] int FleeChance = 75;
    [Range(1, 150)][SerializeField] int SafeZoneRadius = 5;
    [Range(1, 150)][SerializeField] int AssistTime = 15;
    [Range(10, 50)][SerializeField] float SensoryRange = 20;

    public void SetStats(EnemyStats stats)
    {
        FOV = stats.FOV;
        faceTargetSpeed = stats.faceTargetSpeed;
        AgentSearchDistance = stats.AgentSearchDistance;
        AgentSearchTime = stats.AgentSearchTime;
        AgentSearchPauseTime = stats.AgentSearchPauseTime;
        RoamDist = stats.RoamDist;
        RoamPauseTime = stats.RoamPauseTime;
        LowHPThreshhold = stats.LowHPThreshhold;
        FleeDist = stats.FleeDist;
        FleeTime = stats.FleeTime;  
        MaxFleeCount = stats.MaxFleeCount;
        FleeChance = stats.FleeChance;
        SafeZoneRadius = stats.SafeZoneRadius;
        AssistTime = stats.AssistTime;
        SensoryRange = stats.SensoryRange;

    }

    public int get_FOV() { return FOV; }
    public int get_faceTargetSpeed() {  return faceTargetSpeed; }
    public int get_AgentSearchDistance() {  return AgentSearchDistance; }
    public int get_AgentSearchTime() { return AgentSearchTime; }
    public int get_AgentSearchPauseTime() { return AgentSearchPauseTime; }  
    public int get_RoamDist() { return RoamDist; }
    public int get_RoamPauseTime() { return RoamPauseTime; }
    public int get_LowHPThreshhold() { return LowHPThreshhold; }
    public int get_AssitTime() { return AssistTime; }

    public int get_FleeDist() { return FleeDist; }
    public int get_FleeTime() { return FleeTime; }

    public int get_FleeChance() { return FleeChance; }
    public int get_MaxFleeCount() { return MaxFleeCount; }
    public int get_SafeZoneRadius() { return SafeZoneRadius; }

    public float get_SensoryRange() { return SensoryRange; }




}
