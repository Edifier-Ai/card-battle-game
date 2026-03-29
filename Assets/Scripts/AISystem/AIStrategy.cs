// Assets/Scripts/AISystem/AIStrategy.cs
using System.Collections.Generic;

public interface IAIStrategy
{
    (int cardIndex, int lane) SelectAction(Hand hand, int elixir, Lane[] lanes);
}

public class EasyAIStrategy : IAIStrategy
{
    private System.Random random = new();

    public (int cardIndex, int lane) SelectAction(Hand hand, int elixir, Lane[] lanes)
    {
        // Get playable cards
        var playableCards = new List<int>();
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand.Cards[i].Data.elixirCost <= elixir)
                playableCards.Add(i);
        }

        if (playableCards.Count == 0)
            return (-1, -1);

        // Random card selection
        int cardIndex = playableCards[random.Next(playableCards.Count)];

        // Random lane selection
        int lane = random.Next(lanes.Length);

        return (cardIndex, lane);
    }
}

public class NormalAIStrategy : IAIStrategy
{
    private System.Random random = new();

    public (int cardIndex, int lane) SelectAction(Hand hand, int elixir, Lane[] lanes)
    {
        var playableCards = new List<int>();
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand.Cards[i].Data.elixirCost <= elixir)
                playableCards.Add(i);
        }

        if (playableCards.Count == 0)
            return (-1, -1);

        // Prefer cheaper cards early, save for combos
        int cardIndex = SelectBestCard(hand, playableCards, elixir);
        int lane = SelectBestLane(lanes);

        return (cardIndex, lane);
    }

    private int SelectBestCard(Hand hand, List<int> playable, int elixir)
    {
        // Prefer cards that leave some elixir for response
        foreach (int i in playable)
        {
            if (hand.Cards[i].Data.elixirCost <= elixir - 2)
                return i;
        }
        return playable[0];
    }

    private int SelectBestLane(Lane[] lanes)
    {
        // Prefer lanes with fewer enemy units
        int bestLane = 0;
        int minEnemies = int.MaxValue;

        for (int i = 0; i < lanes.Length; i++)
        {
            int enemyCount = lanes[i].GetEnemyCount(false);
            if (enemyCount < minEnemies)
            {
                minEnemies = enemyCount;
                bestLane = i;
            }
        }

        return bestLane;
    }
}

public class HardAIStrategy : IAIStrategy
{
    private System.Random random = new();

    public (int cardIndex, int lane) SelectAction(Hand hand, int elixir, Lane[] lanes)
    {
        var playableCards = new List<int>();
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand.Cards[i].Data.elixirCost <= elixir)
                playableCards.Add(i);
        }

        if (playableCards.Count == 0)
            return (-1, -1);

        // Consider unit types and lane pressure
        int cardIndex = SelectOptimalCard(hand, playableCards, lanes);
        int lane = SelectOptimalLane(hand, playableCards, lanes);

        return (cardIndex, lane);
    }

    private int SelectOptimalCard(Hand hand, List<int> playable, Lane[] lanes)
    {
        // Find the most needed card type based on game state
        int bestIndex = -1;
        float bestScore = -1;

        foreach (int i in playable)
        {
            float score = EvaluateCard(hand.Cards[i], lanes);
            if (score > bestScore)
            {
                bestScore = score;
                bestIndex = i;
            }
        }

        return bestIndex >= 0 ? bestIndex : playable[0];
    }

    private float EvaluateCard(Card card, Lane[] lanes)
    {
        float score = 0f;

        // Higher value for units that counter threats
        if (card.Data.cardType == CardType.Archer)
        {
            // Check if there are many enemy units
            int totalEnemies = 0;
            foreach (var lane in lanes)
                totalEnemies += lane.GetEnemyCount(false);
            score += totalEnemies * 0.5f;
        }
        else if (card.Data.cardType == CardType.Warrior)
        {
            // Good for defense
            score += 1f;
        }
        else if (card.Data.cardType == CardType.Cavalry)
        {
            // Good for offense when low enemy count
            int totalEnemies = 0;
            foreach (var lane in lanes)
                totalEnemies += lane.GetEnemyCount(false);
            score += (3 - totalEnemies) * 0.5f;
        }

        // Prefer efficient cost
        score += (10f - card.Data.elixirCost) * 0.2f;

        return score;
    }

    private int SelectOptimalLane(Hand hand, List<int> playable, Lane[] lanes)
    {
        // Balance offense and defense
        int bestLane = 1; // Default to middle

        for (int i = 0; i < lanes.Length; i++)
        {
            int enemyCount = lanes[i].GetEnemyCount(false);
            int playerCount = lanes[i].GetEnemyCount(true);

            // Need defense if enemies outnumber
            if (enemyCount > playerCount + 1)
                return i;
        }

        // Attack weakest lane
        int minDefense = int.MaxValue;
        for (int i = 0; i < lanes.Length; i++)
        {
            int playerCount = lanes[i].GetEnemyCount(true);
            if (playerCount < minDefense)
            {
                minDefense = playerCount;
                bestLane = i;
            }
        }

        return bestLane;
    }
}