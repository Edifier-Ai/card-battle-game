// Assets/Scripts/BattleSystem/ElixirManager.cs
using UnityEngine;

public class ElixirManager : MonoBehaviour
{
    [SerializeField] private float regenRate = 0.5f;
    [SerializeField] private int maxElixir = 10;

    public int CurrentElixir { get; private set; }
    public float ElixirProgress { get; private set; }

    public event System.Action<int> OnElixirChanged;

    private void Start()
    {
        CurrentElixir = maxElixir / 2; // Start with half elixir
        ElixirProgress = 0f;
    }

    private void Update()
    {
        if (CurrentElixir >= maxElixir) return;

        ElixirProgress += regenRate * Time.deltaTime;

        if (ElixirProgress >= 1f)
        {
            int gained = Mathf.FloorToInt(ElixirProgress);
            ElixirProgress -= gained;
            CurrentElixir = Mathf.Min(maxElixir, CurrentElixir + gained);
            OnElixirChanged?.Invoke(CurrentElixir);
        }
    }

    public bool TrySpendElixir(int amount)
    {
        if (CurrentElixir >= amount)
        {
            CurrentElixir -= amount;
            OnElixirChanged?.Invoke(CurrentElixir);
            return true;
        }
        return false;
    }

    public void AddElixir(int amount)
    {
        CurrentElixir = Mathf.Min(maxElixir, CurrentElixir + amount);
        OnElixirChanged?.Invoke(CurrentElixir);
    }

    public void Reset()
    {
        CurrentElixir = maxElixir / 2;
        ElixirProgress = 0f;
        OnElixirChanged?.Invoke(CurrentElixir);
    }
}