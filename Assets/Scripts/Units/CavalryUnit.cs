// Assets/Scripts/Units/CavalryUnit.cs
using UnityEngine;

public class CavalryUnit : Unit
{
    [Header("Cavalry Specific")]
    [SerializeField] private float chargeSpeed = 3f;
    [SerializeField] private float chargeMultiplier = 2f;
    [SerializeField] private float chargeDistance = 5f;

    private bool isCharging = false;
    private bool hasCharged = false;
    private Vector3 startPosition;

    public override void Initialize(Card card, bool isPlayerUnit, int lane)
    {
        base.Initialize(card, isPlayerUnit, lane);
        startPosition = transform.position;
    }

    protected override void Move()
    {
        if (!hasCharged && Vector3.Distance(transform.position, startPosition) > chargeDistance)
        {
            isCharging = true;
            float direction = IsPlayerUnit ? 1f : -1f;
            transform.Translate(Vector3.forward * direction * chargeSpeed * Time.deltaTime);
        }
        else
        {
            isCharging = false;
            hasCharged = true;
            base.Move();
        }
    }

    protected override void DealDamage(Unit targetUnit)
    {
        int finalDamage = damage;
        if (isCharging)
        {
            finalDamage = Mathf.RoundToInt(damage * chargeMultiplier);
            isCharging = false;
            hasCharged = true;
        }
        targetUnit.TakeDamage(finalDamage);
    }

    protected override void FindTarget()
    {
        // Cavalry prioritizes archers and buildings
        Collider[] hits = Physics.OverlapSphere(transform.position, 15f);
        float bestPriority = -1;
        Unit bestTarget = null;

        foreach (var hit in hits)
        {
            var unit = hit.GetComponent<Unit>();
            var tower = hit.GetComponent<Tower>();

            if (unit != null && unit.IsPlayerUnit != IsPlayerUnit && unit.State != UnitState.Dead)
            {
                float priority = 0;
                if (unit is ArcherUnit) priority = 2;
                else if (unit is WarriorUnit) priority = 0.5f; // Avoid warriors
                else priority = 1;

                if (priority > bestPriority)
                {
                    bestPriority = priority;
                    bestTarget = unit;
                }
            }
            else if (tower != null && tower.IsPlayerTower != IsPlayerUnit && bestPriority < 1.5f)
            {
                bestPriority = 1.5f;
                bestTarget = tower;
            }
        }

        target = bestTarget;
    }
}