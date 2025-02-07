using SerapKeremGameTools._Game._FPSPlayerSystem;
using UnityEngine;

namespace SerapKeremGameTools._Game._FPSPlayerSystem
{
    /// <summary>
    /// A test interactable object that changes color and logs messages when focused and interacted with.
    /// Inherits from the Interactable base class.
    /// </summary>
    public class TestInteractableObject : Interactable
    {
        [Header("Test Interactable Object")]

        [Tooltip("Message shown when the object is focused.")]
        [SerializeField] private string _messageOnFocus = "Focused!";

        [Tooltip("Message shown when the object is interacted with.")]
        [SerializeField] private string _messageOnInteract = "Interaction Started!";

        [Tooltip("Color of the object when focused.")]
        [SerializeField] private Color _focusColor = Color.yellow;

        private Renderer _objectRenderer;

        private void Start()
        {
            _objectRenderer = GetComponent<Renderer>();

            // Set the object layer to an interactable layer (7).
            gameObject.layer = 7;
        }

        /// <summary>
        /// Called when the player interacts with the object.
        /// Changes the color of the object and logs a message.
        /// </summary>
        public override void OnInteract()
        {
            // Perform something during interaction
#if UNITY_EDITOR
            Debug.Log(_messageOnInteract);
#endif

            // Change the object's color to green upon interaction
            _objectRenderer.material.color = Color.green;
        }

        /// <summary>
        /// Called when the player focuses on the object.
        /// Changes the color of the object and logs a message.
        /// </summary>
        public override void OnFocus()
        {
            // Perform something when focused
#if UNITY_EDITOR
            Debug.Log(_messageOnFocus);
#endif

            // Change the object's color to the focus color when focused
            _objectRenderer.material.color = _focusColor;
        }

        /// <summary>
        /// Called when the player loses focus on the object.
        /// Changes the object's color back to white and logs a message.
        /// </summary>
        public override void OnLoseFocus()
        {
            // Perform something when focus is lost
#if UNITY_EDITOR
            Debug.Log("Focus lost.");
#endif

            // Reset the object's color back to white when focus is lost
            _objectRenderer.material.color = Color.white;
        }
    }
}
