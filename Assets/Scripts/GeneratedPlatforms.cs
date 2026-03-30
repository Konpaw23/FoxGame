using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedPlatforms : MonoBehaviour
{
    [SerializeField] GameObject platformPrefab;
    [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] int PLATFORMS_NUM = 5;
    [SerializeField] float radius = 4.0f;
    [SerializeField] private bool moveClockwise = true;
    private float currentAngle = 0;
    GameObject[] platforms;
    private Vector2 fixedPoint;

    void Awake()
    {
        platforms = new GameObject[PLATFORMS_NUM];

        float angle = 2 * Mathf.PI / PLATFORMS_NUM;

        for(int i = 0; i < PLATFORMS_NUM; i++)
        {
            Vector2 position = new Vector2(Mathf.Cos(i * angle) * radius + transform.position.x, Mathf.Sin(i * angle) * radius + transform.position.y);
            platforms[i] = Instantiate(platformPrefab, position, Quaternion.identity);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        fixedPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        float distance = Vector2.Distance(platforms[0].transform.position, positions[currentWaypoint]);

        if (distance < 0.1f)
            currentWaypoint = (currentWaypoint + 1) % PLATFORMS_NUM;

        for (int i = 0; i < PLATFORMS_NUM; i++)
        {
            platforms[i].transform.position = Vector2.MoveTowards(platforms[i].transform.position, positions[(i+currentWaypoint)%PLATFORMS_NUM], moveSpeed * Time.deltaTime);
        }
        */

        currentAngle = (currentAngle + moveSpeed * Time.deltaTime) % (2*Mathf.PI);

        for(int i = 0; i < PLATFORMS_NUM; i++)
        {
            float currentPlatformAngle = currentAngle + i * (2 * Mathf.PI) / PLATFORMS_NUM;
            Vector2 offset = new Vector2(Mathf.Sin(currentPlatformAngle) * (moveClockwise ? 1 : -1), Mathf.Cos(currentPlatformAngle)) * radius;
            platforms[i].transform.position = fixedPoint + offset;
        }
    }
}
