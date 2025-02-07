using SerapKeremGameTools._Game._InputSystem;
using SerapKeremGameTools._Game._Singleton;
using UnityEngine;

namespace SerapKeremGameTools._Game._FPSPlayerSystem
{
    /// <summary>
    /// Manages interaction between the player and interactable objects in the game.
    /// Handles focus, interaction detection, and interaction execution.
    /// </summary>
    public class FPSInteractionSystem : MonoSingleton<FPSInteractionSystem>
    {
        [Header("Interaction Settings")]

        [Tooltip("Maximum distance at which the player can interact with an object.")]
        [SerializeField] private float _interactionDistance = 3f;

        [Tooltip("Layer mask to specify which objects are interactable.")]
        [SerializeField] private LayerMask _interactionLayer;

        private Interactable _currentInteractable;
        private Camera _playerCamera;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            // Get the main camera for the player
            _playerCamera = Camera.main;
        }

        private void Update()
        {
            HandleFocus();

            // Check if the interact button was pressed
            if (Input.GetKeyDown(PlayerFPSInput.InteractKey))
            {
                TryInteract();
            }
        }

        /// <summary>
        /// Handles the logic for focusing on interactable objects.
        /// Checks if the player is looking at an interactable object within range.
        /// </summary>
        private void HandleFocus()
        {
            RaycastHit hit;

            // Cast a ray to detect interactable objects
            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, _interactionDistance, _interactionLayer))
            {
                Interactable interactableObject = hit.collider.GetComponent<Interactable>();

                // If the object is interactable and not already focused
                if (interactableObject != null && interactableObject != _currentInteractable)
                {
                    _currentInteractable?.OnLoseFocus();
                    _currentInteractable = interactableObject;
                    _currentInteractable.OnFocus();
                }
            }
            else if (_currentInteractable != null)
            {
                // If no interactable object is in focus, lose focus
                _currentInteractable.OnLoseFocus();
                _currentInteractable = null;
            }
        }

        /// <summary>
        /// Tries to interact with the current focused object.
        /// Executes the interaction if there is an interactable object in focus.
        /// </summary>
        public void TryInteract()
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.OnInteract();
            }
        }
    }
}
