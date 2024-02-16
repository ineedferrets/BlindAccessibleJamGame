using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringArmCameraComponent : MonoBehaviour
{
    public GameObject targetObject;
    public GameObject cameraObject;
    public Vector3 armOffset;

    public float cameraMaxVelocity = 20f;
    public float cameraAcceleration = 5.0f;

    private Vector3 previousCameraPosition;
    private Vector3 previousCameraVelocity = Vector3.zero;

    private void Start()
    {
        previousCameraPosition = cameraObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        Vector3 targetPosition = targetObject.transform.position + armOffset;
        Vector3 currentCameraPosition = cameraObject.transform.position;
        Vector3 direction = (targetPosition - currentCameraPosition).normalized;

        Vector3 newVelocity = previousCameraVelocity + direction * cameraAcceleration * deltaTime;
        if (newVelocity.sqrMagnitude < Mathf.Pow(cameraMaxVelocity, 2))
        {
            newVelocity = newVelocity.normalized * cameraMaxVelocity;
        }
        else if (newVelocity.sqrMagnitude < 0.5 * cameraAcceleration)
        {
            newVelocity = Vector3.zero;
        }

         cameraObject.transform.position = currentCameraPosition + newVelocity * deltaTime;
    }

    private void OnValidate()
    {
        if (targetObject == null) { return; }
        if (cameraObject == null) { return; }

        cameraObject.transform.position = targetObject.transform.position + armOffset;
    }
}
