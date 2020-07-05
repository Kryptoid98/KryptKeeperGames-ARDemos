using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KryptKeeperGamesARDemo.Enums;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    public Menu[] allMenusRef;
    [HideInInspector]public Menu activeMenu;
    eMenus menuState;

    bool mainMenuLoadedBefore = false;

    private void Awake()
    {
        instance = this;
    }

    public void ChangeState(eMenus newState)
    {
        menuState = newState;
    }

    public void SpawnMenu(eMenus menu)
    {
        if (menu == eMenus.MAIN_MENU)
        {
            if (mainMenuLoadedBefore)
                StartCoroutine(GameManager.RestartARSession());
            else mainMenuLoadedBefore = true;
        }

        ChangeState(menu);
        if (activeMenu != null)
            Destroy(activeMenu.gameObject);

        activeMenu = Instantiate(allMenusRef[(int)menu], Vector3.zero, Quaternion.identity);

        if (GameManager.gameState != eGameState.MENUS)
            GameManager.ChangeState(eGameState.MENUS);

    }

    public void DestroyActiveMenu()
    {
        Destroy(activeMenu.gameObject);
        activeMenu = null;
        ChangeState(eMenus.NONE);
    }
}
