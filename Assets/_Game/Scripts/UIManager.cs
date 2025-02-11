using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TriInspector;

public class UIManager : MonoBehaviour
{
    [Title("Panels")]
    [SerializeField] private GameObject _gameplayPanel;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _failPanel;
    [SerializeField] private GameObject _settingsPanel;

    [Title("Gameplay UI")]
    [SerializeField] private TextMeshProUGUI _moveCountText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private Button _settingsButton;

    private void Awake()
    {
        SetupUI();
    }

    private void SetupUI()
    {
        _settingsButton.onClick.AddListener(ToggleSettings);
        HideAllPanels();
        ShowGameplayPanel();
    }

    private void HideAllPanels()
    {
        _gameplayPanel.SetActive(false);
        _winPanel.SetActive(false);
        _failPanel.SetActive(false);
        _settingsPanel.SetActive(false);
    }

    public void ShowGameplayPanel()
    {
        HideAllPanels();
        _gameplayPanel.SetActive(true);
    }

    public void ShowWinPanel()
    {
        _winPanel.SetActive(true);
        _winPanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
    }

    public void ShowFailPanel()
    {
        _failPanel.SetActive(true);
        _failPanel.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
    }

    public void UpdateMoveCount(int moves)
    {
        _moveCountText.text = moves.ToString();
    }

    public void UpdateLevel(int level)
    {
        _levelText.text = $"Level {level}";
    }

    public void UpdateCoins(int coins)
    {
        _coinText.text = coins.ToString();
    }

    public void ShowMessage(string message)
    {
        _messageText.text = message;
        _messageText.transform.DOScale(Vector3.one, 0.3f).From(Vector3.zero)
            .OnComplete(() => {
                DOVirtual.DelayedCall(2f, () => {
                    _messageText.transform.DOScale(Vector3.zero, 0.3f);
                });
            });
    }

    private void ToggleSettings()
    {
        _settingsPanel.SetActive(!_settingsPanel.activeSelf);
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
}