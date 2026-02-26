using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MyScore;
public class ScoreboardEntryUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI entryNameText = null;
    [SerializeField] private TextMeshProUGUI Coins = null;
    [SerializeField] private TextMeshProUGUI Kills = null;
    [SerializeField] private TextMeshProUGUI Assists = null;
    [SerializeField] private TextMeshProUGUI HeadShots = null;
    [SerializeField] private TextMeshProUGUI Deaths = null;
    [SerializeField] private Image BackgroundColor = null;

    public Color TeamColor;
    public MyScore.Team Team;
    public bool bTitle = false;
    public void Initialise(MyScore data = null)
    {
        Debug.Log("Score init");
        if (data != null)
        {
            entryNameText.text = data.PlayerName;
            Coins.text = data.Scores[MyScore.Category.Coins].ToString();
            Kills.text = data.Scores[MyScore.Category.Kills].ToString();
            Assists.text = data.Scores[MyScore.Category.Assists].ToString();
            HeadShots.text = data.Scores[MyScore.Category.Headshots].ToString();
            Deaths.text = data.Scores[MyScore.Category.Deaths].ToString();
            TeamColor = GameMode.instance.GetTeamMat(data.Assigned_Team).color;
            Team = data.Assigned_Team; 
            BackgroundColor.color = new Color(TeamColor.r,TeamColor.g,TeamColor.b,BackgroundColor.color.a);
        }
        else
        {
            bTitle = true;
            entryNameText.text = "Player Name";
            Coins.text = "Coins";
            Kills.text = "Kills";
            Assists.text = "Assists";
            HeadShots.text = "Headshot";
            Deaths.text = "Deaths";

        }
    }

    public void Refresh(MyScore data)
    {
        
            entryNameText.text = data.PlayerName;
            Coins.text = data.Scores[MyScore.Category.Coins].ToString();
            Kills.text = data.Scores[MyScore.Category.Kills].ToString();
            Assists.text = data.Scores[MyScore.Category.Assists].ToString();
            HeadShots.text = data.Scores[MyScore.Category.Headshots].ToString();
            Deaths.text = data.Scores[MyScore.Category.Deaths].ToString();
            BackgroundColor.color = new Color(TeamColor.r, TeamColor.g, TeamColor.b, BackgroundColor.color.a);


    }


}

