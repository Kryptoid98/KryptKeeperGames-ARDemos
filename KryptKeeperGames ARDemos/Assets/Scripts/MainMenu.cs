using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KryptKeeperGamesARDemo.Enums;

public class MainMenu : Menu
{
    public Animator backgroundAnimator;

    private eGameMode modeToLoad = eGameMode.TRASHKITBALL_MODE;
    bool preparingToLoad = false;
    bool bgFadeStarted = false;

    public override void Init(GameObject parentObject){
    }

    public void LoadTrashkitBall()
    {
        modeToLoad = eGameMode.TRASHKITBALL_MODE;
        StartCoroutine(PrepareToLoadGameMode());
    }

    public void LoadAutoRTS()
    {
        modeToLoad = eGameMode.AUTORTS_MODE;
        StartCoroutine(PrepareToLoadGameMode());
    }

    public void LoadBlockBuilder()
    {
        modeToLoad = eGameMode.BLOCK_BUILDER;
        StartCoroutine(PrepareToLoadGameMode());
    }

    IEnumerator PrepareToLoadGameMode()
    {
        if (preparingToLoad) yield break;

        preparingToLoad = true;
        StartCoroutine(FadeOut());

        yield return new WaitUntil(() => !fadingOut);

        backgroundAnimator.enabled = true;
        bgFadeStarted = true;
    }

    void StartGameMode()
    {
        GameModeManager.instance.SpawnGameMode(modeToLoad);
        MenuManager.instance.DestroyActiveMenu();
    }

    private void Update()
    {
        if (bgFadeStarted)
        {
            if (backgroundAnimator == null) return;

            if (backgroundAnimator.GetCurrentAnimatorStateInfo(0).IsName("Done")){
                bgFadeStarted = false;
                StartGameMode();
            }
        }
    }
}
