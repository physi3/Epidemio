using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    List<Vector2Int> currentPath;
    Vector3 moveDirection;
    int currentPathIndex;

    public float turnSpeed = 10f;
    public float maxSpeed = 15f;
    private float speed = 0f;
    public float roadOffset = 0.25f;

    public Vector2Int origin;
    public Vector2Int destination;

    public delegate void FollowComplete();
    public event FollowComplete OnFollowComplete;

    private void Start()
    {
        currentPath = RoadSystem.instance.FindShortestPath(origin, destination);
        currentPathIndex = 0;
        transform.position = PathPoint(currentPathIndex);
        moveDirection = PathPoint(currentPathIndex + 1) - transform.position;
        moveDirection = new(Mathf.Round(moveDirection.x), Mathf.Round(moveDirection.y));
    }
    private void Update()
    {
        if (Vector3.Dot(moveDirection, PathPoint(currentPathIndex + 1) - transform.position) <= 0)
        { // If we've reached the next destination along the direction of movement
            if (Vector3.Dot(Vector3.Cross(moveDirection, Vector3.forward), PathPoint(currentPathIndex + 1) - transform.position) != 0)
            { // If we haven't reached the next destination along the perpendicular axis e.g. we need to turn
                transform.position += moveDirection * Vector3.Dot(moveDirection.normalized, PathPoint(currentPathIndex + 1) - transform.position); // Snap to the road
                moveDirection = Vector3.Cross(moveDirection, Vector3.forward); // Turn to face the next destination
                moveDirection *= Mathf.Sign(Vector3.Dot(moveDirection, PathPoint(currentPathIndex + 2) - transform.position));
            }
            currentPathIndex++;
            if (currentPathIndex >= currentPath.Count - 1)
            { // If we've reached the end of the path
                OnFollowComplete?.Invoke();
                Destroy(this);
            }
        }
        transform.position += speed * GameTime.DeltaTime * moveDirection;

        // Smooth the rotation and speed
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(moveDirection, Vector3.back),
            Mathf.Min(Time.deltaTime * turnSpeed * maxSpeed, 1)
            );

        speed = Mathf.Lerp(speed, maxSpeed, GameTime.DeltaTime * 2f);
    }
    private Vector3 PathPoint(int index)
    {
        bool finalPoint = index == currentPath.Count - 1;
        if (finalPoint) index--;

        // Determine the direction the path follower should be moving
        Vector3 origin = new Vector3(currentPath[index].x, currentPath[index].y);
        Vector3 destination = new Vector3(currentPath[index + 1].x, currentPath[index + 1].y);
        // Use the direction to move the path to the left side of the road (using the cross product)
        Vector3 offset = Vector3.Cross(Vector3.forward, destination - origin) * roadOffset;

        if (finalPoint) return destination + offset;
        return origin + offset;
    }
}
