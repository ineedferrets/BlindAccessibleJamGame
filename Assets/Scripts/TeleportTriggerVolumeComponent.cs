using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTriggerVolumeComponent : MonoBehaviour
{
    public Vector2 toTeleportTo {  get; private set; }
    public Camera toChangeCameraTo;

    private void Start()
    {
        Transform childTransform = gameObject.transform.Find("ToTeleportTo");
        if (childTransform != null)
        {
            toTeleportTo = childTransform.position;
        }
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
}
