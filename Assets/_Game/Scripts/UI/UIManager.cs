using SerapKeremGameTools._Game._Singleton;
using UnityEngine;
using System.Collections;
using SerapKeremGameTools._Game._AudioSystem;
using UnityEngine.UIElements;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameplayUI _gameplayUI;
    [SerializeField] private SettingsPanelUI _settingsPanel;
    [SerializeField] private WinPanelUI _winPanel;
    [SerializeField] private FailPanelUI _failPanel;

    void Awake()
    {

        InitializeUI();

    }
    /// <summary>
    /// Checks for missing references and logs error messages.
    /// </summary>
    private void ValidateReferences()
    {
        if (_gameplayUI == null) Debug.LogError("GameplayUI is missing!");
        if (_winPanel == null) Debug.LogError("WinPanel is missing!");
        if (_failPanel == null) Debug.LogError("FailPanel is missing!");
        if (_settingsPanel == null) Debug.LogError("SettingsPanel is missing!");
    }

    /// <summary>
    /// Disables all panels.
    /// </summary>
    private void SetupPanels()
    {
        if (_gameplayUI != null) _gameplayUI.gameObject.SetActive(false);
        if (_winPanel != null) _winPanel.gameObject.SetActive(false);
        if (_failPanel != null) _failPanel.gameObject.SetActive(false);
        if (_settingsPanel != null) _settingsPanel.gameObject.SetActive(false);
    }
    /// <summary>
    /// Initializes the UI by enabling the gameplay panel.
    /// </summary>
    private void InitializeUI()
    {
        ValidateReferences();
        SetupPanels();

    
        if (_gameplayUI != null) _gameplayUI.gameObject.SetActive(true);
        if (_winPanel != null) _winPanel.gameObject.SetActive(false);
        if (_failPanel != null) _failPanel.gameObject.SetActive(false);
        if (_settingsPanel != null) _settingsPanel.gameObject.SetActive(false);

        // Settings butonunu ayarla
        if (_gameplayUI != null)
        {
            _gameplayUI.OnSettingsClicked += ShowSettings;
        }
    }


    /// <summary>
    /// Hides the settings panel and enables hole controller.
    /// </summary>
    public void HideSettings()
    {
        if (_settingsPanel != null)
        {
            _settingsPanel.Hide();
          
        }
    }
    #region Panels Show/Hide

    /// <summary>
    /// Displays the Win panel and disables hole controller.
    /// </summary>
    public void ShowWinPanel(int earnedCoins, int totalCoins)
    {
        if (_winPanel != null)
        {
            _winPanel.gameObject.SetActive(true);
            _winPanel.Show();
            _winPanel.UpdateReward(earnedCoins, totalCoins);
        }
    }

    /// <summary>
    /// Displays the Fail panel and disables hole controller.
    /// </summary>
    public void ShowFailPanel()
    {
        if (_failPanel != null)
        {
            _failPanel.gameObject.SetActive(true);
            _failPanel.Show();
          
        }
    }

    /// <summary>
    /// Displays the settings panel and disables hole controller.
    /// </summary>
    public void ShowSettings()
    {
        Debug.Log("Settings button clicked");
        if (_settingsPanel != null)
        {
            _settingsPanel.gameObject.SetActive(true);
            _settingsPanel.Show();
        }
    }

    #endregion
    #region Game Control

    /// <summary>
    /// Restarts the game.
    /// </summary>
    public void RestartGame()
    {
        DOTween.KillAll();
        Time.timeScale = 1f;
        GameManager.Instance.RestartLevel();
    }
    public void SetLevel(int level)
    {
        if (_gameplayUI != null)
            _gameplayUI.SetLevel(level);
    }

    public void SetMoveCount(int moves)
    {
        if (_gameplayUI != null)
            _gameplayUI.SetMoveCount(moves);
    }
    /// <summary>
    /// Moves to the next level.
    /// </summary>
    public void NextLevel()
    {
        DOTween.KillAll();
        Time.timeScale = 1f;
      //  GameManager.Instance.NextLevel();
    }

    #endregion
}