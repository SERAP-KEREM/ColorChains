using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionControl : MonoBehaviour
{
    public GameManager _gameManager;
    public int CollisionIndex;

  

    private void Update()
    {
        // Detect colliders within the bounds of the object
        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag("CablePiece"))
            {
                _gameManager.CheckCollision(CollisionIndex, false);
            }
            else
            {
                _gameManager.CheckCollision(CollisionIndex, true);
            }
        }

    }

    private void OnDrawGizmos()
    {
        // Draw a red wireframe cube to visualize the bounds
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale / 2);
    }

}
