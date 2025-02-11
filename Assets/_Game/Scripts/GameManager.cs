using SerapKeremGameTools._Game._Singleton;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    GameObject _currentObject;
    GameObject _currentSocket;
    public bool IsMovement;
    [SerializeField] private List<Handcuff> _handcuffs = new List<Handcuff>();


    [Header("Level Settings")]
    public GameObject[] CollisionCheckObjects;
    public GameObject[] Plugs;
    public int TargetSocketCount;
    public List<bool> CollisionStates;
    int _completionCount;
    FinalPlug _finalPlug;
    int CollisionCheckCount;

    [Header("UI Objects")]
    [SerializeField] private GameObject _controlPanel;
    [SerializeField] private TextMeshProUGUI _controlText;
    [SerializeField] private TextMeshProUGUI _moveCountText;
    [SerializeField] private int _moveCount;

    protected override void Awake()
    {
        base.Awake();
        
    }
    void Start()
    {
        _moveCountText.text=_moveCount.ToString();
        for (int i = 0; i < TargetSocketCount-1; i++)
        {
            CollisionStates.Add(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            // Cast a ray from the camera to the mouse position
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100))
            {
                if (hit.collider != null) // Ensure the ray hit a collider
                {
                    //# BOTTOM PLUG
                    if (_currentObject == null && !IsMovement)
                    {
                        // Check if the collider has one of the specified tags
                        if (hit.collider.CompareTag("BluePlug") ||
                            hit.collider.CompareTag("GreenPlug") ||
                            hit.collider.CompareTag("RedPlug"))
                        {
                            _finalPlug = hit.collider.GetComponent<FinalPlug>();

                            _finalPlug.Move("Select", _finalPlug.CurrentSocket,
                                _finalPlug.CurrentSocket.GetComponent<Socket>().MovementPosition);

                            _currentObject = hit.collider.gameObject;
                            _currentSocket = _finalPlug.CurrentSocket;
                            IsMovement = true;

                        }

                    }
                    //# BOTTOM PLUG
                    //# SOCKET
                    if (hit.collider.CompareTag("Socket"))
                    {
                        if (_currentObject != null && !hit.collider.GetComponent<Socket>().IsEmpty &&
                          _currentSocket != hit.collider.gameObject)
                        {
                            _currentSocket.GetComponent<Socket>().IsEmpty = false;
                            Socket _socket = hit.collider.GetComponent<Socket>();

                            _finalPlug.Move("ChangePosition",hit.collider.gameObject,_socket.MovementPosition);

                            _socket.IsEmpty = true;

                            _currentObject = null;
                            _currentSocket = null;
                            IsMovement = true;
                            _moveCount--;
                            _moveCountText.text=_moveCount.ToString();
                        }
                        else if (_currentSocket == hit.collider.gameObject)
                        {
                            _finalPlug.Move("SitOnSocket", hit.collider.gameObject);

                            _currentObject = null;
                            _currentSocket = null;
                            IsMovement = true;
                        }
                    }

                    //# SOCKET

                }

            }
        }
    }

    public void CheckPlugs()
    {
        foreach (var plug in Plugs)
        {
            if (plug.GetComponent<FinalPlug>().CurrentSocket.name == plug.GetComponent<FinalPlug>().SocketColor)
            {
                _completionCount++;
            }
        }
        if (_completionCount == TargetSocketCount)
        {
            Debug.Log("All sockets are in place");

            foreach (var obj in CollisionCheckObjects)
            {
                obj.SetActive(true);
            }
            StartCoroutine(CheckForCollision());
        }
        else
        {
            if (_moveCount <= 0)
            {
                Debug.Log("Lose");
            }
            Debug.Log("Matching not completed");
          
        }
        _completionCount = 0;
    }

    public void CheckCollision(int collisionIndex, bool status)
    {
        CollisionStates[collisionIndex] = status;

    }


    private IEnumerator CheckForCollision()
    {
        Debug.Log("CHECKING...");
        yield return new WaitForSeconds(4f);

       

        foreach (var item in CollisionStates)
        {
            if (!item)
            {
                CollisionCheckCount++;
            }
        }

        if (CollisionCheckCount == CollisionStates.Count)
        {
            Debug.Log("YOU WIN");
            foreach (Handcuff handcuff in _handcuffs)
            {
                handcuff.OpenHandcuff();
            }
            _controlText.text= "YOU WIN";
        }
        else
        {
            Debug.Log("Collision");
            _controlText.text = "THEWRE IS A COLLISION";
          //  Invoke(_controlPanel.SetActive(false), 2f);
            foreach (var obj in CollisionCheckObjects)
            {
                obj.SetActive(false);
            }
            if (_moveCount <= 0)
            {
                Debug.Log("Lose");
            }
        }

    }

}
