using System;
using System.Collections;
using System.Collections.Generic;
using AbilityPack.Enum;
using Enum;
using ItemPack;
using ItemPack.ScriptableObjects;
using Managers;
using Managers.Enums;
using Other;
using PlayerPack.PlayerMovementPack;
using PlayerPack.PlayerOngoingStatsPack;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject itemPrefab;
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

    public ItemPrefab SpawnItem(SoItem item, Vector3? position = null)
    {
        var itemInstance = Instantiate(itemPrefab, UtilsMethods.GetMousePosition(), Quaternion.identity);
        var itemScript = itemInstance.GetComponent<ItemPrefab>();
        itemScript.Setup(item, position);
        return itemScript;
    }
    
    public ItemPrefab SpawnItem(SoItem item, Vector3 initialPos, float range)
    {
        var itemInstance = Instantiate(itemPrefab, initialPos, Quaternion.identity);
        var itemScript = itemInstance.GetComponent<ItemPrefab>();
        itemScript.Setup(item, GetRandomPos(initialPos, range));
        return itemScript;
    }

    private static Vector3 GetRandomPos(Vector2 initialPos, float range)
    {
        var randomAngleRad = Mathf.Deg2Rad * Random.Range(0, 360);
        var normalizedRotationPos = new Vector2(Mathf.Cos(randomAngleRad), Mathf.Sin(randomAngleRad));
        return initialPos + normalizedRotationPos * range;
    }

    public PlayerOptions GetPlayerOptions()
    {
        return _playerOptions ??= new PlayerOptions();
    }
    
}