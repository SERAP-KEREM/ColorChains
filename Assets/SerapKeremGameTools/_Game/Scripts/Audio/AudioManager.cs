using UnityEngine;
using System.Collections.Generic;
using SerapKeremGameTools._Game._objectPool;
using SerapKeremGameTools._Game._Singleton;
using SerapKeremGameTools._Game._SaveLoadSystem;

namespace SerapKeremGameTools._Game._AudioSystem
{
    /// <summary>
    /// Manages audio playback, pooling for AudioPlayers, and provides playback controls.
    /// This class ensures that audio is efficiently managed and reused in the game.
    /// </summary>
    public class AudioManager : MonoSingleton<AudioManager>
    {
        #region Inspector Fields

        [Header("Audio Clips List")]
        [Tooltip("A list of all available audio clips that can be played.")]
        [SerializeField]
        private List<Audio> audioClips = new List<Audio>(); // List to store audio clips

        [Header("AudioPlayer Prefab")]
        [Tooltip("The AudioPlayer prefab used to play audio.")]
        [SerializeField]
        private AudioPlayer audioPlayerPrefab; // Reference to the AudioPlayer prefab

        [Tooltip("The maximum number of AudioPlayers that can be in the pool.")]
        [SerializeField]
        private int poolSize = 10; // Maximum number of AudioPlayers in the pool

        #endregion

        #region Private Variables

        private ObjectPool<AudioPlayer> audioPlayerPool; // Object pool for AudioPlayers
        private string currentAudio = string.Empty; // Holds the currently playing audio name

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the AudioManager instance and sets up the audio pool.
        /// Ensures only one instance of AudioManager exists and loads audio clips.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            InitializeAudioPlayerPool();
            ApplySavedVolume();
            LoadAudioClips();
        }

        /// <summary>
        /// Initializes the ObjectPool for AudioPlayers with a specified pool size.
        /// </summary>
        private void InitializeAudioPlayerPool()
        {
            if (audioPlayerPrefab == null)
            {
                Debug.LogError("AudioPlayer prefab is not assigned! Please assign it in the Inspector.");
                return;
            }

            audioPlayerPool = new ObjectPool<AudioPlayer>(audioPlayerPrefab, poolSize, transform);
        }

        /// <summary>
        /// Applies the saved music volume settings to all audio sources in the scene.
        /// </summary>
        private void ApplySavedVolume()
        {
            float savedVolume = LoadManager.LoadData<float>("MusicVolume", 1f);
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (var source in audioSources)
            {
                source.volume = savedVolume;
            }
        }

        /// <summary>
        /// Loads all audio clips from the Resources/Audio folder.
        /// </summary>
        private void LoadAudioClips()
        {
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio");
            foreach (var clip in clips)
            {
                Audio newAudio = new Audio
                {
                    Name = clip.name,
                    Clip = clip,
                    Volume = 1f,
                    Pitch = 1f,
                    Loop = false // Default loop value set to false
                };
                audioClips.Add(newAudio);
            }

            if (audioClips.Count == 0)
            {
                Debug.LogWarning("No audio clips found in the Resources/Audio folder.");
            }
        }

        #endregion

        #region Audio Playback

        /// <summary>
        /// Plays an audio clip by its name from the audioClips list.
        /// If the audio is already playing, it won't play again unless forced.
        /// </summary>
        /// <param name="audioName">The name of the audio clip to play.</param>
        /// <param name="loop">Whether the audio should loop.</param>
        public void PlayAudio(string audioName, bool loop = false)
        {
            // Find the audio clip by name
            Audio audio = audioClips.Find(a => a.Name == audioName);
            if (audio == null)
            {
                Debug.LogWarning($"Audio not found: {audioName}");
                return;
            }

            // Check if the audio is already playing
            if (currentAudio == audioName && loop)
            {
#if UNITY_EDITOR
                Debug.Log($"Audio {audioName} is already playing in loop.");
#endif
                return;
            }

            // Get an AudioPlayer from the pool and play the audio
            AudioPlayer audioPlayer = audioPlayerPool.GetObject();
            if (audioPlayer != null)
            {
                audioPlayer.PlayAudio(audio, loop);
                currentAudio = audioName;
            }
            else
            {
                Debug.LogError("Failed to retrieve an AudioPlayer from the pool!");
            }
        }

        /// <summary>
        /// Stops the currently playing audio.
        /// </summary>
        public void StopAudio()
        {
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (var source in audioSources)
            {
                if (source.isPlaying)
                {
                    source.Stop();
                }
            }

            currentAudio = string.Empty;
        }

        #endregion

        #region Pause and Resume

        /// <summary>
        /// Pauses all active AudioSources in the scene.
        /// This is useful for pausing all audio during a pause menu or when switching scenes.
        /// </summary>
        public void PauseAllAudio()
        {
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (var source in audioSources)
            {
                if (source.isPlaying)
                {
                    source.Pause();
                }
            }
        }

        /// <summary>
        /// Resumes all paused AudioSources in the scene.
        /// This resumes the playback of any paused audio.
        /// </summary>
        public void ResumeAllAudio()
        {
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (var source in audioSources)
            {
                if (!source.isPlaying)
                {
                    source.UnPause();
                }
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Checks if the audio clip with the given name is currently playing.
        /// </summary>
        /// <param name="audioName">The name of the audio clip to check.</param>
        /// <returns>True if the audio is playing, otherwise false.</returns>
        public bool IsPlaying(string audioName)
        {
            return currentAudio == audioName;
        }

        /// <summary>
        /// Returns the AudioPlayer to the pool after it has finished playing.
        /// </summary>
        /// <param name="audioPlayer">The AudioPlayer to return to the pool.</param>
        public void ReturnAudioPlayerToPool(AudioPlayer audioPlayer)
        {
            if (audioPlayer != null)
            {
                audioPlayerPool.ReturnObject(audioPlayer);
            }
        }

        #endregion
    }
}