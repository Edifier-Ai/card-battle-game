// Assets/Scripts/AISystem/AIController.cs
using UnityEngine;

public enum AIDifficulty { Easy, Normal, Hard, Nightmare }

public class AIController : MonoBehaviour
{
    [SerializeField] private AIDifficulty difficulty = AIDifficulty.Easy;
    [SerializeField] private float decisionInterval = 1f;

    private Hand hand;
    private ElixirManager elixirManager;
    private UnitSpawner unitSpawner;
    private Lane[] lanes;
    private IAIStrategy strategy;
    private float decisionTimer;

    public void Initialize(Deck deck, ElixirManager elixir, Lane[] lanes, UnitSpawner spawner)
    {
        var config = ServiceLocator.Get<BattleConfig>();
        hand = new Hand(deck, config.maxHandSize);
        hand.FillHand();

        elixirManager = elixir;
        this.lanes = lanes;
        unitSpawner = spawner;

        SetDifficulty(difficulty);
        decisionTimer = decisionInterval;
    }

    public void SetDifficulty(AIDifficulty newDifficulty)
    {
        difficulty = newDifficulty;
        strategy = difficulty switch
        {
            AIDifficulty.Easy => new EasyAIStrategy(),
            AIDifficulty.Normal => new NormalAIStrategy(),
            AIDifficulty.Hard => new HardAIStrategy(),
            AIDifficulty.Nightmare => new HardAIStrategy(), // Use Hard for Nightmare
            _ => new EasyAIStrategy()
        };

        // Adjust decision speed
        decisionInterval = difficulty switch
        {
            AIDifficulty.Easy => 2f,
            AIDifficulty.Normal => 1f,
            AIDifficulty.Hard => 0.5f,
            AIDifficulty.Nightmare => 0.25f,
            _ => 1f
        };
    }

    private void Update()
    {
        decisionTimer -= Time.deltaTime;

        if (decisionTimer <= 0)
        {
            MakeDecision();
            decisionTimer = decisionInterval;
        }
    }

    private void MakeDecision()
    {
        if (hand == null || elixirManager == null || lanes == null) return;

        var (cardIndex, lane) = strategy.SelectAction(hand, elixirManager.CurrentElixir, lanes);

        if (cardIndex >= 0 && lane >= 0)
        {
            PlayCard(cardIndex, lane);
        }
    }

    private void PlayCard(int handIndex, int laneIndex)
    {
        if (!hand.CanPlayCard(handIndex, elixirManager.CurrentElixir))
            return;

        var card = hand.PlayCard(handIndex);
        elixirManager.TrySpendElixir(card.Data.elixirCost);

        unitSpawner.SpawnUnit(card, laneIndex, false);
        hand.DrawCard();

        EventBus.Publish(new CardPlayedEvent
        {
            Card = card.Data,
            Lane = laneIndex,
            IsPlayer = false
        });
    }
}