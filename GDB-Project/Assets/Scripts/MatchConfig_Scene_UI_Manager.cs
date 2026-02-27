using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MatchConfig_Scene_UI_Manager : MonoBehaviour
{
    [SerializeField] MatchConfigScreenButtons EventHandler;
    [SerializeField] List<GameObject> Sliders = new();
    [SerializeField] List<GameObject> DropDowns = new();
    [SerializeField] List<GameObject> Buttons = new();


    private void OnEnable()
    {
        if (EventHandler)
        {
            EventHandler.MatchTypeChange += OnCustom;
        }
    }
    private void OnDisable()
    {
        if (EventHandler)
        {
            EventHandler.MatchTypeChange -= OnCustom;
        }
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

}
