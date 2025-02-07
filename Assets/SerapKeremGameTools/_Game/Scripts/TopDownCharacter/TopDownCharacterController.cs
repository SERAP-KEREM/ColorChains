using SerapKeremGameTools._Game._InputSystem;
using System.Collections;
using UnityEngine;

namespace SerapKeremGameTools._Game._TopDownCharacterSystem
{
    /// <summary>
    /// A top-down character controller that manages movement, rotation, gravity, speed boosting, 
    /// and the ability to pause movement. This script works with a CharacterController component 
    /// and uses configurable parameters from a ScriptableObject for flexibility.
    /// </summary>
    public class TopDownCharacterController : MonoBehaviour
    {
        [SerializeField, Tooltip("The CharacterController component responsible for moving the character.")]
        private CharacterController _characterController;

        [SerializeField, Tooltip("ScriptableObject containing movement and other configuration parameters for the character.")]
        private TopDownCharacterConfigSO _characterConfig;

        [SerializeField, Tooltip("Custom gravity value applied to the character. Negative values pull the character down.")]
        private float _customGravity = -9.81f;

        private Vector3 _currentVelocity; // Stores the current horizontal movement velocity of the character
        private Vector3 _verticalVelocity; // Stores the vertical velocity, mainly for handling gravity
        private float _speedBoostMultiplier = 1f; // Temporary multiplier applied to the character's movement speed

        private Coroutine _pauseMovementCoroutine; // Reference to the coroutine for pausing movement

        /// <summary>
        /// Returns the magnitude of the current velocity vector, representing the character's movement speed.
        /// </summary>
        public float CurrentVelocityMagnitude => _currentVelocity.magnitude;

        private void Awake()
        {
            // Ensure required components are assigned and valid
#if UNITY_EDITOR
            if (_characterController == null)
                Debug.LogError("CharacterController reference is missing!");

            if (PlayerInput.Instance == null)
                Debug.LogError("PlayerInput reference is missing!");
#endif
        }

        private void OnEnable()
        {
            // Subscribe to the input system's mouse position event
            PlayerInput.Instance.OnMousePositionInput.AddListener(UpdateCharacterRotation);
        }

        private void OnDisable()
        {
            // Unsubscribe from the input system's mouse position event
            PlayerInput.Instance.OnMousePositionInput.RemoveListener(UpdateCharacterRotation);
        }

        private void Update()
        {
            // Process player movement input and apply gravity effects
            Vector2 input = PlayerInput.Instance.GetMovementInput();
            HandleMovementInput(input);
            ApplyGravity();
        }

        /// <summary>
        /// Handles the character's movement by processing input and applying acceleration or deceleration.
        /// </summary>
        /// <param name="input">The movement input vector from the player (x and y).</param>
        public void HandleMovementInput(Vector2 input)
        {
            // Convert input to a 3D direction vector
            Vector3 movementDirection = new Vector3(input.x, 0, input.y).normalized;

            if (movementDirection.magnitude >= 0.1f)
            {
                // Accelerate towards the target speed
                float targetSpeed = _characterConfig.MovementSpeed * _speedBoostMultiplier;
                _currentVelocity = Vector3.MoveTowards(
                    _currentVelocity,
                    movementDirection * targetSpeed,
                    _characterConfig.Acceleration * Time.deltaTime
                );
            }
            else
            {
                // Decelerate when no input is provided
                _currentVelocity = Vector3.MoveTowards(
                    _currentVelocity,
                    Vector3.zero,
                    _characterConfig.Deceleration * Time.deltaTime
                );
            }

            // Apply movement to the CharacterController
            _characterController.Move(_currentVelocity * Time.deltaTime);
        }

        /// <summary>
        /// Rotates the character to face the mouse position in the world space.
        /// </summary>
        /// <param name="mousePosition">The position of the mouse in world coordinates.</param>
        public void UpdateCharacterRotation(Vector3 mousePosition)
        {
            Vector3 directionToMouse = mousePosition - transform.position;
            directionToMouse.y = 0; // Ignore vertical differences

            if (directionToMouse.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    _characterConfig.RotationSpeed * Time.deltaTime
                );
            }
        }

        /// <summary>
        /// Applies gravity to the character, simulating a downward force when not grounded.
        /// </summary>
        private void ApplyGravity()
        {
            if (!_characterController.isGrounded)
            {
                // Apply gravity over time
                _verticalVelocity.y += _customGravity * Time.deltaTime;
            }
            else
            {
                // Reset vertical velocity when grounded
                _verticalVelocity.y = 0;
            }

            // Move the character downward
            _characterController.Move(_verticalVelocity * Time.deltaTime);
        }

        /// <summary>
        /// Temporarily pauses the character's movement for a specified duration.
        /// </summary>
        /// <param name="duration">The duration (in seconds) to pause movement.</param>
        public void PauseMovement(float duration)
        {
            if (_pauseMovementCoroutine != null)
                StopCoroutine(_pauseMovementCoroutine);

            _pauseMovementCoroutine = StartCoroutine(PauseMovementCoroutine(duration));
        }

        /// <summary>
        /// Coroutine to pause the character's movement for a given duration.
        /// </summary>
        /// <param name="duration">The duration (in seconds) to pause movement.</param>
        private IEnumerator PauseMovementCoroutine(float duration)
        {
            _currentVelocity = Vector3.zero;
            _verticalVelocity = Vector3.zero;
            yield return new WaitForSeconds(duration);
        }

        /// <summary>
        /// Temporarily boosts the character's movement speed by a percentage for a specified duration.
        /// </summary>
        /// <param name="percentageIncrease">The percentage increase in movement speed.</param>
        /// <param name="duration">The duration (in seconds) of the speed boost.</param>
        public void BoostSpeed(float percentageIncrease, float duration)
        {
            _speedBoostMultiplier = 1 + (percentageIncrease / 100);
            StartCoroutine(ResetSpeedBoostAfterTime(duration));
        }

        /// <summary>
        /// Coroutine to reset the character's speed boost after a specified duration.
        /// </summary>
        /// <param name="duration">The duration (in seconds) of the speed boost.</param>
        private IEnumerator ResetSpeedBoostAfterTime(float duration)
        {
            yield return new WaitForSeconds(duration);
            _speedBoostMultiplier = 1f;
        }
    }
}
