using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using Unity.Entities;
using UnityEngine;

public class MatchConfig_Scene_Manager : MonoBehaviour
{
    
    [SerializeField]public List<GameMode_Config> MatchConfigs;
    [SerializeField]public List<EnemyStats> EnemyStats;

    [SerializeField] GameMode_Config Custom_Template;

    [SerializeField] MatchConfigScreenButtons UI_EventHandler;

    public GameMode_Config Custom_EditableConfig;
    public static MatchConfig_Scene_Manager instance;

    void Awake()
    {
        Custom_EditableConfig = ScriptableObject.CreateInstance<GameMode_Config>();
        if (Custom_Template != null)
        {
            Custom_EditableConfig.CopyThis(Custom_Template);
        }
        instance = this;
        
    }

    public class Config_Values
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

    public Config_Values Match_Config_instance = new();
    Config_Values Custom_Values = new();
    Config_Values Defaults =new();

    void MatchTypeChange(int val)
    {
        switch (val)
        {
            case 0:
                //ffa-default
                Un_Subscribe();
                Match_Config_instance = Defaults;
                Custom_EditableConfig.CopyThis(MatchConfigs[val]);
                break;
            case 1:
                //tdm-default
                Un_Subscribe();
                Match_Config_instance = Defaults;
                Custom_EditableConfig.CopyThis(MatchConfigs[val]);
                break;
            case 2:
                //custom
                Match_Config_instance = Custom_Values;
                Subscribe();
                break;

        }
    }
    private void OnEnable()
    {
        UI_EventHandler.MatchTypeChange += MatchTypeChange;
        
    }
    private void OnDisable()
    {
        UI_EventHandler.MatchTypeChange -= MatchTypeChange;
        Un_Subscribe();
    }
    private void Subscribe()
    {
        //bind event listeners
        if (UI_EventHandler)
        {
            
            UI_EventHandler.CustomMatchTypeChange += Match_Config_instance.SetMatch_Index;
            UI_EventHandler.MatchLengthChange += Match_Config_instance.SetLength_Index;
            UI_EventHandler.GoalChange += Match_Config_instance.SetGoal;
            UI_EventHandler.BotDifficultyChanged += Match_Config_instance.SetAI_Index;
            UI_EventHandler.BotNumChange += Match_Config_instance.SetBots_Index;
            UI_EventHandler.EventSave += Save;
        }
    }
    private void Un_Subscribe()
    {
        if (UI_EventHandler)
        {
            
            UI_EventHandler.CustomMatchTypeChange -= Match_Config_instance.SetMatch_Index;
            UI_EventHandler.MatchLengthChange -= Match_Config_instance.SetLength_Index;
            UI_EventHandler.GoalChange -= Match_Config_instance.SetGoal;
            UI_EventHandler.BotDifficultyChanged -= Match_Config_instance.SetAI_Index;
            UI_EventHandler.BotNumChange -= Match_Config_instance.SetBots_Index;
        }
    }

    public EnemyStats GetEnemyStats()
    {
        return EnemyStats[Match_Config_instance.GetAI_Index()];
    }

    private void Save()
    {
        Debug.Log($"Match Index: {Match_Config_instance.GetMatch_Index()}, Length Index: {Match_Config_instance.GetLength_Index()}, Goal Index: {Match_Config_instance.GetGoal()}, AI Index: {Match_Config_instance.GetAI_Index()}, Bots Index: {Match_Config_instance.GetBots_Index()}");
        Custom_EditableConfig.ThisMatch = (GameMode_Config.MatchType)Match_Config_instance.GetMatch_Index();
        Custom_EditableConfig.Difficulty = EnemyStats[Match_Config_instance.GetAI_Index()];
        Custom_EditableConfig.MatchLength = Match_Config_instance.GetLength_Index();
        Custom_EditableConfig.bots = Match_Config_instance.GetBots_Index();
        Custom_EditableConfig.GameGoal = Match_Config_instance.GetGoal();
    }
}

