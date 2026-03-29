// Assets/Scripts/CardSystem/Card.cs
using UnityEngine;

public class Card
{
    public CardData Data { get; private set; }
    public int Level { get; private set; }

    public Card(CardData data, int level = 1)
    {
        Data = data;
        Level = level;
    }

    public int GetHP()
    {
        // +10% per level
        return Mathf.RoundToInt(Data.hp * (1 + (Level - 1) * 0.1f));
    }

    public int GetDamage()
    {
        return Mathf.RoundToInt(Data.damage * (1 + (Level - 1) * 0.1f));
    }
}