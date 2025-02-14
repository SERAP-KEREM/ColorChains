using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TriInspector;
using UnityEngine;

namespace _Main._Mechanics
{
    /// <summary>
    /// Handcuff handles the animation and behavior of opening a handcuff.
    /// </summary>
    public class Handcuff : MonoBehaviour
    {
        #region Inspector Groups

        [Header("Handcuff Settings")]
        [Group("Handcuff Settings/Rotation")]
        [SerializeField, Tooltip("The GameObject representing the rotating part of the handcuff.")]
        private GameObject _rotationPoint;

        [Group("Handcuff Settings/Animation")]
        [SerializeField, Tooltip("The angle (in degrees) to rotate for opening the handcuff.")]
        private float _openRotationAngle = -90f;

        [Group("Handcuff Settings/Animation")]
        [SerializeField, Tooltip("The duration (in seconds) of the rotation animation.")]
        private float _rotationDuration = 1f;

        #endregion

        #region Private Constants

        private const Ease DefaultEase = Ease.InOutSine;

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens the handcuff by smoothly rotating the rotating part around the Y-axis.
        /// </summary>
        public void OpenHandcuff()
        {
            if (_rotationPoint == null)
            {
                Debug.LogError("Rotation point is not assigned! Please assign it in the Inspector.");
                return;
            }

            // Smoothly rotate the handcuff's rotating part
            _rotationPoint.transform.DOLocalRotate(
                new Vector3(0, _openRotationAngle, 0),
                _rotationDuration
            ).SetEase(DefaultEase);
        }

        #endregion
    }
}