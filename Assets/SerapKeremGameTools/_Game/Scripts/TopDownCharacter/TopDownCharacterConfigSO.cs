using UnityEngine;

namespace SerapKeremGameTools._Game._TopDownCharacterSystem
{
    [CreateAssetMenu(fileName = "TopDownCharacterConfig", menuName = "Game/TopDownCharacterConfig", order = 1)]
    public class TopDownCharacterConfigSO : ScriptableObject
    {
        #region Movement Parameters 

        [Header("Movement Parameters")]
        [Tooltip("Character's movement speed.")]
        [Range(1f, 20f)] public float MovementSpeed = 5f;

        [Tooltip("Character's rotation speed.")]
        [Range(1f, 10f)] public float RotationSpeed = 5f;

        [Tooltip("Acceleration applied to the character's movement.")]
        [Range(1f, 10f)] public float Acceleration = 5f;

        [Tooltip("Deceleration applied when the character stops.")]
        [Range(1f, 10f)] public float Deceleration = 5f;

        #endregion

        #region Combat Parameters

        [Header("Combat Parameters")]
        [Tooltip("Damage dealt by the character.")]
        public float AttackDamage = 10f;

        [Tooltip("Attack cooldown in seconds.")]
        [Range(0f, 5f)] public float AttackCooldown = 1f;

        [Tooltip("Range of attack.")]
        [Range(1f, 10f)] public float AttackRange = 3f;

        #endregion

        #region Health Parameters

        [Header("Health Parameters")]
        [Tooltip("Character's total health.")]
        public float MaxHealth = 100f;

        [Tooltip("Character's current health.")]
        [HideInInspector] public float CurrentHealth;

        [Tooltip("Health regeneration rate per second.")]
        public float HealthRegenRate = 1f;

        [Tooltip("Amount of damage taken per hit.")]
        public float DamageTaken = 10f;

        #endregion

        #region Collection and Interaction Parameters

        [Header("Collection & Interaction Parameters")]
        [Tooltip("Interaction range of the character.")]
        public float InteractionRange = 2f;

        [Tooltip("Collection speed for picking up items.")]
        [Range(0.1f, 10f)] public float CollectionSpeed = 1f;

        #endregion

        #region Animation Parameters

        [Header("Animation Parameters")]
        [Tooltip("Animator parameter name for movement speed.")]
        public string SpeedParameter = "Speed";

        [Tooltip("Animator parameter name for hurt animation.")]
        public string HurtParameter = "IsHurt";

        [Tooltip("Animator parameter name for death animation.")]
        public string DeadParameter = "IsDead";

        [Tooltip("Animator parameter name for attack animation.")]
        public string AttackParameter = "IsAttacking";

        [Tooltip("Animator parameter name for victory animation.")]
        public string WinParameter = "IsWin";

        #endregion

        #region Internal State

        [HideInInspector] public bool IsDead = false;

        #endregion

        /// <summary>
        /// Initializes the character's health and sets its state to alive.
        /// </summary>
        public void InitializeCharacter()
        {
            CurrentHealth = MaxHealth;
            IsDead = false;
        }

        /// <summary>
        /// Applies damage to the character and updates its health status.
        /// </summary>
        public void TakeDamage(float amount)
        {
            if (IsDead) return;

            CurrentHealth -= amount;

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                IsDead = true;
            }
        }

        /// <summary>
        /// Heals the character by the specified amount.
        /// </summary>
        public void Heal(float amount)
        {
            if (IsDead) return;

            CurrentHealth += amount;
            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
        }

        /// <summary>
        /// Revives the character, restoring its health to the maximum.
        /// </summary>
        public void Revive()
        {
            IsDead = false;
            CurrentHealth = MaxHealth;
        }
    }
}
           