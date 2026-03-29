// Assets/Scripts/BattleSystem/BattleManager.cs
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Lane[] lanes;
    [SerializeField] private UnitSpawner unitSpawner;
    [SerializeField] private ElixirManager playerElixir;
    [SerializeField] private ElixirManager enemyElixir;
    [SerializeField] private AIController aiController;

    [Header("Settings")]
    [SerializeField] private float battleDuration = 180f;

    private float battleTimer;
    private bool battleActive = false;
    private Deck playerDeck;
    private Hand playerHand;
    private Deck enemyDeck;
    private Hand enemyHand;

    private int playerTowersDestroyed = 0;
    private int enemyTowersDestroyed = 0;
    private bool playerCastleDestroyed = false;
    private bool enemyCastleDestroyed = false;

    public event System.Action<BattleResult> OnBattleEnded;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void Start()
    {
        InitializeBattle();
    }

    public void InitializeBattle()
    {
        BattleConfig config = null;
        if (ServiceLocator.TryGet<BattleConfig>(out var c))
            config = c;

        if (config != null)
            battleDuration = config.battleDuration;

        // Initialize lanes
        foreach (var lane in lanes)
        {
            if (config != null)
                lane.Initialize(config);
        }

        // Create decks from available cards
        var allCards = new List<CardData>(Resources.LoadAll<CardData>("Cards"));
        playerDeck = CardFactory.CreateDeckFromCardData(allCards);
        enemyDeck = CardFactory.CreateDeckFromCardData(allCards);

        playerHand = new Hand(playerDeck, config?.maxHandSize ?? 8);
        enemyHand = new Hand(enemyDeck, config?.maxHandSize ?? 8);

        playerHand.FillHand();
        enemyHand.FillHand();

        // Initialize AI
        if (aiController != null)
            aiController.Initialize(enemyDeck, enemyElixir, lanes, unitSpawner);

        // Subscribe to events
        EventBus.Subscribe<TowerDestroyedEvent>(OnTowerDestroyed);
        EventBus.Subscribe<UnitDeathEvent>(OnUnitDeath);
        EventBus.Subscribe<UnitSpawnedEvent>(OnUnitSpawned);

        battleActive = true;
        battleTimer = battleDuration;

        EventBus.Publish(new BattleStartEvent());
    }

    private void Update()
    {
        if (!battleActive) return;

        battleTimer -= Time.deltaTime;

        if (battleTimer <= 0)
        {
            EndBattle();
        }
    }

    public bool PlayCard(int handIndex, int laneIndex)
    {
        if (handIndex < 0 || handIndex >= playerHand.Count) return false;
        if (!playerHand.CanPlayCard(handIndex, playerElixir.CurrentElixir)) return false;
        if (laneIndex < 0 || laneIndex >= lanes.Length) return false;

        var card = playerHand.PlayCard(handIndex);
        playerElixir.TrySpendElixir(card.Data.elixirCost);

        unitSpawner.SpawnUnit(card, laneIndex, true);
        playerHand.DrawCard();

        EventBus.Publish(new CardPlayedEvent
        {
            Card = card.Data,
            Lane = laneIndex,
            IsPlayer = true
        });

        return true;
    }

    public bool CanPlayCard(int handIndex)
    {
        return playerHand.CanPlayCard(handIndex, playerElixir.CurrentElixir);
    }

    public IReadOnlyList<Card> GetPlayerHand() => playerHand.Cards;

    private void OnTowerDestroyed(TowerDestroyedEvent e)
    {
        if (e.IsPlayerTower)
        {
            enemyTowersDestroyed++;
            if (e.IsMainCastle)
            {
                playerCastleDestroyed = true;
                EndBattle();
            }
        }
        else
        {
            playerTowersDestroyed++;
            if (e.IsMainCastle)
            {
                enemyCastleDestroyed = true;
                EndBattle();
            }
        }
    }

    private void OnUnitDeath(UnitDeathEvent e)
    {
        foreach (var lane in lanes)
        {
            lane.UnregisterUnit(e.Unit);
        }
    }

    private void OnUnitSpawned(UnitSpawnedEvent e)
    {
        lanes[e.Lane].RegisterUnit(e.Unit);
    }

    private void EndBattle()
    {
        battleActive = false;

        var result = DetermineResult();
        OnBattleEnded?.Invoke(result);
        EventBus.Publish(new BattleEndEvent { PlayerWon = result.PlayerWon });
    }

    private BattleResult DetermineResult()
    {
        // Instant win: destroy main castle
        if (enemyCastleDestroyed)
            return new BattleResult { Outcome = BattleOutcome.Victory, BattleTime = battleDuration - battleTimer, TowersDestroyed = playerTowersDestroyed };

        if (playerCastleDestroyed)
            return new BattleResult { Outcome = BattleOutcome.Defeat, BattleTime = battleDuration - battleTimer, TowersLost = enemyTowersDestroyed };

        // Time-based: compare towers destroyed
        if (playerTowersDestroyed > enemyTowersDestroyed)
            return new BattleResult { Outcome = BattleOutcome.Victory, TowersDestroyed = playerTowersDestroyed };

        if (enemyTowersDestroyed > playerTowersDestroyed)
            return new BattleResult { Outcome = BattleOutcome.Defeat, TowersLost = enemyTowersDestroyed };

        return new BattleResult { Outcome = BattleOutcome.Draw };
    }

    public float GetRemainingTime() => battleTimer;
    public bool IsBattleActive() => battleActive;
    public int GetCurrentElixir() => playerElixir?.CurrentElixir ?? 0;
}