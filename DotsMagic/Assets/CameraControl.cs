using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    public float lookSpeedH = 2f;
    public float lookSpeedV = 2f;
    public float zoomSpeed = 2f;
    public float dragSpeed = 5f;

    private float yaw;
    private float pitch;


    private void Start()
    {
        // x - right    pitch
        // y - up       yaw
        // z - forward  roll
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        if (!enabled) return;

        {

            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * 0.2f;
            Vector2 scrollDelta = Mouse.current.scroll.ReadValue();

            //Look around with Right Mouse
            if (Mouse.current.rightButton.isPressed)
            {
                yaw += lookSpeedH * mouseDelta.x;
                pitch -= lookSpeedV * mouseDelta.y;

                transform.eulerAngles = new Vector3(pitch, yaw, 0f);

                Vector3 offset = Vector3.zero;
                float offsetDelta = Time.deltaTime * dragSpeed *4.2f;
                if (Keyboard.current.leftShiftKey.isPressed) offsetDelta *= 2.5f;
                if (Keyboard.current.sKey.isPressed) offset.z -= offsetDelta;
                if (Keyboard.current.wKey.isPressed) offset.z += offsetDelta;
                if (Keyboard.current.aKey.isPressed) offset.x -= offsetDelta;
                if (Keyboard.current.dKey.isPressed) offset.x += offsetDelta;
                if (Keyboard.current.qKey.isPressed) offset.y -= offsetDelta;
                if (Keyboard.current.eKey.isPressed) offset.y += offsetDelta;

                transform.Translate(offset, Space.Self);

                transform.Translate(0, 0, scrollDelta.y * zoomSpeed * 0.1f, Space.Self);
            }

            //drag camera around with Middle Mouse
            if (Mouse.current.middleButton.isPressed)
            {
                transform.Translate(-mouseDelta.x * Time.deltaTime * dragSpeed, -mouseDelta.y * Time.deltaTime * dragSpeed, 0);
            }

            //Zoom in and out with Mouse Wheel
        }
    }
}
