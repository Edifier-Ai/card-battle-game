// Assets/Scripts/BattleSystem/Tower.cs
using UnityEngine;

public class Tower : Unit
{
    [Header("Tower Settings")]
    [SerializeField] private bool isMainCastle = false;
    [SerializeField] private int laneIndex = 0;

    public bool IsPlayerTower { get; private set; }
    public bool IsMainCastle => isMainCastle;
    public int LaneIndex => laneIndex;

    public void Initialize(bool isPlayerTower, int lane, bool isMain, int hp, int dmg, float range, float speed)
    {
        IsPlayerTower = isPlayerTower;
        laneIndex = lane;
        isMainCastle = isMain;
        maxHP = hp;
        currentHP = hp;
        damage = dmg;
        attackRange = range;
        attackSpeed = speed;
    }

    protected override void Move()
    {
        // Towers don't move
    }

    protected override void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        float closestDist = float.MaxValue;
        Unit closestUnit = null;

        foreach (var hit in hits)
        {
            var unit = hit.GetComponent<Unit>();
            if (unit != null && unit.IsPlayerUnit != IsPlayerTower && unit.State != UnitState.Dead)
            {
                float dist = Vector3.Distance(transform.position, unit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestUnit = unit;
                }
            }
        }

        target = closestUnit;
    }

    protected override void Die()
    {
        State = UnitState.Dead;
        EventBus.Publish(new TowerDestroyedEvent
        {
            Lane = laneIndex,
            IsPlayerTower = IsPlayerTower,
            IsMainCastle = isMainCastle
        });
        Destroy(gameObject, 0.5f);
    }
}