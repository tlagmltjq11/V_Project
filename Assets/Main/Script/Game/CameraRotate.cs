using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField]
    GameObject m_player;
    [SerializeField]
    GameObject m_verticalCamRotate;
    Vector2 m_mouseDir = Vector2.zero;
    public float mouseSensitivity = 400f;
    public float xRotation = 0f;
    // Start is called before the first frame update

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        MouseLook();
    }

    void MouseLook()
    {
        m_mouseDir.x = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        m_mouseDir.y = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= m_mouseDir.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        m_verticalCamRotate.transform.localRotation = Quaternion.Slerp(m_verticalCamRotate.transform.localRotation, Quaternion.Euler(xRotation, 0, 0), Time.deltaTime * 35f);
        m_player.transform.Rotate(Vector3.up * m_mouseDir.x);
    }

    public void VerticalCamRotate(float recoilRotX)
    {
        xRotation -= recoilRotX;
    }
}
