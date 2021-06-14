using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    // public PixelPerfectCamera camera
    private Camera mainCamera;
    public GameObject target;

    [Header("Camera Movement Settings")]
    public float cameraSpeed;
    public Vector2 cameraConstraints;
    public Vector3 cameraOffset;

    [Header("Camera Size Settings")]
    public float sizeChangeSpeed;
    public float defaultSize;
    public float moduleSizeIncrease;
    public float size;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = gameObject.GetComponent<Camera>();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null){
            FollowTarget();
        }

        SetSize();
    }

    void FollowTarget(){
        Vector3 targetPosition = target.transform.position + cameraOffset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed);
    }

    void SetSize(){
        int modules = target.GetComponent<PlayerController>().modules;  

        size = defaultSize + moduleSizeIncrease*modules;

        if (mainCamera.orthographicSize != size){
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, size, sizeChangeSpeed);
        }
    }
}
