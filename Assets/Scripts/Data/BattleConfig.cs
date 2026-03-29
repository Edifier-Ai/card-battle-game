// Assets/Scripts/Data/BattleConfig.cs
using UnityEngine;

[CreateAssetMenu(fileName = "BattleConfig", menuName = "Game/BattleConfig")]
public class BattleConfig : ScriptableObject
{
    [Header("Battle Settings")]
    public float battleDuration = 180f; // 3 minutes
    public int maxElixir = 10;
    public float elixirRegenRate = 0.5f; // per second

    [Header("Lane Settings")]
    public int laneCount = 3;
    public float laneWidth = 3f;

    [Header("Tower Settings")]
    public int sideTowerHP = 1500;
    public int mainCastleHP = 3000;
    public int towerDamage = 50;
    public float towerAttackSpeed = 1f;
    public float towerRange = 5f;

    [Header("Starting Hand")]
    public int startingHandSize = 4;
    public int maxHandSize = 8;
}