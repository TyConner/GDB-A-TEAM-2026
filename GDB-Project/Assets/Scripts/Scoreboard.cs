using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] private int maxScoreboardEntries = 5;
    [SerializeField] private Transform highscoresHolderTransform = null;
    [SerializeField] private GameObject scoreboardEntryObject = null;

    [Header("Test")]
    [SerializeField] private string testEntryName = "New Name";
    [SerializeField] private int testEntryScore = 0;
    public List<ScoreboardEntryData> savedScores = new List<ScoreboardEntryData>();
    private void Start()
    {

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

        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (Transform child in highscoresHolderTransform)
        {
            Destroy(child.gameObject);
        }

        foreach (ScoreboardEntryData highscore in savedScores)
        {
            GameObject newEntry = Instantiate(scoreboardEntryObject, highscoresHolderTransform);

            newEntry.GetComponent<ScoreboardEntryUI>().Initialise(highscore);
        }
    }

}
