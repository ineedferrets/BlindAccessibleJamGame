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
    GameObject pressToInteractVisual;

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
            pressToInteractVisual.SetActive(true);

            TextMeshPro text = pressToInteractVisual.GetComponentInChildren<TextMeshPro>();
            if (text)
            {
                ScreenReader.StaticReadText(text.text);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerTopDownController controller = collision.GetComponent<PlayerTopDownController>();
        if (controller)
        {
            controller.RemoveInteractable(this);
            pressToInteractVisual.SetActive(false);
        }
    }

    public void Interact()
    {
        m_OnInteraction.Invoke();
    }

}
