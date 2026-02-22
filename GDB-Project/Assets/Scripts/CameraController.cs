using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sensitivity = 30;
    [SerializeField] int lockVertMin = -90, lockVertMax = 90;
    [SerializeField] bool invertY;

    float camRotX;

    [Header("Camera Shake")]
    [SerializeField] float shakeDamping = 1f;

    Vector3 originalLocalPos;
    Coroutine shakeRoutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalLocalPos = transform.localPosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Time.timeScale == 0f)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.timeScale; //Raw avoids smoothing, we want smoothing
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.timeScale; //multiplying by delta time is inconsistent because mouse get axis is already per frame
        //Multiply by time scale so we can get some slow motion if we want it

        if (invertY)
        {
            camRotX += mouseY;
        }
        else
        {
            camRotX -= mouseY;
        }

        camRotX = Mathf.Clamp(camRotX, lockVertMin, lockVertMax);
        transform.localRotation = Quaternion.Euler(camRotX, 0, 0);

        transform.parent.Rotate(Vector3.up * mouseX);

    }

    public void ShakeCamera(float amplitude, float duration)
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            transform.localPosition = originalLocalPos;
        }

        shakeRoutine = StartCoroutine(CoShake(amplitude, duration));
    }

    IEnumerator CoShake(float amplitude, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float damper = 1f - Mathf.Clamp01(elapsed / duration);
            float currentAmplitude = amplitude * damper * shakeDamping;

            Vector3 offset = UnityEngine.Random.insideUnitSphere * currentAmplitude;
            transform.localPosition = originalLocalPos + offset;

            yield return null;
        }

        transform.localPosition = originalLocalPos;
        shakeRoutine = null;
    }
}
