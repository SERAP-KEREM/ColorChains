using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionControl : MonoBehaviour
{
    public int CollisionIndex;
    [SerializeField] private bool _state;

    
    private int _collisionCount = 0; // Aktif �arp??ma say?s?n? takip etmek i�in

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CablePiece"))
        {
            _collisionCount++;
            GameManager.Instance.CheckCollision(CollisionIndex, true);
            _state = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CablePiece"))
        {
            _collisionCount--;
            if (_collisionCount <= 0)
            {
                _collisionCount = 0; // Negatif de?erleri �nle
                GameManager.Instance.CheckCollision(CollisionIndex, false);
                _state = false;
            }
        }
    }
}
