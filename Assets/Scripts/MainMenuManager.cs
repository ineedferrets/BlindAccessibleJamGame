using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string GameSceneName;
    public void PlayGame()
    {
        SceneChanger sceneChanger = SceneChanger.Instance;
        if (sceneChanger == null)
            SceneManager.LoadSceneAsync(GameSceneName);
        else
            sceneChanger.ChangeScene(GameSceneName);
    }
}
