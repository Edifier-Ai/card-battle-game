// Assets/Scripts/UI/CardHandUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHandUI : MonoBehaviour
{
    [Header("Card Slots")]
    [SerializeField] private Transform handContainer;
    [SerializeField] private GameObject cardUIPrefab;

    private List<GameObject> cardUIs = new List<GameObject>();
    private IReadOnlyList<Card> hand;
    private int selectedCardIndex = -1;

    public event System.Action<int, int> OnCardPlayed; // cardIndex, lane

    public void Initialize(IReadOnlyList<Card> playerHand)
    {
        hand = playerHand;
        RenderHand();
    }

    public void RenderHand()
    {
        if (hand == null) return;

        // Clear existing
        foreach (var cardUI in cardUIs)
            Destroy(cardUI);
        cardUIs.Clear();

        // Create new cards
        for (int i = 0; i < hand.Count; i++)
        {
            var cardData = hand[i].Data;
            var cardObj = Instantiate(cardUIPrefab, handContainer);

            // Setup card visuals
            var nameText = cardObj.transform.Find("Name")?.GetComponent<Text>();
            var costText = cardObj.transform.Find("Cost")?.GetComponent<Text>();
            var artImage = cardObj.transform.Find("Art")?.GetComponent<Image>();
            var typeText = cardObj.transform.Find("Type")?.GetComponent<Text>();

            if (nameText != null) nameText.text = cardData.cardName;
            if (costText != null) costText.text = cardData.elixirCost.ToString();
            if (artImage != null && cardData.cardArt != null)
                artImage.sprite = cardData.cardArt;
            if (typeText != null)
                typeText.text = cardData.cardType.ToString();

            // Add click handler
            int index = i;
            var button = cardObj.GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(() => SelectCard(index));

            cardUIs.Add(cardObj);
        }
    }

    private void SelectCard(int index)
    {
        selectedCardIndex = index;

        // Highlight selected card
        for (int i = 0; i < cardUIs.Count; i++)
        {
            var outline = cardUIs[i].GetComponent<Outline>();
            if (outline != null)
                outline.enabled = (i == index);
        }
    }

    public void OnLaneSelected(int lane)
    {
        if (selectedCardIndex >= 0 && hand != null)
        {
            OnCardPlayed(selectedCardIndex, lane);
            selectedCardIndex = -1;

            // Clear highlights
            foreach (var cardUI in cardUIs)
            {
                var outline = cardUI.GetComponent<Outline>();
                if (outline != null)
                    outline.enabled = false;
            }
        }
    }

    public void UpdateCardInteractability(int currentElixir)
    {
        for (int i = 0; i < cardUIs.Count; i++)
        {
            var button = cardUIs[i].GetComponent<Button>();
            if (button != null && hand != null)
            {
                button.interactable = hand[i].Data.elixirCost <= currentElixir;
            }
        }
    }

    public int GetSelectedCard() => selectedCardIndex;
}