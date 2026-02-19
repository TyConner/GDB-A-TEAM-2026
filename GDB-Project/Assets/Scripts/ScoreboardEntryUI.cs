using UnityEngine;
using TMPro;
public class ScoreboardEntryUI : MonoBehaviour
{
        [SerializeField] private TextMeshProUGUI entryNameText = null;
        [SerializeField] private TextMeshProUGUI entryScoreText = null;

        public void Initialise(ScoreboardEntryData scoreboardEntryData)
        {
            entryNameText.text = scoreboardEntryData.entryName;
            entryScoreText.text = scoreboardEntryData.playerState.PS_Score.Scores[MyScore.Category.Kills].ToString();
            
        }
}

