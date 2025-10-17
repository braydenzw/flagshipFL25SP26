using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;



public class PlayerTimeTravel : MonoBehaviour
{
    private LinkedList<PlayerState> _playerStates; //FIFO
    private int _playerStatesMaxLength;
    [SerializeField] private float _maxTimeTravel = 3;
    public float _travelLocationInSeconds { get; private set; }
    [SerializeField] private float _travelLocation;
    private float _travelTimeIncrement;
    private float _fixedUpdateRate = 100;
    [SerializeField] private Transform _tempTransform;
    [SerializeField] private GameObject _timeGhost;

    
    void Start()
    {
        // setting how often fixed update is called per second
        Time.fixedDeltaTime = 1/_fixedUpdateRate;
        _playerStatesMaxLength = Mathf.FloorToInt(_fixedUpdateRate * _maxTimeTravel);
        _playerStates = new LinkedList<PlayerState>();
        _travelLocation = Mathf.Round(_travelLocation * _fixedUpdateRate) / _fixedUpdateRate;
        UpdatePlayerStates();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePlayerStates();
    }
    void Update()
    {

        UpdateMaxPlayerStatesLength();
        _tempTransform.position += Vector3.right * Time.deltaTime * 1f;
        VisualizePast();
        //LogMostRecentPlayerState();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Travel(_travelLocation, _tempTransform, true);
        }
    }

    void LogMostRecentPlayerState()
    {
        Vector3 playerPos = _playerStates.ElementAt(-1).Pos;
        Debug.Log("X: " + playerPos.x + ", Y: " + playerPos.y + ", Z: " + playerPos.z);
    }

    void LogPlayerState(int idx)
    {
        //Vector3 playerPos = _playerStates[idx].Pos;
        //Debug.Log("X: " + playerPos.x + ", Y: " + playerPos.y + ", Z: " + playerPos.z);
    }

    void LogWholePlayerState()
    {
        for (int i = 0; i < _playerStates.Count; i++)
        {
            Debug.Log("Index " + i);
            //LogPlayerState(i);
            
        }
    }

    void UpdatePlayerStates()
    {
        PlayerState playerState = GetPlayerState();
        _playerStates.Enqueue(playerState);
        
        while (_playerStates.Count > _playerStatesMaxLength)
        {
            _playerStates.Dequeue();
        }
    }

    PlayerState GetPlayerState()
    {
        PlayerState playerState = new PlayerState();
        playerState.Inst(_tempTransform);
        return playerState;
    }

    void UpdateMaxPlayerStatesLength()
    {
        _playerStatesMaxLength = Mathf.FloorToInt(_fixedUpdateRate * _maxTimeTravel);
    }

    void Travel(float travelTime, Transform target, bool clear = false)
    {
        if (travelTime == 0) return;
        int idx = Mathf.FloorToInt(travelTime * _fixedUpdateRate);
        idx = SetMaxTravelTime(idx);
        if(_playerStates.Count != 0) target.position = _playerStates.ElementAt(_playerStates.Count - idx - 1).Pos;
        if (clear)
        {
            _playerStates.ClearAfterPoint(_playerStates.Count - idx);
        }
    }

    int SetMaxTravelTime(int travelTime)
    {
        if(travelTime > _playerStates.Count - 1) travelTime = _playerStates.Count - 1;
        return travelTime;
    }

    void VisualizePast()
    {
        Travel(_travelLocation, _timeGhost.transform);
    }
    
}
