using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public string GameSceneName;

    public Selectable startSelectable;

    public void PlayGame()
    {
        SceneChanger sceneChanger = SceneChanger.Instance;
        if (sceneChanger == null)
            SceneManager.LoadSceneAsync(GameSceneName);
        else
            sceneChanger.ChangeScene(GameSceneName);
    }

    private void Start()
    {
        if (startSelectable)
        {
            startSelectable.Select();
        }
    }
}
