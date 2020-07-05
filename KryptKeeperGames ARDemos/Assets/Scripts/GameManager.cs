using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KryptKeeperGamesARDemo.Enums;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static eGameState gameState = eGameState.INIT;

    //Main Managers
    public MenuManager menuManager;
    public GameModeManager gameModeManager;

    [HideInInspector]public GameObject ARObjectPlacementPrefab;
    private void Awake()
    {
        instance = this;
        Initialize();      
    }

    private void Initialize()
    {
        ARObjectPlacementPrefab = ContentLoader.LoadPrefab(ContentLoader.PrefabID.AR_OBJECT_PLACEMENT);
        menuManager = Instantiate(menuManager, Vector3.zero, Quaternion.identity);
        menuManager.transform.parent = transform;
        gameModeManager = Instantiate(gameModeManager, Vector3.zero, Quaternion.identity);
        gameModeManager.transform.parent = transform;
        //GameData.LoadResources();
        //PlayerData.LoadPlayerData();

        menuManager.SpawnMenu(eMenus.MAIN_MENU);
    }

    private void Start()
    {
        //ARManager.SetPlaneManagerActive(false);
        //ARManager.SetPointCloudManagerActive(false);
        
    }

    public static void ChangeState(eGameState newState)
    {
        switch (newState)
        {
            case eGameState.INIT:
                break;
            case eGameState.MENUS:
                break;
            case eGameState.IN_GAME:
                break;
        }

        gameState = newState;
    }

    public static IEnumerator RestartARSession()
    {
        ARManager.DestroyCurrentARSession();
        yield return null;
        ARManager.AssignNewARSession(Instantiate((GameObject)Resources.Load("AR Session")));

        ARManager.SetPlaneManagerActive(false);
    }

    //private void Update()
    //{
    //    ARDebug.Log(ARManager.arPlaneManager().trackables.count, 0.5f);
    //}

}
