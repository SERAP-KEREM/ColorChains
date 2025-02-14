using _Main._Managers;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace _Main._Mechanics
{
    /// <summary>
    /// CollisionControl manages collision detection for trigger zones and updates the GameManager accordingly.
    /// </summary>
    public class CollisionControl : MonoBehaviour
    {
        #region Inspector Groups

        [Header("Collision Settings")]
        [Group("Collision Settings/Index")]
        [SerializeField, Tooltip("The index of this collision state used in the GameManager.")]
        private int _collisionIndex;

        [Group("Collision Settings/State")]
        [SerializeField, Tooltip("The current state of this collision (whether it's in collision or not).")]
        private bool _isColliding;

        #endregion

        #region Private Variables

        /// <summary>
        /// A count of the number of CablePieces currently colliding with this trigger.
        /// </summary>
        private int _collisionCount;

        #endregion

        #region Unity Events

        /// <summary>
        /// Called when another collider enters the trigger collider attached to this object.
        /// Checks if the object is a CablePiece and updates the collision state.
        /// </summary>
        /// <param name="other">The collider that entered the trigger.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("CablePiece"))
            {
                _collisionCount++;
                UpdateCollisionState(true);
            }
        }

        /// <summary>
        /// Called when another collider exits the trigger collider attached to this object.
        /// Checks if the object is a CablePiece and updates the collision state.
        /// </summary>
        /// <param name="other">The collider that exited the trigger.</param>
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("CablePiece"))
            {
                _collisionCount--;
                _collisionCount = Mathf.Max(_collisionCount, 0); // Ensure collision count doesn't go negative.
                UpdateCollisionState(_collisionCount > 0);
            }
        }

        #endregion

        #region Collision State Management

        /// <summary>
        /// Updates the collision state and informs the GameManager.
        /// </summary>
        /// <param name="state">True if collision is detected, false otherwise.</param>
        private void UpdateCollisionState(bool state)
        {
            _isColliding = state;

            // Check if GameManager.Instance is available before accessing it
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CheckCollision(_collisionIndex, _isColliding);
            }
            else
            {
                Debug.LogError("GameManager.Instance is null! Ensure GameManager is properly initialized.");
            }
        }

        #endregion
    }
}