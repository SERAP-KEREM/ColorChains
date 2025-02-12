using DG.Tweening;
using SerapKeremGameTools._Game._AudioSystem;
using SerapKeremGameTools._Game._InputSystem;
using SerapKeremGameTools._Game._SaveLoadSystem;
using SerapKeremGameTools._Game._Singleton;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
   
    [Header("Game Elements")]
    [SerializeField] private List<Handcuff> _handcuffs = new List<Handcuff>();
    [SerializeField] private GameObject[] _collisionCheckObjects;
    [SerializeField] private GameObject[] _plugs;
    [SerializeField] private int _targetSocketCount;

    [Header("Game State")]
    public List<bool> CollisionStates = new List<bool>();
    [SerializeField] private int _moveCount;
    private int _completionCount;
    private int _collisionCheckCount;


    private GameObject _currentObject;
    private GameObject _currentSocket;
    private FinalPlug _finalPlug;
    public bool IsMovement { get; set; }

    [Header("Panels")]
    [SerializeField] private GameplayUI _gameplayUI;

    [SerializeField, Tooltip("Reference to the UIManager for UI management.")]
    private UIManager _uiManager;



    public GameplayUI GetGameplayUI() => _gameplayUI;
    public UIManager GetUIManager() => _uiManager;

    [Header("Audio")]
    [SerializeField, Tooltip("Name of the background music clip to play.")]
    private string backgroundMusicName = "BackgroundMusic";

    [SerializeField, Tooltip("Reference to the AudioManager.")]
    private AudioManager _audioManager;


    protected override void Awake()
    {
        base.Awake();
        _uiManager = FindObjectOfType<UIManager>();
        _audioManager = AudioManager.Instance;

        if (_uiManager == null)
            Debug.LogError("UIManager bulunamad?!");

    }
    private void Start()
    {
        InitializeGame();
        PlayBackgroundMusic();
        ApplySavedMusicVolume();
        SetupUI();
    }
    private void SetupUI()
    {
        if (_uiManager != null)
        {
            int currentLevel = SceneManager.GetActiveScene().buildIndex;
            _uiManager.SetLevel(currentLevel);
            _uiManager.SetMoveCount(_moveCount);
        }
        else
        {
            Debug.LogError("UIManager reference is missing!");
        }
    }

    private void InitializeGame()
    {
        for (int i = 0; i < _targetSocketCount - 1; i++)
        {
            CollisionStates.Add(false);
        }
        _audioManager = AudioManager.Instance;
        PlayerInput.Instance.OnObjectClickedEvent.AddListener(HandleObjectClick);

        int currentLevel = SceneManager.GetActiveScene().buildIndex + 1;
        DontDestroyOnLoad(Camera.main.gameObject);

        _audioManager = AudioManager.Instance;
    }
    #region Audio Management

    /// <summary>
    /// Plays the background music.
    /// </summary>
    public void PlayBackgroundMusic()
    {
        _audioManager?.PlayAudio(backgroundMusicName, true);
    }

    /// <summary>
    /// Applies the saved volume setting to all audio sources in the scene.
    /// </summary>
    private void ApplySavedMusicVolume()
    {
        float savedVolume = LoadManager.LoadData<float>("MusicVolume", 1f);

        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (var source in audioSources)
        {
            source.volume = savedVolume;
        }
    }

    #endregion
    private void HandleObjectClick(Collider hitCollider)
    {
     

        if (hitCollider != null)
        {
            //# BOTTOM PLUG
            if (_currentObject == null && !IsMovement)
            {
                if (hitCollider.CompareTag("BluePlug") ||
                    hitCollider.CompareTag("GreenPlug") ||
                    hitCollider.CompareTag("RedPlug"))
                {
                    _finalPlug = hitCollider.GetComponent<FinalPlug>();
                    _finalPlug.Move("Select", _finalPlug.CurrentSocket,
                        _finalPlug.CurrentSocket.GetComponent<Socket>().MovementPosition);

                    _currentObject = hitCollider.gameObject;
                    _currentSocket = _finalPlug.CurrentSocket;
                    IsMovement = true;
                }
            }
            //# SOCKET
            else if (hitCollider.CompareTag("Socket"))
            {
                if (_currentObject != null && !hitCollider.GetComponent<Socket>().IsEmpty &&
                    _currentSocket != hitCollider.gameObject)
                {
                    _currentSocket.GetComponent<Socket>().IsEmpty = false;
                    Socket socket = hitCollider.GetComponent<Socket>();

                    _finalPlug.Move("ChangePosition", hitCollider.gameObject, socket.MovementPosition);
                    socket.IsEmpty = true;

                    _currentObject = null;
                    _currentSocket = null;
                    IsMovement = true;
                    _moveCount--;
                    _gameplayUI.SetMoveCount(_moveCount);
                }
                else if (_currentSocket == hitCollider.gameObject)
                {
                    _finalPlug.Move("SitOnSocket", hitCollider.gameObject);
                    _currentObject = null;
                    _currentSocket = null;
                    IsMovement = true;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (PlayerInput.Instance != null)
        {
            PlayerInput.Instance.OnObjectClickedEvent.RemoveListener(HandleObjectClick);
        }
    }
    public void CheckPlugs()
    {
        foreach (var plug in _plugs)
        {
            if (plug.GetComponent<FinalPlug>().CurrentSocket.name == plug.GetComponent<FinalPlug>().SocketColor)
            {
                _completionCount++;
            }
        }
        if (_completionCount == _targetSocketCount)
        {
            Debug.Log("All sockets are in place");
            _gameplayUI.ShowControlPanel();
            _gameplayUI.ShowControlMessage("CHECKING...");
            foreach (var obj in _collisionCheckObjects)
            {
                obj.SetActive(true);
            }
            StartCoroutine(CheckForCollision());
        }
        else
        {
            if (_moveCount <= 0)
            {
                Debug.Log("Lose");
                _gameplayUI.ShowControlMessage("LOSE");
            }
        }
        _completionCount = 0;

    }

    /// <summary>
    /// Ends the level unsuccessfully, triggering the level failed event and showing the fail panel.
    /// </summary>
    private void LevelFailed()
    {
    
        if (_uiManager != null)
        {
            _uiManager.ShowFailPanel();
        }
    }
    public void CheckCollision(int collisionIndex, bool status)
    {
        CollisionStates[collisionIndex] = status;

    }


    private IEnumerator CheckForCollision()
    {
        yield return new WaitForSeconds(4f);

        foreach (var item in CollisionStates)
        {
            if (!item)
            {
                _collisionCheckCount++;
            }
        }

        if (_collisionCheckCount == CollisionStates.Count)
        {
            _gameplayUI.ShowControlPanel();
            _gameplayUI.ShowControlMessage("WIN");

            // Kelepçeleri aç
            foreach (Handcuff handcuff in _handcuffs)
            {
                handcuff.OpenHandcuff();
            }

            // 2 saniye bekle ve sonra win panel'i göster
            yield return new WaitForSeconds(2f);

            int earnedCoins = CalculateReward();
            _gameplayUI.SetCoins(earnedCoins);
            OnLevelCompleted();
         
        }
        else
        {
            _gameplayUI.ShowControlPanel();
            _gameplayUI.ShowControlMessage("THERE IS A COLLISION");
            foreach (var obj in _collisionCheckObjects)
            {
                obj.SetActive(false);
            }
            if (_moveCount <= 0)
            {
                yield return new WaitForSeconds(1f); // Fail mesaj?n? görmek için k?sa bir bekleme
                LevelFailed();
            }
        }
    }


      public void LoadNextLevel()
    {
        DOTween.KillAll();
        int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (nextLevel < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LoadLevelRoutine(nextLevel));
        }
        else
        {
            StartCoroutine(LoadLevelRoutine(1)); // ?lk levele dön
        }
    }
    private IEnumerator LoadLevelRoutine(int levelIndex)
    {
        // Sahne geçi? efekti veya yükleme ekran? eklenebilir
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(levelIndex);
        yield return new WaitForSeconds(0.1f);

        // Yeni sahne yüklendikten sonra UI'? yeniden ba?lat
        InitializeGame();
        SetupUI();
    }
    public void RestartLevel()
    {
        StartCoroutine(LoadLevelRoutine(SceneManager.GetActiveScene().buildIndex));
    }

    public void StartNewGame()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(0); // ?lk level
    }
    private void OnLevelCompleted()
    {
        int earnedCoins = CalculateReward();
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0) + earnedCoins;
        PlayerPrefs.SetInt("TotalCoins", totalCoins);

        foreach (Handcuff handcuff in _handcuffs)
        {
            handcuff.OpenHandcuff();
        }

        _uiManager.ShowWinPanel(earnedCoins, totalCoins);
    }

    private int CalculateReward()
    {
        // Basit bir ödül hesaplama
        return 100 + (_moveCount * 10);
    }

    private void DecreaseMoveCount()
    {
        _moveCount--;
      //  UIManager.Instance.SetMoveCount(_moveCount);

        if (_moveCount <= 0)
        {
       //     UIManager.Instance.ShowFailPanel();
        }
    }
}
