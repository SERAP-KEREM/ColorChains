using _Main._Mechanics;
using _Main._UI;
using DG.Tweening;
using SerapKeremGameTools._Game._AudioSystem;
using SerapKeremGameTools._Game._InputSystem;
using SerapKeremGameTools._Game._SaveLoadSystem;
using SerapKeremGameTools._Game._Singleton;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main._Managers
{
    /// <summary>
    /// GameManager is a singleton class responsible for managing the overall game state, level transitions, UI, and events.
    /// </summary>
    public class GameManager : MonoSingleton<GameManager>
    {
        #region Inspector Groups

        [Header("Game Elements")]
        [Group("Game Elements/Handcuffs")]
        [SerializeField, Tooltip("List of handcuffs in the game.")]
        private List<Handcuff> _handcuffs = new List<Handcuff>();

        [Group("Game Elements/Collision Check Objects")]
        [SerializeField, Tooltip("Objects used for collision checks.")]
        private GameObject[] _collisionCheckObjects;

        [Group("Game Elements/Plugs")]
        [SerializeField, Tooltip("List of plugs in the game.")]
        private GameObject[] _plugs;

        [Group("Game Elements/Settings")]
        [SerializeField, Tooltip("Target socket count to complete the level.")]
        private int _targetSocketCount;

        [Header("Game State")]
        [Group("Game State/Collision States")]
        [SerializeField, Tooltip("States of collisions.")]
        public List<bool> CollisionStates = new List<bool>();

        [Group("Game State/Gameplay Settings")]
        [SerializeField, Tooltip("Initial move count for the player.")]
        private int _moveCount;

        [Group("Game State/Audio")]
        [SerializeField, Tooltip("Name of the background music clip to play.")]
        private string backgroundMusicName = "BackgroundMusic";

        [Group("Game State/UI")]
        [SerializeField, Tooltip("Reference to the UIManager for UI management.")]
        private UIManager _uiManager;

        /// <summary>
        /// Returns the reference to the UIManager.
        /// </summary>
        public UIManager GetUIManager() => _uiManager;

        [Group("Game State/UI")]
        [SerializeField, Tooltip("Reference to the GameplayUI.")]
        private GameplayUI _gameplayUI;

        [Group("Game State/Audio")]
        [SerializeField, Tooltip("Reference to the AudioManager.")]
        private AudioManager _audioManager;

        #endregion

        #region Private Variables

        private int _completionCount;
        private int _collisionCheckCount;
        private GameObject _currentObject;
        private GameObject _currentSocket;
        private FinalPlug _finalPlug;

        /// <summary>
        /// Indicates whether the player is currently moving an object.
        /// </summary>
        public bool IsMovement { get; set; }

        #endregion

        #region UI and Audio Management

        /// <summary>
        /// Sets up the UI elements at the start of the game.
        /// </summary>
        private void SetupUI()
        {
            if (_uiManager != null)
            {
                int currentLevel = SceneManager.GetActiveScene().buildIndex + 1;
                _uiManager.SetLevel(currentLevel);
                _uiManager.SetMoveCount(_moveCount);

                int totalCoins = LoadManager.LoadData<int>("TotalCoins", 0);
                SaveManager.SaveData("TotalCoins", totalCoins);

                _gameplayUI.SetCoins(totalCoins);
            }
            else
            {
                Debug.LogError("UIManager reference is missing!");
            }
        }

        /// <summary>
        /// Applies the saved music volume settings to all audio sources in the scene.
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

        /// <summary>
        /// Plays the background music.
        /// </summary>
        public void PlayBackgroundMusic()
        {
            if (_audioManager != null)
            {
                _audioManager.PlayAudio(backgroundMusicName, true);
            }
        }

        /// <summary>
        /// Initializes the singleton instance.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _audioManager = AudioManager.Instance;
            EnsureSingleCamera();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the game when the scene starts.
        /// </summary>
        private void Start()
        {
            InitializeGame();
            PlayBackgroundMusic();
            ApplySavedMusicVolume();
            SetupUI();
        }

        /// <summary>
        /// Initializes the game state, including collision states and event listeners.
        /// </summary>
        private void InitializeGame()
        {
            for (int i = 0; i < _targetSocketCount - 1; i++)
            {
                CollisionStates.Add(false);
            }

            PlayerInput.Instance.OnObjectClickedEvent.AddListener(HandleObjectClick);
            EnsureSingleCamera();

            if (_gameplayUI != null)
            {
                _gameplayUI.SetMoveCount(_moveCount);
                _gameplayUI.HideControlPanel();
            }
        }

        #endregion

        #region Camera Management

        /// <summary>
        /// Ensures only one main camera exists in the scene.
        /// </summary>
        private void EnsureSingleCamera()
        {
            if (Camera.main != null)
            {
                // If a main camera exists, ensure it persists across scenes
                DontDestroyOnLoad(Camera.main.gameObject);

                // Destroy any extra cameras
                Camera[] allCameras = FindObjectsOfType<Camera>();
                foreach (var camera in allCameras)
                {
                    if (camera.gameObject != Camera.main.gameObject)
                    {
                        Destroy(camera.gameObject);
                    }
                }
            }
            else
            {
                // If no main camera exists, create one
                GameObject mainCamera = new GameObject("Main Camera");
                mainCamera.AddComponent<Camera>();
                mainCamera.tag = "MainCamera";
                DontDestroyOnLoad(mainCamera);
            }
        }

        #endregion

        #region Object Handling

        /// <summary>
        /// Handles the logic when the player clicks on an object.
        /// </summary>
        private void HandleObjectClick(Collider hitCollider)
        {
            if (hitCollider == null) return;

            if (_currentObject == null && !IsMovement)
            {
                HandlePlugSelection(hitCollider);
            }
            else if (hitCollider.CompareTag("Socket"))
            {
                HandleSocketInteraction(hitCollider);
            }
        }

        /// <summary>
        /// Handles the selection of a plug object.
        /// </summary>
        private void HandlePlugSelection(Collider hitCollider)
        {
            if (hitCollider.CompareTag("BluePlug") || hitCollider.CompareTag("GreenPlug") ||
                hitCollider.CompareTag("PurplePlug") || hitCollider.CompareTag("YellowPlug") ||
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

        /// <summary>
        /// Handles interactions with sockets.
        /// </summary>
        private void HandleSocketInteraction(Collider hitCollider)
        {
            if (_currentObject == null) return;

            Socket socket = hitCollider.GetComponent<Socket>();

            if (!socket.IsEmpty && _currentSocket != hitCollider.gameObject)
            {
                _currentSocket.GetComponent<Socket>().Occupy();
                _finalPlug.Move("ChangePosition", hitCollider.gameObject, socket.MovementPosition);
                socket.Free();
                _currentObject = null;
                _currentSocket = null;
                IsMovement = true;
                _moveCount--;
                _gameplayUI?.SetMoveCount(_moveCount);
            }
            else if (_currentSocket == hitCollider.gameObject)
            {
                _finalPlug.Move("SitOnSocket", hitCollider.gameObject);
                _currentObject = null;
                _currentSocket = null;
                IsMovement = true;
            }
        }

        #endregion

        #region Level Completion and Collision Checks

        /// <summary>
        /// Updates the collision state for a specific index.
        /// </summary>
        public void CheckCollision(int collisionIndex, bool status)
        {
            if (collisionIndex >= 0 && collisionIndex < CollisionStates.Count)
            {
                CollisionStates[collisionIndex] = status;
            }
            else
            {
                Debug.LogWarning($"Invalid collision index: {collisionIndex}");
            }
        }

        /// <summary>
        /// Checks if all plugs are correctly placed in their sockets.
        /// </summary>
        public void CheckPlugs()
        {
            _completionCount = 0;

            foreach (var plug in _plugs)
            {
                if (plug.GetComponent<FinalPlug>().CurrentSocket.name == plug.GetComponent<FinalPlug>().SocketColor)
                {
                    _completionCount++;
                }
            }

            if (_completionCount == _targetSocketCount)
            {
                _gameplayUI.ShowControlPanel();
                _gameplayUI.ShowControlMessage("CHECKING...");

                foreach (var obj in _collisionCheckObjects)
                {
                    obj.SetActive(true);
                }

                StartCoroutine(CheckForCollision());
            }
            else if (_moveCount <= 0)
            {
                _gameplayUI?.ShowControlMessage("LOSE");
                Invoke(nameof(LevelFailed), 2);
            }
        }

        /// <summary>
        /// Waits for collision checks and evaluates the result.
        /// </summary>
        private IEnumerator CheckForCollision()
        {
            yield return new WaitForSeconds(4f);

            _collisionCheckCount = 0;

            foreach (var item in CollisionStates)
            {
                if (!item)
                {
                    _collisionCheckCount++;
                }
            }

            if (_collisionCheckCount == CollisionStates.Count)
            {
                _gameplayUI?.ShowControlMessage("WIN");

                foreach (Handcuff handcuff in _handcuffs)
                {
                    handcuff.OpenHandcuff();
                }

                yield return new WaitForSeconds(2f);
                OnLevelCompleted();
            }
            else
            {
                _gameplayUI?.ShowControlMessage("THERE IS A COLLISION");

                foreach (var obj in _collisionCheckObjects)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                    }
                }

                if (_moveCount <= 0)
                {
                    yield return new WaitForSeconds(1f);
                    LevelFailed();
                }
            }
        }

        /// <summary>
        /// Handles the logic when the level is completed successfully.
        /// </summary>
        private void OnLevelCompleted()
        {
            int earnedCoins = CalculateReward();
            int totalCoins = LoadManager.LoadData<int>("TotalCoins", 0) + earnedCoins;
            SaveManager.SaveData("TotalCoins", totalCoins);

            foreach (Handcuff handcuff in _handcuffs)
            {
                handcuff.OpenHandcuff();
            }

            _uiManager?.ShowWinPanel(earnedCoins, totalCoins);
        }

        /// <summary>
        /// Calculates the reward based on remaining moves.
        /// </summary>
        private int CalculateReward()
        {
            return 100 + (_moveCount * 10);
        }

        #endregion

        #region Level Management

        /// <summary>
        /// Loads the next level in the game.
        /// </summary>
        public void LoadNextLevel()
        {
            DOTween.KillAll();

            int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
            int targetLevel = nextLevel < SceneManager.sceneCountInBuildSettings ? nextLevel : 0;

            StartCoroutine(LoadLevelRoutine(targetLevel));
        }

        /// <summary>
        /// Loads a level asynchronously and initializes the game state.
        /// </summary>
        private IEnumerator LoadLevelRoutine(int levelIndex)
        {
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene(levelIndex);
            yield return new WaitForSeconds(0.1f);

            InitializeGame();
            SetupUI();
        }

        /// <summary>
        /// Restarts the current level.
        /// </summary>
        public void RestartLevel()
        {
            StartCoroutine(LoadLevelRoutine(SceneManager.GetActiveScene().buildIndex));
        }

        /// <summary>
        /// Ends the level unsuccessfully and shows the fail panel.
        /// </summary>
        public void LevelFailed()
        {
            _uiManager?.ShowFailPanel();
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Cleans up resources when the game object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            if (PlayerInput.Instance != null)
            {
                PlayerInput.Instance.OnObjectClickedEvent.RemoveListener(HandleObjectClick);
            }
        }

        #endregion
    }
}