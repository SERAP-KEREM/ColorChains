using System.Collections;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private ParticleSystem _breakEffect; // Kopma efekti
    private bool _hasFailed = false; // Fail durumunu kontrol etmek için flag

    private void Start()
    {
        Invoke(nameof(BreakingPoint), 2f);
    }

    private void BreakingPoint()
    {
        var hingeJoint = GetComponent<HingeJoint>();
        if (hingeJoint != null)
        {
            hingeJoint.breakForce = 5400;
            hingeJoint.breakTorque = 5200;
        }
        else
        {
            Debug.LogError("HingeJoint not found!");
        }
    }

    private void Update()
    {
        // Eğer HingeJoint yok olduysa ve daha önce fail durumu tetiklenmediyse
        if (GetComponent<HingeJoint>() == null && !_hasFailed)
        {
            _hasFailed = true; // Fail durumu aktif edildi
            PlayBreakEffect(); // Parçacık efektini tetikle
            Invoke(nameof(LevelFailed), 2f); // 2 saniye sonra LevelFailed çağır
        }
    }

    private void PlayBreakEffect()
    {
        if (_breakEffect != null)
        {
            ParticleSystem effect = Instantiate(_breakEffect, transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, 2f); // 2 saniye sonra efekt nesnesini yok et
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
            Debug.LogError("GameManager instance is null");
        }
    }
}
