using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static MatchConfig_Scene_Manager;

public class MatchConfig_Scene_UI_Manager : MonoBehaviour
{
    [SerializeField] MatchConfigScreenButtons EventHandler;
    [SerializeField] List<GameObject> Sliders = new();
    [SerializeField] List<GameObject> DropDowns = new();
    [SerializeField] List<GameObject> Buttons = new();
    [SerializeField] TMP_Text MatchLength;
    [SerializeField] TMP_Text BotNum;
    [SerializeField] TMP_Text MatchGoal;
    [SerializeField] UnityEngine.UI.Slider MatchLengthSlider;
    [SerializeField] UnityEngine.UI.Slider BotNumSlider;
    [SerializeField] UnityEngine.UI.Slider MatchGoalSlider;
    [SerializeField] TMP_Dropdown MatchType;
    [SerializeField] TMP_Dropdown BotDifficulty;
    [SerializeField] GameObject SavedPopUp;

    private void OnEnable()
    {
        if (EventHandler)
        {
            EventHandler.MatchTypeChange += OnCustom;
            EventHandler.MatchLengthChange += UpdateMatchLength;
            EventHandler.GoalChange += UpdateGameGoal;
            EventHandler.BotNumChange += updateBotCount;
            EventHandler.EventSave += onSave;
        }
    }

    private void OnDisable()
    {
        if (EventHandler)
        {
            EventHandler.MatchTypeChange -= OnCustom;
            EventHandler.MatchLengthChange -= UpdateMatchLength;
            EventHandler.GoalChange -= UpdateGameGoal;
            EventHandler.BotNumChange -= updateBotCount;
            EventHandler.EventSave -= onSave;
        }
    }
    void onSave()
    {
        StartCoroutine(PopUp_Message_Saved());
    }
    void UpdateMatchLength(int val)
    {
        if (MatchLength != null) {
            Debug.Log(val);
            MatchLength.text = val.ToString();
        }
    }

    void UpdateGameGoal(int val)
    {
        if (BotNum != null)
        {
            MatchGoal.text = val.ToString();
        }
    }
    void updateBotCount(int val)
    {
        if (BotNum != null)
        {
            BotNum.text = val.ToString();
        }
    }

    public void updateUI(Config_Values config)
    {
        MatchGoalSlider.value = config.GetGoal();
        UpdateGameGoal(config.GetGoal());

        BotNumSlider.value = config.GetBots_Index();
        updateBotCount(config.GetBots_Index());

        MatchLengthSlider.value = config.GetLength_Index();
        UpdateMatchLength(config.GetLength_Index());

     
        MatchType.value = config.GetMatch_Index();
        BotDifficulty.value = config.GetAI_Index();

    }
    
    private void OnCustom(int val)
    {
        if (val == 2)
        {
            foreach (GameObject obj in Sliders)
            {
                MakeInteractable(obj);
            }
            foreach (GameObject obj in DropDowns)
            {
                MakeInteractable(obj);
            }
            foreach(GameObject obj in Buttons)
            {
                MakeVisible(obj);
            }
        }
        else
        {
            foreach (GameObject obj in Sliders)
            {
                MakeUninteractable(obj);
            }
            foreach (GameObject obj in DropDowns)
            {
                MakeUninteractable(obj);
            }
            foreach (GameObject obj in Buttons)
            {
                MakeInvisible(obj);
            }
        }

    }

    private void MakeVisible(GameObject obj)
    {
        obj.SetActive(true);
    }
    
    private void MakeInvisible(GameObject obj)
    {
        obj.SetActive(false);
    }
    private void MakeUninteractable(GameObject obj)
    {
        UnityEngine.UI.Slider sldr = obj.GetComponent<UnityEngine.UI.Slider>();
        TMP_Dropdown down = obj.GetComponent<TMP_Dropdown>();
        if (sldr)
        {
            sldr.interactable = false;
        }
        if (down)
        {
            down.interactable = false;
        }
        
    }
    private void MakeInteractable(GameObject obj)
    {
        UnityEngine.UI.Slider sldr = obj.GetComponent<UnityEngine.UI.Slider>();
        TMP_Dropdown down = obj.GetComponent<TMP_Dropdown>();
        if (sldr)
        {
            sldr.interactable = true;
            //print(sldr.interactable);
        }
        if (down)
        {
            down.interactable = true;
            //print(down.interactable);
        }

    }

    IEnumerator PopUp_Message_Saved()
    {
        if(SavedPopUp.activeSelf == false)
        {
            SavedPopUp.SetActive(true);
            yield return new WaitForSeconds(1f);
            SavedPopUp.SetActive(false);
        }
    }

    
}
