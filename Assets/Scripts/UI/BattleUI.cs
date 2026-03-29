// Assets/Scripts/UI/BattleUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    [Header("Elixir")]
    [SerializeField] private Slider elixirBar;
    [SerializeField] private TMP_Text elixirText;

    [Header("Timer")]
    [SerializeField] private TMP_Text timerText;

    [Header("Results")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button returnButton;

    [Header("Card Hand")]
    [SerializeField] private CardHandUI cardHandUI;

    [Header("Lane Buttons")]
    [SerializeField] private Button[] laneButtons;

    private BattleManager battleManager;
    private ElixirManager elixirManager;

    private void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        elixirManager = FindObjectOfType<ElixirManager>();

        if (elixirManager != null)
            elixirManager.OnElixirChanged += UpdateElixirUI;

        EventBus.Subscribe<BattleEndEvent>(OnBattleEnd);

        resultPanel?.SetActive(false);
        if (returnButton != null)
            returnButton.onClick.AddListener(OnReturnClicked);

        SetupLaneButtons();
        SetupCardHand();
    }

    private void Update()
    {
        UpdateTimer();
        UpdateCardInteractability();
    }

    private void SetupLaneButtons()
    {
        for (int i = 0; i < laneButtons.Length; i++)
        {
            int lane = i;
            laneButtons[i].onClick.AddListener(() => OnLaneSelected(lane));
        }
    }

    private void SetupCardHand()
    {
        if (battleManager != null && cardHandUI != null)
        {
            cardHandUI.Initialize(battleManager.GetPlayerHand());
            cardHandUI.OnCardPlayed += OnCardPlayed;
        }
    }

    private void UpdateElixirUI(int amount)
    {
        if (elixirBar != null)
            elixirBar.value = amount;
        if (elixirText != null)
            elixirText.text = amount.ToString();
    }

    private void UpdateTimer()
    {
        if (battleManager == null || timerText == null) return;

        float remaining = battleManager.GetRemainingTime();
        int minutes = Mathf.FloorToInt(remaining / 60);
        int seconds = Mathf.FloorToInt(remaining % 60);
        timerText.text = $"{minutes}:{seconds:00}";
    }

    private void UpdateCardInteractability()
    {
        if (cardHandUI != null)
            cardHandUI.UpdateCardInteractability(battleManager?.GetCurrentElixir() ?? 0);
    }

    private void OnBattleEnd(BattleEndEvent e)
    {
        if (resultPanel != null)
            resultPanel.SetActive(true);
        if (resultText != null)
            resultText.text = e.PlayerWon ? "Victory!" : "Defeat";
    }

    private void OnReturnClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnLaneSelected(int lane)
    {
        if (cardHandUI != null)
            cardHandUI.OnLaneSelected(lane);
    }

    private void OnCardPlayed(int cardIndex, int lane)
    {
        if (battleManager != null)
        {
            battleManager.PlayCard(cardIndex, lane);
            cardHandUI?.RenderHand();
        }
    }

    private void OnDestroy()
    {
        if (elixirManager != null)
            elixirManager.OnElixirChanged -= UpdateElixirUI;

        EventBus.Unsubscribe<BattleEndEvent>(OnBattleEnd);

        if (cardHandUI != null)
            cardHandUI.OnCardPlayed -= OnCardPlayed;
    }
}