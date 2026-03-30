using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 1.0f;

    private int currentWaypoint = 0;


    // Start is called before the first frame update
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, waypoints[currentWaypoint].transform.position);

        if (distance < 0.1f )
            currentWaypoint = (currentWaypoint + 1)%waypoints.Length;

        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypoint].transform.position, moveSpeed * Time.deltaTime);
    }
}
