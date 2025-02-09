using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionControl : MonoBehaviour
{
    public GameManager _gameManager;
    public int CollisionIndex;
    public bool state;

    private int _collisionCount = 0; // Aktif �arp??ma say?s?n? takip etmek i�in

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CablePiece"))
        {
            _collisionCount++;
            _gameManager.CheckCollision(CollisionIndex, true);
            state = true;
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
                _gameManager.CheckCollision(CollisionIndex, false);
                state = false;
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    // Draw a red wireframe cube to visualize the bounds
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(transform.position, transform.localScale / 2);
    //}
}
