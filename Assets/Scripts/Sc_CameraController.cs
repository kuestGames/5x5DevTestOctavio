using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_CameraController : MonoBehaviour
{
    private Transform cameraTransform;
    private Camera cameraComponent;
    [SerializeField] private float cameraShakeDuration = 0.5f;
    [SerializeField] private float cameraShakeAmount = 1f;

    private void Start()
    {
        cameraTransform = transform;
        cameraComponent = this.GetComponent<Camera>();
    }
    /// <summary>
    /// public method called from the VFX manager that triggers the shake behavior coroutine
    /// </summary>
    public void ShakeCamera()
    {
        StartCoroutine("ShakeCameraCoroutine");

    }

    /// <summary>
    /// Coroutine that handles the camera shaking effect
    /// </summary>
    /// <returns></returns>
    IEnumerator ShakeCameraCoroutine()
    {
        var originalPos = cameraTransform.localPosition;
        var duration = cameraShakeDuration;
        while (duration > 0)
        {
            cameraTransform.localPosition = originalPos + Random.insideUnitSphere * cameraShakeAmount;
            duration -= Time.deltaTime;
            yield return null;
        }
        cameraTransform.localPosition = originalPos;
    }
}
