using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MyScore : MonoBehaviour
{

   public enum Category {Coins,  Kills, Assists, Deaths, Headshots}
   public List<int> Scores = new List<int> { 
        0, // Coins
        0, // Kills
        0, // Assists
        0, // Deaths
        0  //Headshots
    };

    public void ChangeScore(Category Stat, int amount)
    {
       Scores[(int)Stat] += amount;
    }
}
