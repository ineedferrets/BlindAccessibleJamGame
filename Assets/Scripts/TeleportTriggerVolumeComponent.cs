using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTriggerVolumeComponent : MonoBehaviour
{
    public Vector2 toTeleportTo {  get; private set; }
    public Camera toChangeCameraTo;
    public GameObject visualOnEnterTrigger;

    private void Start()
    {
        if (visualOnEnterTrigger != null)
            visualOnEnterTrigger.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 position = new Vector3(toTeleportTo.x, toTeleportTo.y, transform.position.z);
        Gizmos.DrawSphere(position, 0.1f);
    }

    private void OnValidate()
    {
        Transform childTransform = gameObject.transform.Find("ToTeleportTo");
        if (childTransform != null)
        {
            toTeleportTo = childTransform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerTopDownController controller = collision.GetComponent<PlayerTopDownController>();
        if (controller && visualOnEnterTrigger)
        {
            visualOnEnterTrigger.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerTopDownController controller = collision.GetComponent<PlayerTopDownController>();
        if (controller && visualOnEnterTrigger)
        {
            visualOnEnterTrigger.gameObject.SetActive(false);
        }
    }
}
