using SerapKeremGameTools._Game._AudioSystem;
using SerapKeremGameTools._Game._InputSystem;
using System;
using System.Collections;
using UnityEngine;

namespace SerapKeremGameTools._Game._FPSPlayerSystem
{
    /// <summary>
    /// This class handles the player's movement system in a first-person shooter game. 
    /// It includes walking, sprinting, crouching, jumping, and features like head bobbing, footstep sounds, stamina, and zooming. 
    /// The movement system integrates with the input system for player control, the audio system for footstep sounds based on terrain, 
    /// and camera management for smooth first-person view control. The system supports jumping mechanics, stamina regeneration, 
    /// smooth transitions between crouching and standing positions, and more.
    /// </summary>
    public class FPSMovement : MonoBehaviour
    {
        public bool CanMove { get; private set; } = true;

        [Header("Functional Options")]
        [Tooltip("Enables or disables head bobbing effect when moving.")]
        [SerializeField] private bool _canUseHeadbob = true;

        [Tooltip("Enables or disables footstep sounds when the player moves.")]
        [SerializeField] private bool _useFootSteps = true;

        [Tooltip("Enables or disables zoom functionality.")]
        [SerializeField] private bool _canZoom = true;

        [Tooltip("Enables or disables sprinting.")]
        [SerializeField] private bool _canSprint = true;

        [Tooltip("Enables or disables stamina usage during sprinting.")]
        [SerializeField] private bool _useStamina = true;

        [Header("Movement Parameters")]
        [Tooltip("The player's walking speed.")]
        [SerializeField] private float _walkSpeed = 3.0f;

        [Tooltip("The player's sprinting speed.")]
        [SerializeField] private float _sprintSpeed = 6.0f;

        [Tooltip("The player's crouching speed.")]
        [SerializeField] private float _crouchSpeed = 1.5f;

        [Tooltip("The player's speed when sliding on slopes.")]
        [SerializeField] private float _slopeSpeed = 8f;

        [Tooltip("Enables or disables jumping.")]
        [SerializeField] private bool _canJump = true;

        [Tooltip("Indicates whether the player is allowed to crouch. Set to false to disable crouching.")]
        [SerializeField] private bool _canCrouch = true;

        [Header("Stamina Parameters")]
        [Tooltip("The maximum stamina the player can have.")]
        [SerializeField] private float _maxStamina = 100;

        [Tooltip("Multiplier for stamina consumption during sprinting.")]
        [SerializeField] private float _staminaUseMultiplier = 5;

        [Tooltip("The time delay before stamina starts regenerating.")]
        [SerializeField] private float _timeBeforeStaminaRegenStarts = 5;

        [Tooltip("Amount of stamina restored per increment.")]
        [SerializeField] private float _staminaValueIncrement = 2;

        [Tooltip("Time duration between stamina regeneration increments.")]
        [SerializeField] private float _staminaTimeIncrement = 0.1f;

        private float _currentStamina; // Current stamina level, decreases when sprinting and regenerates after a delay.
        private Coroutine _regeneratingStamina;
        public static Action<float> OnStaminaChange;

        [Header("Jumping Parameters")]
        [Tooltip("Force applied to the player when jumping.")]
        [SerializeField] private float _jumpForce = 8.0f;

        [Tooltip("Gravitational force affecting the player.")]
        [SerializeField] private float _gravity = 15.0f;

        [Tooltip("Determines whether the player will slide down slopes.")]
        [SerializeField] private bool _willSlideOnSlopes = true;

        [Header("Zoom Parameters")]
        [Tooltip("Time it takes to zoom in or out.")]
        [SerializeField] private float _timeToZoom = 0.3f;

        [Tooltip("Field of view when the player is zoomed in.")]
        [SerializeField] private float _zoomFOV = 30f;

        private float _defaultFOV;
        private Coroutine _zoomRoutine;

        [Header("Crouch Parameters")]
        [Tooltip("Height of the player when crouching.")]
        [SerializeField] private float _crouchHeight = 0.5f;

        [Tooltip("Height of the player when standing.")]
        [SerializeField] private float _standingHeight = 2f;

        [Tooltip("Time it takes to crouch or stand.")]
        [SerializeField] private float _timeToCrouch = 0.25f;

        [Header("Headbob Parameters")]
        [Tooltip("Speed of the headbob while walking.")]
        [SerializeField] private float _walkBobSpeed = 14f;

        [Tooltip("Amount of headbob while walking.")]
        [SerializeField] private float _walkBobAmount = 0.05f;

        [Tooltip("Speed of the headbob while sprinting.")]
        [SerializeField] private float _sprintBobSpeed = 18f;

        [Tooltip("Amount of headbob while sprinting.")]
        [SerializeField] private float _sprintBobAmount = 0.11f;

        [Tooltip("Speed of the headbob while crouching.")]
        [SerializeField] private float _crouchBobSpeed = 8f;

        [Tooltip("Amount of headbob while crouching.")]
        [SerializeField] private float _crouchBobAmount = 0.025f;

        [Header("Footstep Parameters")]
        [Tooltip("Base step speed for footstep sound.")]
        [SerializeField] private float _baseStepSpeed = 0.5f;

        [Tooltip("Multiplier for footstep sound while crouching.")]
        [SerializeField] private float _crouchStepMultipler = 1.5f;

        [Tooltip("Multiplier for footstep sound while sprinting.")]
        [SerializeField] private float _sprintStepMultipler = 0.6f;

        [Header("Audio Parameters")]
        [Tooltip("Audio clip for footsteps on wood surface.")]
        [SerializeField] private string _woodAudioName;

        [Tooltip("Audio clip for footsteps on metal surface.")]
        [SerializeField] private string _metalAudioName;

        [Tooltip("Audio clip for footsteps on grass surface.")]
        [SerializeField] private string _grassAudioName;

        [Tooltip("Audio clip for sprinting footsteps on wood surface.")]
        [SerializeField] private string _woodAudioNameSprint;

        [Tooltip("Audio clip for sprinting footsteps on metal surface.")]
        [SerializeField] private string _metalAudioNameSprint;

        [Tooltip("Audio clip for sprinting footsteps on grass surface.")]
        [SerializeField] private string _grassAudioNameSprint;

        [Tooltip("Audio clip for jump sound on wood surface.")]
        [SerializeField] private string _woodAudioNameJump;

        [Tooltip("Audio clip for jump sound on metal surface.")]
        [SerializeField] private string _metalAudioNameJump;

        [Tooltip("Audio clip for jump sound on grass surface.")]
        [SerializeField] private string _grassAudioNameJump;

        // Private variables
        private Camera _playerCamera;
        private CharacterController _characterController;
        private Vector3 _moveDirection;
        private Vector2 _currentInput;
        private float _footstepTimer = 0;
        private float _timer = 0; // Timer for various cooldowns like stamina regen and footstep sounds.

        private bool _isCrouching = false;
        private bool _duringCrouchAnimation = false;
        private float _defaultYPos;

        [SerializeField] private Vector3 _crouchingCenter = new Vector3(0, 0.5f, 0);
        [SerializeField] private Vector3 _standingCenter = new Vector3(0, 0, 0);

        //// Mouse Look Variables
        [Header("Mouse Look Settings")]
        [Tooltip("Mouse sensitivity for vertical look (up/down).")]
        [SerializeField] private float _mouseLookSpeedX = 2.0f;

        [Tooltip("Mouse sensitivity for horizontal look (left/right).")]
        [SerializeField] private float _mouseLookSpeedY = 2.0f;

        private float _currentRotationX = 0f; // The current vertical rotation of the player, used to limit looking up and down.

        // Initialization
        /// <summary>
        /// Initializes the player camera, character controller, default position, FOV, and cursor lock state.
        /// </summary>
        void Awake()
        {
            _playerCamera = GetComponentInChildren<Camera>();
            _characterController = GetComponent<CharacterController>();
            _defaultYPos = _playerCamera.transform.localPosition.y;
            _defaultFOV = _playerCamera.fieldOfView;
            _currentStamina = _maxStamina;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // SLIDING PARAMETERS
        /// <summary>
        /// Checks if the player is sliding on a slope based on the angle of the surface underfoot.
        /// </summary>
        private Vector3 hitPointNormal;
        private bool IsSliding
        {
            get
            {
                Debug.DrawRay(transform.position, Vector3.down, Color.red);
                if (_characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
                {
                    hitPointNormal = slopeHit.normal;
                    return Vector3.Angle(hitPointNormal, Vector3.up) > _characterController.slopeLimit;
                }
                else
                {
                    return false;
                }
            }
        }

        // Main Update Loop
        /// <summary>
        /// Main update loop that checks if the player can move and handles movement, camera look, and other behaviors like jumping and crouching.
        /// </summary>
        void Update()
        {
            if (CanMove)
            {
                HandleMovementInput();
                HandleMouseLook();

                if (PlayerFPSInput.IsShouldJump) HandleJump();
                if (PlayerFPSInput.IsShouldCrouch) StartCoroutine(CrouchStand());
                if (_canUseHeadbob) HandleHeadbob();
                if (_useFootSteps) HandleFootSteps();
                if (_canZoom) HandleZoom();
                if (_useStamina) HandleStamina();
                if (_canJump) HandleJump();
                if (_canCrouch) HandleCrouch();
                ApplyFinalMovements();
            }
        }

        // Handle movement input and calculate direction
        /// <summary>
        /// Handles the player's movement input, calculates the movement direction, and adjusts speed based on sprinting or crouching.
        /// </summary>
        private void HandleMovementInput()
        {
            float currentSpeed;

            // Sprinting is only possible when stamina is available, otherwise switch to walking speed
            if (PlayerFPSInput.IsSprinting && _currentStamina > 0)
            {
                currentSpeed = _sprintSpeed;
            }
            else
            {
                currentSpeed = _isCrouching ? _crouchSpeed : _walkSpeed;
            }

            _currentInput = new Vector2(currentSpeed * Input.GetAxis("Vertical"), currentSpeed * Input.GetAxis("Horizontal"));

            float moveDirectionY = _moveDirection.y;
            _moveDirection = (transform.TransformDirection(Vector3.forward) * _currentInput.x) + (transform.TransformDirection(Vector3.right) * _currentInput.y);
            _moveDirection.y = moveDirectionY;
        }

        // Handle mouse look (camera rotation)
        /// <summary>
        /// Controls the camera's vertical and horizontal rotation based on mouse input.
        /// Limits vertical rotation to avoid unnatural angles.
        /// </summary>
        private void HandleMouseLook()
        {
            // Mouse movement for vertical axis (Y)
            float rotationX = Input.GetAxis("Mouse Y") * _mouseLookSpeedY;  // Vertical sensitivity
            _currentRotationX -= rotationX;  // Accumulate up/down look movement
            _currentRotationX = Mathf.Clamp(_currentRotationX, -80f, 80f);  // Clamp vertical rotation to a specified range

            // Update the camera's local rotation (vertical)
            _playerCamera.transform.localRotation = Quaternion.Euler(_currentRotationX, 0, 0);

            // Update the character's horizontal rotation (turning left/right)
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _mouseLookSpeedX, 0);
        }

        // Handle jump behavior
        /// <summary>
        /// Handles the player's jump behavior, applying the jump force and playing the jump sound when the player is grounded.
        /// </summary>
        private void HandleJump()
        {
            if (PlayerFPSInput.IsShouldJump && _characterController.isGrounded && !_duringCrouchAnimation)
            {
                _moveDirection.y = _jumpForce;
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 5))
                {
                    string jumpAudio = GetFootstepAudioNameJump(hit.collider.tag);
                    if (!string.IsNullOrEmpty(jumpAudio))
                    {
                      //  AudioManager.Instance.PlayOneShotAudio(jumpAudio); // Play the jump sound
                    }
                }
            }
        }

        // Handle crouch behavior
        /// <summary>
        /// Handles the crouch action, triggering the crouch animation when the player presses the crouch button.
        /// </summary>
        private void HandleCrouch()
        {
            if (PlayerFPSInput.IsShouldCrouch)
                StartCoroutine(CrouchStand());
        }

        // Handle zoom (FOV change)
        /// <summary>
        /// Toggles the player's zoom state by changing the field of view (FOV) when the zoom key is pressed or released.
        /// </summary>
        private void HandleZoom()
        {
            if (Input.GetKeyDown(PlayerFPSInput.ZoomKey))
            {
                if (_zoomRoutine != null)
                {
                    StopCoroutine(_zoomRoutine);
                }
                _zoomRoutine = StartCoroutine(ToggleZoom(true));
            }
            if (Input.GetKeyUp(PlayerFPSInput.ZoomKey))
            {
                if (_zoomRoutine != null)
                {
                    StopCoroutine(_zoomRoutine);
                }
                _zoomRoutine = StartCoroutine(ToggleZoom(false));
            }
        }

        // Handle headbob for walking, sprinting, crouching
        /// <summary>
        /// Applies headbob effect based on movement type (walking, sprinting, crouching) to simulate natural camera movement.
        /// </summary>
        private void HandleHeadbob()
        {
            if (!_characterController.isGrounded) return;

            if (Mathf.Abs(_currentInput.x) > 0.1f || Mathf.Abs(_currentInput.y) > 0.1f)
            {
                _timer += Time.deltaTime * (_isCrouching ? _crouchBobSpeed : PlayerFPSInput.IsSprinting ? _sprintBobSpeed : _walkBobSpeed);
                _playerCamera.transform.localPosition = new Vector3(_playerCamera.transform.localPosition.x,
                    _defaultYPos + Mathf.Sin(_timer) * (_isCrouching ? _crouchBobAmount : PlayerFPSInput.IsSprinting ? _sprintBobAmount : _walkBobAmount),
                    _playerCamera.transform.localPosition.z);
            }
        }

        // Handle footstep sounds based on surface type
        /// <summary>
        /// Plays footstep sounds based on the surface type underfoot (wood, metal, grass) and whether the player is sprinting.
        /// </summary>
        private void HandleFootSteps()
        {
            if (!_characterController.isGrounded || _currentInput == Vector2.zero) return;

            _footstepTimer -= Time.deltaTime;

            if (_footstepTimer <= 0)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 5))
                {
                    string footstepAudio = PlayerFPSInput.IsSprinting ? GetFootstepAudioNameSprint(hit.collider.tag) : GetFootstepAudioName(hit.collider.tag);
                    if (!string.IsNullOrEmpty(footstepAudio))
                    {
                       // AudioManager.Instance.PlayOneShotAudio(footstepAudio);
                    }
                }

                _footstepTimer = GetCurrentOffset();
            }
        }

        /// <summary>
        /// Returns the appropriate footstep audio based on the surface tag for normal movement.
        /// </summary>
        private string GetFootstepAudioName(string surfaceTag)
        {
            switch (surfaceTag)
            {
                case "FootSteps/WOOD": return _woodAudioName;
                case "FootSteps/METAL": return _metalAudioName;
                case "FootSteps/GRASS": return _grassAudioName;
                default: return null;
            }
        }

        /// <summary>
        /// Returns the appropriate footstep audio based on the surface tag when sprinting.
        /// </summary>
        private string GetFootstepAudioNameSprint(string surfaceTag)
        {
            switch (surfaceTag)
            {
                case "FootSteps/WOOD": return _woodAudioNameSprint;
                case "FootSteps/METAL": return _metalAudioNameSprint;
                case "FootSteps/GRASS": return _grassAudioNameSprint;
                default: return null;
            }
        }

        /// <summary>
        /// Returns the appropriate footstep audio for jumping based on the surface tag.
        /// </summary>
        private string GetFootstepAudioNameJump(string surfaceTag)
        {
            switch (surfaceTag)
            {
                case "FootSteps/WOOD": return _woodAudioNameJump;
                case "FootSteps/METAL": return _metalAudioNameJump;
                case "FootSteps/GRASS": return _grassAudioNameJump;
                default: return null;
            }
        }

        /// <summary>
        /// Returns the current footstep offset based on whether the player is crouching or sprinting.
        /// </summary>
        private float GetCurrentOffset()
        {
            return _isCrouching ? _baseStepSpeed * _crouchStepMultipler : PlayerFPSInput.IsSprinting ? _baseStepSpeed * _sprintStepMultipler : _baseStepSpeed;
        }

        // Apply final movement (gravity and slopes)
        /// <summary>
        /// Applies gravity and handles movement on slopes by adjusting the player's position accordingly.
        /// </summary>
        private void ApplyFinalMovements()
        {
            if (!_characterController.isGrounded)
                _moveDirection.y -= _gravity * Time.deltaTime;

            if (_willSlideOnSlopes && IsSliding)
                _moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * _slopeSpeed;
            _characterController.Move(_moveDirection * Time.deltaTime);
        }

        // Crouch stand coroutine (animation)
        /// <summary>
        /// Coroutine that handles the crouch and stand animations. It smoothly transitions the player's height and center position.
        /// </summary>
        private IEnumerator CrouchStand()
        {
            if (_isCrouching && Physics.Raycast(_playerCamera.transform.position, Vector3.up, 1f))
                yield break;

            _duringCrouchAnimation = true;

            float timeElapsed = 0;
            float targetHeight = _isCrouching ? _standingHeight : _crouchHeight;
            float currentHeight = _characterController.height;
            Vector3 targetCenter = _isCrouching ? _standingCenter : _crouchingCenter;
            Vector3 currentCenter = _characterController.center;

            while (timeElapsed < _timeToCrouch)
            {
                _characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / _timeToCrouch);
                _characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / _timeToCrouch);
                timeElapsed += Time.deltaTime;

                yield return null;
            }
            _characterController.height = targetHeight;
            _characterController.center = targetCenter;

            _isCrouching = !_isCrouching;

            _duringCrouchAnimation = false;
        }

        /// <summary>
        /// Coroutine that smoothly changes the field of view (FOV) when zooming in or out.
        /// </summary>
        private IEnumerator ToggleZoom(bool isEnter)
        {
            float targetFOV = isEnter ? _zoomFOV : _defaultFOV;
            float startingFOV = _playerCamera.fieldOfView;
            float timeElapsed = 0;

            while (timeElapsed < _timeToZoom)
            {
                _playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / _timeToZoom);
                timeElapsed += Time.deltaTime;
                yield return null;

                _playerCamera.fieldOfView = targetFOV;
                _zoomRoutine = null;
            }
        }

        /// <summary>
        /// Handles stamina usage when sprinting, decreasing stamina when the player is moving and regenerating it when idle.
        /// </summary>
        private void HandleStamina()
        {
            if (PlayerFPSInput.IsSprinting && _currentInput != Vector2.zero)
            {
                if (_currentStamina > 0)
                {
                    _currentStamina -= _staminaUseMultiplier * Time.deltaTime;

                    if (_currentStamina < 0)
                        _currentStamina = 0;

                    OnStaminaChange?.Invoke(_currentStamina);
                }

                if (_currentStamina <= 0)
                {
                    _canSprint = false; // Disable sprinting when stamina is empty
                }
            }

            if (!PlayerFPSInput.IsSprinting && _currentStamina < _maxStamina && _regeneratingStamina == null)
            {
                _regeneratingStamina = StartCoroutine(RegenerateStamina());
            }
        }

        /// <summary>
        /// Coroutine to regenerate stamina over time when the player is not sprinting.
        /// </summary>
        private IEnumerator RegenerateStamina()
        {
            yield return new WaitForSeconds(_timeBeforeStaminaRegenStarts);

            WaitForSeconds timeToWait = new WaitForSeconds(_staminaTimeIncrement);

            while (_currentStamina < _maxStamina)
            {
                if (_currentStamina > 0)
                    _canSprint = true;

                _currentStamina += _staminaValueIncrement;

                if (_currentStamina > _maxStamina)
                    _currentStamina = _maxStamina;

                yield return timeToWait;
            }

            _regeneratingStamina = null;
        }
    }
}