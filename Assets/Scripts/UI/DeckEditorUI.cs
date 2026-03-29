// Assets/Scripts/UI/DeckEditorUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeckEditorUI : MonoBehaviour
{
    [SerializeField] private Transform availableCardsContainer;
    [SerializeField] private Transform deckContainer;
    [SerializeField] private GameObject cardUIPrefab;
    [SerializeField] private Button backButton;
    [SerializeField] private Text deckCountText;
    [SerializeField] private int maxDeckSize = 8;

    private List<CardData> allCards;
    private List<CardData> currentDeck = new List<CardData>();

    private void Start()
    {
        LoadAllCards();
        RenderAvailableCards();
        RenderDeck();

        if (backButton != null)
            backButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }

    private void LoadAllCards()
    {
        allCards = new List<CardData>(Resources.LoadAll<CardData>("Cards"));
    }

    private void RenderAvailableCards()
    {
        if (availableCardsContainer == null) return;

        foreach (Transform child in availableCardsContainer)
            Destroy(child.gameObject);

        foreach (var card in allCards)
        {
            var cardObj = Instantiate(cardUIPrefab, availableCardsContainer);
            SetupCardUI(cardObj, card);

            var button = cardObj.GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(() => AddToDeck(card));
        }
    }

    private void RenderDeck()
    {
        if (deckContainer == null) return;

        foreach (Transform child in deckContainer)
            Destroy(child.gameObject);

        foreach (var card in currentDeck)
        {
            var cardObj = Instantiate(cardUIPrefab, deckContainer);
            SetupCardUI(cardObj, card);

            var button = cardObj.GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(() => RemoveFromDeck(card));
        }

        if (deckCountText != null)
            deckCountText.text = string.Format("Deck: {0}/{1}", currentDeck.Count, maxDeckSize);
    }

    private void SetupCardUI(GameObject obj, CardData card)
    {
        var nameText = obj.transform.Find("Name")?.GetComponent<Text>();
        var costText = obj.transform.Find("Cost")?.GetComponent<Text>();
        var typeText = obj.transform.Find("Type")?.GetComponent<Text>();

        if (nameText != null) nameText.text = card.cardName;
        if (costText != null) costText.text = card.elixirCost.ToString();
        if (typeText != null) typeText.text = card.cardType.ToString();
    }

    private void AddToDeck(CardData card)
    {
        if (currentDeck.Count < maxDeckSize && !currentDeck.Contains(card))
        {
            currentDeck.Add(card);
            RenderDeck();
        }
    }

    private void RemoveFromDeck(CardData card)
    {
        currentDeck.Remove(card);
        RenderDeck();
    }

    public List<CardData> GetCurrentDeck() => currentDeck;
}