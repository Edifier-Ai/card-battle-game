// Assets/Scripts/CardSystem/Hand.cs
using System.Collections.Generic;

public class Hand
{
    private List<Card> cards = new();
    private Deck deck;
    private int maxSize;

    public int Count => cards.Count;
    public IReadOnlyList<Card> Cards => cards;

    public Hand(Deck deck, int maxSize)
    {
        this.deck = deck;
        this.maxSize = maxSize;
    }

    public bool CanPlayCard(int index, int currentElixir)
    {
        if (index < 0 || index >= cards.Count) return false;
        return cards[index].Data.elixirCost <= currentElixir;
    }

    public Card PlayCard(int index)
    {
        if (index < 0 || index >= cards.Count) return null;
        var card = cards[index];
        cards.RemoveAt(index);
        return card;
    }

    public void DrawCard()
    {
        if (cards.Count >= maxSize) return;
        var card = deck.DrawCard();
        if (card != null)
            cards.Add(card);
    }

    public void FillHand()
    {
        while (cards.Count < maxSize)
        {
            var card = deck.DrawCard();
            if (card == null) break;
            cards.Add(card);
        }
    }
}