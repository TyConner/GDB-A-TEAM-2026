using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] int maxScoreboardEntries = 5;
    [SerializeField] Transform highscoresHolderTransform = null;
    [SerializeField] GameObject scoreboardEntryObject = null;

    [Header("Test")]
    [SerializeField] string testEntryName = "New Name";
    [SerializeField] int testEntryScore = 0;

    public List<ScoreboardEntryData> savedScores = new List<ScoreboardEntryData>();

    void Start()
    {
        CenterHolderIfUI();
        UpdateUI();
    }

    [ContextMenu("Add Test Entry")]
    public void AddTestEntry()
    {
        AddEntry(new ScoreboardEntryData()
        {
            entryName = testEntryName,
            entryScore = testEntryScore
        });
    }

    public void AddEntry(ScoreboardEntryData scoreboardEntryData)
    {
        savedScores.Add(scoreboardEntryData);

        SortAndClampScores();
        UpdateUI();
    }

    void SortAndClampScores()
    {
        savedScores.Sort((a, b) => b.entryScore.CompareTo(a.entryScore));

        if (savedScores.Count > maxScoreboardEntries)
        {
            savedScores.RemoveRange(maxScoreboardEntries, savedScores.Count - maxScoreboardEntries);
        }
    }

    public void UpdateUI()
    {
        if (highscoresHolderTransform == null || scoreboardEntryObject == null)
            return;

        for (int i = highscoresHolderTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(highscoresHolderTransform.GetChild(i).gameObject);
        }

        SortAndClampScores();

        for (int i = 0; i < savedScores.Count; i++)
        {
            ScoreboardEntryData highscore = savedScores[i];

            GameObject newEntry = Instantiate(scoreboardEntryObject, highscoresHolderTransform);

            ScoreboardEntryUI ui = newEntry.GetComponent<ScoreboardEntryUI>();
            if (ui != null)
            {
                ui.Initialise(highscore);
            }
        }
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