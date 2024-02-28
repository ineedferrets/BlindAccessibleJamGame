using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InteractableComponent : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField]
    public GameObject pressToInteractVisual;

    private void Start()
    {
        pressToInteractVisual.SetActive(false);
    }

    [Serializable]
    public class InteractionEvent : UnityEvent {}

    [Header("Interaction")]
    [SerializeField]
    private InteractionEvent m_OnInteraction = new InteractionEvent();

    public InteractionEvent onInteraction
    {
        get { return m_OnInteraction; }
        set { m_OnInteraction = value; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerTopDownController controller = collision.GetComponent<PlayerTopDownController>();
        if (controller)
        {
            controller.AddInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerTopDownController controller = collision.GetComponent<PlayerTopDownController>();
        if (controller)
        {
            controller.RemoveInteractable(this);
        }
    }

    public void Interact()
    {
        m_OnInteraction.Invoke();
    }

    public void ToggleVisuals(bool toggle)
    {
        if (!pressToInteractVisual)
            return;
            
        if (toggle && !pressToInteractVisual.activeSelf)
        {
            pressToInteractVisual.SetActive(toggle);
            TextMeshPro textMeshProUGUI = pressToInteractVisual.GetComponent<TextMeshPro>();
            string textToRead = textMeshProUGUI ? textMeshProUGUI.text : "";
            if (textToRead != "" && Application.platform != RuntimePlatform.WebGLPlayer)
            {
                ScreenReader.StaticReadText(textToRead);
            }
        }

        pressToInteractVisual.SetActive(toggle);
    }

}
