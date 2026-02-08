using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MyScore : MonoBehaviour
{
   
    public enum Team { A , B, FFA}

    public Team Assigned_Team;

    public enum Category { Coins, Kills, Assists, Deaths, Headshots }
    
    public Dictionary<Category, int> Scores = new Dictionary<Category, int> {
        {Category.Coins, 0}, {Category.Kills, 0 }, {Category.Assists, 0}, {Category.Deaths, 0 }, {Category.Headshots, 0}

    };

    public void ChangeScore(Category Stat, int amount)
    {
       Scores[Stat] += amount;
    }

    public int GetScore(Category Stat)
    {
        return Scores[Stat];
    }

    public void ResetScore()
    {
       
    }
}
