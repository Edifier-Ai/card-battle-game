// Assets/Scripts/Core/EventBus.cs
using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> events = new();

    public static void Subscribe<T>(Action<T> callback) where T : struct
    {
        var type = typeof(T);
        if (!events.ContainsKey(type))
            events[type] = new List<Delegate>();
        events[type].Add(callback);
    }

    public static void Unsubscribe<T>(Action<T> callback) where T : struct
    {
        var type = typeof(T);
        if (events.TryGetValue(type, out var delegates))
            delegates.Remove(callback);
    }

    public static void Publish<T>(T eventData) where T : struct
    {
        var type = typeof(T);
        if (events.TryGetValue(type, out var delegates))
        {
            foreach (var callback in delegates.ToArray())
                ((Action<T>)callback)?.Invoke(eventData);
        }
    }
}

// Event definitions
public struct BattleStartEvent { }
public struct BattleEndEvent { public bool PlayerWon; }
public struct UnitSpawnedEvent { public Unit Unit; public int Lane; public bool IsPlayerUnit; }
public struct UnitDeathEvent { public Unit Unit; }
public struct TowerDestroyedEvent { public int Lane; public bool IsPlayerTower; public bool IsMainCastle; }
public struct CardPlayedEvent { public CardData Card; public int Lane; public bool IsPlayer; }