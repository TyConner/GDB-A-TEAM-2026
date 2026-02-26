using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    //[SerializeField] int maxScoreboardEntries = 12;
    [SerializeField] Transform highscoresHolderTransform = null;
    [SerializeField] GameObject scoreboardEntryObject = null;


    public List<ScoreboardEntryUI> entries = new();

    private ScoreboardEntryUI title;

    void Awake()
    {
        CenterHolderIfUI();
        AddTestEntry();
        //UpdateUI();
    }

    [ContextMenu("Add Test Entry")]
    public void AddTestEntry()
    {
        GameObject newEntry = Instantiate(scoreboardEntryObject, highscoresHolderTransform);
        ScoreboardEntryUI entry = newEntry.GetComponent<ScoreboardEntryUI>();
        entry.Initialise(null);
        title = entry;
        entries.Add(entry);
    }

    public ScoreboardEntryUI AddEntry(MyScore scoreboardEntryData)
    {
        GameObject newEntry = Instantiate(scoreboardEntryObject, highscoresHolderTransform);
        ScoreboardEntryUI entry = newEntry.GetComponent<ScoreboardEntryUI>();
        entry.Initialise(scoreboardEntryData);
        entries.Add(entry);
        //UpdateUI();
        return entry;
    }

    //void SortAndClampScores()
    //{
    //   entries.Sort()
    //    switch (GameMode.instance.config.ThisMatch)
    //    {
    //        //MoveEntry()
    //        case GameMode_Config.MatchType.TDM:
    //            break;
    //        case GameMode_Config.MatchType.FFA:
    //            break;

    //    }
    //}

    void MoveEntry(ScoreboardEntryUI entry, int index)
    {
        entry.transform.position = new Vector3(entry.transform.position.x, -40 -(index * 45), entry.transform.position.y);
    }
    public void UpdateUI()
    {


    }

    void CenterHolderIfUI()
    {
        RectTransform rt = highscoresHolderTransform as RectTransform;
        if (rt == null)
            return;

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
    }
}