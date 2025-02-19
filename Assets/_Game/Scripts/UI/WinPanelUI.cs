using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using _Main._UI;
using _Main._Managers;

public class WinPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _earnedCoinText;
    [SerializeField] private TextMeshProUGUI _totalCoinText;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private TextMeshProUGUI _earnedCoinsText;
    [SerializeField] private TextMeshProUGUI _totalCoinsText;
    private UIManager _uiManager;

    /// <summary>
    /// Initializes the WinPanel, sets up button listeners.
    /// </summary>
    private void Awake()
    {
        _uiManager = GameManager.Instance.GetUIManager();
        SetupButtons();
    }

    /// <summary>
    /// Sets up the button listeners for next level and restart actions.
    /// </summary>
    private void SetupButtons()
    {
        if (_nextLevelButton != null)
            _nextLevelButton.onClick.AddListener(() => GameManager.Instance.LoadNextLevel());

        if (_restartButton != null)
            _restartButton.onClick.AddListener(() => _uiManager.RestartGame());
    }
    public void UpdateReward(int earned, int total)
    {
        if (_earnedCoinsText != null)
            _earnedCoinsText.text = $"{earned}";

        if (_totalCoinsText != null)
            _totalCoinsText.text = $"Total: {total}";
    }
    #region UI Control

    /// <summary>
    /// Displays the win panel with an animation.
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.3f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }
    #endregion

    #region Cleanup

    /// <summary>
    /// Removes all button listeners to prevent memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        if (_nextLevelButton != null)
            _nextLevelButton.onClick.RemoveAllListeners();
        if (_restartButton != null)
            _restartButton.onClick.RemoveAllListeners();
    }

    #endregion




}