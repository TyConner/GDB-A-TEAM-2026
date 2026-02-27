using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MatchConfig_Scene_Manager : MonoBehaviour
{
    
    [SerializeField]public List<GameMode_Config> MatchConfigs;
    [SerializeField]public List<EnemyStats> EnemyStats;

    [SerializeField] GameMode_Config Custom_Template;

    [SerializeField] MatchConfigScreenButtons UI_EventHandler;

    public GameMode_Config Custom_EditableConfig; 

    void Awake()
    {
        Custom_EditableConfig = ScriptableObject.CreateInstance<GameMode_Config>();
        if (Custom_Template != null)
        {
            Custom_EditableConfig.CopyThis(Custom_Template);
        }
        
    }

    public struct Config_Values
    {

        int MatchType_index;
        int BotDifficulty_Index;
        int MatchLength_Index;
        int MatchGoal_Index;
        int NumBots_Index;

        public void Clear()
        {
            MatchType_index = 0;
            BotDifficulty_Index = 0;
            MatchLength_Index = 0;
            MatchGoal_Index = 0;
            NumBots_Index = 0;

        }

        //Setters
        public void SetMatch_Index(int index) { MatchType_index = index; }
        public void SetLength_Index(int index) { MatchLength_Index = index; }
        public void SetGoal(int index) { MatchGoal_Index = index; }
        public void SetAI_Index(int index) { BotDifficulty_Index = index; }
        public void SetBots_Index(int index) { NumBots_Index = index; }
        
       //Getters
        public int GetMatch_Index() { return MatchType_index; }
        public int GetLength_Index() { return MatchLength_Index; }
        public int GetGoal() { return MatchGoal_Index; }
        public int GetAI_Index() { return BotDifficulty_Index; }
        public int GetBots_Index() { return NumBots_Index; }
        

    }

    public Config_Values Selected_Values;

    private void OnEnable()
    {
        //bind event listeners
        if (UI_EventHandler)
        {
            UI_EventHandler.MatchTypeChange += Selected_Values.SetMatch_Index;
            UI_EventHandler.MatchLengthChange += Selected_Values.SetLength_Index;
            UI_EventHandler.GoalChange += Selected_Values.SetGoal;
            UI_EventHandler.BotDifficultyChanged += Selected_Values.SetAI_Index;
            UI_EventHandler.BotNumChange += Selected_Values.SetBots_Index;
            UI_EventHandler.EventSave += Save;
        }
    }
    private void OnDisable()
    {
        if (UI_EventHandler)
        {
            UI_EventHandler.MatchLengthChange -= Selected_Values.SetMatch_Index;
            UI_EventHandler.MatchLengthChange -= Selected_Values.SetLength_Index;
            UI_EventHandler.GoalChange -= Selected_Values.SetGoal;
            UI_EventHandler.BotDifficultyChanged -= Selected_Values.SetAI_Index;
            UI_EventHandler.BotNumChange -= Selected_Values.SetBots_Index;
        }
    }

    public EnemyStats GetEnemyStats()
    {
        return EnemyStats[Selected_Values.GetAI_Index()];
    }

    private void Save()
    {

    }
}

