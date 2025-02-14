using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace _Main._Mechanics
{
    /// <summary>
    /// Socket represents a socket in the game that can hold plugs and provide movement positions.
    /// </summary>
    public class Socket : MonoBehaviour
    {
        #region Inspector Groups

        [Header("Socket Settings")]
        [Group("Socket Settings/State")]
        [SerializeField, Tooltip("Indicates whether the socket is empty or occupied.")]
        private bool _isEmpty = true;

        [Group("Socket Settings/Movement")]
        [SerializeField, Tooltip("The position where objects should move when interacting with this socket.")]
        private GameObject _movementPosition;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether the socket is empty.
        /// </summary>
        public bool IsEmpty
        {
            get => _isEmpty;
            set => _isEmpty = value;
        }

        /// <summary>
        /// Gets or sets the movement position for this socket.
        /// </summary>
        public GameObject MovementPosition
        {
            get => _movementPosition;
            set => _movementPosition = value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Occupies the socket by setting it as not empty.
        /// </summary>
        public void Occupy()
        {
            IsEmpty = false;
        }

        /// <summary>
        /// Frees the socket by setting it as empty.
        /// </summary>
        public void Free()
        {
            IsEmpty = true;
        }

        #endregion
    }
}