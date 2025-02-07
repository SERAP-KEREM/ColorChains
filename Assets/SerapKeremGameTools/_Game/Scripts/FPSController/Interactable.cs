using System.Collections.Generic;
using UnityEngine;

namespace SerapKeremGameTools._Game._FPSPlayerSystem
{
    /// <summary>
    /// Abstract class that defines interactable objects in the game.
    /// Any object that can be interacted with should inherit from this class.
    /// </summary>
    public abstract class Interactable : MonoBehaviour
    {
        private void Awake()
        {
            // Set the object layer to interactable (Layer 9).
            gameObject.layer = 9;
        }

        /// <summary>
        /// Method to be implemented for interaction with the object.
        /// Called when the player interacts with the object.
        /// </summary>
        public abstract void OnInteract();

        /// <summary>
        /// Method to be implemented for focus behavior of the object.
        /// Called when the player focuses on the object.
        /// </summary>
        public abstract void OnFocus();

        /// <summary>
        /// Method to be implemented for lose focus behavior of the object.
        /// Called when the player loses focus from the object.
        /// </summary>
        public abstract void OnLoseFocus();
    }
}
