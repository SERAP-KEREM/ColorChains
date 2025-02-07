using SerapKeremGameTools._Game._InputSystem;
using System;
using System.Collections;
using UnityEngine;
namespace SerapKeremGameTools._Game._FPSPlayerSystem
{
    /// <summary>
    /// Handles the health system for the FPS character, including taking damage,
    /// health regeneration, and player death.
    /// </summary>
    public class FPSDamage : MonoBehaviour
    {
        [Header("Health Parameters")]
        [Tooltip("Maximum health value the player can have.")]
        [SerializeField] private float _maxHealth = 100;

        [Tooltip("Time before health regeneration starts after taking damage.")]
        [SerializeField] private float _timeBeforeRegenStarts = 3;

        [Tooltip("Amount of health regained per regeneration increment.")]
        [SerializeField] private float _healthValueIncrement = 1;

        [Tooltip("Time between each health regeneration increment.")]
        [SerializeField] private float _healthTimeIncrement = 0.1f;

        private float _currentHealth;
        private Coroutine _regeneratingHealth;

        /// <summary>
        /// Event triggered when the player takes damage.
        /// The float parameter represents the current health after damage.
        /// </summary>
        public static Action<float> OnTakeDamage;

        /// <summary>
        /// Event triggered when the player's health changes due to damage.
        /// </summary>
        public static Action<float> OnDamage;

        /// <summary>
        /// Event triggered when the player's health increases due to regeneration.
        /// </summary>
        public static Action<float> OnHeal;

        private void OnEnable()
        {
            OnTakeDamage += ApplyDamage;
        }

        private void OnDisable()
        {
            OnTakeDamage -= ApplyDamage;
        }

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        /// <summary>
        /// Applies damage to the player and triggers regeneration if the player is still alive.
        /// </summary>
        /// <param name="damage">Amount of damage to apply.</param>
        private void ApplyDamage(float damage)
        {
            _currentHealth -= damage;

            // Clamp health between 0 and maximum health.
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

            OnDamage?.Invoke(_currentHealth);

            if (_currentHealth <= 0)
            {
                KillPlayer();
            }
            else if (_regeneratingHealth != null)
            {
                StopCoroutine(_regeneratingHealth);
            }

            _regeneratingHealth = StartCoroutine(RegenerateHealth());
        }

        /// <summary>
        /// Handles player death by stopping regeneration and setting health to 0.
        /// </summary>
        private void KillPlayer()
        {
            _currentHealth = 0;

            if (_regeneratingHealth != null)
                StopCoroutine(_regeneratingHealth);

#if UNITY_EDITOR
            Debug.Log("Player is dead.");
#endif
        }

        /// <summary>
        /// Regenerates health over time after a delay.
        /// </summary>
        /// <returns>IEnumerator for coroutine.</returns>
        private IEnumerator RegenerateHealth()
        {
            yield return new WaitForSeconds(_timeBeforeRegenStarts);
            WaitForSeconds timeToWait = new WaitForSeconds(_healthTimeIncrement);

            while (_currentHealth < _maxHealth)
            {
                _currentHealth += _healthValueIncrement;

                if (_currentHealth > _maxHealth)
                    _currentHealth = _maxHealth;

                OnHeal?.Invoke(_currentHealth);
                yield return timeToWait;
            }

            _regeneratingHealth = null;
        }
    }
}
