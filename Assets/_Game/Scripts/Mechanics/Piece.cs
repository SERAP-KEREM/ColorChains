using _Main._Managers;
using System.Collections;
using UnityEngine;
namespace _Main._Mechanics
{
    public class Piece : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem _breakEffect; // Break effect when piece fails

    private bool _hasFailed = false; // Flag to check if failure has occurred

    private HingeJoint _hingeJoint;

    // Constants for break force and torque values
    private const float BreakForce = 5400f;
    private const float BreakTorque = 5200f;

    private void Start()
    {
        // Cache the HingeJoint component at the start
        _hingeJoint = GetComponent<HingeJoint>();

        if (_hingeJoint != null)
        {
            Invoke(nameof(BreakingPoint), 2f);
        }
        else
        {
            Debug.LogError("HingeJoint not found on the object!");
        }
    }

    private void BreakingPoint()
    {
        if (_hingeJoint != null)
        {
            _hingeJoint.breakForce = BreakForce;
            _hingeJoint.breakTorque = BreakTorque;
        }
    }

    private void Update()
    {
        // If HingeJoint is destroyed and failure hasn't been triggered yet
        if (_hingeJoint == null && !_hasFailed)
        {
            _hasFailed = true; // Mark failure
            PlayBreakEffect(); // Trigger break effect
            Invoke(nameof(LevelFailed), 2f); // Level failed after 2 seconds
        }
    }

    private void PlayBreakEffect()
    {
        if (_breakEffect != null)
        {
            ParticleSystem effect = Instantiate(_breakEffect, transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, 2f); // Destroy the effect after 2 seconds
        }
        else
        {
            Debug.LogError("Break effect is not assigned in the Inspector!");
        }
    }

    private void LevelFailed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LevelFailed();
        }
        else
        {
            Debug.LogError("GameManager instance is null.");
        }
    }
    }
}
