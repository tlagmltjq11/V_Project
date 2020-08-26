using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField]
    GameObject m_player;
    Vector2 m_mouseDir = Vector2.zero;
    public float mouseSensitivity = 400f;
    float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MouseLook();
    }

    void MouseLook()
    {
        m_mouseDir.x = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        m_mouseDir.y = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (m_mouseDir != Vector2.zero)
        {
            xRotation -= m_mouseDir.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            m_player.transform.Rotate(Vector3.up * m_mouseDir.x);
        }
    }
}
