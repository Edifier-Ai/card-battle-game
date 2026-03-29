// Assets/Scripts/UI/MainMenuUI.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button deckEditorButton;

    private void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
        if (deckEditorButton != null)
            deckEditorButton.onClick.AddListener(OnDeckEditorClicked);
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene("Battle");
    }

    private void OnDeckEditorClicked()
    {
        SceneManager.LoadScene("DeckEditor");
    }
}