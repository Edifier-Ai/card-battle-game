// Assets/Scripts/BattleSystem/Unit.cs
using UnityEngine;
using System;

public enum UnitState { Idle, Moving, Attacking, Dead }

public class Unit : MonoBehaviour
{
    [Header("Stats")]
    protected int currentHP;
    protected int maxHP;
    protected int damage;
    protected float attackSpeed;
    protected float moveSpeed;
    protected float attackRange;

    [Header("State")]
    public UnitState State { get; protected set; } = UnitState.Idle;
    public bool IsPlayerUnit { get; private set; }
    public int Lane { get; private set; }

    protected Unit target;
    protected float attackTimer;
    protected Animator animator;

    public event Action<Unit> OnDeath;

    public virtual void Initialize(Card card, bool isPlayerUnit, int lane)
    {
        maxHP = card.GetHP();
        currentHP = maxHP;
        damage = card.GetDamage();
        attackSpeed = card.Data.attackSpeed;
        moveSpeed = card.Data.moveSpeed;
        attackRange = card.Data.attackRange;
        IsPlayerUnit = isPlayerUnit;
        Lane = lane;

        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (State == UnitState.Dead) return;

        attackTimer -= Time.deltaTime;

        FindTarget();

        if (target != null && IsInRange())
        {
            Attack();
        }
        else
        {
            Move();
        }
    }

    protected virtual void FindTarget()
    {
        // Override in subclasses for specific targeting
    }

    protected bool IsInRange()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.transform.position) <= attackRange;
    }

    protected virtual void Move()
    {
        if (State == UnitState.Attacking && animator != null)
            animator.SetBool("Attacking", false);

        State = UnitState.Moving;

        // Move towards enemy side
        float direction = IsPlayerUnit ? 1f : -1f;
        transform.Translate(Vector3.forward * direction * moveSpeed * Time.deltaTime);
    }

    protected virtual void Attack()
    {
        State = UnitState.Attacking;

        if (animator != null)
            animator.SetBool("Attacking", true);

        if (attackTimer <= 0)
        {
            DealDamage(target);
            attackTimer = 1f / attackSpeed;
        }
    }

    protected virtual void DealDamage(Unit targetUnit)
    {
        targetUnit.TakeDamage(damage);
    }

    public virtual void TakeDamage(int amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        State = UnitState.Dead;
        OnDeath?.Invoke(this);
        EventBus.Publish(new UnitDeathEvent { Unit = this });
        Destroy(gameObject, 0.5f);
    }

    public int GetCurrentHP() => currentHP;
    public int GetMaxHP() => maxHP;
}