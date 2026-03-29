// Assets/Scripts/Units/ArcherUnit.cs
using UnityEngine;

public class ArcherUnit : Unit
{
    [Header("Archer Specific")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    protected override void DealDamage(Unit targetUnit)
    {
        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            var projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            // Simple projectile logic would go here
        }
        base.DealDamage(targetUnit);
    }

    protected override void FindTarget()
    {
        // Archers prioritize air units if they can target them
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange * 2);
        float closestDist = float.MaxValue;
        Unit closestUnit = null;

        foreach (var hit in hits)
        {
            var unit = hit.GetComponent<Unit>();
            if (unit != null && unit.IsPlayerUnit != IsPlayerUnit && unit.State != UnitState.Dead)
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

        // If no unit, target tower
        if (target == null)
        {
            foreach (var hit in hits)
            {
                var tower = hit.GetComponent<Tower>();
                if (tower != null && tower.IsPlayerTower != IsPlayerUnit)
                {
                    target = tower;
                    break;
                }
            }
        }
    }

    protected override void Move()
    {
        // Archers stay at range
        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist <= attackRange)
                return; // Stay in place
        }
        base.Move();
    }
}