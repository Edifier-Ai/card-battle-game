// Assets/Scripts/BattleSystem/UnitSpawner.cs
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] playerSpawnPoints; // Per lane
    [SerializeField] private Transform[] enemySpawnPoints;

    public Unit SpawnUnit(Card card, int lane, bool isPlayerUnit)
    {
        var prefab = card.Data.unitPrefab;
        if (prefab == null)
        {
            Debug.LogError($"No prefab for card {card.Data.cardName}");
            return null;
        }

        Transform spawnPoint = isPlayerUnit ? playerSpawnPoints[lane] : enemySpawnPoints[lane];
        var go = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        var unit = go.GetComponent<Unit>();
        if (unit == null)
        {
            // Add appropriate unit component based on card type
            unit = AddUnitComponent(go, card.Data.cardType);
        }

        unit.Initialize(card, isPlayerUnit, lane);

        EventBus.Publish(new UnitSpawnedEvent
        {
            Unit = unit,
            Lane = lane,
            IsPlayerUnit = isPlayerUnit
        });

        return unit;
    }

    private Unit AddUnitComponent(GameObject go, CardType cardType)
    {
        return cardType switch
        {
            CardType.Warrior => go.AddComponent<WarriorUnit>(),
            CardType.Archer => go.AddComponent<ArcherUnit>(),
            CardType.Cavalry => go.AddComponent<CavalryUnit>(),
            _ => go.AddComponent<Unit>()
        };
    }
}