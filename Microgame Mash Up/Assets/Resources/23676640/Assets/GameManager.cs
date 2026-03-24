using System;
using TMPro;
using UnityEngine;

public class GameManager : GAWGameManager
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if (_instance == null)
                {
                    Debug.LogError("Game Manager has not been assgiend");
                }
            }
            return _instance;
        }
    }

    TileManager tileManager;
    InputManager inputManager;
    PlayerManager playerManager;

    MicroGBase activeGame;

    [SerializeField] private GameData gameData; // data for sprites etc

    public GameData GetGameData => gameData;
    public bool HasStarted // global check if game started
    {
        get
        {
            return GameMaster.GetGameState() == Game.State.RUNNING;
        }
    }
    public bool HasEnded
    {
        get
        {
            return (GameMaster.GetGameState() == Game.State.SUCCESS) || (GameMaster.GetGameState() == Game.State.FAILED);
        }
    }
    public override void OnGameLoad()
    {
        tileManager = TileManager.Instance;
        inputManager = InputManager.Instance;
        playerManager = PlayerManager.Instance;

        activeGame = MicroGBase.Instance;

        if (activeGame)
        {
            activeGame.OnMGAwake(this);
        }

        Debug.Log("--- Passed Game Load ---");
    }

    public override void OnGameStart()
    {
        tileManager.OnStart();
        inputManager.OnStart();
        playerManager.OnStart();

        Vector3 startPos = tileManager.GetPlayerSpawnPos;

        playerManager.SetStartPos(startPos);

        int difficulty = (int)GameMaster.GetDifficulty();
        
        if (activeGame)
        {
            activeGame.OnMGStart(GameMaster.GetTotalTime(),difficulty);
        }

        Debug.Log("--- Passed Game Start ---");
        SoundManager.PlayMusic(GetGameData.GetMainMusic);
        
    }

    private void Update()
    {
        OnGameUpdate();

    }

    private void OnGameUpdate()
    {
        if (!HasStarted || HasEnded) return;
        tileManager.OnUpdate();
        inputManager.OnUpdate();
        playerManager.OnUpdate();

        if (activeGame)
        {
            activeGame.OnMGUpdate();
        }
    }

    private void LateUpdate()
    {
        OnGameLateUpdate();
    }
    private void OnGameLateUpdate()
    {
        inputManager.OnLateUpdate();
    }
    public override void OnGameSucceeded() 
    {
        OnGameEnd(activeGame.OnMGWin);
    }

    public override void OnGameFailed()
    {
        
        OnGameEnd(activeGame.OnMGLose);
    }

    private void OnGameEnd(Action outcomeAction)
    {
        SoundManager.StopMusic();
        activeGame.OnMGEnd();
        outcomeAction?.Invoke();


        
        tileManager.OnDestroy();
        playerManager.OnDestroy();
        inputManager.OnDestroy();
        activeGame.OnDestroy();

    }
}
