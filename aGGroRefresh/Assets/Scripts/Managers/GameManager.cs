using System;
using System.Collections;
using System.Collections.Generic;
using AbilityPack.Enum;
using Enum;
using Managers;
using Managers.Enums;
using Other;
using PlayerPack.PlayerMovementPack;
using PlayerPack.PlayerOngoingStatsPack;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    public static GameManager Instance { get; private set; }

    private PlayerOptions _playerOptions = null;

    public PlayerMovement PlayerMovementScript { get; private set; }
    public EGameState GameState { get; private set; }
    public bool IsPaused { get; private set; }
    public GameObject PlayerObject { get; private set; }
    public Vector3 PlayerPosition => PlayerObject.transform.position;

    private void Awake()
    {
        if (Instance != this && Instance != null) Destroy(gameObject);
        else Instance = this;
        
        StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
            Time.timeScale = IsPaused ? 0 : 1;
        }
    }

    private void UnPause()
    {
        IsPaused = false;
    }

    private void StartGame()
    {
        var spawnedPlayer = Instantiate(playerPrefab, Vector2.zero, Quaternion.identity);
        PlayerObject = spawnedPlayer;
        PlayerMovementScript = spawnedPlayer.GetComponent<PlayerMovement>();
        
        StartArena(true);
    }

    public void StartArena(bool startedGame = false)
    {
        GameState = EGameState.Arena;
    }

    private void EnterWaitingRoom()
    {
        
    }

    public PlayerOptions GetPlayerOptions()
    {
        return _playerOptions ??= new PlayerOptions();
    }
    
}