using _Main._Managers;
using DG.Tweening;
using TriInspector;
using UnityEngine;

namespace _Main._Mechanics
{
    /// <summary>
    /// FinalPlug handles the movement, rotation, and socket interaction logic for plug objects in the game.
    /// </summary>
    public class FinalPlug : MonoBehaviour
    {
        #region Inspector Groups

        [Header("Socket Settings")]
        [Group("Socket Settings/Current Socket")]
        [SerializeField, Tooltip("The current socket this plug is connected to.")]
        public GameObject CurrentSocket;

        [Group("Socket Settings/Color")]
        [SerializeField, Tooltip("The color of the socket this plug belongs to.")]
        public string SocketColor;

        [Header("Movement Settings")]
        [Group("Movement Settings/Rotations")]
        [SerializeField, Tooltip("Default rotation for the plug when selected.")]
        private Vector3 DefaultRotation = new Vector3(0, 0, 0);

        [Group("Movement Settings/Rotations")]
        [SerializeField, Tooltip("Changed rotation for the plug when moved or placed.")]
        private Vector3 ChangedRotation = new Vector3(0, 90, -90);

        [Group("Movement Settings/Speed")]
        [SerializeField, Tooltip("Speed for smooth movement.")]
        private float MoveSpeed = 0.04f;

        [Group("Movement Settings/Easing")]
        [SerializeField, Tooltip("Easing type for animations.")]
        private Ease defaultEase = Ease.InOutSine;

        #endregion

        #region Private Variables

        private bool _isSelected;
        private bool _positionChanged;
        private bool _isSocketOccupied;
        private GameObject _movementPosition;
        private GameObject _socketItself;

        private const string SelectAction = "Select";
        private const string ChangePositionAction = "ChangePosition";
        private const string SitOnSocketAction = "SitOnSocket";

        #endregion

        #region Public Methods

        /// <summary>
        /// Moves the plug based on the specified action.
        /// </summary>
        /// <param name="action">The action to perform ("Select", "ChangePosition", "SitOnSocket").</param>
        /// <param name="socket">The target socket.</param>
        /// <param name="targetObject">The target position (optional for some actions).</param>
        public void Move(string action, GameObject socket, GameObject targetObject = null)
        {
            if (socket == null || targetObject == null && action != SitOnSocketAction)
            {
                Debug.LogWarning("Missing required parameters for the specified action.");
                return;
            }

            switch (action)
            {
                case SelectAction:
                    HandleSelectAction(targetObject);
                    break;
                case ChangePositionAction:
                    HandleChangePositionAction(socket, targetObject);
                    break;
                case SitOnSocketAction:
                    HandleSitOnSocketAction(socket);
                    break;
                default:
                    Debug.LogWarning("Invalid action specified.");
                    break;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the selection of the object and applies the default rotation.
        /// </summary>
        private void HandleSelectAction(GameObject targetObject)
        {
            _movementPosition = targetObject;
            transform.DOLocalRotate(DefaultRotation, 1f).SetEase(defaultEase);
            _isSelected = true;
        }

        /// <summary>
        /// Handles changing the object's position and applies the changed rotation.
        /// </summary>
        private void HandleChangePositionAction(GameObject socket, GameObject targetObject)
        {
            _socketItself = socket;
            _movementPosition = targetObject;
            transform.DOLocalRotate(ChangedRotation, 1f).SetEase(defaultEase);
            _positionChanged = true;
        }

        /// <summary>
        /// Handles the object sitting on the socket and applies the changed rotation.
        /// </summary>
        private void HandleSitOnSocketAction(GameObject socket)
        {
            _socketItself = socket;
            _isSocketOccupied = true;
            transform.DOLocalRotate(ChangedRotation, 1f).SetEase(defaultEase);
        }

        /// <summary>
        /// Smoothly moves the object towards a target position.
        /// </summary>
        private void SmoothMoveToTarget(Vector3 targetPosition, ref bool actionFlag)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, MoveSpeed);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                actionFlag = false;
            }
        }

        /// <summary>
        /// Updates the object's position and manages movement logic.
        /// </summary>
        private void Update()
        {
            if (_isSelected)
            {
                SmoothMoveToTarget(_movementPosition.transform.position, ref _isSelected);
            }

            if (_positionChanged)
            {
                SmoothMoveToTarget(_movementPosition.transform.position, ref _positionChanged);
                if (!_positionChanged)
                {
                    _isSocketOccupied = true;
                }
            }

            if (_isSocketOccupied)
            {
                SmoothMoveToTarget(_socketItself.transform.position, ref _isSocketOccupied);
                if (!_isSocketOccupied)
                {
                    GameManager.Instance.IsMovement = false;
                    CurrentSocket = _socketItself;
                    GameManager.Instance.CheckPlugs();
                }
            }
        }

        #endregion
    }
}