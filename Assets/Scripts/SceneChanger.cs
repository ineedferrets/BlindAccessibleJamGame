using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    enum FadeStatus
    {
        fading_in,
        fading_out,
        none
    }

    public static SceneChanger Instance;
    public CanvasGroup fadeImageGroup;
    public float fadeDelay;
    public float fadeDuration;

    private FadeStatus currentFadeStatus = FadeStatus.none;
    private float fadeTimer;
    private string sceneToLoad;

    private struct ImageInformation
    {
        public Image image;
        public Color color;

        public ImageInformation(Image image, Color color)
        {
            this.image = image;
            this.color = color;
        }
    }

    private List<ImageInformation> images;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        images = new List<ImageInformation>();
        foreach(Image image in fadeImageGroup.GetComponentsInChildren<Image>())
        {
            ImageInformation imageInfo = new ImageInformation(image, image.color);
            images.Add(imageInfo);
        }
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //scene loaded, running fade-in
        currentFadeStatus = FadeStatus.fading_in;
    }

    public void ChangeScene(string _name)
    {
        sceneToLoad = _name;
        currentFadeStatus = FadeStatus.fading_out;
    }

    void Update()
    {
        if (currentFadeStatus != FadeStatus.none)
        {
            fadeTimer += Time.deltaTime;
            float fadeTotalDuration = fadeDelay + fadeDuration;

            if (fadeTimer > fadeTotalDuration) //done fading
            {
                fadeTimer = 0;

                if (currentFadeStatus == FadeStatus.fading_out)
                {
                    SceneManager.LoadScene(sceneToLoad);
                    foreach (ImageInformation image in images)
                    {
                        image.image.color = image.color;
                    }
                }
                else
                {
                    foreach (ImageInformation image in images)
                    {
                        image.image.color = Color.clear;
                    }
                }

                currentFadeStatus = FadeStatus.none;
            }
            else //still fading
            {
                float alpha = 0;
                
                if (currentFadeStatus == FadeStatus.fading_out)
                {
                    if (fadeTimer < fadeDuration)
                        alpha = Mathf.Lerp(0, 1, fadeTimer / fadeDuration);
                    else
                        alpha = 1;
                }
                else
                {
                    if (fadeTimer < fadeDelay)
                        alpha = 1;
                    else
                        alpha = Mathf.Lerp(1, 0, (fadeTimer - fadeDelay) / fadeDuration);
                }
                    
                foreach (ImageInformation image in images)
                {
                    image.image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                }
            }
        }
    }
}