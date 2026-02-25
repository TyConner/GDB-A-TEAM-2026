using NUnit.Framework;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MatchConfigScreen : MonoBehaviour
{
    [Header("       Menus        ")]
    [SerializeField] GameObject Panel;
    [SerializeField] Prefab InGameUIPrefab;
    [SerializeField]public List<GameMode_Config> MatchConfigs;
    [SerializeField]public List<EnemyStats> EnemyStats;
}
