using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 7f;
    [Header("Zoom")]
    [SerializeField] float scrollSpeed = 1f;
    [SerializeField] float scrollSnap = 10f;
    [SerializeField] float lower = 1f;
    [SerializeField] float upper = 10f;

    Camera cameraComponent;

    Vector3 mouseBeginPosition;
    Vector3 targetPosition;

    float targetZoom;

    bool following = false;
    Transform targetTransform;
    
    public void Awake()
    {
        cameraComponent = GetComponent<Camera>();
        targetPosition = transform.position;
        targetZoom = cameraComponent.orthographicSize;
    }

    public void Update()
    {
        if (following)
        {
            targetPosition = new Vector3(targetTransform.position.x, targetTransform.position.y, -10f);
        }
        if (Input.GetMouseButtonDown(0))
        {
            mouseBeginPosition = MouseWorldPosition();
            following = false;
        }
        if (Input.GetMouseButton(0))
        {
            targetPosition = transform.position + mouseBeginPosition - MouseWorldPosition();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (Vector3.Distance(mouseBeginPosition, MouseWorldPosition()) < 0.1f)
                TownSystem.instance.SelectGroup(TownSystem.instance.PositionToGroup(MouseWorldPosition()));
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        targetZoom += Input.mouseScrollDelta.y * scrollSpeed;
        targetZoom = Mathf.Clamp(targetZoom, lower, upper);
        cameraComponent.orthographicSize = Mathf.Lerp(cameraComponent.orthographicSize, targetZoom, scrollSnap * Time.deltaTime);
    }

    public void FollowTarget(Transform target)
    {
        targetTransform = target;
        targetZoom = 2f;
        following = true;
    }

    Vector3 MouseWorldPosition()
    {
        return cameraComponent.ScreenToWorldPoint(Input.mousePosition);
    }

}
