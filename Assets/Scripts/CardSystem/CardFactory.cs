// Assets/Scripts/CardSystem/CardFactory.cs
using System.Collections.Generic;
using UnityEngine;

public static class CardFactory
{
    public static Deck CreateDeckFromCardData(List<CardData> cardDataList)
    {
        var deck = new Deck();
        foreach (var data in cardDataList)
        {
            deck.AddCard(new Card(data));
        }
        deck.Shuffle();
        return deck;
    }

    public static Card CreateCard(CardData data, int level = 1)
    {
        return new Card(data, level);
    }
}