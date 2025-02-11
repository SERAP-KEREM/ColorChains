using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FinalPlug : MonoBehaviour
{
    public GameObject CurrentSocket;
    public string SocketColor;

    private bool _isSelected;
    private bool _positionChanged;
    private bool _isSocketOccupied;

    private GameObject _movementPosition;
    private GameObject SocketItself;


    public void Move(string action, GameObject socket, GameObject targetObject = null)
    {
        switch (action)
        {
            case "Select":
                _movementPosition = targetObject;
                gameObject.transform.DOLocalRotate(new Vector3(0, 0,0), 1f).SetEase(Ease.InOutSine);
                _isSelected = true;
                break;

            case "ChangePosition":
                SocketItself = socket;
                _movementPosition = targetObject;
                gameObject.transform.DOLocalRotate(new Vector3(0, 90, -90), 1f).SetEase(Ease.InOutSine);
              
                _positionChanged = true;
                break;

            case "SitOnSocket":
                SocketItself = socket;
                _isSocketOccupied = true;
                gameObject.transform.DOLocalRotate(new Vector3(0, 90, -90), 1f).SetEase(Ease.InOutSine);
                break;

            default:
                Debug.LogWarning("Invalid action specified.");
                break;
        }
    }

    private void Update()
    {
        if (_isSelected)
        {
            // Smoothly move the object towards the target position
            transform.position = Vector3.Lerp(transform.position, _movementPosition.transform.position, 0.04f);

            // Check if the object is close enough to the target position
            if (Vector3.Distance(transform.position, _movementPosition.transform.position) < 0.01f)
            {
                _isSelected = false; // Deselect the object once it reaches the target
            }
        }
        if (_positionChanged)
        {
            // Smoothly move the object towards the target position
            transform.position = Vector3.Lerp(transform.position, _movementPosition.transform.position, 0.04f);

            // Check if the object is close enough to the target position
            if (Vector3.Distance(transform.position, _movementPosition.transform.position) < 0.01f)
            {
                _positionChanged = false; // Deselect the object once it reaches the target
                _isSocketOccupied = true;
            }
        }
        if (_isSocketOccupied)
        {
            // Smoothly move the object towards the target position
            transform.position = Vector3.Lerp(transform.position, SocketItself.transform.position, 0.04f);

            // Check if the object is close enough to the target position
            if (Vector3.Distance(transform.position, SocketItself.transform.position) < 0.01f)
            {
                _isSocketOccupied = false;
                GameManager.Instance.IsMovement = false;
                CurrentSocket = SocketItself;
                GameManager.Instance.CheckPlugs();
            }
        }
    }
}
