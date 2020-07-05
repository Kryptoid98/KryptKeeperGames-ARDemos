using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KryptKeeperGamesARDemo.Enums;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager instance;

    public GameMode[] allGameModeRef;
    [HideInInspector]public GameMode activeGameMode;
    eGameMode gameMode;

    private void Awake()
    {
        instance = this;
    }

    public void ChangeState(eGameMode newMode)
    {
        gameMode = newMode;
    }

    public void SpawnGameMode(eGameMode p_newMode)
    {
        ChangeState(p_newMode);
        if (activeGameMode != null)
            Destroy(activeGameMode);

        activeGameMode = Instantiate(allGameModeRef[(int)p_newMode], Vector3.zero, Quaternion.identity);
        activeGameMode.Init(gameObject);

        if (GameManager.gameState != eGameState.IN_GAME)
            GameManager.ChangeState(eGameState.IN_GAME);
    }

    public void DestroyActiveGameMode()
    {
        Destroy(activeGameMode.gameObject);
        activeGameMode = null;
        ChangeState(eGameMode.NONE);
    }
}
