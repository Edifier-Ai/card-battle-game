// Assets/Scripts/Units/WarriorUnit.cs
using UnityEngine;

public class WarriorUnit : Unit
{
    [Header("Warrior Specific")]
    [SerializeField] private int armor = 10; // Damage reduction

    public override void TakeDamage(int amount)
    {
        int reducedDamage = Mathf.Max(1, amount - armor);
        base.TakeDamage(reducedDamage);
    }

    protected override void FindTarget()
    {
        // Warriors target nearest enemy unit or building
        Collider[] hits = Physics.OverlapSphere(transform.position, 10f);
        float closestDist = float.MaxValue;
        Unit closestUnit = null;
        Tower closestTower = null;

        foreach (var hit in hits)
        {
            var unit = hit.GetComponent<Unit>();
            var tower = hit.GetComponent<Tower>();

            if (unit != null && unit.IsPlayerUnit != IsPlayerUnit && unit.State != UnitState.Dead)
            {
                float dist = Vector3.Distance(transform.position, unit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestUnit = unit;
                }
            }
            else if (tower != null && tower.IsPlayerTower != IsPlayerUnit)
            {
                float dist = Vector3.Distance(transform.position, tower.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestTower = tower;
                }
            }
        }

        target = closestUnit ?? (closestTower as Unit);
    }
}