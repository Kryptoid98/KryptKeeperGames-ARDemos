using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitManager : MonoBehaviour
{
    AsyncOperation aLoadScene;
    private string mainScene = "MainScene";

    private void Start()
    {
        aLoadScene = SceneManager.LoadSceneAsync(mainScene);
        aLoadScene.allowSceneActivation = false;
    }

    private void Update()
    {
        if (aLoadScene.progress >= 0.9f)
        {
            aLoadScene.allowSceneActivation = true;
        }
    }
}
