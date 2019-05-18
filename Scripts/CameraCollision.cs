using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour {

    public GameObject Camera;
    public float smooth = 10.0f;

    private float minDistance = 1.0f;
    private float maxDistance = 4.0f;
    private Vector3 dollyDir;
    private Vector3 dollyDirAdjusted;
    private float distance;
    private Transform mainCamera;
    private Transform pivot;
    private Transform character;


    // use this for initialization
    void Awake()
    {
        mainCamera = Camera.transform;
        pivot = Camera.transform.parent;
        character = pivot.parent.Find("Cat");
        maxDistance = Mathf.Abs(pivot.position.x - mainCamera.position.x);

        dollyDir = mainCamera.localPosition.normalized;
        distance = mainCamera.localPosition.magnitude;
    }

    // Update is called once per frame
    void Update () 
    {
        Vector3 desiredCameraPos = pivot.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        if (Physics.Linecast(pivot.position, desiredCameraPos, out hit))
        {
            distance = Mathf.Clamp(hit.distance * 0.87f, minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        mainCamera.localPosition = Vector3.Lerp(mainCamera.localPosition, dollyDir * distance , Time.deltaTime * smooth);
    }
}
