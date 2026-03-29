// Assets/Scripts/CardSystem/Deck.cs
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> cards = new();
    private System.Random random = new();

    public int Count => cards.Count;

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
    }

    public void Shuffle()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }

    public Card DrawCard()
    {
        if (cards.Count == 0) return null;
        var card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public List<Card> GetCards() => new List<Card>(cards);
}