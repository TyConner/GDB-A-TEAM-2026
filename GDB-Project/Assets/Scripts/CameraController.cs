using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sensitivity = 30;
    [SerializeField] int lockVertMin = -90, lockVertMax = 90;
    [SerializeField] bool invertY;

    float camRotX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
}
