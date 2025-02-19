using _Main._Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace _Main._UI
{
    public class GameplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _moveCountText;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private GameObject _controlPanel;
    [SerializeField] private TextMeshProUGUI _controlText;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private CanvasGroup _canvasGroup;

    private UIManager _uiManager;
    public void SetLevel(int level) => _levelText.text = $"Level: {level}";
    public void SetMoveCount(int moves) => _moveCountText.text = $"Moves: {moves}";
    public void SetCoins(int coins) => _coinText.text = $"{coins}";


    public event System.Action OnSettingsClicked;

    private void Awake()
    {
        _uiManager = GameManager.Instance.GetUIManager();
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();
        InitializeUI();

        SetupSettingsButton();
    }

    private void InitializeUI()
    {
        ValidateReferences();
      
    }
    /// <summary>
    /// Sets up the settings button click listener.
    /// </summary>
    private void SetupSettingsButton()
    {
        if (_settingsButton != null)
        {
            _settingsButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.AddListener(() => _uiManager.ShowSettings());
        }
    }

    /// <summary>
    /// Validates the essential UI references.
    /// </summary>
    private void ValidateReferences()
    {
        if (_levelText == null) Debug.LogError("Level Text is missing!");
       
        if (_settingsButton == null) Debug.LogError("Settings Button is missing!");
    }
    public void ShowControlMessage(string message)
    {
        _controlText.text = message;
        _controlPanel.SetActive(true);
    }

  
    public void HideControlPanel() => _controlPanel.SetActive(false);
    public void ShowControlPanel() => _controlPanel.SetActive(true);



}
}