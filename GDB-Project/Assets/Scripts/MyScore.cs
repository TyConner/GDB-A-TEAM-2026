using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MyScore
{
    public MyScore()
    {
        PlayerName = "Unassigned";
        Assigned_Team = Team.None;
        Scores = new Dictionary<Category, int> {
            { Category.Coins, 0 }, { Category.Kills, 0 }, { Category.Assists, 0 }, { Category.Deaths, 0 }, { Category.Headshots, 0 }
        };
    }
    public string PlayerName;
    public enum Team { A , B, FFA, Environment, None}

    public Team Assigned_Team;

    public enum Category { Coins, Kills, Assists, Deaths, Headshots }

    public Dictionary<Category, int> Scores;

    private void Start()
    {
    }
    public void ChangeScore(Category Stat, int amount)
    {
       Scores[Stat] += amount;
        switch(Stat)
        {
            case Category.Kills:
                ChangeScore(Category.Coins, GameMode.instance.config.Kill_Coin_Reward);
                break;
            case Category.Assists:
                ChangeScore(Category.Coins, GameMode.instance.config.Assist_Coin_Reward);
                break;
            case Category.Headshots:
                ChangeScore(Category.Coins, GameMode.instance.config.Headshot_Coin_Reward);
                break;
        }
    }

    public int GetScore(Category Stat)
    {
        return Scores[Stat];
    }

    public void ResetScore()
    {
       
    }
}
