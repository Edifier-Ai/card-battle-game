// Assets/Scripts/BattleSystem/Lane.cs
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    [SerializeField] private int laneIndex;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private Tower playerSideTower;
    [SerializeField] private Tower enemySideTower;

    private List<Unit> playerUnits = new();
    private List<Unit> enemyUnits = new();

    public int LaneIndex => laneIndex;
    public Transform PlayerSpawnPoint => playerSpawnPoint;
    public Transform EnemySpawnPoint => enemySpawnPoint;

    public void Initialize(BattleConfig config)
    {
        // Initialize towers with config values
        playerSideTower.Initialize(true, laneIndex, false, config.sideTowerHP,
            config.towerDamage, config.towerRange, config.towerAttackSpeed);
        enemySideTower.Initialize(false, laneIndex, false, config.sideTowerHP,
            config.towerDamage, config.towerRange, config.towerAttackSpeed);
    }

    public void RegisterUnit(Unit unit)
    {
        if (unit.IsPlayerUnit)
            playerUnits.Add(unit);
        else
            enemyUnits.Add(unit);
    }

    public void UnregisterUnit(Unit unit)
    {
        playerUnits.Remove(unit);
        enemyUnits.Remove(unit);
    }

    public List<Unit> GetEnemyUnits(bool forPlayer)
    {
        return forPlayer ? enemyUnits : playerUnits;
    }

    public int GetEnemyCount(bool forPlayer)
    {
        return forPlayer ? enemyUnits.Count : playerUnits.Count;
    }
}