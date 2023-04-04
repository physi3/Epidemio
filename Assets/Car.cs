using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public void MakeJouney(Vector2Int origin, Vector2Int destination, PathFollower.FollowComplete callback)
    {
        PathFollower pf = gameObject.AddComponent<PathFollower>();

        pf.origin = origin;
        pf.destination = destination;

        pf.OnFollowComplete += callback;
        pf.OnFollowComplete += DeleteCar;
    }

    void DeleteCar()
    {
        Destroy(gameObject);
    }
}
