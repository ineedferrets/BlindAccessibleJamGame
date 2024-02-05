using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string GameSceneName;
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(GameSceneName);
    }
}
