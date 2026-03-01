using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.STP;

public class MatchConfig_Scene_Manager : MonoBehaviour
{
    
    [SerializeField]public List<GameMode_Config> MatchConfigs;
    [SerializeField]public List<EnemyStats> EnemyStats;

    [SerializeField] GameMode_Config Custom_Template;

    [SerializeField] MatchConfigScreenButtons UI_EventHandler;

    [SerializeField] MatchConfig_Scene_UI_Manager UI_Manager;

    public static GameMode_Config Custom_EditableConfig;
    public Config_Values MyMatchConfig = new();
    void Awake()
    {
        Custom_EditableConfig = ScriptableObject.CreateInstance<GameMode_Config>();
        if (Custom_Template != null)
        {
            Custom_EditableConfig.CopyThis(Custom_Template);
        }
       
}
    private void Start()
    {
        MyMatchConfig.LoadFromConfig(MatchConfigs[0], EnemyStats);
        UI_Manager.updateUI(MyMatchConfig);
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
        public void LoadFromConfig(GameMode_Config config, List<EnemyStats> stats)
        {
            SetMatch_Index((int)config.ThisMatch);
            SetAI_Index(stats.FindIndex(0, stats.Count, x => x == config.Difficulty));
            SetBots_Index(config.bots);
            SetLength_Index(config.MatchLength);
            SetGoal(config.GameGoal);
        }

        public void SaveToConfig(GameMode_Config config, List<EnemyStats> stats)
        {
            config.ThisMatch = (GameMode_Config.MatchType)GetMatch_Index();
            config.Difficulty = stats[GetAI_Index()];
            config.MatchLength = GetLength_Index();
            config.bots = GetBots_Index();
            config.GameGoal = GetGoal();
         
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

   

    void MatchTypeChange(int val)
    {
        switch (val)
        {
            case 0:
            case 1:
                //default
                MyMatchConfig.LoadFromConfig(MatchConfigs[val], EnemyStats);
                Un_Subscribe();
                UI_Manager.updateUI(MyMatchConfig);
                
                break;
        
            case 2:
                //custom
                MyMatchConfig.LoadFromConfig(Custom_EditableConfig, EnemyStats);
                Subscribe();
                UI_Manager.updateUI(MyMatchConfig);
                break;

        }
    }
    private void OnEnable()
    {
        UI_EventHandler.MatchTypeChange += MatchTypeChange;
        Subscribe();
    }
    private void OnDisable()
    {
        UI_EventHandler.MatchTypeChange -= MatchTypeChange;
        UI_EventHandler.EventStartGame -= StartGame;
        UI_EventHandler.EventClose -= close;
        Un_Subscribe();
    }
    private void Subscribe()
    {
        //bind event listeners
        if (UI_EventHandler)
        {
            
            UI_EventHandler.CustomMatchTypeChange += MyMatchConfig.SetMatch_Index;
            UI_EventHandler.MatchLengthChange += MyMatchConfig.SetLength_Index;
            UI_EventHandler.GoalChange += MyMatchConfig.SetGoal;
            UI_EventHandler.BotDifficultyChanged += MyMatchConfig.SetAI_Index;
            UI_EventHandler.BotNumChange += MyMatchConfig.SetBots_Index;
            UI_EventHandler.EventSave += Save;
            UI_EventHandler.EventStartGame += StartGame;
            UI_EventHandler.EventClose += close;

        }
    }
    private void Un_Subscribe()
    {
        if (UI_EventHandler)
        {
            
            UI_EventHandler.CustomMatchTypeChange -= MyMatchConfig.SetMatch_Index;
            UI_EventHandler.MatchLengthChange -= MyMatchConfig.SetLength_Index;
            UI_EventHandler.GoalChange -= MyMatchConfig.SetGoal;
            UI_EventHandler.BotDifficultyChanged -= MyMatchConfig.SetAI_Index;
            UI_EventHandler.BotNumChange -= MyMatchConfig.SetBots_Index;
            UI_EventHandler.EventSave -= Save;
         
        }
    }

    public EnemyStats GetEnemyStats()
    {
        return EnemyStats[MyMatchConfig.GetAI_Index()];
    }

    private void Save()
    {

        MyMatchConfig.SaveToConfig(Custom_EditableConfig, EnemyStats);
        Debug.Log($"Match Index: {MyMatchConfig.GetMatch_Index()}, Length Index: {MyMatchConfig.GetLength_Index()}, Goal Index: {MyMatchConfig.GetGoal()}, AI Index: {MyMatchConfig.GetAI_Index()}, Bots Index: {MyMatchConfig.GetBots_Index()}");
    }

    private void close()
    {
        if(Match_Config_Instance.instance != null)
        {
            Destroy(Match_Config_Instance.instance);
        }
        SceneManager.LoadSceneAsync(0);
        
    }

    private void StartGame()
    {
        MyMatchConfig.SaveToConfig(Custom_EditableConfig, EnemyStats);
        if(Match_Config_Instance.instance != null)
        {
            Match_Config_Instance.instance.config = Custom_EditableConfig;

        }
        else
        {
            GameObject MatchConfig_Object = new GameObject("Persistant_Config");
            MatchConfig_Object.AddComponent<Match_Config_Instance>();
            Match_Config_Instance.instance.config = Custom_EditableConfig;
        }
        SceneManager.LoadSceneAsync(2);

    }
}

