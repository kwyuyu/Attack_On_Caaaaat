using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour {

    public GameObject Camera;

    private float MouseSensitivity = 1.5f;
    private float KeyboardSensitivity = 0.165f;
    private float OrbitDampening = 9.0f;

    private Transform mainCamera;
    private Transform pivot;
    private Transform character;
    private Vector3 localRotation;
    private float pivot_offset;
    private string controller;
    private Quaternion Qt;


    // Use this for initialization
    void Start () 
    {
        mainCamera = Camera.transform;
        pivot = Camera.transform.parent;
        character = pivot.parent.Find("Cat");

        pivot_offset = pivot.position.y - character.position.y;

        controller = character.parent.gameObject.GetComponent<CharacterMotion>().controller;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}

    private void LateUpdate()
    {

        pivot.position = character.position + new Vector3(0f, pivot_offset, 0f);


        if (controller == "mouse")
        {
            // mouse controller
            localRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity;
            localRotation.z -= Input.GetAxis("Mouse Y") * MouseSensitivity;
        }
        else if (controller == "wasd")
        {
            // direction
            if (Input.GetKey("a"))
            {
                localRotation.x -= 10f * KeyboardSensitivity;
            }
            if (Input.GetKey("d"))
            {
                localRotation.x += 10f * KeyboardSensitivity;
            }
            if (Input.GetKey("w"))
            {
                localRotation.z -= 10f * KeyboardSensitivity;
            }
            if (Input.GetKey("s"))
            {
                localRotation.z += 10f * KeyboardSensitivity;
            }
        }
        else if (controller == "direction")
        {
            // direction
            if (Input.GetKey("left"))
            {
                localRotation.x -= 10f * KeyboardSensitivity;
            }
            if (Input.GetKey("right"))
            {
                localRotation.x += 10f * KeyboardSensitivity;
            }
            if (Input.GetKey("up"))
            {
                localRotation.z -= 10f * KeyboardSensitivity;
            }
            if (Input.GetKey("down"))
            {
                localRotation.z += 10f * KeyboardSensitivity;
            }
        }

        // camera rotation update
        localRotation.z = Mathf.Clamp(localRotation.z, -90f, 90f);
        Qt = Quaternion.Euler(0f, localRotation.x, localRotation.z);
        pivot.rotation = Quaternion.Lerp(pivot.rotation, Qt, Time.deltaTime * OrbitDampening);
        //pivot.rotation = Qt;

        // do not rotate x axis
        pivot.eulerAngles = new Vector3(0f, pivot.eulerAngles.y, pivot.eulerAngles.z);

    }

}
