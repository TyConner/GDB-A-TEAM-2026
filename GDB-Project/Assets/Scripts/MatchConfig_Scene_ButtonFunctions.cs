using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MatchConfigScreenButtons : MonoBehaviour
{
    public event System.Action<int> MatchLengthChange;
    public event System.Action<int> GoalChange;
    public event System.Action<int> BotNumChange;
    public event System.Action<int> MatchTypeChange;
    public event System.Action<int> CustomMatchTypeChange;
    public event System.Action<int> BotDifficultyChanged;

    public event System.Action EventSave;
    public event System.Action EventClose;
    public event System.Action EventStartGame;

    public void OnSave()
    {
        EventSave?.Invoke();
    }

    public void Close()
    {
        EventClose?.Invoke();
    }

    public void StartGame()
    {
        EventStartGame?.Invoke();
    }

    public void OnMatchLengthChange(float val)
    {
        MatchLengthChange?.Invoke((int)val);
    }

    public void OnGameGoalChange(float val)
    {
        GoalChange?.Invoke((int)val);
    }

    public void OnBotNumberChange(float val)
    {
        BotNumChange?.Invoke((int)val);
    }
    public void OnMatchTypeChanged(int val)
    {
        MatchTypeChange?.Invoke(val);
    }
    public void OnCustomMatchTypeChanged(int val)
    {
        CustomMatchTypeChange?.Invoke(val);
    }

    public void OnBotDifficultyChanged(int val)
    {
       BotDifficultyChanged?.Invoke(val);
    }
}

